using System.Collections.Generic;
using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public abstract class ArkArrayBase<T> : List<T>, IArkArray<T> {

        public abstract ArkName Type { get; }

        public abstract void Init(ArkArchive archive, PropertyArray property);

        public abstract int CalculateSize(NameSizeCalculator nameSizer);

        public virtual void CollectNames(NameCollector collector) { }

    }

}
