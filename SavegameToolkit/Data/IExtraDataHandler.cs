
namespace SavegameToolkit.Data {

    public interface IExtraDataHandler {

        bool CanHandle(GameObject gameObject, int length);

        IExtraData ReadBinary(GameObject gameObject, ArkArchive archive, int length);

    }

}
