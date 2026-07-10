using System.Collections.Generic;
using GamePlay.Factory;
using UI.Factory;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Tracing {
    public class TracingTrailRenderer {
        private const float TrailThickness = 200f;
        private const float TrailExtensionFactor = 0.2f;
        private const float OuterSegmentExtension = 70f;
        private const float MinSegmentLength = 0.001f;
        private const float ClosedLoopThreshold = 120f;

        private readonly RectTransform _container;
        private readonly IGameplayFactory _gameplayFactory;

        private readonly List<RectTransform> _trailSegments = new();
        private readonly List<RectTransform> _completedTrailSegments = new();
        private List<Vector2> _strokePoints;
        private bool _isClosedLoop;

        public TracingTrailRenderer(GamePlayPanel gamePlayPanel, IGameplayFactory gameplayFactory) {
            _container = gamePlayPanel.TracingContainer;
            _gameplayFactory = gameplayFactory;
        }

        public void Spawn(List<Vector2> points, Color color) {
            DestroyGameObjects(_trailSegments);
            _trailSegments.Clear();
            _strokePoints = points;

            _isClosedLoop = points.Count > 1
                && Vector2.Distance(points[0], points[^1]) < ClosedLoopThreshold;

            for (int k = 0; k < points.Count - 1; k++) {
                Image segImg = _gameplayFactory.CreateTrailSegment(_container, color);
                _trailSegments.Add(segImg.rectTransform);
            }
        }

        public void Update(int currentPointIndex, Vector2 mascotLocalPos) {
            for (int k = 0; k < _trailSegments.Count; k++) {
                RectTransform segRect = _trailSegments[k];
                
                if (k < currentPointIndex)
                    DrawSegment(segRect, _strokePoints[k], _strokePoints[k + 1], k == 0, k == _trailSegments.Count - 1);
                else if (k == currentPointIndex)
                    DrawSegment(segRect, _strokePoints[k], mascotLocalPos, k == 0, k == _trailSegments.Count - 1);
                else
                    segRect.sizeDelta = Vector2.zero;
            }
        }

        public void ArchiveCompletedSegments() {
            foreach (var segment in _trailSegments) {
                if (segment != null)
                    _completedTrailSegments.Add(segment);
            }
            _trailSegments.Clear();
        }

        public void Clear() {
            DestroyGameObjects(_trailSegments);
            _trailSegments.Clear();
            DestroyGameObjects(_completedTrailSegments);
            _completedTrailSegments.Clear();
        }

        private void DrawSegment(RectTransform rect, Vector2 a, Vector2 b, bool isFirst, bool isLast) {
            Vector2 dir = b - a;
            float length = dir.magnitude;

            if (length < MinSegmentLength) {
                rect.sizeDelta = Vector2.zero;
                return;
            }

            Vector2 u = dir / length;

            float startExt = (isFirst && !_isClosedLoop) ? OuterSegmentExtension : 0f;
            float endExt = (isLast && !_isClosedLoop) ? OuterSegmentExtension : 0f;

            Vector2 aVirtual = a - u * startExt;
            Vector2 bVirtual = b + u * endExt;

            Vector2 newDir = bVirtual - aVirtual;
            float newLength = newDir.magnitude;

            float extension = TrailThickness * TrailExtensionFactor;
            float extendedLength = newLength + extension * 2f;

            rect.anchoredPosition = aVirtual + newDir * 0.5f;
            rect.sizeDelta = new Vector2(extendedLength, TrailThickness);
            rect.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg);
        }

        private static void DestroyGameObjects(List<RectTransform> list) {
            foreach (var item in list) {
                if (item != null)
                    Object.Destroy(item.gameObject);
            }
        }
    }
}
