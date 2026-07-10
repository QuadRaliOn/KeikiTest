using TMPro;
using UnityEngine;
using GamePlay;
using Architecture.Services;
using Zenject;

namespace UI.Factory {
    public class MainMenuCategoryView : MonoBehaviour {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _levelContainer;

        private IUIFactory _uiFactory;

        [Inject]
        private void Construct(IUIFactory uiFactory) {
            _uiFactory = uiFactory;
        }

        public void Configure(CategoryType categoryType, string title, Sprite icon, ILevelService levelService, System.Action<CategoryType, int> onLevelSelected) {
            _label.text = title;

            foreach (Transform child in _levelContainer) {
                Destroy(child.gameObject);
            }

            int totalLevels = levelService.GetTotalLevelsInCategory(categoryType);
            for (int i = 0; i < totalLevels; i++) {
                var buttonInstance = _uiFactory.CreateMainMenuLevelButton(_levelContainer);
                LevelData levelData = levelService.GetLevel(categoryType, i);
                Color color = levelData != null ? levelData.trailColor : Color.white;

                buttonInstance.Initialize(categoryType, i, icon, color, onLevelSelected);
            }
        }
    }
}
