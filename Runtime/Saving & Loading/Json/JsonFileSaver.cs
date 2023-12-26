using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Cubusky.Heatgrids
{
    [Serializable]
    public class JsonFileSaver : IHeatgridSaver
    {
        [field: SerializeField] public string directory { get; set; } = "Heatgrids/LevelName/CharacterName";
        [SerializeField, Guid] private string _guid;

        public Guid guid
        {
            get => new(_guid);
            set => _guid = value.ToString();
        }

        public string filePath => Path.Combine(Application.persistentDataPath, directory, guid.ToString()) + ".json";

        void IHeatgridSaver.Save(IHeatgrid heatgrid)
        {
            var json = JsonHeatgrid.ToJson(heatgrid);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, json);
        }
    }
}
