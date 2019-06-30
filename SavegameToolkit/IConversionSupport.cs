
namespace SavegameToolkit {

    public interface IConversionSupport {
        void ReadBinary(ArkArchive archive, ReadingOptions options);

        int CalculateSize();
    }

}
