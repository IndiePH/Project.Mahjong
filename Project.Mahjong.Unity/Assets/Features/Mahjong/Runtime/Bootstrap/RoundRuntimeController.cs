using System;
using System.Text;
using System.Collections.Generic;
using ProjectMahjong.Features.Mahjong.Data.Configs;
using ProjectMahjong.Features.Mahjong.Runtime.Gameplay.Dealing;
using UnityEngine;

namespace ProjectMahjong.Features.Mahjong.Runtime.Bootstrap
{
    /// <summary>
    /// Holds an active round instance and advances it turn-by-turn for interactive prototype flow.
    /// </summary>
    public sealed class RoundRuntimeController : MonoBehaviour
    {
        private MatchSetup _setup;
        private bool _hasRound;
        private int _maxTurns;
        private List<MahjongTile> _wall;
        private DealResult _dealResult;
        private RoundState _currentState;
        private int _simulatedTurns;
        private readonly Dictionary<int, int> _seat0DiscardPlanByTurn = new();

        public event Action<RoundState> RoundStateChanged;

        public bool HasRound => _hasRound;
        public RoundState CurrentState => _currentState;

        public bool InitializeRound(
            MatchSetup setup,
            TileSetDefinition full136TileSet,
            TileSetDefinition removeCharactersTileSet,
            bool useDeterministicShuffle,
            int shuffleSeed,
            int maxTurns,
            out string error)
        {
            if (!WallBuilder.TryBuildShuffledWall(
                    setup.Rules,
                    full136TileSet,
                    removeCharactersTileSet,
                    useDeterministicShuffle ? shuffleSeed : null,
                    out var wall,
                    out var wallError))
            {
                error = wallError;
                return false;
            }

            if (!Dealer.TryDealStartingHands(
                    wall,
                    setup.SeatCount,
                    setup.Rules.StartingHandTileCount,
                    out var dealResult,
                    out var dealError))
            {
                error = dealError;
                return false;
            }

            _setup = setup;
            _maxTurns = Mathf.Max(1, maxTurns);
            _wall = wall;
            _dealResult = dealResult;
            _simulatedTurns = 0;
            _seat0DiscardPlanByTurn.Clear();
            _hasRound = true;

            if (!RecomputeState(out error))
            {
                _hasRound = false;
                return false;
            }

            return true;
        }

        public bool StepTurn(out string error)
        {
            if (!_hasRound)
            {
                error = "RoundRuntimeController: no active round.";
                return false;
            }

            if (_currentState.TurnResult.RoundEndReason == RoundEndReason.Win ||
                _currentState.TurnResult.RoundEndReason == RoundEndReason.WallExhausted)
            {
                error = "RoundRuntimeController: round already ended.";
                return false;
            }

            if (_simulatedTurns < _maxTurns)
            {
                _simulatedTurns++;
            }

            return RecomputeState(out error);
        }

        public bool Discard(int tileIndex, out string error)
        {
            if (!_hasRound)
            {
                error = "RoundRuntimeController: no active round.";
                return false;
            }

            if (_currentState.CurrentActiveSeat != 0)
            {
                error = $"RoundRuntimeController: seat S0 cannot discard now. Active seat is S{_currentState.CurrentActiveSeat}.";
                return false;
            }

            var hand = _currentState.TurnResult.Hands.Length > 0 ? _currentState.TurnResult.Hands[0] : null;
            if (hand == null || hand.Count == 0)
            {
                error = "RoundRuntimeController: player hand is empty.";
                return false;
            }

            if (tileIndex < 0 || tileIndex >= hand.Count)
            {
                error = $"RoundRuntimeController: discard index out of range ({tileIndex}).";
                return false;
            }

            var nextTurn = _simulatedTurns + 1;
            _seat0DiscardPlanByTurn[nextTurn] = tileIndex;
            return StepTurn(out error);
        }

