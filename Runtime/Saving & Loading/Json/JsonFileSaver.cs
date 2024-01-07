using System;
using System.IO;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    public class JsonFileSaver : IHeatgridSaver
    {
        [field: SerializeField] public string directory { get; set; } = "Heatgrids/LevelName/CharacterName";

        public string directoryPath => Path.Combine(Application.persistentDataPath, directory);

        void IHeatgridSaver.Save(IHeatgrid heatgrid)
        {
            Directory.CreateDirectory(directoryPath);

            var json = JsonHeatgrid.ToJson(heatgrid);
            var filePath = Path.Combine(directoryPath, Guid.NewGuid().ToString() + ".json");
            File.WriteAllText(filePath, json);
        }
    }
}
