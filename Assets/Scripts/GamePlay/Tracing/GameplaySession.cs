using System;
using System.Collections;
using Architecture.Boot;
using Architecture.Services;
using DG.Tweening;
using UI.Factory;
using UnityEngine;
using Zenject;

namespace GamePlay.Tracing {
    public class GameplaySession : IDisposable {
        private static readonly string[] SuccessPhrases = { "Awesome", "Excellent", "Thats_good" };
        
        private readonly GamePlayPanel _gamePlayPanel;
        private readonly LevelData _activeLevel;
        private readonly ISoundService _soundService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IIdleHintService _idleHintService;
        private readonly TracingStrokeView _strokeView;
        private readonly TracingInputProcessor _inputProcessor;

        private Action _onLevelCompleted;
        private int _currentStrokeIndex;
        private bool _inputProcessorEnabled;
        private bool _isDisposed;

        public GameplaySession(
            GamePlayPanel gamePlayPanel,
            LevelData activeLevel,
            ISoundService soundService,
            ICoroutineRunner coroutineRunner,
            IIdleHintService idleHintService,
            TracingStrokeView strokeView,
            TracingInputProcessor inputProcessor) {
            _gamePlayPanel = gamePlayPanel;
            _activeLevel = activeLevel;
            _soundService = soundService;
            _coroutineRunner = coroutineRunner;
            _idleHintService = idleHintService;
            _strokeView = strokeView;
            _inputProcessor = inputProcessor;
        }

        public void Start(Action onLevelCompleted) {
            _onLevelCompleted = onLevelCompleted;
            _coroutineRunner.StartCoroutine(PlayIntroAndStartTracing());
        }

        public void Tick() {
            _idleHintService.Tick(Time.deltaTime);

            if (_inputProcessorEnabled)
                _inputProcessor.ProcessInput();
        }

        public void Dispose() {
            if (_isDisposed) return;
            _isDisposed = true;
            _inputProcessorEnabled = false;

            _idleHintService.StopTracking();
            _soundService.StopAudio();

            _strokeView?.DestroyVisuals();

            DOTween.Clear();
        }

        private IEnumerator PlayIntroAndStartTracing() {
            _gamePlayPanel.AnimateShow();
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
                    if (_isDisposed)
                        return;
                    _inputProcessorEnabled = true;
                    _idleHintService.StartTracking(
                        _activeLevel.audioPath,
                        stroke.points,
                        _gamePlayPanel.TracingContainer,
                        _gamePlayPanel.GetComponent<RectTransform>());
                }
            );

            _inputProcessor.ResetForStroke(stroke.points.Count, OnStrokeCompleted);
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
            float duration = _soundService.PlayRandomPhrase(SuccessPhrases);
            float waitLimit = Mathf.Max(duration > 0f ? duration : 1.5f, 1.2f);

            _gamePlayPanel.TracingContainer.DOScale(1.15f, 0.4f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(waitLimit);

            if (_isDisposed) yield break;

            bool hideDone = false;
            _gamePlayPanel.AnimateHide(() => hideDone = true);

            while (!hideDone)
                yield return null;

            yield return new WaitForSeconds(0.3f);

            if (_isDisposed) yield break;
            _onLevelCompleted?.Invoke();
        }

        public class Factory : PlaceholderFactory<GamePlayPanel, LevelData, GameplaySession> { }
    }
}
