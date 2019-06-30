﻿using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyInt64 : PropertyBase<long> {
        public static readonly ArkName TYPE = ArkName.ConstantPlain("Int64Property");
        public override ArkName Type => TYPE;

        public PropertyInt64() { }

        public PropertyInt64(string name, long value) :
                base(ArkName.From(name), 0, value) { }

        public PropertyInt64(string name, int index, long value) :
                base(ArkName.From(name), index, value) { }

        public override void Init(ArkArchive archive, ArkName name) {
            base.Init(archive, name);

            Value = archive.ReadLong();
        }
        protected override int calculateDataSize(NameSizeCalculator nameSizer) => sizeof(long);
    }

}
