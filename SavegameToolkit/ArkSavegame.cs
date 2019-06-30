using System;
using System.Collections.Generic;
using System.Linq;
using SavegameToolkit.Types;

namespace SavegameToolkit
{

    public class ArkSavegame : GameObjectContainerMixin, IConversionSupport
    {
        public short SaveVersion { get; private set; }

        /// <summary>
        /// How long has this map been running
        /// </summary>
        public float GameTime { get; private set; }

        /// <summary>
        /// How often has this map-save been written
        /// </summary>
        public int SaveCount { get; private set; }

        public List<string> OldNameList { get; private set; }

        public List<string> DataFiles { get; } = new List<string>();

        public List<EmbeddedData> EmbeddedData { get; } = new List<EmbeddedData>();

        public Dictionary<int, List<string[]>> DataFilesObjectMap { get; } = new Dictionary<int, List<string[]>>();

        public override List<GameObject> Objects { get; } = new List<GameObject>();

        private int hibernationOffset;

        private int nameTableOffset;

        private int propertiesBlockOffset;

        private int hibernationV8Unknown1;

        private int hibernationV8Unknown2;

        private int hibernationV8Unknown3;

        private int hibernationV8Unknown4;

        private int hibernationUnknown1;

        private int hibernationUnknown2;

        private readonly List<string> hibernationClasses = new List<string>();

        private readonly List<int> hibernationIndices = new List<int>();

        public List<HibernationEntry> HibernationEntries { get; } = new List<HibernationEntry>();

        public bool HasUnknownNames => OldNameList != null;

        public bool HasUnknownData { get; set; }

        private HashSet<string> nameTableForWriteBinary;

        #region readBinary

        public void ReadBinary(ArkArchive archive, ReadingOptions options)
        {
            readBinaryHeader(archive);

            if (SaveVersion > 5)
            {
                // Name table is located after the objects block, but will be needed to read the objects block
                readBinaryNameTable(archive);
            }

            readBinaryDataFiles(archive, options);
            readBinaryEmbeddedData(archive, options);
            readBinaryDataFilesObjectMap(archive, options);
            readBinaryObjects(archive, options);
            readBinaryObjectProperties(archive, options);

            if (SaveVersion > 6)
            {
                readBinaryHibernation(archive, options);
            }

            OldNameList = archive.HasUnknownNames ? archive.NameTable : null;
            HasUnknownData = archive.HasUnknownData;
        }

        private void readBinaryHeader(ArkArchive archive)
        {
            SaveVersion = archive.ReadShort();

            if (SaveVersion < 5 || SaveVersion > 9)
            {
                throw new NotSupportedException("Found unknown Version " + SaveVersion);
            }

            if (SaveVersion > 6)
            {
                hibernationOffset = archive.ReadInt();
                var shouldBeZero = archive.ReadInt();
                if (shouldBeZero != 0)
                {
                    throw new NotSupportedException("The stuff at this position should be zero: " + (archive.Position - 4).ToString("X4"));
                }
            }
            else
            {
                hibernationOffset = 0;
            }

            if (SaveVersion > 5)
            {
                nameTableOffset = archive.ReadInt();
                propertiesBlockOffset = archive.ReadInt();
            }
            else
            {
                nameTableOffset = 0;
                propertiesBlockOffset = 0;
            }

            GameTime = archive.ReadFloat();

            SaveCount = SaveVersion > 8 ? archive.ReadInt() : 0;
        }

        private void readBinaryNameTable(ArkArchive archive)
        {
            var position = archive.Position;

            archive.Position = nameTableOffset;

            var nameCount = archive.ReadInt();
            var nameTable = new List<string>(nameCount);
            for (var n = 0; n < nameCount; n++)
            {
                nameTable.Add(archive.ReadString());
            }

            archive.SetNameTable(nameTable);

            archive.Position = position;
        }

        private void readBinaryDataFiles(ArkArchive archive, ReadingOptions options)
        {
            var count = archive.ReadInt();

            DataFiles.Clear();
            if (options.DataFiles)
            {
                for (var n = 0; n < count; n++)
                {
                    DataFiles.Add(archive.ReadString());
                }
            }
            else
            {
                archive.HasUnknownData = true;
                for (var n = 0; n < count; n++)
                {
                    archive.SkipString();
                }
            }
        }

