using UnityEngine;
using UnityEngine.UI;
using GamePlay;

namespace UI.Factory {
    [RequireComponent(typeof(Button))]
    public class MainMenuLevelButton : MonoBehaviour {
        [SerializeField] private Image _iconImage;
        
        private Button _button;
        private CategoryType _category;
        private System.Action<CategoryType, int> _onClickCallback;
        private int _levelIndex;

        private void Awake() {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandleClick);
        }

        public void Initialize(CategoryType category, int levelIndex, Sprite icon, Color color, System.Action<CategoryType, int> onClickCallback) {
            _category = category;
            _levelIndex = levelIndex;
            _onClickCallback = onClickCallback;
            _iconImage.sprite = icon;
            _iconImage.color = color;
            _iconImage.preserveAspect = true;
            
        }

        private void HandleClick() {
            _onClickCallback?.Invoke(_category, _levelIndex);
        }

        private void OnDestroy() {
            _button.onClick.RemoveListener(HandleClick);
        }
    }
}
