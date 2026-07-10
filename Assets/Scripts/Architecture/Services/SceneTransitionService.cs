using DG.Tweening;
using UnityEngine;
using System.Collections;
using UI.Factory;

namespace Architecture.Services {
    public class SceneTransitionService : ISceneTransitionService {
        private readonly IUIFactory _uiFactory;
        private CanvasGroup _fadeOverlay;

        public SceneTransitionService(IUIFactory uiFactory) {
            _uiFactory = uiFactory;
        }

        public IEnumerator FadeOut() {
            EnsureOverlayCreated();

            bool fadeDone = false;
            _fadeOverlay.gameObject.SetActive(true);
            _fadeOverlay.blocksRaycasts = true;
            _fadeOverlay.alpha = 0f;

            _fadeOverlay.DOFade(1f, 0.4f).SetUpdate(true).OnComplete(() => fadeDone = true);

            while (!fadeDone)
                yield return null;
        }

        public IEnumerator FadeIn() {
            EnsureOverlayCreated();

            bool fadeDone = false;

            _fadeOverlay.DOFade(0f, 0.4f).SetUpdate(true).OnComplete(() => {
                _fadeOverlay.gameObject.SetActive(false);
                _fadeOverlay.blocksRaycasts = false;
                fadeDone = true;
            });

            while (!fadeDone)
                yield return null;
        }

        private void EnsureOverlayCreated() {
            if (_fadeOverlay != null) return;

            _fadeOverlay = _uiFactory.CreateSceneTransitionOverlay();
            _fadeOverlay.alpha = 0f;
            _fadeOverlay.blocksRaycasts = false;
            _fadeOverlay.gameObject.SetActive(false);
        }
    }
}
