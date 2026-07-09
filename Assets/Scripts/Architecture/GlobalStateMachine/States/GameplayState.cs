using Architecture.Services;
using UI.Factory;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Architecture.GlobalStateMachine.States {
    public class GameplayState : IState, ITickable {
        private readonly SceneLoader _sceneLoader;
        private readonly UIFactory _uiFactory;
        private readonly LevelService _levelService;
        private readonly IGameStateMachine _stateMachine;
        private readonly IAssetProvider _assetProvider;
        private readonly GameplaySession.Factory _sessionFactory;

        private GamePlayPanel _gamePlayPanel;
        private GameplaySession _activeSession;
        private LevelData _activeLevel;
        private bool _isActive;

        public GameplayState(
            SceneLoader sceneLoader,
            UIFactory uiFactory,
            LevelService levelService,
            IGameStateMachine stateMachine,
            IAssetProvider assetProvider,
            GameplaySession.Factory sessionFactory) {
            _sceneLoader = sceneLoader;
            _uiFactory = uiFactory;
            _levelService = levelService;
            _stateMachine = stateMachine;
            _assetProvider = assetProvider;
            _sessionFactory = sessionFactory;
        }

        public void Enter() {
            _isActive = true;
            _sceneLoader.LoadScene(Scenes.GameplayScene, OnSceneLoaded);
        }

        public void Tick() {
            if (_isActive) _activeSession?.Tick();
        }

        public void Exit() {
            _isActive = false;

            _activeSession?.Dispose();
            _activeSession = null;

            if (_gamePlayPanel != null) {
                Object.Destroy(_gamePlayPanel.gameObject);
                _gamePlayPanel = null;
            }

            _activeLevel = null;
        }

        // ────────── Private ──────────

        private void OnSceneLoaded() {
            if (!_isActive) return;

            _activeLevel = _levelService.GetActiveLevel();
            _gamePlayPanel = _uiFactory.CreateGamePlayPanel();

            if (_gamePlayPanel == null) return;

            Sprite silhouette = _assetProvider.LoadAsset<Sprite>(_activeLevel.spritePath);
            _gamePlayPanel.Initialize(silhouette, OnHomePressed);

            _activeSession = _sessionFactory.Create(_gamePlayPanel, _activeLevel);
            _activeSession.Start(OnLevelCompleted);
        }

        private void OnLevelCompleted() {
            if (!_isActive) return;

            int totalLevels = _levelService.GetTotalLevelsInCategory(_activeLevel.category);
            int nextLvlIdx = (_levelService.ActivePayload.LevelIndex + 1) % totalLevels;
            _levelService.ActivePayload = new GameplayStatePayload(_activeLevel.category, nextLvlIdx);

            _stateMachine.Enter<GameplayState>();
        }

        private void OnHomePressed() => _stateMachine.Enter<MainMenuState>();
    }
}