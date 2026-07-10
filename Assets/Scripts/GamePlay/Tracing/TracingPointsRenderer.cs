using System.Collections.Generic;
using DG.Tweening;
using GamePlay.Factory;
using UI.Factory;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Tracing {
    public class TracingPointsRenderer {
        private const float TotalStaggerDuration = 0.7f;

        private readonly RectTransform _container;
        private readonly IGameplayFactory _gameplayFactory;

        private readonly List<GameObject> _circleGos = new();
        private GameObject _starGo;

        public TracingPointsRenderer(GamePlayPanel gamePlayPanel, IGameplayFactory gameplayFactory) {
            _container = gamePlayPanel.TracingContainer;
            _gameplayFactory = gameplayFactory;
        }

        public void Spawn(List<Vector2> points) {
            Clear();
            float staggerDelay = TotalStaggerDuration / points.Count;

            for (int i = 0; i < points.Count; i++) {
                Image circleImg = _gameplayFactory.CreatePoint(_container, points[i], i, staggerDelay);
                _circleGos.Add(circleImg.gameObject);
            }

            Image starImg = _gameplayFactory.CreateStar(_container, points[^1]);
            _starGo = starImg.gameObject;
        }

        public void BringStarToFront() {
            if (_starGo != null) {
                _starGo.transform.SetAsLastSibling();
            }
        }

        public void AnimateOut(float duration) {
            if (_starGo != null)
                _starGo.transform.DOScale(0f, duration).SetEase(Ease.InBack);

            foreach (var circle in _circleGos) {
                if (circle != null)
                    circle.transform.DOScale(0f, duration).SetEase(Ease.InBack);
            }
        }

        public void Clear() {
            if (_starGo != null) Object.Destroy(_starGo);
            _starGo = null;

            foreach (var circle in _circleGos) {
                if (circle != null) Object.Destroy(circle);
            }
            _circleGos.Clear();
        }
    }
}
