using System.Collections.Generic;
using SavegameToolkit.Propertys;
using SavegameToolkit.Structs;
using SavegameToolkit.Types;

namespace SavegameToolkit.Data {

    public class ExtraDataFoliageHandler : IExtraDataHandler {
        private static readonly ArkName className = ArkName.ConstantPlain("InstancedFoliageActor");

        public bool CanHandle(GameObject gameObject, int length) => gameObject.ClassName == className;

        public IExtraData ReadBinary(GameObject gameObject, ArkArchive archive, int length) {
            var shouldBeZero = archive.ReadInt();
            if (shouldBeZero != 0) {
                throw new UnexpectedDataException($"Expected int after properties to be 0 but found {shouldBeZero} at {archive.Position - 4:X4}");
            }

            var structMapCount = archive.ReadInt();

            var structMapList = new List<Dictionary<string, StructPropertyList>>(structMapCount);

            try {
                for (var structMapIndex = 0; structMapIndex < structMapCount; structMapIndex++) {
                    var structCount = archive.ReadInt();
                    var structMap = new Dictionary<string, StructPropertyList>();

                    for (var structIndex = 0; structIndex < structCount; structIndex++) {
                        var structName = archive.ReadString();
                        var properties = new StructPropertyList(archive);

                        var shouldBeZero2 = archive.ReadInt();
                        if (shouldBeZero2 != 0) {
                            throw new UnexpectedDataException($"Expected int after properties to be 0 but found {shouldBeZero2} at {archive.Position - 4:X4}");
                        }

                        structMap[structName] = properties;
                    }

                    structMapList.Add(structMap);
                }
            } catch (UnreadablePropertyException upe) {
                throw new UnexpectedDataException(upe);
            }

            var extraDataFoliage = new ExtraDataFoliage {
                    StructMapList = structMapList
            };

            return extraDataFoliage;
        }

    }

}
