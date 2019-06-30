﻿using System.Collections.Generic;
using SavegameToolkit.Structs;

namespace SavegameToolkit.Data {

    public class ExtraDataFoliage : IExtraData, INameContainer {
        public const string NullPlaceholder = "/NULL_KEY";

        public List<Dictionary<string, StructPropertyList>> StructMapList { get; set; }

        public int CalculateSize(NameSizeCalculator nameSizer) {
            var size = sizeof(int) * 2;

            size += sizeof(int) * StructMapList.Count;
            foreach (var structMap in StructMapList) {
                foreach (var entry in structMap) {
                    size += ArkArchive.GetStringLength(entry.Key);
                    size += entry.Value.Size(nameSizer);
                    size += sizeof(int);
                }
            }

            return size;
        }

        public void CollectNames(NameCollector collector) {
            foreach (var structMap in StructMapList) {
                foreach (var entry in structMap) {
                    entry.Value.CollectNames(collector);
                }
            }
        }

    }

}
