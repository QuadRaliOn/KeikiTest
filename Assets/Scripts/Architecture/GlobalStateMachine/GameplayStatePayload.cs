using Architecture.Services;
using GamePlay;

namespace Architecture.GlobalStateMachine
{
    public class GameplayStatePayload
    {
        public CategoryType Category { get; }
        public int LevelIndex { get; }

        public GameplayStatePayload(CategoryType category, int levelIndex)
        {
            Category = category;
            LevelIndex = levelIndex;
        }
    }
}
