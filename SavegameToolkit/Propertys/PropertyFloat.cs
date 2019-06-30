﻿using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyFloat : PropertyBase<float> {
        public static readonly ArkName TYPE = ArkName.ConstantPlain("FloatProperty");
        public override ArkName Type => TYPE;

        public PropertyFloat() { }

        public PropertyFloat(string name, float value) : base(ArkName.From(name), 0, value) { }

        public PropertyFloat(string name, int index, float value) : base(ArkName.From(name), index, value) { }

        public override void Init(ArkArchive archive, ArkName name) {
            base.Init(archive, name);
            Value = archive.ReadFloat();
        }

        protected override int calculateDataSize(NameSizeCalculator nameSizer) => sizeof(float);
    }

}
