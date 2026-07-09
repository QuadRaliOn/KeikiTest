using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Factory {
    public class GamePlayPanel : MonoBehaviour {
        [SerializeField] private Button _homeButton;
        [SerializeField] private RectTransform _tracingContainer;
        [SerializeField] private Image _silhouetteImage;

        public RectTransform TracingContainer => _tracingContainer;

        public void Initialize(Sprite silhouetteSprite, Action onHomePressed) {
            AddListeners(onHomePressed);
            SetSilhouetteImage(silhouetteSprite);
        }
        

        private void AddListeners(Action onHomePressed) {
            _homeButton.onClick.AddListener(() => onHomePressed?.Invoke());
        }

        private void SetSilhouetteImage(Sprite silhouetteSprite) {
            _silhouetteImage.sprite = silhouetteSprite;
            _silhouetteImage.color = new Color(1f, 1f, 1f, 0.15f); // Semi-transparent contour-hint
            _silhouetteImage.preserveAspect = true;
            _silhouetteImage.gameObject.SetActive(true);
        }
    }
}