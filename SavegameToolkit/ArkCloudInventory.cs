using System;
using System.Collections.Generic;
using System.Linq;
using SavegameToolkit.Propertys;

namespace SavegameToolkit
{
    public sealed class ArkCloudInventory : GameObjectContainerMixin, IConversionSupport, IPropertyContainer
    {
        public int InventoryVersion { get; set; }

        public List<IProperty> Properties => inventoryData.Properties;

        private GameObject inventoryData;

        public GameObject InventoryData
        {
            get => inventoryData;
            set
            {
                if (inventoryData != null)
                {
                    var oldIndex = Objects.IndexOf(inventoryData);
                    if (oldIndex > -1)
                    {
                        Objects.RemoveAt(oldIndex);
                    }
                }

                inventoryData = value;
                if (value != null && Objects.IndexOf(value) == -1)
                {
                    Objects.Insert(0, value);
                }
            }
        }

        public void ReadBinary(ArkArchive archive, ReadingOptions options)
        {
            InventoryVersion = archive.ReadInt();

            if (InventoryVersion < 1 || InventoryVersion > 4)
            {
                throw new NotSupportedException("Unknown Cloud Inventory Version " + InventoryVersion);
            }

            var objectCount = archive.ReadInt();

            Objects.Clear();
            ObjectMap.Clear();
            for (var i = 0; i < objectCount; i++)
            {
                addObject(new GameObject(archive), options.BuildComponentTree);
            }

            for (var i = 0; i < objectCount; i++)
            {
                var obj = Objects[i];
                if (obj.ClassString == "ArkCloudInventoryData")
                {
                    inventoryData = obj;
                }

                obj.LoadProperties(archive, i < objectCount - 1 ? Objects[i + 1] : null, 0);
            }
        }

        public int CalculateSize()
        {
            var size = sizeof(int) * 2;

            var nameSizer = ArkArchive.GetNameSizer(false);

            size += Objects.Sum(o => o.Size(nameSizer));

            size += Objects.Sum(o => o.PropertiesSize(nameSizer));
            return size;
        }

    }

}