        private void readBinaryEmbeddedData(ArkArchive archive, ReadingOptions options)
        {
            var count = archive.ReadInt();

            EmbeddedData.Clear();
            if (options.EmbeddedData)
            {
                for (var n = 0; n < count; n++)
                {
                    EmbeddedData.Add(new EmbeddedData(archive));
                }
            }
            else
            {
                archive.HasUnknownData = true;
                for (var n = 0; n < count; n++)
                {
                    Types.EmbeddedData.Skip(archive);
                }
            }
        }

        private void readBinaryDataFilesObjectMap(ArkArchive archive, ReadingOptions options)
        {
            DataFilesObjectMap.Clear();
            if (options.DataFilesObjectMap)
            {
                var dataFilesCount = archive.ReadInt();
                for (var n = 0; n < dataFilesCount; n++)
                {
                    var level = archive.ReadInt();
                    var count = archive.ReadInt();
                    var names = new string[count];
                    for (var index = 0; index < count; index++)
                    {
                        names[index] = archive.ReadString();
                    }

                    if (!DataFilesObjectMap.ContainsKey(level) || DataFilesObjectMap[level] == null)
                    {
                        DataFilesObjectMap.Add(level, new List<string[]> { names });
                    }
                }
            }
            else
            {
                archive.HasUnknownData = true;
                var count = archive.ReadInt();
                for (var entry = 0; entry < count; entry++)
                {
                    archive.SkipBytes(4);
                    var stringCount = archive.ReadInt();
                    for (var stringIndex = 0; stringIndex < stringCount; stringIndex++)
                    {
                        archive.SkipString();
                    }
                }
            }
        }

        private void readBinaryObjects(ArkArchive archive, ReadingOptions options)
        {
            if (options.GameObjects)
            {
                var count = archive.ReadInt();

                Objects.Clear();
                ObjectMap.Clear();
                while (count-- > 0)
                {
                    addObject(new GameObject(archive), options.BuildComponentTree);
                }
            }
            else
            {
                archive.HasUnknownData = true;
                archive.HasUnknownNames = true;
            }
        }

        private void readBinaryObjectProperties(ArkArchive archive, ReadingOptions options)
        {
            if (options.GameObjects && options.GameObjectProperties)
            {
                //if (options.isParallel()) {
                //    ParallelQuery<int> parallelQuery = Enumerable.Range(0, Objects.Count).AsParallel();

                //    if (options.ObjectFilter != null) {
                //        parallelQuery = parallelQuery.Where(n => options.ObjectFilter(Objects[n]));
                //    }

                //    parallelQuery.ForAll(n => readBinaryObjectPropertiesImpl(n, archive.Clone()));
                //} else {

                var stream = Enumerable.Range(0, Objects.Count);

                if (options.ObjectFilter != null)
                {
                    stream = stream.Where(n => options.ObjectFilter(Objects[n]));
                }

                foreach (var n in stream)
                {
                    readBinaryObjectPropertiesImpl(n, archive);
                }
                //}

                if (options.ObjectFilter != null)
                {
                    archive.HasUnknownData = true;
                    archive.HasUnknownNames = true;
                }
            }
            else
            {
                archive.HasUnknownData = true;
                archive.HasUnknownNames = true;
            }
        }

        private void readBinaryObjectPropertiesImpl(int n, ArkArchive archive)
        {
            Objects[n].LoadProperties(archive, (n < Objects.Count - 1) ? Objects[n + 1] : null, propertiesBlockOffset);
        }

        private void readBinaryHibernation(ArkArchive archive, ReadingOptions options)
        {
            if (!options.Hibernation)
            {
                hibernationV8Unknown1 = 0;
                hibernationV8Unknown2 = 0;
                hibernationV8Unknown3 = 0;
                hibernationV8Unknown4 = 0;
                hibernationUnknown1 = 0;
                hibernationUnknown2 = 0;
                hibernationClasses.Clear();
                hibernationIndices.Clear();
                HibernationEntries.Clear();
                archive.HasUnknownData = true;
                return;
            }

            archive.Position = hibernationOffset;

            if (SaveVersion > 7)
            {
                hibernationV8Unknown1 = archive.ReadInt();
                hibernationV8Unknown2 = archive.ReadInt();
                hibernationV8Unknown3 = archive.ReadInt();
                hibernationV8Unknown4 = archive.ReadInt();
            }

            // No hibernate section if we reached the nameTable
            if (archive.Position == nameTableOffset)
            {
                return;
            }

            hibernationUnknown1 = archive.ReadInt();
            hibernationUnknown2 = archive.ReadInt();

            var hibernatedClassesCount = archive.ReadInt();

            hibernationClasses.Clear();
            hibernationClasses.Capacity = hibernatedClassesCount;
            for (var index = 0; index < hibernatedClassesCount; index++)
            {
                hibernationClasses.Add(archive.ReadString());
            }

            var hibernatedIndicesCount = archive.ReadInt();

            if (hibernatedIndicesCount != hibernatedClassesCount)
            {
                archive.DebugMessage("hibernatedClassesCount does not match hibernatedIndicesCount");
                throw new NotSupportedException();
            }

            hibernationIndices.Clear();
            hibernationIndices.Capacity = hibernatedIndicesCount;
            for (var index = 0; index < hibernatedIndicesCount; index++)
            {
                hibernationIndices.Add(archive.ReadInt());
            }

            var hibernatedObjectsCount = archive.ReadInt();

            HibernationEntries.Clear();
            HibernationEntries.Capacity = hibernatedObjectsCount;
            for (var index = 0; index < hibernatedObjectsCount; index++)
            {
                HibernationEntries.Add(new HibernationEntry(archive, options));
            }
        }

