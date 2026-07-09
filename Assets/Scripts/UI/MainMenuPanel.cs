using Architecture.GlobalStateMachine;
using Architecture.GlobalStateMachine.States;
using Architecture.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Factory
{
    public class MainMenuPanel : MonoBehaviour
    {
        private LevelService _levelService;
        private IGameStateMachine _stateMachine;
        private IAssetProvider _assetProvider;

        [Inject]
        private void Construct(LevelService levelService, IGameStateMachine stateMachine, IAssetProvider assetProvider)
        {
            _levelService = levelService;
            _stateMachine = stateMachine;
            _assetProvider = assetProvider;
        }

        private void Start()
        {
            InitializeCategories();
        }

        private void InitializeCategories()
        {
            Transform content = transform.Find("Levels/VerticalScroll/Viewport/Content");
            if (content == null)
            {
                Debug.LogError("MainMenuPanel: Levels content path not found!");
                return;
            }

            // Categories in hierarchy: Category, Category (1), Category (2)
            string[] categoryTitles = { "Letters", "Numbers", "Shapes" };
            CategoryType[] categoryTypes = { CategoryType.Letters, CategoryType.Numbers, CategoryType.Shapes };
            string[] shapePaths = { "Graphics/shape_A", "Graphics/shape_1", "Graphics/shape_circle" };

            int categoryCount = Mathf.Min(content.childCount, 3);
            for (int i = 0; i < categoryCount; i++)
            {
                Transform categoryTransform = content.GetChild(i);
                
                // 1. Set Title Label
                TMP_Text label = categoryTransform.Find("Label")?.GetComponent<TMP_Text>();
                if (label != null)
                {
                    label.text = categoryTitles[i];
                }

                // 2. Set Level buttons
                Transform levelContainer = categoryTransform.Find("HorizontalScroll/Viewport/Content");
                if (levelContainer == null)
                {
                    Debug.LogWarning($"MainMenuPanel: Level container not found for category {categoryTitles[i]}");
                    continue;
                }

                CategoryType catType = categoryTypes[i];
                string shapePath = shapePaths[i];
                Sprite iconSprite = _assetProvider.LoadAsset<Sprite>(shapePath);

                int totalLevels = _levelService.GetTotalLevelsInCategory(catType);
                if (levelContainer.childCount == 0 && totalLevels > 0)
                {
                    Debug.LogWarning($"MainMenuPanel: No level button templates found in container for category {categoryTitles[i]}");
                    continue;
                }

                Transform buttonTemplate = levelContainer.childCount > 0 ? levelContainer.GetChild(0) : null;

                // Instantiate missing buttons
                for (int j = levelContainer.childCount; j < totalLevels; j++)
                {
                    Instantiate(buttonTemplate, levelContainer);
                }

                // Configure buttons, hiding any extra templates
                for (int j = 0; j < levelContainer.childCount; j++)
                {
                    Transform levelButton = levelContainer.GetChild(j);
                    if (j >= totalLevels)
                    {
                        levelButton.gameObject.SetActive(false);
                        continue;
                    }

                    levelButton.gameObject.SetActive(true);
                    Button btn = levelButton.GetComponent<Button>();
                    
                    if (btn != null)
                    {
                        // Clean previous listeners
                        btn.onClick.RemoveAllListeners();
                        
                        int levelIdx = j;
                        btn.onClick.AddListener(() => OnLevelSelected(catType, levelIdx));
                    }

                    // Set icon
                    Image iconImg = levelButton.Find("Icon")?.GetComponent<Image>();
                    if (iconImg != null)
                    {
                        iconImg.sprite = iconSprite;
                        
                        LevelData levelData = _levelService.GetLevel(catType, j);
                        iconImg.color = levelData != null ? levelData.trailColor : Color.white;
                        
                        // Maintain aspect ratio nicely
                        iconImg.preserveAspect = true;
                    }
                }
            }
        }

        private void OnLevelSelected(CategoryType category, int levelIndex)
        {
            _levelService.ActivePayload = new GameplayStatePayload(category, levelIndex);
            _stateMachine.Enter<GameplayState>();
        }
    }
}