using Architecture.Services;
using Zenject;

namespace UI.Factory {
    public class UIFactory {
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
    }
}