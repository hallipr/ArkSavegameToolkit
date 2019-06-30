using System.Linq;

namespace SavegameToolkit.Types {

    public class EmbeddedData {

        public EmbeddedData() { }

        public EmbeddedData(ArkArchive archive) {
            readBinary(archive);
        }

        public string Path { get; set; }

        public byte[][][] Data { get; set; }

        public int Size {
            get {
                var size = ArkArchive.GetStringLength(Path) + 4;

                if (Data == null)
                {
                    return size;
                }

                size += Data.Length * 4;
                foreach (var partData in Data) {
                    if (partData != null) {
                        size += partData.Length * 4;
                        size += partData.Sum(blobData => blobData.Length);
                    }
                }

                return size;
            }
        }

        private void readBinary(ArkArchive archive) {
            Path = archive.ReadString();

            var partCount = archive.ReadInt();

            Data = new byte[partCount][][];
            for (var part = 0; part < partCount; part++) {
                var blobCount = archive.ReadInt();
                var partData = new byte[blobCount][];

                for (var blob = 0; blob < blobCount; blob++) {
                    var blobSize = archive.ReadInt() * 4; // Array of 32 bit values
                    partData[blob] = archive.ReadBytes(blobSize);
                }

                Data[part] = partData;
            }
        }

        public static void Skip(ArkArchive archive) {
            archive.SkipString();

            var partCount = archive.ReadInt();
            for (var part = 0; part < partCount; part++) {
                var blobCount = archive.ReadInt();
                for (var blob = 0; blob < blobCount; blob++) {
                    var blobSize = archive.ReadInt() * 4;
                    archive.Position += blobSize;
                }
            }
        }

    }

}
