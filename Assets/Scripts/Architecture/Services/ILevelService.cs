using Architecture.GlobalStateMachine;
using GamePlay;

namespace Architecture.Services {
    public interface ILevelService {
        GameplayStatePayload ActivePayload { get; set; }
        LevelData GetActiveLevel();
        LevelData GetLevel(CategoryType category, int levelIndex);
        int GetTotalLevelsInCategory(CategoryType category);
    }
}
