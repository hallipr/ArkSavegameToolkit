using System;

namespace SavegameToolkit.Structs
{
    public class StructLinearColor : StructBase
    {
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }
        public float A { get; private set; }

        public override void Init(ArkArchive archive)
        {
            R = archive.ReadFloat();
            G = archive.ReadFloat();
            B = archive.ReadFloat();
            A = archive.ReadFloat();
        }

        public bool ShouldSerializeR() => Math.Abs(R) > 0f;
        public bool ShouldSerializeG() => Math.Abs(G) > 0f;
        public bool ShouldSerializeB() => Math.Abs(B) > 0f;
        public bool ShouldSerializeA() => Math.Abs(A) > 0f;

        public override int Size(NameSizeCalculator nameSizer) => sizeof(float) * 4;
    }
}
