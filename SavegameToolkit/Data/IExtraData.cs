using System;

namespace SavegameToolkit.Data {

    public interface IExtraData {
        int CalculateSize(NameSizeCalculator nameSizer);
    }
}
