using UnityEngine;
using UnityEngine.UI;

namespace GamePlay {
    public class Hint : MonoBehaviour, IGameplayHint {
        [field: SerializeField] public GameObject container { get; private set; }
        [field: SerializeField] public CanvasGroup canvasGroup { get; private set; }
        [field: SerializeField] public RectTransform rectTransform { get; private set; }
        [field: SerializeField] public Image image { get; private set; }
    }

    public interface IGameplayHint {
        public GameObject container { get; }
        public CanvasGroup canvasGroup { get; }
        public RectTransform rectTransform { get; }
        public Image image { get; }
    }
}