        #endregion

        public int CalculateSize()
        {
            // calculateHeaderSize checks for valid known versions
            var calculator = ArkArchive.GetNameSizer(SaveVersion > 5);

            var size = calculateHeaderSize();
            size += calculateDataFilesSize();
            size += calculateEmbeddedDataSize();
            size += calculateDataFilesObjectMapSize();
            size += calculateObjectsSize(calculator);

            if (SaveVersion > 6)
            {
                hibernationOffset = size;
                size += calculateHibernationSize();
            }

            if (SaveVersion > 5)
            {
                nameTableOffset = size;

                nameTableForWriteBinary = OldNameList != null ? new ListAppendingSet<string>(OldNameList) : new HashSet<string>();

                Objects.ForEach(o => o.CollectNames(arkName => nameTableForWriteBinary.Add(arkName.Name)));

                if (OldNameList != null)
                {
                    size += 4 + ((ListAppendingSet<string>)nameTableForWriteBinary).List.Sum(ArkArchive.GetStringLength);
                }
                else
                {
                    size += 4 + nameTableForWriteBinary.Sum(ArkArchive.GetStringLength);
                }
            }
            else
            {
                nameTableForWriteBinary = null;
            }

            propertiesBlockOffset = size;

            size += calculateObjectPropertiesSize(calculator);
            return size;
        }

        #region calculate sizes

        private int calculateHeaderSize()
        {
            if (SaveVersion < 5 || SaveVersion > 9)
            {
                throw new NotSupportedException("Version " + SaveVersion + " is unknown and cannot be written in binary form");
            }

            // saveVersion + gameTime
            var size = sizeof(short) + sizeof(float);

            if (SaveVersion > 5)
            {
                // nameTableOffset + propertiesBlockOffset
                size += sizeof(int) * 2;
            }

            if (SaveVersion > 6)
            {
                // hibernationOffset + shouldBeZero
                size += sizeof(int) * 2;
            }

            if (SaveVersion > 8)
            {
                // saveCount
                size += sizeof(int);
            }

            return size;
        }

        private int calculateDataFilesSize()
        {
            return 4 + DataFiles.Sum(ArkArchive.GetStringLength);
        }

        private int calculateEmbeddedDataSize()
        {
            return 4 + EmbeddedData.Sum(data => data.Size);
        }

        private int calculateDataFilesObjectMapSize()
        {
            var size = 4;
            foreach (var namesListList in DataFilesObjectMap.Values)
            {
                size += namesListList.Count * 8;
                foreach (var namesList in namesListList)
                {
                    size += namesList.Sum(ArkArchive.GetStringLength);
                }
            }

            return size;
        }

        private int calculateObjectsSize(NameSizeCalculator nameSizer)
        {
            return 4 + Objects.AsParallel().Sum(o => o.Size(nameSizer));
        }

        private int calculateObjectPropertiesSize(NameSizeCalculator nameSizer)
        {
            return Objects.AsParallel().Sum(o => o.PropertiesSize(nameSizer));
        }

        private int calculateHibernationSize()
        {
            var size = SaveVersion > 7 ? sizeof(int) * 4 : 0;

            if (HibernationEntries.Count <= 0)
            {
                return size;
            }

            size += sizeof(int) * (5 + hibernationIndices.Count);
            size += hibernationClasses.Sum(ArkArchive.GetStringLength);
            size += HibernationEntries.Sum(hibernationEntry => hibernationEntry.GetSizeAndCollectNames());
            return size;
        }

        #endregion
    }
}
