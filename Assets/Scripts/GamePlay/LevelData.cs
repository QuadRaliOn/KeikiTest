using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay {
    [Serializable]
    public class LevelData {
        public string name;
        public string spritePath;
        public string audioPath;
        public Color trailColor;
        public CategoryType category;
        public List<StrokeData> strokes = new();
    }

    public enum CategoryType {
        Letters = 0,
        Numbers = 1,
        Shapes = 2
    }

    [Serializable]
    public class StrokeData {
        public List<Vector2> points = new();
    }
}