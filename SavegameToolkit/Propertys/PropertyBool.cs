using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyBool : PropertyBase<bool> {
        public static readonly ArkName TYPE = ArkName.ConstantPlain("BoolProperty");
        public override ArkName Type => TYPE;

        public PropertyBool() { }

        public PropertyBool(string name, bool value) : base(ArkName.From(name), 0, value) { }

        public PropertyBool(string name, int index, bool value) : base(ArkName.From(name), index, value) { }

        public override void Init(ArkArchive archive, ArkName name) {
            base.Init(archive, name);
            Value = archive.ReadByte() != 0;
        }

        // Special case: value of PropertyBool is not considered "data"
        protected override int calculateAdditionalSize(NameSizeCalculator nameSizer) => 1;

        protected override int calculateDataSize(NameSizeCalculator nameSizer) => 0;
    }

}
