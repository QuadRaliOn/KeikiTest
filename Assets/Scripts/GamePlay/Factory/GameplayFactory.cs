using System;
using Architecture.Services;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GamePlay.Factory {
    public class GameplayFactory : IGameplayFactory {
        private readonly DiContainer _container;
        private readonly IAssetProvider _assetProvider;

        [Inject]
        public GameplayFactory(DiContainer container, IAssetProvider assetProvider) {
            _container = container;
            _assetProvider = assetProvider;
        }

        public Image CreatePoint(Transform parent, Vector2 position, int index, float staggerDelay) {
            var point = InstantiateImage(AssetPath.Point, parent);
            point.rectTransform.anchoredPosition = position;
            point.color = new Color(1f, 1f, 1f, 0f);

            point.rectTransform.DOScale(1.0f, 0.3f).From(0f).SetDelay(index * staggerDelay);
            point.DOFade(1f, 0.3f).SetDelay(index * staggerDelay);

            return point;
        }

        public Image CreateStar(Transform parent, Vector2 position) {
            Image img = InstantiateImage(AssetPath.Star, parent);

            RectTransform rect = img.rectTransform;
            rect.anchoredPosition = position;

            rect.transform.SetAsLastSibling();

            rect.DOScale(1.0f, 0.5f).From(0f).SetDelay(0.7f).SetEase(Ease.OutBack);

            return img;
        }

        public Image CreateMascot(Transform parent, Vector2 startPosition, Action onReady) {
            Image img = InstantiateImage(AssetPath.Mascot, parent);
            img.rectTransform.anchoredPosition = startPosition;

            img.transform.DOScale(1.0f, 0.5f).From(0f).SetDelay(0.7f).SetEase(Ease.OutBack)
                .OnComplete(() => onReady?.Invoke());

            return img;
        }

        public Image CreateTrailSegment(Transform parent, Color color) {
            Image img = InstantiateImage(AssetPath.TrailSegment, parent);
            img.color = color;
            return img;
        }

        public Image CreateFingerHint(Transform parent, Vector2 position) {
            var image = InstantiateImage(AssetPath.Finger, parent);
            
            image.rectTransform.anchoredPosition = position;
            image.rectTransform.SetAsLastSibling();
            return image;
        }

        private Image InstantiateImage(string path, Transform parent) {
            GameObject prefab = _assetProvider.LoadAsset(path);
            return _container.InstantiatePrefabForComponent<Image>(prefab, parent);
        }
    }
}