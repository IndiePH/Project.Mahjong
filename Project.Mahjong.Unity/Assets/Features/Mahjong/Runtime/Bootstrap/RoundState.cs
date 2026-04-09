using ProjectMahjong.Features.Mahjong.Runtime.Gameplay.Dealing;
using ProjectMahjong.Features.Mahjong.Runtime.UI;

namespace ProjectMahjong.Features.Mahjong.Runtime.Bootstrap
{
    /// <summary>
    /// Data snapshot produced by RoundController.
    /// </summary>
    public readonly struct RoundState
    {
        public RoundState(
            MatchSetup setup,
            int wallTileCount,
            int remainingAfterDeal,
            int remainingAfterTurns,
            int turnsPlayed,
            int currentActiveSeat,
            DealResult dealResult,
            TurnLoopResult turnResult,
            string handSizesSummary,
            string discardSummary,
            string winningHandsSummary,
            string callSummary,
            string windowSummary,
            string lastCall)
        {
            Setup = setup;
            WallTileCount = wallTileCount;
            RemainingAfterDeal = remainingAfterDeal;
            RemainingAfterTurns = remainingAfterTurns;
            TurnsPlayed = turnsPlayed;
            CurrentActiveSeat = currentActiveSeat;
            DealResult = dealResult;
            TurnResult = turnResult;
            HandSizesSummary = handSizesSummary;
            DiscardSummary = discardSummary;
            WinningHandsSummary = winningHandsSummary;
            CallSummary = callSummary;
            WindowSummary = windowSummary;
            LastCall = lastCall;
        }

        public MatchSetup Setup { get; }
        public int WallTileCount { get; }
        public int RemainingAfterDeal { get; }
        public int RemainingAfterTurns { get; }
        public int TurnsPlayed { get; }
        public int CurrentActiveSeat { get; }
        public DealResult DealResult { get; }
        public TurnLoopResult TurnResult { get; }
        public string HandSizesSummary { get; }
        public string DiscardSummary { get; }
        public string WinningHandsSummary { get; }
        public string CallSummary { get; }
        public string WindowSummary { get; }
        public string LastCall { get; }

        public MahjongHudViewState ToHudViewState()
        {
            return new MahjongHudViewState(
                Setup.SeatCount,
                RemainingAfterTurns,
                TurnsPlayed,
                CurrentActiveSeat,
                TurnResult.RoundEndReason.ToString(),
                TurnResult.WinnerSeat,
                CallSummary,
                WindowSummary,
                LastCall,
                DiscardSummary,
                WinningHandsSummary);
        }
    }
}

