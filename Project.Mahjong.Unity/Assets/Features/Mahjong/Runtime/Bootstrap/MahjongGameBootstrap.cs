using UnityEngine;
using ProjectMahjong.Features.Mahjong.Data.Configs;
using ProjectMahjong.Features.Mahjong.Runtime.UI;

namespace ProjectMahjong.Features.Mahjong.Runtime.Bootstrap
{
    public sealed class MahjongGameBootstrap : MonoBehaviour
    {
        [SerializeField] private MatchSetupLoader _setupLoader;
        [SerializeField] private UIScreenCoordinator _screenCoordinator;
        [SerializeField] private RoundRuntimeController _roundRuntimeController;
        [SerializeField] private MahjongHudBinder _hudBinder;
        [SerializeField] private TileSetDefinition _full136TileSet;
        [SerializeField] private TileSetDefinition _removeCharactersTileSet;

        [Header("Debug / Determinism")]
        [SerializeField] private bool _useDeterministicShuffle;
        [SerializeField] private int _shuffleSeed = 12345;
        [SerializeField] private int _simulateMaxTurns = 8;

        private void OnEnable()
        {
            if (_roundRuntimeController != null)
            {
                _roundRuntimeController.RoundStateChanged += OnRoundStateChanged;
            }
        }

        private void OnDisable()
        {
            if (_roundRuntimeController != null)
            {
                _roundRuntimeController.RoundStateChanged -= OnRoundStateChanged;
            }
        }

        public void OnMatchSetupLoaded()
        {
            if (_setupLoader == null)
            {
                Debug.LogError("Match setup loaded, but MatchSetupLoader reference is missing.", this);
                return;
            }

            if (!_setupLoader.HasSetup)
            {
                Debug.LogError("Match setup loaded event received, but loader has no setup.", this);
                return;
            }

            var setup = _setupLoader.CurrentSetup;

            Debug.Log(
                $"Setup loaded: rules={setup.Rules.DisplayName}, scoring={setup.Scoring.DisplayName}, seats={setup.SeatCount}, ai={setup.AiDifficulty}",
                this);

            if (_roundRuntimeController == null)
            {
                Debug.LogError("Match setup loaded, but RoundRuntimeController reference is missing.", this);
                return;
            }

            if (!_roundRuntimeController.InitializeRound(
                    setup,
                    _full136TileSet,
                    _removeCharactersTileSet,
                    _useDeterministicShuffle,
                    _shuffleSeed,
                    _simulateMaxTurns,
                    out var roundError))
            {
                Debug.LogError(roundError, this);
                return;
            }

            var roundState = _roundRuntimeController.CurrentState;
            _screenCoordinator?.GoToGameplay();

            Debug.Log(
                $"Wall built: tiles={roundState.WallTileCount}, tileSetMode={setup.Rules.TileSetMode}, deterministicShuffle={_useDeterministicShuffle}",
                this);

            Debug.Log(
                $"Dealt starting hands: seats={setup.SeatCount}, tilesPerHand={setup.Rules.StartingHandTileCount}, handSizes={roundState.HandSizesSummary}, remainingWall={roundState.RemainingAfterDeal}",
                this);

            Debug.Log(
                $"Turn loop simulated: turns={roundState.TurnResult.TurnsPlayed}, drawPerTurn={setup.Rules.DrawPerTurn}, roundEnd={roundState.TurnResult.RoundEndReason}, winnerSeat={roundState.TurnResult.WinnerSeat}, calls={roundState.CallSummary}, windows={roundState.WindowSummary}, lastCall={roundState.LastCall}, discards={roundState.DiscardSummary}, winningHands={roundState.WinningHandsSummary}, remainingWall={roundState.RemainingAfterTurns}",
                this);

            _hudBinder?.Apply(roundState.ToHudViewState());
        }

        public void OnMatchSetupFailed(string error)
        {
            Debug.LogError(error, this);
        }

        private void OnRoundStateChanged(RoundState roundState)
        {
            _hudBinder?.Apply(roundState.ToHudViewState());

            if (roundState.TurnResult.RoundEndReason == Gameplay.Dealing.RoundEndReason.Win ||
                roundState.TurnResult.RoundEndReason == Gameplay.Dealing.RoundEndReason.WallExhausted)
            {
                _screenCoordinator?.GoToResults();
            }
        }
    }
}

