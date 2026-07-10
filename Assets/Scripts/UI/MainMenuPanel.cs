using Architecture.GlobalStateMachine;
using Architecture.GlobalStateMachine.States;
using Architecture.Services;
using GamePlay;
using UnityEngine;
using Zenject;

namespace UI.Factory {
    public class MainMenuPanel : MonoBehaviour {
        private readonly string[] _categoryTitles = { "Trace letters", "Trace numbers", "Trace shapes" };
        private readonly string[] _shapePaths = { AssetPath.Shapes.ShapeA,  AssetPath.Shapes.Shape1,  AssetPath.Shapes.ShapeO };
        private readonly CategoryType[] _categoryTypes = { CategoryType.Letters, CategoryType.Numbers, CategoryType.Shapes };
        
        [SerializeField] private RectTransform _categoriesContent;

        private ILevelService _levelService;
        private IGameStateMachine _stateMachine;
        private IAssetProvider _assetProvider;
        private IUIFactory _uiFactory;
        
        [Inject]
        private void Construct(ILevelService levelService, IGameStateMachine stateMachine, IAssetProvider assetProvider, IUIFactory uiFactory) {
            _levelService = levelService;
            _stateMachine = stateMachine;
            _assetProvider = assetProvider;
            _uiFactory = uiFactory;
        }

        private void Start() {
            InitializeCategories();
        }

        private void InitializeCategories() {
            for (int i = 0; i < _categoryTypes.Length; i++) {
                var categoryView = _uiFactory.CreateMainMenuCategory(_categoriesContent);
                Sprite iconSprite = _assetProvider.LoadAsset<Sprite>(_shapePaths[i]);
                categoryView.Configure(_categoryTypes[i], _categoryTitles[i], iconSprite, _levelService, OnLevelSelected);
            }
        }

        private void OnLevelSelected(CategoryType category, int levelIndex) {
            _levelService.ActivePayload = new GameplayStatePayload(category, levelIndex);
            _stateMachine.Enter<GameplayState>();
        }
    }
}