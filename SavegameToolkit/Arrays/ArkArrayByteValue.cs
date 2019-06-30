using System.Linq;
using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public class ArkArrayByteValue : ArkArrayBase<ArkByteValue> {

        //private static long serialVersionUID = 1L;

        public override void Init(ArkArchive archive, PropertyArray property) {
            var size = archive.ReadInt();

            for (var n = 0; n < size; n++) {
                Add(new ArkByteValue(archive.ReadName()));
            }
        }

        public override ArkName Type => ArkArrayByteHandler.TYPE;

        public override int CalculateSize(NameSizeCalculator nameSizer) {
            return sizeof(int) + this.Sum(abv => nameSizer(abv.NameValue));
        }

        public override void CollectNames(NameCollector collector) {
            foreach (var bv in this) {
                collector(bv.NameValue);
            }
        }

    }

}
