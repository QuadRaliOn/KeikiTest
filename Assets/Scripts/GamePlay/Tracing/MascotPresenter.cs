using System;
using DG.Tweening;
using GamePlay.Factory;
using UI.Factory;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Tracing {
    public class MascotPresenter {
        private readonly RectTransform _container;
        private readonly RectTransform _mascotParent;
        private readonly IGameplayFactory _gameplayFactory;

        private GameObject _mascotGo;
        private RectTransform _mascotRect;

        public MascotPresenter(GamePlayPanel gamePlayPanel, IGameplayFactory gameplayFactory) {
            _container = gamePlayPanel.TracingContainer;
            _mascotParent = gamePlayPanel.GetComponent<RectTransform>();
            _gameplayFactory = gameplayFactory;
        }

        public void Spawn(Vector2 position, Action onReady) {
            Clear();
            Vector3 worldPos = _container.TransformPoint(position);
            Vector2 parentPos = _mascotParent.InverseTransformPoint(worldPos);

            Image mascotImg = _gameplayFactory.CreateMascot(_mascotParent, parentPos, onReady);
            _mascotGo = mascotImg.gameObject;
            _mascotRect = mascotImg.rectTransform;
        }

        public void SetPosition(Vector2 localPosition) {
            if (_mascotRect == null) return;
            Vector3 worldPos = _container.TransformPoint(localPosition);
            _mascotRect.anchoredPosition = _mascotParent.InverseTransformPoint(worldPos);
        }

        public Vector2 GetPosition() {
            if (_mascotRect == null) return Vector2.zero;
            Vector3 worldPos = _mascotParent.TransformPoint(_mascotRect.anchoredPosition);
            return _container.InverseTransformPoint(worldPos);
        }

        public void AnimateOut(float duration) {
            if (_mascotGo != null)
                _mascotGo.transform.DOScale(0f, duration).SetEase(Ease.InBack);
        }

        public void Clear() {
            if (_mascotGo != null) Object.Destroy(_mascotGo);
            _mascotGo = null;
            _mascotRect = null;
        }
    }
}
