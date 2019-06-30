using System;
using System.Collections.Generic;
using System.Linq;
using SavegameToolkit.Propertys;

namespace SavegameToolkit
{

    public class ArkProfile : GameObjectContainerMixin, IConversionSupport, IPropertyContainer
    { 
        public int ProfileVersion { get; set; }

        public List<IProperty> Properties => profile.Properties;

        private GameObject profile;

        public GameObject Profile
        {
            get => profile;
            set
            {
                if (profile != null)
                {
                    var oldIndex = Objects.IndexOf(profile);
                    if (oldIndex > -1)
                    {
                        Objects.RemoveAt(oldIndex);
                    }
                }

                profile = value;
                if (value != null && Objects.IndexOf(value) == -1)
                {
                    Objects.Insert(0, value);
                }
            }
        }

        public void ReadBinary(ArkArchive archive, ReadingOptions options)
        {
            ProfileVersion = archive.ReadInt();

            if (ProfileVersion != 1)
            {
                throw new NotSupportedException("Unknown Profile Version " + ProfileVersion);
            }

            var profilesCount = archive.ReadInt();

            Objects.Clear();
            ObjectMap.Clear();
            for (var i = 0; i < profilesCount; i++)
            {
                addObject(new GameObject(archive), options.BuildComponentTree);
            }

            for (var i = 0; i < profilesCount; i++)
            {
                var gameObject = Objects[i];
                if (gameObject.ClassString == "PrimalPlayerData" || gameObject.ClassString == "PrimalPlayerDataBP_C")
                {
                    profile = gameObject;
                }

                gameObject.LoadProperties(archive, i < profilesCount - 1 ? Objects[i + 1] : null, 0);
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
