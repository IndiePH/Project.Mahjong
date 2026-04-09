namespace ProjectMahjong.Features.Mahjong.Runtime.UI
{
    /// <summary>
    /// Snapshot payload for HUD binding.
    /// Keep this data-only to preserve clear state flow and simple testing.
    /// </summary>
    public readonly struct MahjongHudViewState
    {
        public MahjongHudViewState(
            int seatCount,
            int wallRemaining,
            int turnsPlayed,
            int currentActiveSeat,
            string roundEnd,
            int winnerSeat,
            string calls,
            string windows,
            string lastCall,
            string discards,
            string winningHands)
        {
            SeatCount = seatCount;
            WallRemaining = wallRemaining;
            TurnsPlayed = turnsPlayed;
            CurrentActiveSeat = currentActiveSeat;
            RoundEnd = roundEnd;
            WinnerSeat = winnerSeat;
            Calls = calls;
            Windows = windows;
            LastCall = lastCall;
            Discards = discards;
            WinningHands = winningHands;
        }

        public int SeatCount { get; }
        public int WallRemaining { get; }
        public int TurnsPlayed { get; }
        public int CurrentActiveSeat { get; }
        public string RoundEnd { get; }
        public int WinnerSeat { get; }
        public string Calls { get; }
        public string Windows { get; }
        public string LastCall { get; }
        public string Discards { get; }
        public string WinningHands { get; }
    }
}

