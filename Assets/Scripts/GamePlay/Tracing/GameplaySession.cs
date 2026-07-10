using System;
using Architecture.Services;
using DG.Tweening;
using UI.Factory;
using UnityEngine;
using Zenject;

namespace GamePlay.Tracing {
    public class GameplaySession : IDisposable {
        private readonly GamePlayPanel _gamePlayPanel;
        private readonly LevelData _activeLevel;
        private readonly IIdleHintService _idleHintService;
        private readonly TracingInputProcessor _inputProcessor;
        private readonly GameplayAnimator _animator;

        private int _currentStrokeIndex;
        private bool _inputProcessorEnabled;
        private bool _isDisposed;

        public event Action LevelCompleted;

        public GameplaySession(
            GamePlayPanel gamePlayPanel, LevelData activeLevel,
            IIdleHintService idleHintService, TracingInputProcessor inputProcessor,
            GameplayAnimator animator) {
            _gamePlayPanel = gamePlayPanel;
            _activeLevel = activeLevel;
            _idleHintService = idleHintService;
            _inputProcessor = inputProcessor;
            _animator = animator;
        }

        public void Start() {
            _animator.PlayIntro(_activeLevel.audioPath, () => {
                if (_isDisposed) 
                    return;
                _currentStrokeIndex = 0;
                StartCurrentStroke();
            });
        }

        public void Tick() {
            if (_inputProcessorEnabled)
                _inputProcessor.ProcessInput();
        }

        public void Dispose() {
            if (_isDisposed) 
                return;
            _isDisposed = true;
            _inputProcessorEnabled = false;

            _idleHintService.StopTracking();
            _animator.Dispose();

            DOTween.Clear();
            LevelCompleted = null;
        }

        private void StartCurrentStroke() {
            if (_currentStrokeIndex >= _activeLevel.strokes.Count) {
                _animator.PlayLevelCompletion(() => LevelCompleted?.Invoke());
                return;
            }

            StrokeData stroke = _activeLevel.strokes[_currentStrokeIndex];
            _inputProcessorEnabled = false;

            _animator.SpawnStroke(stroke, _activeLevel.trailColor, () => {
                if (_isDisposed) return;
                _inputProcessorEnabled = true;
                _idleHintService.StartTracking(
                    _activeLevel.audioPath,
                    stroke.points,
                    _gamePlayPanel.TracingContainer,
                    _gamePlayPanel.GetComponent<RectTransform>());
            });

            _inputProcessor.ResetForStroke(stroke.points.Count, OnStrokeCompleted);
        }

        private void OnStrokeCompleted() {
            _inputProcessorEnabled = false;

            _animator.PlayStrokeCompletion(() => {
                if (_isDisposed) return;
                _currentStrokeIndex++;
                StartCurrentStroke();
            });
        }

        public class Factory : PlaceholderFactory<GamePlayPanel, LevelData, GameplaySession> { }
    }
}
