using System;
using ProjectMahjong.Features.Mahjong.Data.Configs;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectMahjong.Features.Mahjong.Runtime.UI
{
    /// <summary>
    /// Centralized, data-driven screen navigation coordinator.
    /// </summary>
    public sealed class UIScreenCoordinator : MonoBehaviour
    {
        [Serializable]
        public sealed class ScreenChangedEvent : UnityEvent<string>
        {
        }

        [Header("Flow")]
        [SerializeField] private UIScreenFlowConfig _flowConfig;
        [SerializeField] private bool _initializeOnAwake = true;

        [Header("Events")]
        [SerializeField] private ScreenChangedEvent _onScreenChanged;

        private UIScreenId _currentScreen = UIScreenId.MainMenu;
        private bool _isInitialized;

        public event Action<UIScreenId> ScreenChanged;

        public UIScreenId CurrentScreen => _currentScreen;
        public bool IsInitialized => _isInitialized;

        private void Awake()
        {
            if (_initializeOnAwake)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (_flowConfig == null)
            {
                Debug.LogWarning("UIScreenCoordinator: Missing UIScreenFlowConfig; fallback to MainMenu.", this);
                _currentScreen = UIScreenId.MainMenu;
            }
            else
            {
                _currentScreen = _flowConfig.DefaultEntryScreen;
            }

            _isInitialized = true;
            NotifyScreenChanged(_currentScreen);
        }

        public bool TryGoTo(UIScreenId target)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (_flowConfig != null && !_flowConfig.IsTransitionAllowed(_currentScreen, target))
            {
                Debug.LogWarning($"UIScreenCoordinator: blocked transition {_currentScreen} -> {target}.", this);
                return false;
            }

            _currentScreen = target;
            NotifyScreenChanged(target);
            return true;
        }

        public bool GoToMainMenu() => TryGoTo(UIScreenId.MainMenu);
        public bool GoToLobbySetup() => TryGoTo(UIScreenId.LobbySetup);
        public bool GoToGameplay() => TryGoTo(UIScreenId.Gameplay);
        public bool GoToResults() => TryGoTo(UIScreenId.Results);

        private void NotifyScreenChanged(UIScreenId screen)
        {
            ScreenChanged?.Invoke(screen);
            _onScreenChanged?.Invoke(screen.ToString());
            Debug.Log($"UIScreenCoordinator: current screen -> {screen}", this);
        }
    }
}

