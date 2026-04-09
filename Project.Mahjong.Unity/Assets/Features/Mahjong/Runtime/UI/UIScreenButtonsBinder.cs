using ProjectMahjong.Features.Mahjong.Data.Configs;
using ProjectMahjong.Features.Mahjong.Runtime.Bootstrap;
using ProjectMahjong.Features.Mahjong.Runtime.Gameplay.Dealing;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace ProjectMahjong.Features.Mahjong.Runtime.UI
{
    /// <summary>
    /// Wires basic prototype navigation buttons to UIScreenCoordinator.
    /// </summary>
    public sealed class UIScreenButtonsBinder : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private UIScreenCoordinator _screenCoordinator;
        [SerializeField] private MatchSetupLoader _setupLoader;
        [SerializeField] private RoundRuntimeController _roundRuntimeController;
        [SerializeField] private MatchConfigSet[] _availableMatchConfigs;

        private Button _mainMenuStartButton;
        private Button _lobbyStartButton;
        private Button _lobbyBackButton;
        private Button _resultsRematchButton;
        private Button _resultsMenuButton;
        private Button _gameplayNextTurnButton;
        private Button _gameplayEndRoundButton;

        private DropdownField _lobbyRulesetDropdown;
        private DropdownField _lobbySeatCountDropdown;
        private DropdownField _lobbyDifficultyDropdown;
        private Label _lobbySelectionSummary;
        private VisualElement _playerHandContainer;
        private readonly List<string> _rulesetChoices = new();

        private void Awake()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning("UIScreenButtonsBinder: UIDocument/rootVisualElement is missing.", this);
                return;
            }

            var root = _uiDocument.rootVisualElement;
            _mainMenuStartButton = root.Q<Button>("mainmenu-start-button");
            _lobbyStartButton = root.Q<Button>("lobby-start-button");
            _lobbyBackButton = root.Q<Button>("lobby-back-button");
            _resultsRematchButton = root.Q<Button>("results-rematch-button");
            _resultsMenuButton = root.Q<Button>("results-menu-button");
            _gameplayNextTurnButton = root.Q<Button>("gameplay-nextturn-button");
            _gameplayEndRoundButton = root.Q<Button>("gameplay-endround-button");
            _lobbyRulesetDropdown = root.Q<DropdownField>("lobby-ruleset-dropdown");
            _lobbySeatCountDropdown = root.Q<DropdownField>("lobby-seatcount-dropdown");
            _lobbyDifficultyDropdown = root.Q<DropdownField>("lobby-difficulty-dropdown");
            _lobbySelectionSummary = root.Q<Label>("lobby-selection-summary");
            _playerHandContainer = root.Q<VisualElement>("player-hand-container");

            ConfigureLobbyFields();
        }

        private void OnEnable()
        {
            Register(_mainMenuStartButton, OnMainMenuStartClicked);
            Register(_lobbyStartButton, OnLobbyStartClicked);
            Register(_lobbyBackButton, OnLobbyBackClicked);
            Register(_resultsRematchButton, OnResultsRematchClicked);
            Register(_resultsMenuButton, OnResultsMenuClicked);
            Register(_gameplayNextTurnButton, OnGameplayNextTurnClicked);
            Register(_gameplayEndRoundButton, OnGameplayEndRoundClicked);

            RegisterValueChanged(_lobbyRulesetDropdown, OnLobbySelectionChanged);
            RegisterValueChanged(_lobbySeatCountDropdown, OnLobbySelectionChanged);
            RegisterValueChanged(_lobbyDifficultyDropdown, OnLobbySelectionChanged);

            UpdateLobbySelectionSummary();

            if (_roundRuntimeController != null)
            {
                _roundRuntimeController.RoundStateChanged += OnRoundStateChanged;
                if (_roundRuntimeController.HasRound)
                {
                    RenderPlayerHand(_roundRuntimeController.CurrentState);
                }
            }
        }

        private void OnDisable()
        {
            Unregister(_mainMenuStartButton, OnMainMenuStartClicked);
            Unregister(_lobbyStartButton, OnLobbyStartClicked);
            Unregister(_lobbyBackButton, OnLobbyBackClicked);
            Unregister(_resultsRematchButton, OnResultsRematchClicked);
            Unregister(_resultsMenuButton, OnResultsMenuClicked);
            Unregister(_gameplayNextTurnButton, OnGameplayNextTurnClicked);
            Unregister(_gameplayEndRoundButton, OnGameplayEndRoundClicked);

            UnregisterValueChanged(_lobbyRulesetDropdown, OnLobbySelectionChanged);
            UnregisterValueChanged(_lobbySeatCountDropdown, OnLobbySelectionChanged);
            UnregisterValueChanged(_lobbyDifficultyDropdown, OnLobbySelectionChanged);

            if (_roundRuntimeController != null)
            {
                _roundRuntimeController.RoundStateChanged -= OnRoundStateChanged;
            }
        }

        private void OnMainMenuStartClicked()
        {
            _screenCoordinator?.GoToLobbySetup();
        }

        private void OnLobbyStartClicked()
        {
            if (_setupLoader == null)
            {
                Debug.LogError("UIScreenButtonsBinder: MatchSetupLoader reference is missing.", this);
                return;
            }

            var selectedConfig = ResolveSelectedConfig();
            var selectedSeatCount = ResolveSelectedSeatCount();
            var selectedDifficulty = ResolveSelectedDifficulty();

            var loaded = _setupLoader.TryLoadSetup(selectedConfig, selectedSeatCount, selectedDifficulty);
            if (!loaded)
            {
                // Keep user in lobby to adjust selections.
                _screenCoordinator?.GoToLobbySetup();
                return;
            }
        }

        private void OnLobbyBackClicked()
        {
            _screenCoordinator?.GoToMainMenu();
        }

        private void OnResultsRematchClicked()
        {
            _screenCoordinator?.GoToLobbySetup();
        }

        private void OnResultsMenuClicked()
        {
            _screenCoordinator?.GoToMainMenu();
        }

        private void OnGameplayNextTurnClicked()
        {
            if (_roundRuntimeController == null)
            {
                Debug.LogError("UIScreenButtonsBinder: RoundRuntimeController reference is missing.", this);
                return;
            }

            if (_roundRuntimeController.CurrentState.CurrentActiveSeat == 0)
            {
                // Prefer explicit tile click for player seat; keep fallback stepping for non-player turns.
                Debug.Log("UIScreenButtonsBinder: S0 turn detected. Click a tile to discard.", this);
                return;
            }

            if (!_roundRuntimeController.StepTurn(out var error))
            {
                Debug.LogWarning($"UIScreenButtonsBinder: step turn not applied ({error})", this);
            }
        }

        private void OnGameplayEndRoundClicked()
        {
            _screenCoordinator?.GoToResults();
        }

        private static void Register(Button button, System.Action callback)
        {
            if (button != null)
            {
                button.clicked += callback;
            }
        }

        private static void Unregister(Button button, System.Action callback)
        {
            if (button != null)
            {
                button.clicked -= callback;
            }
        }

        private void ConfigureLobbyFields()
        {
            ConfigureRulesetField();
            ConfigureSeatCountField();
            ConfigureDifficultyField();
            UpdateLobbySelectionSummary();
        }

        private void ConfigureRulesetField()
        {
            if (_lobbyRulesetDropdown == null)
            {
                return;
            }

            _rulesetChoices.Clear();
            if (_availableMatchConfigs != null)
            {
                for (var i = 0; i < _availableMatchConfigs.Length; i++)
                {
                    var cfg = _availableMatchConfigs[i];
                    if (cfg == null)
                    {
                        continue;
                    }

                    _rulesetChoices.Add(cfg.name);
                }
            }

            if (_rulesetChoices.Count == 0)
            {
                var fallback = _setupLoader != null && _setupLoader.CurrentConfigSet != null
                    ? _setupLoader.CurrentConfigSet.name
                    : "Default";
                _rulesetChoices.Add(fallback);
            }

            _lobbyRulesetDropdown.choices = _rulesetChoices;
            _lobbyRulesetDropdown.value = _rulesetChoices[0];
        }

        private void ConfigureSeatCountField()
        {
            if (_lobbySeatCountDropdown == null)
            {
                return;
            }

            _lobbySeatCountDropdown.choices = new List<string> { "1", "2", "3", "4" };
            _lobbySeatCountDropdown.value = "4";
        }

        private void ConfigureDifficultyField()
        {
            if (_lobbyDifficultyDropdown == null)
            {
                return;
            }

            _lobbyDifficultyDropdown.choices = new List<string>
            {
                DifficultyLevel.Easy.ToString(),
                DifficultyLevel.Medium.ToString(),
                DifficultyLevel.Hard.ToString()
            };
            _lobbyDifficultyDropdown.value = DifficultyLevel.Medium.ToString();
        }

        private MatchConfigSet ResolveSelectedConfig()
        {
            if (_availableMatchConfigs == null || _availableMatchConfigs.Length == 0)
            {
                return _setupLoader != null ? _setupLoader.CurrentConfigSet : null;
            }

            var selectedName = _lobbyRulesetDropdown != null ? _lobbyRulesetDropdown.value : string.Empty;
            for (var i = 0; i < _availableMatchConfigs.Length; i++)
            {
                var cfg = _availableMatchConfigs[i];
                if (cfg == null)
                {
                    continue;
                }

                if (cfg.name == selectedName)
                {
                    return cfg;
                }
            }

            return _availableMatchConfigs[0];
        }

        private int ResolveSelectedSeatCount()
        {
            if (_lobbySeatCountDropdown == null)
            {
                return 4;
            }

            return int.TryParse(_lobbySeatCountDropdown.value, out var seatCount)
                ? Mathf.Clamp(seatCount, 1, 4)
                : 4;
        }

        private DifficultyLevel ResolveSelectedDifficulty()
        {
            if (_lobbyDifficultyDropdown == null)
            {
                return DifficultyLevel.Medium;
            }

            return System.Enum.TryParse(_lobbyDifficultyDropdown.value, out DifficultyLevel parsed)
                ? parsed
                : DifficultyLevel.Medium;
        }

        private void OnRoundStateChanged(RoundState state)
        {
            RenderPlayerHand(state);
        }

        private void RenderPlayerHand(RoundState state)
        {
            if (_playerHandContainer == null)
            {
                return;
            }

            _playerHandContainer.Clear();

            if (state.TurnResult.Hands.Length == 0)
            {
                return;
            }

            var canDiscard = state.CurrentActiveSeat == 0 &&
                             state.TurnResult.RoundEndReason != RoundEndReason.Win &&
                             state.TurnResult.RoundEndReason != RoundEndReason.WallExhausted;

            var hand = state.TurnResult.Hands[0];
            for (var i = 0; i < hand.Count; i++)
            {
                var tile = hand[i];
                var tileIndex = i;
                var button = new Button(() => OnPlayerTileClicked(tileIndex))
                {
                    text = FormatTile(tile)
                };
                button.AddToClassList("tile-button");
                if (!canDiscard)
                {
                    button.SetEnabled(false);
                }

                _playerHandContainer.Add(button);
            }
        }

        private void OnPlayerTileClicked(int tileIndex)
        {
            if (_roundRuntimeController == null)
            {
                return;
            }

            if (!_roundRuntimeController.Discard(tileIndex, out var error))
            {
                Debug.LogWarning($"UIScreenButtonsBinder: discard not applied ({error})", this);
            }
        }

        private static string FormatTile(MahjongTile tile)
        {
            if (tile.Suit == TileSuit.Honor)
            {
                return tile.Honor switch
                {
                    HonorType.East => "E",
                    HonorType.South => "S",
                    HonorType.West => "W",
                    HonorType.North => "N",
                    HonorType.Red => "RD",
                    HonorType.Green => "GD",
                    HonorType.White => "WD",
                    _ => "H"
                };
            }

            var suit = tile.Suit switch
            {
                TileSuit.Dots => "D",
                TileSuit.Bamboo => "B",
                TileSuit.Characters => "C",
                _ => "?"
            };
            return $"{suit}{tile.Rank}";
        }

        private void OnLobbySelectionChanged(ChangeEvent<string> _)
        {
            UpdateLobbySelectionSummary();
        }

        private void UpdateLobbySelectionSummary()
        {
            if (_lobbySelectionSummary == null)
            {
                return;
            }

            var ruleset = _lobbyRulesetDropdown != null ? _lobbyRulesetDropdown.value : "N/A";
            var selectedConfig = ResolveSelectedConfig();
            var selectedSeatCount = ResolveSelectedSeatCount();
            var selectedDifficulty = ResolveSelectedDifficulty();

            var isValid = MatchSetupLoader.TryBuildSetup(
                selectedConfig,
                selectedSeatCount,
                selectedDifficulty,
                out _,
                out var error);

            _lobbySelectionSummary.RemoveFromClassList("setup-summary-ok");
            _lobbySelectionSummary.RemoveFromClassList("setup-summary-error");

            if (isValid)
            {
                _lobbySelectionSummary.AddToClassList("setup-summary-ok");
                _lobbySelectionSummary.text =
                    $"Selection: Ruleset={ruleset} | Seats={selectedSeatCount} | AI={selectedDifficulty} | Status=Ready";
            }
            else
            {
                _lobbySelectionSummary.AddToClassList("setup-summary-error");
                _lobbySelectionSummary.text =
                    $"Selection: Ruleset={ruleset} | Seats={selectedSeatCount} | AI={selectedDifficulty} | Status=Invalid ({error})";
            }
        }

        private static void RegisterValueChanged(
            BaseField<string> field,
            EventCallback<ChangeEvent<string>> callback)
        {
            if (field != null)
            {
                field.RegisterValueChangedCallback(callback);
            }
        }

        private static void UnregisterValueChanged(
            BaseField<string> field,
            EventCallback<ChangeEvent<string>> callback)
        {
            if (field != null)
            {
                field.UnregisterValueChangedCallback(callback);
            }
        }
    }
}

