using System;
using System.Collections.Generic;
using System.Linq;
using SavegameToolkit.Propertys;

namespace SavegameToolkit {

    public class ArkLocalProfile : GameObjectContainerMixin, IConversionSupport, IPropertyContainer {

        private static readonly int UNKNOWN_DATA_2_SIZE = 0xc;

        public int LocalProfileVersion { get; set; }

        private byte[] unknownData;

        private byte[] unknownData2;

        private GameObject localProfile;
        public GameObject LocalProfile {
            get => localProfile;
            set {
                if (localProfile != null) {
                    var oldIndex = Objects.IndexOf(localProfile);
                    if (oldIndex > -1) {
                        Objects.RemoveAt(oldIndex);
                    }
                }

                localProfile = value;
                if (value != null && Objects.IndexOf(value) == -1) {
                    Objects.Insert(0, value);
                }
            }
        }

        public List<IProperty> Properties => localProfile.Properties;

        public void ReadBinary(ArkArchive archive, ReadingOptions options) {
            LocalProfileVersion = archive.ReadInt();

            if (LocalProfileVersion != 1 && LocalProfileVersion != 3 && LocalProfileVersion != 4) {
                throw new NotSupportedException("Unknown Profile Version " + LocalProfileVersion);
            }

            if (LocalProfileVersion < 4) {
                var unknownDataSize = archive.ReadInt();

                unknownData = archive.ReadBytes(unknownDataSize);

                if (LocalProfileVersion == 3) {
                    unknownData2 = archive.ReadBytes(UNKNOWN_DATA_2_SIZE);
                }
            }

            var objectCount = archive.ReadInt();

            Objects.Clear();
            ObjectMap.Clear();
            for (var i = 0; i < objectCount; i++) {
                addObject(new GameObject(archive), options.BuildComponentTree);
            }

            for (var i = 0; i < objectCount; i++) {
                var gameObject = Objects[i];
                if (gameObject.ClassString == "PrimalLocalProfile") {
                    localProfile = gameObject;
                }

                gameObject.LoadProperties(archive, i < objectCount - 1 ? Objects[i + 1] : null, 0);
            }
        }

        public int CalculateSize() {
            int size;

            if (LocalProfileVersion > 3) {
                size = sizeof(int) * 2;
            } else {
                size = sizeof(int) * 3;

                if (LocalProfileVersion == 3) {
                    if (unknownData2 == null) {
                        unknownData2 = new byte[UNKNOWN_DATA_2_SIZE];
                    } else if (unknownData2.Length < UNKNOWN_DATA_2_SIZE) {
                        var temp = new byte[UNKNOWN_DATA_2_SIZE];
                        unknownData2.CopyTo(temp, 0);
                        unknownData2 = temp;
                    }
                }

                size += unknownData.Length;

                if (LocalProfileVersion == 3) {
                    size += UNKNOWN_DATA_2_SIZE;
                }
            }

            var nameSizer = ArkArchive.GetNameSizer(false);

            size += Objects.Sum(o => o.Size(nameSizer));

            size += Objects.Sum(o => o.PropertiesSize(nameSizer));

            return size;
        }
    }

}
