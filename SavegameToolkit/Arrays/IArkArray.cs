using System.Collections.Generic;
using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public interface IArkArray : INameContainer {

        ArkName Type { get; }

        void Init(ArkArchive archive, PropertyArray property);
        int CalculateSize(NameSizeCalculator nameSizer);

    }

    public interface IArkArray<T> : IArkArray, IList<T> { }

}
