using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyDouble : PropertyBase<double> {
        public static readonly ArkName TYPE = ArkName.ConstantPlain("DoubleProperty");
        public override ArkName Type => TYPE;

        public PropertyDouble() { }

        public PropertyDouble(string name, double value) : base(ArkName.From(name), 0, value) { }

        public PropertyDouble(string name, int index, double value) : base(ArkName.From(name), index, value) { }

        public override void Init(ArkArchive archive, ArkName name) {
            base.Init(archive, name);
            Value = archive.ReadDouble();
        }

        protected override int calculateDataSize(NameSizeCalculator nameSizer) => sizeof(double);
    }

}
