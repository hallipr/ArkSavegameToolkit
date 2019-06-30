using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public class ArkArrayUnknown : ArkArrayBase<byte> {

        public override ArkName Type { get; }

        public ArkArrayUnknown(ArkArchive archive, int size, ArkName type) {
            AddRange(archive.ReadBytes(size));
            Type = type;
        }

        public override void Init(ArkArchive archive, PropertyArray property) {
            throw new System.NotImplementedException();
        }

        public override int CalculateSize(NameSizeCalculator nameSizer) {
            return Count;
        }

    }

}
