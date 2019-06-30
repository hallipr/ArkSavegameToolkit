using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit {

    public sealed class ArkSavFile : GameObjectContainerMixin, IConversionSupport, IPropertyContainer {

        private string className;

        public List<IProperty> Properties { get; } = new List<IProperty>();

        public void ReadBinary(ArkArchive archive, ReadingOptions options) {
            className = archive.ReadString();

            Properties.Clear();
            try {
                var property = PropertyRegistry.ReadBinary(archive);

                while (property != null) {
                    Properties.Add(property);
                    property = PropertyRegistry.ReadBinary(archive);
                }
            } catch (UnreadablePropertyException upe) {
                Debug.WriteLine(upe.Message);
                Debug.WriteLine(upe.StackTrace);
            }

            // TODO: verify 0 int at end
        }

        public int CalculateSize() {
            var size = sizeof(int) + ArkArchive.GetStringLength(className);

            var nameSizer = ArkArchive.GetNameSizer(false);

            size += nameSizer(ArkName.NameNone);

            size += Properties.Sum(p => p.CalculateSize(nameSizer));
            return size;
        }
    }

}
