using Architecture.Services;
using UnityEngine;
using Zenject;

namespace UI.Factory {
    public class UIFactory : IUIFactory {
        private IAssetProvider _assetProvider;
        private DiContainer _container;

        [Inject]
        private void Construct(IAssetProvider assetProvider, DiContainer container) {
            _container = container;
            _assetProvider = assetProvider;
        }

        public MainMenuPanel CreateMainMenuPanel() {
            var panel = _assetProvider.LoadAsset(AssetPath.MainMenu);
            return _container.InstantiatePrefabForComponent<MainMenuPanel>(panel);
        }

        public GamePlayPanel CreateGamePlayPanel() {
            var panel = _assetProvider.LoadAsset(AssetPath.GamePlay);
            return _container.InstantiatePrefabForComponent<GamePlayPanel>(panel);
        }

        public CanvasGroup CreateSceneTransitionOverlay() {
            var prefab = _assetProvider.LoadAsset(AssetPath.SceneTransitionOverlay);
            var go = _container.InstantiatePrefab(prefab);
            return go.GetComponent<CanvasGroup>();
        }

        public MainMenuCategoryView CreateMainMenuCategory(Transform parent) {
            var prefab = _assetProvider.LoadAsset(AssetPath.Category);
            return _container.InstantiatePrefabForComponent<MainMenuCategoryView>(prefab, parent);
        }

        public MainMenuLevelButton CreateMainMenuLevelButton(Transform parent) {
            var prefab = _assetProvider.LoadAsset(AssetPath.Level);
            return _container.InstantiatePrefabForComponent<MainMenuLevelButton>(prefab, parent);
        }
    }
}