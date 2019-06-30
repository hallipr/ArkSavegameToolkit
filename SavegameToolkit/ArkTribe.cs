using System;
using System.Collections.Generic;
using System.Linq;
using SavegameToolkit.Propertys;

namespace SavegameToolkit {

    public class ArkTribe : GameObjectContainerMixin, IConversionSupport, IPropertyContainer {

        public int TribeVersion { get; set; }

        public GameObject Tribe { get; set; }

        public List<IProperty> Properties => Tribe.Properties;

        private int propertiesBlockOffset;

        public void ReadBinary(ArkArchive archive, ReadingOptions options) {
            TribeVersion = archive.ReadInt();

            if (TribeVersion != 1) {
                throw new NotSupportedException("Unknown Tribe Version " + TribeVersion);
            }

            var tribesCount = archive.ReadInt();

            Objects.Clear();
            ObjectMap.Clear();
            for (var i = 0; i < tribesCount; i++) {
                addObject(new GameObject(archive), options.BuildComponentTree);
            }

            for (var i = 0; i < tribesCount; i++) {
                var gameObject = Objects[i];
                if (gameObject.ClassString == "PrimalTribeData") {
                    Tribe = gameObject;
                }

                gameObject.LoadProperties(archive, i < tribesCount - 1 ? Objects[i + 1] : null, 0);
            }
        }

        public int CalculateSize() {
            var size = sizeof(int) * 2;

            var nameSizer = ArkArchive.GetNameSizer(false);

            size += Objects.Sum(o => o.Size(nameSizer));

            propertiesBlockOffset = size;

            size += Objects.Sum(o => o.PropertiesSize(nameSizer));
            return size;
        }
    }

}
