using System.Collections.Generic;
using UnityEngine;

namespace GamePlay {
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "KeikiTest/LevelDatabase")]
    public class LevelDatabase : ScriptableObject {
        public List<LevelData> Levels = new();
    }
}