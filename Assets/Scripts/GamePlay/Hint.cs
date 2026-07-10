using System;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay {
    public class Hint : MonoBehaviour, IGameplayHint {
        public bool isAvaliable { get; private set; }
        [field: SerializeField] public GameObject container { get; private set; }
        [field: SerializeField] public CanvasGroup canvasGroup { get; private set; }
        [field: SerializeField] public RectTransform rectTransform { get; private set; }
        private void Awake() {
            isAvaliable = true;
        }

        private void OnDestroy() {
            isAvaliable = false;
        }
    }

    public interface IGameplayHint {
        public bool isAvaliable { get; }
        public GameObject container { get; }
        public CanvasGroup canvasGroup { get; }
        public RectTransform rectTransform { get; }
    }
}