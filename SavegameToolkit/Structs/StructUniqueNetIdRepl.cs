
namespace SavegameToolkit.Structs {

    public class StructUniqueNetIdRepl : StructBase {

        public int Unk { get; private set; }

        public string NetId { get; private set; }

        public override void Init(ArkArchive archive) {
            Unk = archive.ReadInt();
            NetId = archive.ReadString();
        }





        public override int Size(NameSizeCalculator nameSizer) => sizeof(int) + ArkArchive.GetStringLength(NetId);
    }

}
