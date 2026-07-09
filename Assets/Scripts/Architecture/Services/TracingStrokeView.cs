using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.Services {
    public class TracingStrokeView {
        private readonly RectTransform _container;
        private readonly RectTransform _mascotParent;
        private readonly IGameplayFactory _gameplayFactory;

        private readonly List<GameObject> _circleGos = new();
        private readonly List<RectTransform> _trailSegments = new();
        private readonly List<RectTransform> _completedTrailSegments = new();
        private List<Vector2> _strokePoints;

        private GameObject _mascotGo;
        private GameObject _starGo;
        private RectTransform _mascotRect;

        private const float TrailThickness = 200f;
        private const float TrailExtensionFactor = 0.2f;

        public TracingStrokeView(
            RectTransform container,
            RectTransform mascotParent,
            IGameplayFactory gameplayFactory) {
            _container = container;
            _mascotParent = mascotParent;
            _gameplayFactory = gameplayFactory;
        }

        public void SpawnStroke(StrokeData stroke, Color trailColor, Action onReady) {
            CleanupPreviousStroke();

            _strokePoints = stroke.points;
            _trailSegments.Clear();

            SpawnCircles(stroke.points);
            SpawnTrailSegments(stroke.points.Count - 1, trailColor);
            SpawnStar(stroke.points[^1]);
            SpawnMascot(stroke.points[0], onReady);
        }

        public void SetMascotPosition(Vector2 localPosition) {
            if (_mascotRect == null || _mascotParent == null || _container == null) return;

            Vector3 worldPos = _container.TransformPoint(localPosition);
            _mascotRect.anchoredPosition = _mascotParent.InverseTransformPoint(worldPos);
        }

        public Vector2 GetMascotPosition() {
            if (_mascotRect == null || _mascotParent == null || _container == null) return Vector2.zero;

            Vector3 worldPos = _mascotParent.TransformPoint(_mascotRect.anchoredPosition);
            return _container.InverseTransformPoint(worldPos);
        }

        public void UpdateTrail(int currentPointIndex, Vector2 mascotLocalPos) {
            for (int k = 0; k < _trailSegments.Count; k++) {
                RectTransform segRect = _trailSegments[k];

                if (k < currentPointIndex) {
                    DrawSegment(segRect, _strokePoints[k], _strokePoints[k + 1]);
                } else if (k == currentPointIndex) {
                    DrawSegment(segRect, _strokePoints[k], mascotLocalPos);
                } else {
                    segRect.sizeDelta = Vector2.zero;
                }
            }
        }

        public Vector2 GetPointAt(int index) => _strokePoints[index];

        public void AnimateCompletion(Action onComplete) {
            AnimateScaleOut(_mascotGo, 0.4f);
            AnimateScaleOut(_starGo, 0.4f);

            foreach (var circle in _circleGos)
                AnimateScaleOut(circle, 0.3f);

            DOVirtual.DelayedCall(0.5f, () => {
                DestroyStrokeElements();
                ArchiveTrailSegments();
                onComplete?.Invoke();
            });
        }

        public void DestroyVisuals() {
            DestroyStrokeElements();
            DestroyGameObjects(_trailSegments);
            _trailSegments.Clear();
            DestroyGameObjects(_completedTrailSegments);
            _completedTrailSegments.Clear();
        }

        // ────────── Private: Spawning ──────────

        private void SpawnCircles(List<Vector2> points) {
            float staggerDelay = 0.7f / points.Count;

            for (int i = 0; i < points.Count; i++) {
                Image circleImg = _gameplayFactory.CreatePoint(_container, points[i], i, staggerDelay);
                _circleGos.Add(circleImg.gameObject);
            }
        }

        private void SpawnTrailSegments(int count, Color color) {
            for (int k = 0; k < count; k++) {
                Image segImg = _gameplayFactory.CreateTrailSegment(_container, color);
                _trailSegments.Add(segImg.rectTransform);
            }
        }

        private void SpawnStar(Vector2 position) {
            Image starImg = _gameplayFactory.CreateStar(_container, position);
            _starGo = starImg.gameObject;
        }

        private void SpawnMascot(Vector2 position, Action onReady) {
            Vector3 worldPos = _container.TransformPoint(position);
            Vector2 parentPos = _mascotParent.InverseTransformPoint(worldPos);

            Image mascotImg = _gameplayFactory.CreateMascot(_mascotParent, parentPos, onReady);
            _mascotGo = mascotImg.gameObject;
            _mascotRect = mascotImg.rectTransform;
        }

        // ────────── Private: Drawing ──────────

        private void DrawSegment(RectTransform rect, Vector2 a, Vector2 b) {
            Vector2 dir = b - a;
            float length = dir.magnitude;

            if (length < 0.001f) {
                rect.sizeDelta = Vector2.zero;
                return;
            }

            float extension = TrailThickness * TrailExtensionFactor;
            float extendedLength = length + extension * 2f;

            rect.anchoredPosition = a + dir * 0.5f;
            rect.sizeDelta = new Vector2(extendedLength, TrailThickness);
            rect.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        // ────────── Private: Cleanup ──────────

        private void CleanupPreviousStroke() {
            SafeDestroy(ref _mascotGo);
            _mascotRect = null;
            SafeDestroy(ref _starGo);

            foreach (var circle in _circleGos)
                if (circle != null) UnityEngine.Object.Destroy(circle);

            _circleGos.Clear();
        }

        private void DestroyStrokeElements() {
            SafeDestroy(ref _mascotGo);
            _mascotRect = null;
            SafeDestroy(ref _starGo);

            foreach (var circle in _circleGos)
                if (circle != null) UnityEngine.Object.Destroy(circle);

            _circleGos.Clear();
        }

        private void ArchiveTrailSegments() {
            foreach (var segment in _trailSegments)
                if (segment != null)
                    _completedTrailSegments.Add(segment);

            _trailSegments.Clear();
        }

        private static void DestroyGameObjects(List<RectTransform> list) {
            foreach (var item in list)
                if (item != null) UnityEngine.Object.Destroy(item.gameObject);
        }

        private static void SafeDestroy(ref GameObject go) {
            if (go == null) return;
            UnityEngine.Object.Destroy(go);
            go = null;
        }

        private static void AnimateScaleOut(GameObject go, float duration) {
            if (go != null) go.transform.DOScale(0f, duration).SetEase(Ease.InBack);
        }
    }
}