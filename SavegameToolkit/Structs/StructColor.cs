using System;

namespace SavegameToolkit.Structs
{
    public class StructColor : StructBase
    {
        public byte B { get; private set; }
        public byte G { get; private set; }
        public byte R { get; private set; }
        public byte A { get; private set; }

        public override void Init(ArkArchive archive)
        {
            B = archive.ReadByte();
            G = archive.ReadByte();
            R = archive.ReadByte();
            A = archive.ReadByte();
        }
        public bool ShouldSerializeR() => Math.Abs(R) > 0f;
        public bool ShouldSerializeG() => Math.Abs(G) > 0f;
        public bool ShouldSerializeB() => Math.Abs(B) > 0f;
        public bool ShouldSerializeA() => Math.Abs(A) > 0f;

        public override int Size(NameSizeCalculator nameSizer) => sizeof(byte) * 4;
    }

}
