using System;
using System.Collections.Generic;
using DG.Tweening;
using GamePlay;
using GamePlay.Factory;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Architecture.Services {
    public class IdleHintService : IIdleHintService {
        private const float VoiceHintDelay = 7f;
        private const float FingerHintDelay = 14f;
        private const float FingerMoveSpeed = 250f;

        private readonly ISoundService _soundService;
        private readonly IGameplayFactory _gameplayFactory;

        private List<Vector2> _currentStrokePoints;
        private IGameplayHint _hint;
        private RectTransform _container;
        private RectTransform _fingerParent;
        private Sequence _fingerSequence;

        private float _idleTimer;
        private string _audioPath;

        private enum State {
            Inactive,
            WaitingVoice,
            WaitingFinger,
            FingerShowing
        }

        private State _state = State.Inactive;

        public IdleHintService(ISoundService soundService, IGameplayFactory gameplayFactory) {
            _soundService = soundService;
            _gameplayFactory = gameplayFactory;
        }

        public void StartTracking(string audioPath, List<Vector2> strokePoints, RectTransform container,
            RectTransform fingerParent) {
            HideFingerHint();

            _audioPath = audioPath;
            _currentStrokePoints = strokePoints;
            _container = container;
            _fingerParent = fingerParent;
            _idleTimer = 0f;
            _state = State.WaitingVoice;
        }

        public void StopTracking() {
            HideFingerHint();
            _state = State.Inactive;
        }

        public void ResetIdleTimer() {
            _idleTimer = 0f;

            if (_state == State.FingerShowing) {
                HideFingerHint();
                _state = State.WaitingVoice;
            }
            else if (_state != State.Inactive)
                _state = State.WaitingVoice;
        }

        public void Tick() {
            if (_state == State.Inactive)
                return;

            Pointer pointer = Pointer.current;
            if (pointer != null && pointer.press.isPressed)
                ResetIdleTimer();

            _idleTimer += Time.deltaTime;

            if (_state == State.WaitingVoice && _idleTimer >= VoiceHintDelay) {
                _soundService.PlayAudio(_audioPath);
                _state = State.WaitingFinger;
            }

            if (_state == State.WaitingFinger && _idleTimer >= FingerHintDelay) {
                ShowFingerHint();
                _state = State.FingerShowing;
            }
        }

        public void Dispose() =>
            StopTracking();

        private void ShowFingerHint() {
            var startPos = ContainerToFingerParent(_currentStrokePoints[0]);
            _hint = _gameplayFactory.CreateFingerHint(_fingerParent, startPos);

            _hint.canvasGroup.alpha = 0f;
            _hint.canvasGroup.DOFade(1f, 0.3f).OnComplete(StartFingerCycle);
        }

        private void StartFingerCycle() {
            _fingerSequence?.Kill();
            var seq = DOTween.Sequence();

            for (int i = 0; i < _currentStrokePoints.Count - 1; i++) {
                var to = ContainerToFingerParent(_currentStrokePoints[i + 1]);
                var from = ContainerToFingerParent(_currentStrokePoints[i]);
                float duration = Vector2.Distance(from, to) / FingerMoveSpeed;
                seq.Append(_hint.rectTransform.DOAnchorPos(to, duration).SetEase(Ease.Linear));
            }

            seq.AppendInterval(0.5f);
            seq.Append(_hint.canvasGroup.DOFade(0f, 0.2f));
            seq.AppendCallback(() => {
                _hint.rectTransform.anchoredPosition = ContainerToFingerParent(_currentStrokePoints[0]);
            });
            seq.Append(_hint.canvasGroup.DOFade(1f, 0.2f));
            seq.OnComplete(StartFingerCycle);

            _fingerSequence = seq;
        }

        private void HideFingerHint() {
            _fingerSequence?.Kill();
            _fingerSequence = null;

            if (_hint is { isAvaliable: true }) 
                _hint.canvasGroup.DOFade(0f, 0.2f)
                    .OnComplete(() => UnityEngine.Object.Destroy(_hint.container));
        }

        private Vector2 ContainerToFingerParent(Vector2 containerLocal) {
            Vector3 world = _container.TransformPoint(containerLocal);
            return _fingerParent.InverseTransformPoint(world);
        }
    }
}