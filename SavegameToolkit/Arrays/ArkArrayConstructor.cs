using SavegameToolkit.Propertys;

namespace SavegameToolkit.Arrays {

    public class ArkArrayConstructor {

        public delegate IArkArray Binary(ArkArchive archive, PropertyArray property);
        public Binary BinaryConstructor { get; }

        public ArkArrayConstructor(Binary binaryConstructor)
        {
            BinaryConstructor = binaryConstructor;
        }
    }

}