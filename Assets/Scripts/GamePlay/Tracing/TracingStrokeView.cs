using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Tracing {
    public class TracingStrokeView {
        private const float MascotFadeOutDuration = 0.4f;
        private const float PointsFadeOutDuration = 0.3f;
        private const float CompletionCleanupDelay = 0.5f;

        private readonly MascotPresenter _mascotPresenter;
        private readonly TracingTrailRenderer _trailRenderer;
        private readonly TracingPointsRenderer _pointsRenderer;
        private List<Vector2> _strokePoints;

        public TracingStrokeView(
            MascotPresenter mascotPresenter,
            TracingTrailRenderer trailRenderer,
            TracingPointsRenderer pointsRenderer) {
            _mascotPresenter = mascotPresenter;
            _trailRenderer = trailRenderer;
            _pointsRenderer = pointsRenderer;
        }

        public void SpawnStroke(StrokeData stroke, Color trailColor, Action onReady) {
            CleanupPreviousStroke();

            _strokePoints = stroke.points;

            _pointsRenderer.Spawn(stroke.points);
            _trailRenderer.Spawn(stroke.points, trailColor);
            _pointsRenderer.BringStarToFront();
            _mascotPresenter.Spawn(stroke.points[0], onReady);
        }

        public void SetMascotPosition(Vector2 localPosition) => _mascotPresenter.SetPosition(localPosition);

        public Vector2 GetMascotPosition() => _mascotPresenter.GetPosition();

        public void UpdateTrail(int currentPointIndex, Vector2 mascotLocalPos) =>
            _trailRenderer.Update(currentPointIndex, mascotLocalPos);

        public Vector2 GetPointAt(int index) => _strokePoints[index];

        public void AnimateCompletion(Action onComplete) {
            _mascotPresenter.AnimateOut(MascotFadeOutDuration);
            _pointsRenderer.AnimateOut(PointsFadeOutDuration);

            DG.Tweening.DOVirtual.DelayedCall(CompletionCleanupDelay, () => {
                _mascotPresenter.Clear();
                _pointsRenderer.Clear();
                _trailRenderer.ArchiveCompletedSegments();
                onComplete?.Invoke();
            });
        }

        public void DestroyVisuals() {
            _mascotPresenter.Clear();
            _pointsRenderer.Clear();
            _trailRenderer.Clear();
        }

        private void CleanupPreviousStroke() {
            _mascotPresenter.Clear();
            _pointsRenderer.Clear();
        }
    }
}