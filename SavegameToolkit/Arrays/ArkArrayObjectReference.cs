﻿using System.Linq;
using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public class ArkArrayObjectReference : ArkArrayBase<ObjectReference> {

        public static readonly ArkName TYPE = ArkName.ConstantPlain("ObjectProperty");

        //private static long serialVersionUID = 1L;

        public override void Init(ArkArchive archive, PropertyArray property) {
            var size = archive.ReadInt();

            for (var n = 0; n < size; n++) {
                Add(new ObjectReference(archive, 8)); // Fixed size?
            }
        }

        public override ArkName Type => TYPE;

        public override int CalculateSize(NameSizeCalculator nameSizer) {
            var size = sizeof(int);

            size += this.Sum(or => or.Size(nameSizer));

            return size;
        }

        public override void CollectNames(NameCollector collector) {
            ForEach(or => or.CollectNames(collector));
        }

    }

}
