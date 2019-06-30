using System;

namespace SavegameToolkit.Structs {

    public class StructVector : StructBase {

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        public override void Init(ArkArchive archive) {
            X = archive.ReadFloat();
            Y = archive.ReadFloat();
            Z = archive.ReadFloat();
        }

        public bool ShouldSerializeX() => Math.Abs(X) > 0f;
        public bool ShouldSerializeY() => Math.Abs(Y) > 0f;
        public bool ShouldSerializeZ() => Math.Abs(Z) > 0f;

        public override int Size(NameSizeCalculator nameSizer) => sizeof(float) * 3;
    }

}
