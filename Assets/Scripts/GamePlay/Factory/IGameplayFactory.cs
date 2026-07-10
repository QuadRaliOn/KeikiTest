using System;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Factory
{
    public interface IGameplayFactory
    {
        Image CreatePoint(Transform parent, Vector2 position, int index, float staggerDelay);
        Image CreateStar(Transform parent, Vector2 position);
        Image CreateMascot(Transform parent, Vector2 startPosition, Action onReady);
        Image CreateTrailSegment(Transform parent, Color color);
        Image CreateFingerHint(Transform parent, Vector2 position);
    }
}
