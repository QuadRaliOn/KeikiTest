using System.Collections.Generic;
using UnityEngine;

namespace Architecture.Services {
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "KeikiTest/LevelDatabase")]
    public class LevelDatabase : ScriptableObject {
        public List<LevelData> Levels = new();
    }
}