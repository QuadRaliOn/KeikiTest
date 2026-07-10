using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Factory {
    public class GamePlayPanel : MonoBehaviour {
        [SerializeField] private Button _homeButton;
        [SerializeField] private RectTransform _tracingContainer;
        [SerializeField] private Image _silhouetteImage;

        public RectTransform TracingContainer => _tracingContainer;
        public Action OnHomePressed;

        private void Awake() {
            AddListeners();
        }

        private void OnDestroy() {
            RemoveListeners();
        }

        public void Initialize(Sprite silhouetteSprite) {
            SetSilhouetteImage(silhouetteSprite);
        }

        public void AnimateShow(Action onComplete = null) {
            _silhouetteImage.transform.localScale = Vector3.one * 0.8f;
            _silhouetteImage.color = new Color(1f, 1f, 1f, 0f);
            
            _silhouetteImage.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            _silhouetteImage.DOFade(0.15f, 0.5f).OnComplete(() => onComplete?.Invoke());
        }

        public void AnimateHide(Action onComplete = null) {
            _silhouetteImage.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack);
            _silhouetteImage.DOFade(0f, 0.5f).OnComplete(() => onComplete?.Invoke());
        }

        private void SetSilhouetteImage(Sprite silhouetteSprite) {
            _silhouetteImage.sprite = silhouetteSprite;
            _silhouetteImage.color = new Color(1f, 1f, 1f, 0.15f);
            _silhouetteImage.preserveAspect = true;
            _silhouetteImage.gameObject.SetActive(true);
        }

        private void RemoveListeners() => 
            _homeButton.onClick.RemoveAllListeners();

        private void AddListeners() => 
            _homeButton.onClick.AddListener(() => OnHomePressed?.Invoke());
    }
}