using System;
using System.Collections.Generic;
using System.Linq;
using SavegameToolkit.Types;

namespace SavegameToolkit {

    public class HibernationEntry : GameObjectContainerMixin {

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public byte UnkByte { get; set; }
        public float UnkFloat { get; set; }

        public List<ArkName> ZoneVolumes { get; } = new List<ArkName>();

        public override List<GameObject> Objects { get; } = new List<GameObject>();

        public int UnkInt1 { get; set; }

        public int ClassIndex { get; set; }

        private List<string> nameTable;

        private int nameTableSize;

        private int objectsSize;

        private int propertiesStart;

        public HibernationEntry(ArkArchive archive, ReadingOptions options) {
            readBinary(archive, options);
        }

        private void readBinary(ArkArchive archive, ReadingOptions options) {
            X = archive.ReadFloat();
            Y = archive.ReadFloat();
            Z = archive.ReadFloat();
            UnkByte = archive.ReadByte();
            UnkFloat = archive.ReadFloat();

            if (options.HibernationObjectProperties) {
                var nameArchive = archive.Slice(archive.ReadInt());
                readBinaryNameTable(nameArchive);
            } else {
                archive.SkipBytes(archive.ReadInt());
                nameTable = null;

                // Unknown data since the missed names are unrelated to the main nameTable
                archive.HasUnknownData = true;
            }

            var objectArchive = archive.Slice(archive.ReadInt());
            readBinaryObjects(objectArchive, options);

            UnkInt1 = archive.ReadInt();
            ClassIndex = archive.ReadInt();
        }

        private void readBinaryNameTable(ArkArchive archive) {
            var version = archive.ReadInt();
            if (version != 3) {
                archive.DebugMessage($"Found unknown Version {version}", -4);
                throw new NotSupportedException();
            }

            var count = archive.ReadInt();
            nameTable = new List<string>(count);

            for (var index = 0; index < count; index++) {
                nameTable.Add(archive.ReadString());
            }

            var zoneCount = archive.ReadInt();

            for (var index = 0; index < zoneCount; index++) {
                ZoneVolumes.Add(archive.ReadName());
            }
        }

        private void readBinaryObjects(ArkArchive archive, ReadingOptions options) {
            var count = archive.ReadInt();

            Objects.Clear();
            Objects.Capacity = count;
            ObjectMap.Clear();
            for (var index = 0; index < count; index++) {
                addObject(new GameObject(archive), options.BuildComponentTree);
            }

            if (nameTable == null)
            {
                return;
            }

            archive.SetNameTable(nameTable, 0, true);

            for (var index = 0; index < count; index++) {
                Objects[index].LoadProperties(archive, index + 1 < count ? Objects[index + 1] : null, 0);
            }
        }

        public int GetSizeAndCollectNames() {
            // x y z unkFloat, unkByte, unkInt1 classIndex nameTableSize objectsSize
            const int size = (sizeof(float) * 4) + 1 + (sizeof(int) * 4);

            var names = new HashSet<string>();
            foreach (var gameObject in Objects) {
                gameObject.CollectPropertyNames(name => names.Add(name.ToString()));
            }

            var objectSizer = ArkArchive.GetNameSizer(false);
            var propertiesSizer = ArkArchive.GetNameSizer(true, true);

            nameTableSize = sizeof(int) * 3;
            nameTable = new List<string>(names);

            nameTableSize += nameTable.Sum(ArkArchive.GetStringLength);
            nameTableSize += ZoneVolumes.Sum(name => objectSizer(name));

            objectsSize = sizeof(int);

            objectsSize += Objects.Sum(go => go.Size(objectSizer));

            propertiesStart = objectsSize;

            objectsSize += Objects.Sum(go => go.PropertiesSize(propertiesSizer));

            return size + nameTableSize + objectsSize;
        }

        public override GameObject this[int id] => id > 0 && id <= Objects.Count ? Objects[id - 1] : null;

        public override GameObject this[ObjectReference reference] {
            get {
                if (reference == null || !reference.IsId) {
                    return null;
                }

                if (reference.ObjectId > 0 && reference.ObjectId <= Objects.Count) {
                    return Objects[reference.ObjectId - 1];
                }

                return null;
            }
        }

        //public override GameObject GetObject(ObjectReference reference) {
        //    if (reference == null || !reference.IsId) {
        //        return null;
        //    }

        //    if (reference.ObjectId > 0 && reference.ObjectId <= Objects.Count) {
        //        return Objects[reference.ObjectId - 1];
        //    }

        //    return null;
        //}
    }

}
