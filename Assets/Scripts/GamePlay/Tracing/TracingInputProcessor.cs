using System;
using UI.Factory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay.Tracing {
    public class TracingInputProcessor {
        private const float OutOfBoundsPixelRadius = 150f;
        private const float ReachedPointPixelRadius = 10f;
        private const float MascotLerpSpeed = 25f;
        
        private readonly GamePlayPanel _gamePlayPanel;
        private readonly TracingStrokeView _view;
        private Action _onStrokeCompleted;

        private int _totalPoints;
        private int _currentPointIndex;
        private bool _isTracing;
        private bool _isComplete;

        private RectTransform Container => _gamePlayPanel.TracingContainer;

        public TracingInputProcessor(GamePlayPanel gamePlayPanel, TracingStrokeView view) {
            _gamePlayPanel = gamePlayPanel;
            _view = view;
        }

        public void ResetForStroke(int totalPoints, Action onStrokeCompleted) {
            _totalPoints = totalPoints;
            _onStrokeCompleted = onStrokeCompleted;
            _currentPointIndex = 0;
            _isTracing = false;
            _isComplete = false;
        }

        public void ProcessInput() {
            if (_isComplete) 
                return;

            Pointer pointer = Pointer.current;
            if (!pointer.press.isPressed) {
                _isTracing = false;
                return;
            }

            Vector2 screenPos = pointer.position.ReadValue();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    Container, screenPos, null, out Vector2 localPoint))
                return;

            Vector2 mascotPos = _view.GetMascotPosition();

            if (!_isTracing) {
                if (Vector2.Distance(localPoint, mascotPos) < OutOfBoundsPixelRadius)
                    _isTracing = true;
                else
                    return;
            }

            ProcessTracing(localPoint);
        }

        private void ProcessTracing(Vector2 localPoint) {
            Vector2 source = _view.GetPointAt(_currentPointIndex);
            Vector2 target = _view.GetPointAt(_currentPointIndex + 1);

            Vector2 projection = ProjectOntoSegment(localPoint, source, target);
            float distToPath = Vector2.Distance(localPoint, projection);

            if (distToPath >= OutOfBoundsPixelRadius) {
                _isTracing = false;
                return;
            }

            Vector2 currentMascotPos = _view.GetMascotPosition();
            Vector2 smoothedPos = Vector2.Lerp(currentMascotPos, projection, MascotLerpSpeed * Time.deltaTime);

            _view.SetMascotPosition(smoothedPos);
            _view.UpdateTrail(_currentPointIndex, smoothedPos);

            if (Vector2.Distance(smoothedPos, target) < ReachedPointPixelRadius)
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
