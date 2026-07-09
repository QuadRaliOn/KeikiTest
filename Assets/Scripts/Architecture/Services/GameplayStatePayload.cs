namespace Architecture.Services
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
