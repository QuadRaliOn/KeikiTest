#if UNITY_EDITOR
using System.Collections.Generic;
using GamePlay;
using UnityEngine;

namespace Editor {
    [ExecuteInEditMode]
    public class LevelPathEditor : MonoBehaviour {
        [Header("Database Reference")] public LevelDatabase database;

        [Header("Active Level Selection")] 
        public string targetLevelName ;
        public int strokeIndex = 0;

        [Header("Current Stroke Points")] public List<Vector2> points = new();

        public void LoadFromDatabase() {
            if (database == null) {
                Debug.LogWarning("LevelPathEditor: Database reference is missing!");
                return;
            }

            LevelData level = database.Levels.Find(l => l.name == targetLevelName);
            if (level != null) {
                if (strokeIndex >= 0 && strokeIndex < level.strokes.Count) {
                    points = new List<Vector2>(level.strokes[strokeIndex].points);
                }
                else {
                    points = new List<Vector2>();
                    Debug.LogWarning($"Stroke index {strokeIndex} is out of bounds for level '{targetLevelName}'. Loaded empty list.");
                }
            }
            else 
                Debug.LogWarning($"Level '{targetLevelName}' not found in database.");
            
        }

        public void SaveToDatabase() {
            if (database == null) {
                Debug.LogWarning("LevelPathEditor: Database reference is missing!");
                return;
            }

            LevelData level = database.Levels.Find(l => l.name == targetLevelName);
            if (level == null) {
                Debug.LogWarning($"Level '{targetLevelName}' not found in database.");
                return;
            }
            
            while (level.strokes.Count <= strokeIndex) 
                level.strokes.Add(new StrokeData { points = new List<Vector2>() });

            level.strokes[strokeIndex].points = new List<Vector2>(points);
            UnityEditor.EditorUtility.SetDirty(database);
            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log($"Saved {points.Count} points to Level '{targetLevelName}', Stroke {strokeIndex}.");
        }
    }
}
#endif