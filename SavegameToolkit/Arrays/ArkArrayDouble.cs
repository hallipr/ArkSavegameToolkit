﻿using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public class ArkArrayDouble : ArkArrayBase<double> {

        public static readonly ArkName TYPE = ArkName.ConstantPlain("DoubleProperty");

        //private static long serialVersionUID = 1L;

        public override void Init(ArkArchive archive, PropertyArray property) {
            var size = archive.ReadInt();

            for (var n = 0; n < size; n++) {
                Add(archive.ReadDouble());
            }
        }

        public override ArkName Type => TYPE;

        public override int CalculateSize(NameSizeCalculator nameSizer) {
            return sizeof(int) + (Count * sizeof(double));
        }

    }

}
