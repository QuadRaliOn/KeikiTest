using System;
using System.Collections;
using Architecture.Boot;
using Architecture.Services;
using DG.Tweening;
using UI.Factory;
using UnityEngine;

namespace GamePlay.Tracing {
    public class GameplayAnimator {
        private static readonly string[] SuccessPhrases = { "Awesome", "Excellent", "Thats_good" };

        private readonly GamePlayPanel _gamePlayPanel;
        private readonly ISoundService _soundService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly TracingStrokeView _strokeView;

        private bool _isDisposed;

        public GameplayAnimator(
            GamePlayPanel gamePlayPanel,
            ISoundService soundService,
            ICoroutineRunner coroutineRunner,
            TracingStrokeView strokeView) {
            _gamePlayPanel = gamePlayPanel;
            _soundService = soundService;
            _coroutineRunner = coroutineRunner;
            _strokeView = strokeView;
        }

        public void PlayIntro(string audioPath, Action onIntroFinished) {
            _coroutineRunner.StartCoroutine(IntroRoutine(audioPath, onIntroFinished));
        }

        public void PlayStrokeCompletion(Action onComplete) {
            _strokeView.AnimateCompletion(() => {
                if (!_isDisposed)
                    onComplete?.Invoke();
            });
        }

        public void PlayLevelCompletion(Action onComplete) {
            _coroutineRunner.StartCoroutine(LevelCompletionRoutine(onComplete));
        }

        public void SpawnStroke(StrokeData stroke, Color trailColor, Action onReady) {
            _strokeView.SpawnStroke(stroke, trailColor, onReady);
        }

        public void Dispose() {
            _isDisposed = true;
            _soundService.StopAudio();
            _strokeView.DestroyVisuals();
        }

        private IEnumerator IntroRoutine(string audioPath, Action onIntroFinished) {
            _gamePlayPanel.AnimateShow();
            float duration = _soundService.PlayAudio(audioPath);
            yield return new WaitForSeconds(duration);

            if (!_isDisposed)
                onIntroFinished?.Invoke();
        }

        private IEnumerator LevelCompletionRoutine(Action onComplete) {
            float duration = _soundService.PlayRandomPhrase(SuccessPhrases);
            float waitLimit = Mathf.Max(duration > 0f ? duration : 1.5f, 1.2f);

            _gamePlayPanel.TracingContainer.DOScale(1.15f, 0.4f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(waitLimit);

            if (_isDisposed) 
                yield break;

            bool hideDone = false;
            _gamePlayPanel.AnimateHide(() => hideDone = true);

            while (!hideDone)
                yield return null;

            yield return new WaitForSeconds(0.3f);

            if (!_isDisposed)
                onComplete?.Invoke();
        }
    }
}
