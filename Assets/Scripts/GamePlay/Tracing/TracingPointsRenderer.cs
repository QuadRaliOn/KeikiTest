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
            _starGo.transform.SetAsLastSibling();
        }

        public void AnimateOut(float duration) {
            _starGo.transform.DOScale(0f, duration).SetEase(Ease.InBack);

            foreach (var circle in _circleGos) {
                circle.transform.DOScale(0f, duration).SetEase(Ease.InBack);
            }
        }

        public void Clear() {
            Object.Destroy(_starGo);

            foreach (var circle in _circleGos) {
                Object.Destroy(circle);
            }
            _circleGos.Clear();
        }
    }
}
