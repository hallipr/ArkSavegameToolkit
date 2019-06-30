namespace SavegameToolkit.Data {

    public class ExtraDataCharacterHandler : IExtraDataHandler {
        private static readonly ExtraDataCharacter instance = new ExtraDataCharacter();

        public bool CanHandle(GameObject gameObject, int length) {
            if ((gameObject.ClassString.Contains("_Character_") || gameObject.ClassString.StartsWith("PlayerPawnTest_")) && length == 8)
            {
                return true;
            }

            if ((gameObject.ClassString == "Pteroteuthis_Char_BP_C" || gameObject.ClassString == "Pteroteuthis_Char_BP_Surface_C") && length == 8) {
                return true;
            }

            return false;
        }


        public IExtraData ReadBinary(GameObject gameObject, ArkArchive archive, int length) {
            var shouldBeZero = archive.ReadInt();
            if (shouldBeZero != 0) {
                throw new UnexpectedDataException($"Expected int after properties to be 0 but found {shouldBeZero} at {archive.Position - 4:X4}");
            }

            var shouldBeOne = archive.ReadInt();
            if (shouldBeOne != 1) {
                throw new UnexpectedDataException($"Expected int after properties to be 1 but found {shouldBeOne} at {archive.Position - 4:X4}");
            }

            return instance;
        }

    }

}
