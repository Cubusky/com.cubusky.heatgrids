using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cubusky.Heatgrids
{
    [Serializable]
    public class JsonDirectoryLoader : IHeatgridLoader
    {
        [field: SerializeField] public string directory { get; set; } = "Heatgrids/LevelName/CharacterName";
        [field: SerializeField] public SearchOption searchOption { get; set; } = SearchOption.TopDirectoryOnly;

        public string fileDirectory => Path.Combine(Application.persistentDataPath, directory);

        Dictionary<Vector3Int, int> IHeatgridLoader.Load(out float cellSize)
        {
            cellSize = default;

            var filePaths = Directory.GetFiles(fileDirectory, "*.json", searchOption);
            var grids = new Dictionary<Vector3Int, int>[filePaths.Length];
            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                var json = File.ReadAllText(filePath);
                grids[i] = JsonHeatgrid.FromJson(json, out float newCellSize);

                if (cellSize != newCellSize && i != 0)
                {
                    throw new ArgumentException($"The {nameof(IHeatgrid.cellSize)} of the {nameof(IHeatgrid)}'s needs to be equal in order for them to merge.", nameof(IHeatgrid.cellSize));
                }
                cellSize = newCellSize;
            }

            return HeatgridUtility.MergeGrids(grids);
        }
    }
}
