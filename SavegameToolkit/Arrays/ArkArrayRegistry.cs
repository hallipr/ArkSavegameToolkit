using System.Collections.Generic;
using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public static class ArkArrayRegistry {

        private static readonly Dictionary<ArkName, ArkArrayConstructor> types = new Dictionary<ArkName, ArkArrayConstructor>();

        private static void addType(ArkName name, ArkArrayConstructor.Binary binaryConstructor) {
            types.Add(name, new ArkArrayConstructor(binaryConstructor));
        }

        private static ArkArrayConstructor.Binary binaryConstructorFunction<T>() where T : IArkArray, new() {
            return (archive, property) => {
                var t = new T();
                t.Init(archive, property);
                return t;
            };
        }

        static ArkArrayRegistry() {
            addType(ArkArrayInt8.TYPE, binaryConstructorFunction<ArkArrayInt8>());
            addType(ArkArrayByteHandler.TYPE, ArkArrayByteHandler.create);
            addType(ArkArrayInt16.TYPE, binaryConstructorFunction<ArkArrayInt16>());
            addType(ArkArrayUInt16.TYPE, binaryConstructorFunction<ArkArrayUInt16>());
            addType(ArkArrayInt.TYPE, binaryConstructorFunction<ArkArrayInt>());
            addType(ArkArrayUInt32.TYPE, binaryConstructorFunction<ArkArrayUInt32>());
            addType(ArkArrayInt64.TYPE, binaryConstructorFunction<ArkArrayInt64>());
            addType(ArkArrayUInt64.TYPE, binaryConstructorFunction<ArkArrayUInt64>());
            addType(ArkArrayFloat.TYPE, binaryConstructorFunction<ArkArrayFloat>());
            addType(ArkArrayDouble.TYPE, binaryConstructorFunction<ArkArrayDouble>());
            addType(ArkArrayBool.TYPE, binaryConstructorFunction<ArkArrayBool>());
            addType(ArkArrayString.TYPE, binaryConstructorFunction<ArkArrayString>());
            addType(ArkArrayName.TYPE, binaryConstructorFunction<ArkArrayName>());
            addType(ArkArrayObjectReference.TYPE, binaryConstructorFunction<ArkArrayObjectReference>());
            addType(ArkArrayStruct.TYPE, binaryConstructorFunction<ArkArrayStruct>());
        }

        public static IArkArray ReadBinary(ArkArchive archive, ArkName arrayType, PropertyArray property) {
            if (arrayType != null && types.TryGetValue(arrayType, out var constructor)) {
                return constructor.BinaryConstructor(archive, property);
            }

            throw new UnreadablePropertyException($"Unknown Array Type {arrayType} at {archive.Position:X4}");
        }
    }

}
