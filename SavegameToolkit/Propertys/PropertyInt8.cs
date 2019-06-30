using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyInt8 : PropertyBase<sbyte> {
        public static readonly ArkName TYPE = ArkName.ConstantPlain("Int8Property");
        public override ArkName Type => TYPE;

        public PropertyInt8() { }

        public PropertyInt8(string name, sbyte value) : base(ArkName.From(name), 0, value) { }

        public PropertyInt8(string name, int index, sbyte value) : base(ArkName.From(name), index, value) { }

        public override void Init(ArkArchive archive, ArkName name) {
            base.Init(archive, name);
            Value = archive.ReadSByte();
        }

        protected override int calculateDataSize(NameSizeCalculator nameSizer) => sizeof(byte);
    }

}
