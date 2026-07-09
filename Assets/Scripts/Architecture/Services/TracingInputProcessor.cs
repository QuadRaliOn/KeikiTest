using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Architecture.Services {
    public class TracingInputProcessor {
        private readonly RectTransform _container;
        private readonly TracingStrokeView _view;
        private readonly int _totalPoints;
        private readonly float _outOfBoundsRadius;
        private readonly float _reachedPointRadius;
        private readonly Action _onStrokeCompleted;

        private int _currentPointIndex;
        private bool _isTracing;
        private bool _isComplete;

        public TracingInputProcessor(
            RectTransform container,
            TracingStrokeView view,
            int totalPoints,
            float outOfBoundsRadius,
            float reachedPointRadius,
            Action onStrokeCompleted) {
            _container = container;
            _view = view;
            _totalPoints = totalPoints;
            _outOfBoundsRadius = outOfBoundsRadius;
            _reachedPointRadius = reachedPointRadius;
            _onStrokeCompleted = onStrokeCompleted;
        }

        public void ProcessInput() {
            if (_isComplete) return;

            Pointer pointer = Pointer.current;
            if (pointer == null) return;

            if (!pointer.press.isPressed) {
                _isTracing = false;
                return;
            }

            Vector2 screenPos = pointer.position.ReadValue();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _container, screenPos, null, out Vector2 localPoint))
                return;

            Vector2 mascotPos = _view.GetMascotPosition();

            if (!_isTracing) {
                if (Vector2.Distance(localPoint, mascotPos) < _outOfBoundsRadius)
                    _isTracing = true;
                else
                    return;
            }

            ProcessTracing(localPoint);
        }

        private void ProcessTracing(Vector2 localPoint) {
            Vector2 a = _view.GetPointAt(_currentPointIndex);
            Vector2 b = _view.GetPointAt(_currentPointIndex + 1);

            Vector2 projection = ProjectOntoSegment(localPoint, a, b);
            float distToPath = Vector2.Distance(localPoint, projection);

            if (distToPath >= _outOfBoundsRadius) {
                _isTracing = false;
                return;
            }

            _view.SetMascotPosition(projection);
            _view.UpdateTrail(_currentPointIndex, projection);

            if (Vector2.Distance(projection, b) < _reachedPointRadius)
                AdvanceToNextPoint();
        }

        private void AdvanceToNextPoint() {
            _currentPointIndex++;

            if (_currentPointIndex >= _totalPoints - 1) {
                _isComplete = true;
                _isTracing = false;
                _onStrokeCompleted?.Invoke();
            }
        }

        private static Vector2 ProjectOntoSegment(Vector2 point, Vector2 a, Vector2 b) {
            Vector2 ab = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / Vector2.Dot(ab, ab));
            return a + t * ab;
        }
    }
}
