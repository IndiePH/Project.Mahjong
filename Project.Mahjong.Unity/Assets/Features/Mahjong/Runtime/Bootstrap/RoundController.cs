using System.Text;
using ProjectMahjong.Features.Mahjong.Data.Configs;
using ProjectMahjong.Features.Mahjong.Runtime.Gameplay.Dealing;

namespace ProjectMahjong.Features.Mahjong.Runtime.Bootstrap
{
    /// <summary>
    /// Orchestrates wall build, deal, and turn simulation into a single RoundState snapshot.
    /// </summary>
    public static class RoundController
    {
        public static bool TryRunRound(
            MatchSetup setup,
            TileSetDefinition full136TileSet,
            TileSetDefinition removeCharactersTileSet,
            bool useDeterministicShuffle,
            int shuffleSeed,
            int simulateMaxTurns,
            out RoundState state,
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
                state = default;
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
                state = default;
                error = dealError;
                return false;
            }

            if (!Dealer.TrySimulateTurnLoop(
                    wall,
                    dealResult.TilesConsumedFromWall,
                    dealResult,
                    setup.Rules,
                    setup.Rules.DrawPerTurn,
                    simulateMaxTurns,
                    out var turnResult,
                    out var turnError))
            {
                state = default;
                error = turnError;
                return false;
            }

            var handSizes = BuildHandSizesSummary(dealResult);
            var discards = BuildDiscardCountsSummary(turnResult);
            var winningHands = BuildWinningStatesSummary(turnResult);
            var calls = BuildCallSummary(turnResult.CallSummary);
            var windows = BuildWindowSummary(turnResult.ReactionWindowSummary);
            var lastCall = turnResult.HasLastResolvedCall
                ? $"{turnResult.LastResolvedCall}@S{turnResult.LastResolvedCaller}"
                : "None";

            state = new RoundState(
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

            error = string.Empty;
            return true;
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

