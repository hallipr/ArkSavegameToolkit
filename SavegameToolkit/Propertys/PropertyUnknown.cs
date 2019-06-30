using System;
using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public class PropertyUnknown : PropertyBase<byte[]> {

        public override ArkName Type { get; }

        public PropertyUnknown(ArkArchive archive, ArkName name) : base(name, 0, null) {
            base.Init(archive, name);
            Type = name;
            Value = archive.ReadBytes(DataSize);
        }

        protected override int calculateDataSize(NameSizeCalculator nameSizer) => Value.Length;
    }
}
