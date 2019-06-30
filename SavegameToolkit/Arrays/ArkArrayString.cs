﻿using System.Linq;
using SavegameToolkit.Propertys;
using SavegameToolkit.Types;

namespace SavegameToolkit.Arrays {

    public class ArkArrayString : ArkArrayBase<string> {

        public static readonly ArkName TYPE = ArkName.ConstantPlain("StrProperty");

        //private static long serialVersionUID = 1L;

        public override void Init(ArkArchive archive, PropertyArray property) {
            var size = archive.ReadInt();

            for (var n = 0; n < size; n++) {
                Add(archive.ReadString());
            }
        }

        public override ArkName Type => TYPE;

        public override int CalculateSize(NameSizeCalculator nameSizer) {
            var size = sizeof(int);

            size += this.Sum(ArkArchive.GetStringLength);

            return size;
        }

    }

}
