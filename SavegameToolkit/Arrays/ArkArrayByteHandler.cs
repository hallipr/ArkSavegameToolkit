using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public class ArkArrayByteHandler {

        public static readonly ArkName TYPE = ArkName.ConstantPlain("ByteProperty");

        public static IArkArray create(ArkArchive archive, PropertyArray property) {
            var size = archive.ReadInt();

            if (property.DataSize < size + 4) {
                throw new UnreadablePropertyException("Found Array of ByteProperty with unexpected size.");
            }

            archive.Position -= 4;

            if (property.DataSize > size + 4) {
                var arkArrayByteValue = new ArkArrayByteValue();
                arkArrayByteValue.Init(archive, property);
                return arkArrayByteValue;
            }

            var arkArrayUInt8 = new ArkArrayUInt8();
            arkArrayUInt8.Init(archive, property);
            return arkArrayUInt8;
        }

    }

}
