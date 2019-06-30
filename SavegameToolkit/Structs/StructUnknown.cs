using System;
using SavegameToolkit.Propertys;

namespace SavegameToolkit.Structs {
    public class StructUnknown : StructBase {
        private readonly byte[] value;

        public StructUnknown(ArkArchive archive, int dataSize) {
            value = archive.ReadBytes(dataSize);
        }

        public override void Init(ArkArchive archive) => throw new NotImplementedException();

        public override int Size(NameSizeCalculator nameSizer) => value.Length;
    }
}
