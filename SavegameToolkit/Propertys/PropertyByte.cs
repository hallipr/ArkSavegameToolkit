using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyByte : PropertyBase<ArkByteValue> {
        public static readonly ArkName TYPE = ArkName.ConstantPlain("ByteProperty");

        public override ArkName Type => TYPE;

        public ArkName EnumType { get; private set; }

        public PropertyByte() { }

        public PropertyByte(string name, ArkName value, ArkName enumType) : base(ArkName.From(name), 0, new ArkByteValue(value)) {
            EnumType = enumType;
        }

        public PropertyByte(string name, int index, ArkName value, ArkName enumType) : base(ArkName.From(name), index, new ArkByteValue(value)) {
            EnumType = enumType;
        }

        public PropertyByte(string name, byte value) : base(ArkName.From(name), 0, new ArkByteValue(value)) {
            EnumType = ArkName.NameNone;
        }

        public PropertyByte(string name, int index, byte value) : base(ArkName.From(name), index, new ArkByteValue(value)) {
            EnumType = ArkName.NameNone;
        }

        public override void Init(ArkArchive archive, ArkName name) {
            base.Init(archive, name);
            EnumType = archive.ReadName();
            Value = new ArkByteValue(archive, EnumType != ArkName.NameNone);
        }

        protected override int calculateAdditionalSize(NameSizeCalculator nameSizer) => nameSizer(EnumType);

        protected override int calculateDataSize(NameSizeCalculator nameSizer) => Value.getSize(nameSizer);

        public override void CollectNames(NameCollector collector) {
            base.CollectNames(collector);
            collector(EnumType);
            Value.CollectNames(collector);
        }
    }

}
