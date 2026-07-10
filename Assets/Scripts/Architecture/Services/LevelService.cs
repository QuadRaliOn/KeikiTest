using System.Collections.Generic;
using Architecture.GlobalStateMachine;
using GamePlay;

namespace Architecture.Services {
    public class LevelService : ILevelService {
        private readonly LevelDatabase _database;

        public LevelService(LevelDatabase database) {
            _database = database;
        }

        public GameplayStatePayload ActivePayload { get; set; }

        public LevelData GetActiveLevel() {
            if (ActivePayload == null) {
                ActivePayload = new GameplayStatePayload(CategoryType.Letters, 0);
            }

            return GetLevel(ActivePayload.Category, ActivePayload.LevelIndex);
        }

        public LevelData GetLevel(CategoryType category, int levelIndex) {
            if (_database == null || _database.Levels == null) return null;

            var categoryLevels = _database.Levels.FindAll(l => l.category == category);
            if (categoryLevels.Count == 0) return null;

            int idx = levelIndex % categoryLevels.Count;
            return categoryLevels[idx];
        }

        public int GetTotalLevelsInCategory(CategoryType category) {
            if (_database == null || _database.Levels == null) return 0;

            return _database.Levels.FindAll(l => l.category == category).Count;
        }
    }
}