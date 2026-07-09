using System;
using System.Collections;
using Architecture.Boot;
using DG.Tweening;
using UI.Factory;
using UnityEngine;
using Zenject;

namespace Architecture.Services {
    public class GameplaySession : IDisposable {
        private readonly GamePlayPanel _gamePlayPanel;
        private readonly LevelData _activeLevel;
        private readonly IGameplayFactory _gameplayFactory;
        private readonly ISoundService _soundService;
        private readonly ICoroutineRunner _coroutineRunner;

        private TracingStrokeView _strokeView;
        private TracingInputProcessor _inputProcessor;
        private Action _onLevelCompleted;

        private int _currentStrokeIndex;
        private bool _inputProcessorEnabled;
        private bool _isDisposed;

        private const float OutOfBoundsPixelRadius = 80f;
        private const float ReachedPointPixelRadius = 35f;

        public GameplaySession(
            GamePlayPanel gamePlayPanel,
            LevelData activeLevel,
            IGameplayFactory gameplayFactory,
            ISoundService soundService,
            ICoroutineRunner coroutineRunner) {
            _gamePlayPanel = gamePlayPanel;
            _activeLevel = activeLevel;
            _gameplayFactory = gameplayFactory;
            _soundService = soundService;
            _coroutineRunner = coroutineRunner;
        }

        public void Start(Action onLevelCompleted) {
            _onLevelCompleted = onLevelCompleted;

            _strokeView = new TracingStrokeView(
                _gamePlayPanel.TracingContainer,
                _gamePlayPanel.GetComponent<RectTransform>(),
                _gameplayFactory
            );

            _coroutineRunner.StartCoroutine(PlayIntroAndStartTracing());
        }

        public void Tick() {
            if (_isDisposed || !_inputProcessorEnabled || _inputProcessor == null) return;
            _inputProcessor.ProcessInput();
        }

        public void Dispose() {
            if (_isDisposed) return;
            _isDisposed = true;
            _inputProcessorEnabled = false;

            _strokeView?.DestroyVisuals();
            _strokeView = null;
            _inputProcessor = null;

            DOTween.Clear();
        }

        // ────────── Private: Flow ──────────

        private IEnumerator PlayIntroAndStartTracing() {
            float duration = _soundService.PlayAudio(_activeLevel.audioPath);
            yield return new WaitForSeconds(duration);

            if (_isDisposed) yield break;

            _currentStrokeIndex = 0;
            StartCurrentStroke();
        }

        private void StartCurrentStroke() {
            if (_currentStrokeIndex >= _activeLevel.strokes.Count) {
                _coroutineRunner.StartCoroutine(CompleteLevelRoutine());
                return;
            }

            StrokeData stroke = _activeLevel.strokes[_currentStrokeIndex];
            _inputProcessorEnabled = false;

            _strokeView.SpawnStroke(
                stroke,
                _activeLevel.trailColor,
                onReady: () => {
                    if (!_isDisposed) _inputProcessorEnabled = true;
                }
            );

            _inputProcessor = new TracingInputProcessor(
                _gamePlayPanel.TracingContainer,
                _strokeView,
                stroke.points.Count,
                OutOfBoundsPixelRadius,
                ReachedPointPixelRadius,
                onStrokeCompleted: OnStrokeCompleted
            );
        }

        private void OnStrokeCompleted() {
            _inputProcessorEnabled = false;

            _strokeView.AnimateCompletion(() => {
                if (_isDisposed) return;
                _currentStrokeIndex++;
                StartCurrentStroke();
            });
        }

        private IEnumerator CompleteLevelRoutine() {
            string[] successPhrases = { "Awesome", "Excellent", "Thats_good" };
            float duration = _soundService.PlayRandomPhrase(successPhrases);
            yield return new WaitForSeconds(duration > 0f ? duration : 1.0f);

            if (_isDisposed) yield break;
            _onLevelCompleted?.Invoke();
        }

        public class Factory : PlaceholderFactory<GamePlayPanel, LevelData, GameplaySession> { }
    }
}
