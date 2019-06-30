using System.Collections.Generic;
using SavegameToolkit.Types;

namespace SavegameToolkit.Propertys {

    public static class PropertyRegistry {

        private static readonly Dictionary<ArkName, PropertyConstructor> typeMap = new Dictionary<ArkName, PropertyConstructor>();

        private static void addProperty(ArkName name, PropertyConstructor.Binary binaryConstructor) {
            typeMap.Add(name, new PropertyConstructor(binaryConstructor));
        }

        private static PropertyConstructor.Binary binaryConstructorFunction<T>() where T : IProperty, new() {
            return (archive, name) => {
                var t = new T();
                t.Init(archive, name);
                return t;
            };
        }

        static PropertyRegistry() {
            addProperty(PropertyInt8.TYPE, binaryConstructorFunction<PropertyInt8>());
            addProperty(PropertyByte.TYPE, binaryConstructorFunction<PropertyByte>());
            addProperty(PropertyInt16.TYPE, binaryConstructorFunction<PropertyInt16>());
            addProperty(PropertyUInt16.TYPE, binaryConstructorFunction<PropertyUInt16>());
            addProperty(PropertyInt.TYPE, binaryConstructorFunction<PropertyInt>());
            addProperty(PropertyUInt32.TYPE, binaryConstructorFunction<PropertyUInt32>());
            addProperty(PropertyInt64.TYPE, binaryConstructorFunction<PropertyInt64>());
            addProperty(PropertyUInt64.TYPE, binaryConstructorFunction<PropertyUInt64>());
            addProperty(PropertyFloat.TYPE, binaryConstructorFunction<PropertyFloat>());
            addProperty(PropertyDouble.TYPE, binaryConstructorFunction<PropertyDouble>());
            addProperty(PropertyBool.TYPE, binaryConstructorFunction<PropertyBool>());
            addProperty(PropertyString.TYPE, binaryConstructorFunction<PropertyString>());
            addProperty(PropertyName.TYPE, binaryConstructorFunction<PropertyName>());
            addProperty(PropertyObject.TYPE, binaryConstructorFunction<PropertyObject>());
            addProperty(PropertyArray.TYPE, binaryConstructorFunction<PropertyArray>());
            addProperty(PropertyStruct.TYPE, binaryConstructorFunction<PropertyStruct>());
        }

        public static IProperty ReadBinary(ArkArchive archive) {
            var name = archive.ReadName();

            if (name == null || string.IsNullOrEmpty(name.ToString())) {
                archive.HasUnknownData = true;
                throw new UnreadablePropertyException(
                        $"Property name is {(name == null ? "null" : "empty")}, indicating a corrupt file. Ignoring remaining properties.");
            }

            if (name == ArkName.NameNone) {
                return null;
            }

            var type = archive.ReadName();

            if (type != null && typeMap.TryGetValue(type, out var constructor)) {
                return constructor.BinaryConstructor(archive, name);
            }

            archive.DebugMessage($"Unknown property type {name}");
            archive.HasUnknownNames = true;
            return new PropertyUnknown(archive, name);
        }

        private class PropertyConstructor {

            public delegate IProperty Binary(ArkArchive archive, ArkName name);

            public Binary BinaryConstructor { get; }

            public PropertyConstructor(Binary binaryConstructor) {
                BinaryConstructor = binaryConstructor;
            }
        }

    }

}
