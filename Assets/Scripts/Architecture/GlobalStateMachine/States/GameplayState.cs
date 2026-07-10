using Architecture.Services;
using GamePlay;
using GamePlay.Tracing;
using UI.Factory;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Architecture.GlobalStateMachine.States {
    public class GameplayState : IState, ITickable {
        private readonly ISceneLoader _sceneLoader;
        private readonly IUIFactory _uiFactory;
        private readonly ILevelService _levelService;
        private readonly IGameStateMachine _stateMachine;
        private readonly IAssetProvider _assetProvider;
        private readonly GameplaySession.Factory _sessionFactory;

        private GamePlayPanel _gamePlayPanel;
        private GameplaySession _activeSession;
        private LevelData _activeLevel;
        private bool _isActive;

        public GameplayState(
            ISceneLoader sceneLoader,
            IUIFactory uiFactory,
            ILevelService levelService,
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
            _activeSession?.Tick();
        }

        public void Exit() {
            _isActive = false;
            
            _activeSession.LevelCompleted -= OnLevelCompleted;
            _activeSession.Dispose();
            
            _gamePlayPanel.OnHomePressed -= OnHomePressed;
            Object.Destroy(_gamePlayPanel.gameObject);
        }

        private void OnSceneLoaded() {
            if (!_isActive) 
                return;

            _activeLevel = _levelService.GetActiveLevel();
            _gamePlayPanel = _uiFactory.CreateGamePlayPanel();

            Sprite silhouette = _assetProvider.LoadAsset<Sprite>(_activeLevel.spritePath);
            _gamePlayPanel.Initialize(silhouette);
            _gamePlayPanel.OnHomePressed += OnHomePressed;

            _activeSession = _sessionFactory.Create(_gamePlayPanel, _activeLevel);
            _activeSession.LevelCompleted += OnLevelCompleted;
            _activeSession.Start();
        }

        private void OnLevelCompleted() {
            _activeSession.LevelCompleted -= OnLevelCompleted;
            
            int totalLevels = _levelService.GetTotalLevelsInCategory(_activeLevel.category);
            int nextLvlIdx = (_levelService.ActivePayload.LevelIndex + 1) % totalLevels;
            _levelService.ActivePayload = new GameplayStatePayload(_activeLevel.category, nextLvlIdx);

            _stateMachine.Enter<GameplayState>();
        }

        private void OnHomePressed() => _stateMachine.Enter<MainMenuState>();
    }
}