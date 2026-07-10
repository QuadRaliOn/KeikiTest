using UnityEngine;

namespace UI.Factory {
    public interface IUIFactory {
        MainMenuPanel CreateMainMenuPanel();
        GamePlayPanel CreateGamePlayPanel();
        CanvasGroup CreateSceneTransitionOverlay();
        MainMenuCategoryView CreateMainMenuCategory(Transform parent);
        MainMenuLevelButton CreateMainMenuLevelButton(Transform parent);
    }
}