        private bool RecomputeState(out string error)
        {
            if (!Dealer.TrySimulateTurnLoop(
                    _wall,
                    _dealResult.TilesConsumedFromWall,
                    _dealResult,
                    _setup.Rules,
                    _setup.Rules.DrawPerTurn,
                    _simulatedTurns,
                    out var turnResult,
                    out var turnError,
                    _seat0DiscardPlanByTurn))
            {
                error = turnError;
                return false;
            }

            var state = BuildRoundState(_setup, _wall, _dealResult, turnResult);
            _currentState = state;
            RoundStateChanged?.Invoke(state);
            error = string.Empty;
            return true;
        }

        private static RoundState BuildRoundState(
            MatchSetup setup,
            List<MahjongTile> wall,
            DealResult dealResult,
            TurnLoopResult turnResult)
        {
            var handSizes = BuildHandSizesSummary(dealResult);
            var discards = BuildDiscardCountsSummary(turnResult);
            var winningHands = BuildWinningStatesSummary(turnResult);
            var calls = BuildCallSummary(turnResult.CallSummary);
            var windows = BuildWindowSummary(turnResult.ReactionWindowSummary);
            var lastCall = turnResult.HasLastResolvedCall
                ? $"{turnResult.LastResolvedCall}@S{turnResult.LastResolvedCaller}"
                : "None";

            return new RoundState(
                setup,
                wall.Count,
                wall.Count - dealResult.TilesConsumedFromWall,
                wall.Count - turnResult.WallIndex,
                turnResult.TurnsPlayed,
                ResolveCurrentActiveSeat(setup.SeatCount, turnResult),
                dealResult,
                turnResult,
                handSizes,
                discards,
                winningHands,
                calls,
                windows,
                lastCall);
        }

        private static int ResolveCurrentActiveSeat(int seatCount, TurnLoopResult turnResult)
        {
            if (seatCount <= 0)
            {
                return 0;
            }

            if (turnResult.RoundEndReason == RoundEndReason.Win && turnResult.WinnerSeat >= 0)
            {
                return turnResult.WinnerSeat;
            }

            return turnResult.TurnsPlayed % seatCount;
        }

        private static string BuildHandSizesSummary(DealResult dealResult)
        {
            var sb = new StringBuilder(64);
            for (var seat = 0; seat < dealResult.Hands.Length; seat++)
            {
                if (seat > 0)
                {
                    sb.Append(", ");
                }

                sb.Append("S");
                sb.Append(seat);
                sb.Append("=");
                sb.Append(dealResult.Hands[seat].Count);
            }

            return sb.ToString();
        }

        private static string BuildDiscardCountsSummary(TurnLoopResult result)
        {
            var sb = new StringBuilder(64);
            for (var seat = 0; seat < result.DiscardPiles.Length; seat++)
            {
                if (seat > 0)
                {
                    sb.Append(", ");
                }

                sb.Append("S");
                sb.Append(seat);
                sb.Append("=");
                sb.Append(result.DiscardPiles[seat].Count);
            }

            return sb.ToString();
        }

        private static string BuildWinningStatesSummary(TurnLoopResult result)
        {
            var sb = new StringBuilder(64);
            for (var seat = 0; seat < result.Hands.Length; seat++)
            {
                if (seat > 0)
                {
                    sb.Append(", ");
                }

                sb.Append("S");
                sb.Append(seat);
                sb.Append("=");
                sb.Append(HandValidator.IsStandardWinningHand(result.Hands[seat]));
            }

            return sb.ToString();
        }

        private static string BuildCallSummary(CallSummary summary)
        {
            return $"W={summary.WinCount}, K={summary.KongCount}, P={summary.PongCount}, C={summary.ChowCount}";
        }

        private static string BuildWindowSummary(ReactionWindowSummary summary)
        {
            return $"opened={summary.OpenedCount}, accepted={summary.AcceptedCount}, timedOut={summary.TimedOutCount}, immediate={summary.ImmediateResolveCount}";
        }
    }
}

