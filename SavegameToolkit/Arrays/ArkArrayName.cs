using System.Linq;
using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {
    public class ArkArrayName: ArkArrayBase<ArkName> {

        public static readonly ArkName TYPE = ArkName.ConstantPlain("NameProperty");

        //private static long serialVersionUID = 1L;

        public override void Init(ArkArchive archive, PropertyArray property) {
            var size = archive.ReadInt();

            for (var n = 0; n < size; n++) {
                Add(archive.ReadName());
            }
        }

        public override ArkName Type => TYPE;

        public override int CalculateSize(NameSizeCalculator nameSizer) {
            var size = sizeof(int);

            size += this.Sum(name => nameSizer(name));

            return size;
        }

        public override void CollectNames(NameCollector collector) {
            foreach (var arkName in this) {
                collector(arkName);
            }
        }

    }
}
