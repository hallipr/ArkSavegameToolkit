using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyInt16 : PropertyBase<short> {
        public static readonly ArkName TYPE = ArkName.ConstantPlain("Int16Property");
        public override ArkName Type => TYPE;

        public PropertyInt16() { }

        public PropertyInt16(string name, short value) : base(ArkName.From(name), 0, value) { }

        public PropertyInt16(string name, int index, short value) : base(ArkName.From(name), index, value) { }

        public override void Init(ArkArchive archive, ArkName name) {
            base.Init(archive, name);
            Value = archive.ReadShort();
        }

        protected override int calculateDataSize(NameSizeCalculator nameSizer) => sizeof(short);
    }

}
