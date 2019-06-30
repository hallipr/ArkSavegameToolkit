using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public abstract class PropertyBase<T> : IProperty<T> {
        public ArkName Name { get; private set; }
        public string NameString => Name.ToString();

        public abstract ArkName Type { get; }
        public string TypeString => Type.ToString();

        public int Index { get; private set; }

        public T Value { get; protected set; }

        public int DataSize { get; private set; }

        protected PropertyBase() { }

        protected PropertyBase(ArkName name, int index, T value) {
            Name = name;
            Index = index;
            Value = value;
        }

        public virtual void Init(ArkArchive archive, ArkName name) {
            Name = name;
            DataSize = archive.ReadInt();
            Index = archive.ReadInt();
        }

        /// <summary>
        /// Calculates the value for the dataSize field
        /// </summary>
        /// <param name="nameSizer">function to calculate the size of a name in bytes in the current context</param>
        /// <returns>value of dataSize field</returns>
        protected abstract int calculateDataSize(NameSizeCalculator nameSizer);

        /// <summary>
        /// Calculates additional space required to serialize fields of this property.
        /// </summary>
        /// <param name="nameSizer"></param>
        /// <returns></returns>
        protected virtual int calculateAdditionalSize(NameSizeCalculator nameSizer) => 0;

        /// <inheritdoc />
        /// <summary>
        /// Side-effect: calling this function will change the value of the dataSize field.
        /// This makes sure that the value can be used by the write function without having to calculate it twice</summary>
        /// <param name="nameSizer"></param>
        /// <returns></returns>
        public int CalculateSize(NameSizeCalculator nameSizer) {
            // dataSize index
            var size = sizeof(int) * 2;
            DataSize = calculateDataSize(nameSizer);

            size += nameSizer(Name);
            size += nameSizer(Type);
            size += calculateAdditionalSize(nameSizer);
            size += DataSize;

            return size;
        }


        /// <summary>
        /// Determines if the dataSize cannot be calculated and thus needs to be recorded.
        /// Used when writing the JSON representation of the property
        /// <code>true</code> if dataSize needs to be recorded
        /// </summary>
        protected virtual bool isDataSizeNeeded => false;

        public virtual void CollectNames(NameCollector collector) {
            collector(Name);
            collector(Type);
        }

        public override string ToString() => $"{GetType().Name} [name={Name}{(Index != 0 ? $", index={Index}" : "")}, value={Value}{(DataSize > 0 ? $", dataSize={DataSize}" : "")}]";
    }

}
