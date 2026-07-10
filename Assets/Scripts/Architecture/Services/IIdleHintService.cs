using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Architecture.Services {
    public interface IIdleHintService : IDisposable,ITickable {
        void StartTracking(string audioPath, List<Vector2> strokePoints, RectTransform container, RectTransform fingerParent);
        void StopTracking();
    }
}
