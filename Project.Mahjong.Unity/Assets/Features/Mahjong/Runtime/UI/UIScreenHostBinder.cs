using ProjectMahjong.Features.Mahjong.Data.Configs;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectMahjong.Features.Mahjong.Runtime.UI
{
    /// <summary>
    /// Toggles top-level UI Toolkit panels based on UIScreenCoordinator state.
    /// </summary>
    public sealed class UIScreenHostBinder : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private UIScreenCoordinator _screenCoordinator;

        private VisualElement _mainMenuScreen;
        private VisualElement _lobbySetupScreen;
        private VisualElement _gameplayScreen;
        private VisualElement _resultsScreen;

        private void Awake()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning("UIScreenHostBinder: UIDocument/rootVisualElement is missing.", this);
                return;
            }

            var root = _uiDocument.rootVisualElement;
            _mainMenuScreen = root.Q<VisualElement>("main-menu-screen");
            _lobbySetupScreen = root.Q<VisualElement>("lobby-setup-screen");
            _gameplayScreen = root.Q<VisualElement>("gameplay-screen");
            _resultsScreen = root.Q<VisualElement>("results-screen");
        }

        private void OnEnable()
        {
            if (_screenCoordinator != null)
            {
                _screenCoordinator.ScreenChanged += OnScreenChanged;
            }

            if (_screenCoordinator != null && _screenCoordinator.IsInitialized)
            {
                ApplyScreen(_screenCoordinator.CurrentScreen);
            }
            else
            {
                ApplyScreen(UIScreenId.MainMenu);
            }
        }

        private void OnDisable()
        {
            if (_screenCoordinator != null)
            {
                _screenCoordinator.ScreenChanged -= OnScreenChanged;
            }
        }

        private void OnScreenChanged(UIScreenId screen)
        {
            ApplyScreen(screen);
        }

        private void ApplyScreen(UIScreenId screen)
        {
            SetVisible(_mainMenuScreen, screen == UIScreenId.MainMenu);
            SetVisible(_lobbySetupScreen, screen == UIScreenId.LobbySetup);
            SetVisible(_gameplayScreen, screen == UIScreenId.Gameplay);
            SetVisible(_resultsScreen, screen == UIScreenId.Results);
        }

        private static void SetVisible(VisualElement panel, bool visible)
        {
            if (panel == null)
            {
                return;
            }

            panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}

