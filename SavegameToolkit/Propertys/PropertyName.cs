﻿using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyName : PropertyBase<ArkName> {
        public static readonly ArkName TYPE = ArkName.ConstantPlain("NameProperty");
        public override ArkName Type => TYPE;

        public PropertyName() { }

        public PropertyName(string name, ArkName value) : base(ArkName.From(name), 0, value) { }

        public PropertyName(string name, int index, ArkName value) : base(ArkName.From(name), index, value) { }

        public override void Init(ArkArchive archive, ArkName name) {
            base.Init(archive, name);
            Value = archive.ReadName();
        }
        protected override int calculateDataSize(NameSizeCalculator nameSizer) => nameSizer(Value);

        public override void CollectNames(NameCollector collector)
        {
            base.CollectNames(collector);
            collector(Value);
        }
    }

}
