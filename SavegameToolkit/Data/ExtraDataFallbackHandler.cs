using System;

namespace SavegameToolkit.Data {

    public class ExtraDataFallbackHandler : IExtraDataHandler {
        public bool CanHandle(GameObject gameObject, int length) {
            return true;
        }

        public IExtraData ReadBinary(GameObject gameObject, ArkArchive archive, int length) {
            var extraData = new ExtraDataBlob();

            archive.DebugMessage($"Unknown extended data for {gameObject.ClassString} with length {length}");
            extraData.Data = archive.ReadBytes(length);
            archive.HasUnknownNames = true;

            return extraData;
        }
    }

}
