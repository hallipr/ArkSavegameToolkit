using System.IO;
using System.Linq;
using SavegameToolkit.Arrays;

namespace SavegameToolkit {

    public sealed class ArkContainer : GameObjectContainerMixin, IConversionSupport {
        private int propertiesBlockOffset;

        public ArkContainer(ArkArrayUInt8 source) {
            var buffer = new MemoryStream(source.ToArray());

            var archive = new ArkArchive(buffer);
            ReadBinary(archive, new ReadingOptions());
        }

        public ArkContainer(ArkArrayInt8 source) {
            var buffer = new MemoryStream(source.ToArray());

            var archive = new ArkArchive(buffer);
            ReadBinary(archive, new ReadingOptions());
        }

        public void ReadBinary(ArkArchive archive, ReadingOptions options) {
            var objectCount = archive.ReadInt();

            Objects.Clear();
            ObjectMap.Clear();
            for (var i = 0; i < objectCount; i++) {
                addObject(new GameObject(archive), options.BuildComponentTree);
            }

            for (var i = 0; i < objectCount; i++) {
                Objects[i].LoadProperties(archive, i < objectCount - 1 ? Objects[i + 1] : null, 0);
            }
        }

        public int CalculateSize() {
            var size = sizeof(int);
            var nameSizer = ArkArchive.GetNameSizer(false);

            size += Objects.Sum(o => o.Size(nameSizer));

            propertiesBlockOffset = size;
            size += Objects.Sum(o => o.PropertiesSize(nameSizer));
            return size;
        }

    }

}
