using System;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture.Services {
    public interface IIdleHintService : IDisposable {
        void StartTracking(string audioPath, List<Vector2> strokePoints, RectTransform container, RectTransform fingerParent);
        void StopTracking();
        void ResetIdleTimer();
        void Tick(float deltaTime);
    }
}
