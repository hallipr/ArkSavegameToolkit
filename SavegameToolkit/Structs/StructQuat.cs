using System;

namespace SavegameToolkit.Structs {
    public class StructQuat : StructBase {

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float W { get; private set; }

        public override void Init(ArkArchive archive) {
            X = archive.ReadFloat();
            Y = archive.ReadFloat();
            Z = archive.ReadFloat();
            W = archive.ReadFloat();
        }

        public bool ShouldSerializeX() => Math.Abs(X) > 0f;
        public bool ShouldSerializeY() => Math.Abs(Y) > 0f;
        public bool ShouldSerializeZ() => Math.Abs(Z) > 0f;
        public bool ShouldSerializeW() => Math.Abs(W) > 0f;

        public override int Size(NameSizeCalculator nameSizer) => sizeof(float) * 4;
    }
}
