using System;
using System.Collections.Generic;
using ProjectMahjong.Features.Mahjong.Data.Configs;

namespace ProjectMahjong.Features.Mahjong.Runtime.Gameplay.Dealing
{
    public enum RoundEndReason
    {
        None = 0,
        Win = 1,
        WallExhausted = 2,
        TurnLimitReached = 3
    }

    public static class Dealer
    {
        // Deterministic simulation rule: reaction windows below this duration timeout.
        private const int MinReactionWindowMsForAutoResolve = 250;

        public static bool TryDealStartingHands(
            IReadOnlyList<MahjongTile> wall,
            int seatCount,
            int startingHandTileCount,
            out DealResult result,
            out string error)
        {
            if (wall == null)
            {
                result = default;
                error = "Deal failed: Wall is null.";
                return false;
            }

            if (seatCount < 1 || seatCount > 4)
            {
                result = default;
                error = $"Deal failed: seatCount must be in [1..4], got {seatCount}.";
                return false;
            }

            if (startingHandTileCount < 1)
            {
                result = default;
                error = $"Deal failed: startingHandTileCount must be >= 1, got {startingHandTileCount}.";
                return false;
            }

            var required = seatCount * startingHandTileCount;
            if (wall.Count < required)
            {
                result = default;
                error = $"Deal failed: wall has {wall.Count} tiles, requires {required}.";
                return false;
            }

            var hands = new List<MahjongTile>[seatCount];
            for (var s = 0; s < seatCount; s++)
            {
                hands[s] = new List<MahjongTile>(startingHandTileCount);
            }

            // Simple sequential deal: seat 0..N, repeated.
            // Can be swapped later to match real-world dealing order/animation without changing data contracts.
            var wallIndex = 0;
            for (var i = 0; i < startingHandTileCount; i++)
            {
                for (var s = 0; s < seatCount; s++)
                {
                    hands[s].Add(wall[wallIndex++]);
                }
            }

            result = new DealResult(hands, wallIndex);
            error = string.Empty;
            return true;
        }

        public static bool TrySimulateTurnLoop(
            IReadOnlyList<MahjongTile> wall,
            int initialWallIndex,
            DealResult dealResult,
            RuleSetConfig rules,
            int drawPerTurn,
            int maxTurns,
            out TurnLoopResult result,
            out string error,
            IReadOnlyDictionary<int, int> seat0DiscardPlanByTurn = null)
        {
            if (wall == null)
            {
                result = default;
                error = "Turn loop failed: wall is null.";
                return false;
            }

            if (dealResult.Hands == null || dealResult.Hands.Length == 0)
            {
                result = default;
                error = "Turn loop failed: deal result has no hands.";
                return false;
            }

            if (rules == null)
            {
                result = default;
                error = "Turn loop failed: RuleSetConfig is null.";
                return false;
            }

            if (drawPerTurn < 1)
            {
                result = default;
                error = $"Turn loop failed: drawPerTurn must be >= 1, got {drawPerTurn}.";
                return false;
            }

            if (maxTurns < 0)
            {
                result = default;
                error = $"Turn loop failed: maxTurns must be >= 0, got {maxTurns}.";
                return false;
            }

            var seatCount = dealResult.Hands.Length;
            var hands = new List<MahjongTile>[seatCount];
            var discardPiles = new List<MahjongTile>[seatCount];

            for (var seat = 0; seat < seatCount; seat++)
            {
                hands[seat] = new List<MahjongTile>(dealResult.Hands[seat]);
                discardPiles[seat] = new List<MahjongTile>();
            }

            var wallIndex = initialWallIndex;
            var turnsPlayed = 0;
            var activeSeat = 0;
            var winnerSeat = -1;
            var roundEndReason = RoundEndReason.None;
            var callSummary = new CallSummary(0, 0, 0, 0);
            var lastResolvedCall = CallType.Win;
            var hasLastResolvedCall = false;
            var lastResolvedCaller = -1;
            var reactionWindowSummary = new ReactionWindowSummary(0, 0, 0, 0);

            while (turnsPlayed < maxTurns)
            {
                if (wallIndex + drawPerTurn > wall.Count)
                {
                    roundEndReason = RoundEndReason.WallExhausted;
                    break;
                }

                for (var draw = 0; draw < drawPerTurn; draw++)
                {
                    hands[activeSeat].Add(wall[wallIndex++]);
                }

                if (HandValidator.IsStandardWinningHand(hands[activeSeat]))
                {
                    turnsPlayed++;
                    winnerSeat = activeSeat;
                    roundEndReason = RoundEndReason.Win;
                    break;
                }

                if (hands[activeSeat].Count == 0)
                {
                    result = default;
                    error = $"Turn loop failed: seat {activeSeat} has no tile to discard.";
                    return false;
                }

                var discardIndex = hands[activeSeat].Count - 1;
                var turnNumber = turnsPlayed + 1;
                if (activeSeat == 0 &&
                    seat0DiscardPlanByTurn != null &&
                    seat0DiscardPlanByTurn.TryGetValue(turnNumber, out var plannedIndex) &&
                    plannedIndex >= 0 &&
                    plannedIndex < hands[activeSeat].Count)
                {
                    discardIndex = plannedIndex;
                }
                var discarded = hands[activeSeat][discardIndex];
                hands[activeSeat].RemoveAt(discardIndex);
                discardPiles[activeSeat].Add(discarded);

                var reaction = ResolveReaction(
                    discarded,
                    activeSeat,
                    hands,
                    rules);

                if (reaction.HasReaction)
                {
                    if (rules.EnableReactionWindow)
                    {
                        reactionWindowSummary = new ReactionWindowSummary(
                            reactionWindowSummary.OpenedCount + 1,
                            reactionWindowSummary.AcceptedCount,
                            reactionWindowSummary.TimedOutCount,
                            reactionWindowSummary.ImmediateResolveCount);

                        var resolvesInTime = rules.ReactionWindowMs >= MinReactionWindowMsForAutoResolve;
                        if (!resolvesInTime)
                        {
                            reactionWindowSummary = new ReactionWindowSummary(
                                reactionWindowSummary.OpenedCount,
                                reactionWindowSummary.AcceptedCount,
                                reactionWindowSummary.TimedOutCount + 1,
                                reactionWindowSummary.ImmediateResolveCount);

                            // Window timed out: no call is applied this turn.
                            reaction = new ReactionResolution(false, CallType.Win, -1);
                        }
                        else
                        {
                            reactionWindowSummary = new ReactionWindowSummary(
                                reactionWindowSummary.OpenedCount,
                                reactionWindowSummary.AcceptedCount + 1,
                                reactionWindowSummary.TimedOutCount,
                                reactionWindowSummary.ImmediateResolveCount);
                        }
                    }
                    else
                    {
                        reactionWindowSummary = new ReactionWindowSummary(
                            reactionWindowSummary.OpenedCount,
                            reactionWindowSummary.AcceptedCount,
                            reactionWindowSummary.TimedOutCount,
                            reactionWindowSummary.ImmediateResolveCount + 1);
                    }
                }

                if (reaction.HasReaction)
                {
                    callSummary = IncrementCallSummary(callSummary, reaction.ResolvedCall);
                    lastResolvedCall = reaction.ResolvedCall;
                    hasLastResolvedCall = true;
                    lastResolvedCaller = reaction.CallerSeat;

                    if (reaction.ResolvedCall == CallType.Win)
                    {
                        turnsPlayed++;
                        winnerSeat = reaction.CallerSeat;
                        roundEndReason = RoundEndReason.Win;
                        break;
                    }
                }

                turnsPlayed++;
                activeSeat = (activeSeat + 1) % seatCount;
            }

            if (roundEndReason == RoundEndReason.None && turnsPlayed >= maxTurns)
            {
                roundEndReason = RoundEndReason.TurnLimitReached;
            }

            result = new TurnLoopResult(
                turnsPlayed,
                wallIndex,
                hands,
                discardPiles,
                roundEndReason,
                winnerSeat,
                callSummary,
                hasLastResolvedCall,
                lastResolvedCall,
                lastResolvedCaller,
                reactionWindowSummary);
            error = string.Empty;
            return true;
        }

        private static ReactionResolution ResolveReaction(
            MahjongTile discarded,
            int discarderSeat,
            List<MahjongTile>[] hands,
            RuleSetConfig rules)
        {
            var seatCount = hands.Length;
            var candidatesByCall = new Dictionary<CallType, int>();
            var nextSeat = (discarderSeat + 1) % seatCount;

            for (var seat = 0; seat < seatCount; seat++)
            {
                if (seat == discarderSeat)
                {
                    continue;
                }

                var hand = hands[seat];

                if (CanWinOnDiscard(hand, discarded))
                {
                    TrySetFirst(candidatesByCall, CallType.Win, seat);
                }

                if (rules.AllowKong && CanKongOnDiscard(hand, discarded))
                {
                    TrySetFirst(candidatesByCall, CallType.Kong, seat);
                }

                if (rules.AllowPong && CanPongOnDiscard(hand, discarded))
                {
                    TrySetFirst(candidatesByCall, CallType.Pong, seat);
                }

                if (rules.AllowChow && seat == nextSeat && CanChowOnDiscard(hand, discarded))
                {
                    TrySetFirst(candidatesByCall, CallType.Chow, seat);
                }
            }

            var priority = rules.CallPriority;
            if (priority == null || priority.Length == 0)
            {
                priority = new[] { CallType.Win, CallType.Kong, CallType.Pong, CallType.Chow };
            }

            for (var i = 0; i < priority.Length; i++)
            {
                var call = priority[i];
                if (!candidatesByCall.TryGetValue(call, out var seat))
                {
                    continue;
                }

                return new ReactionResolution(true, call, seat);
            }

            return new ReactionResolution(false, CallType.Win, -1);
        }

        private static void TrySetFirst(Dictionary<CallType, int> map, CallType callType, int seat)
        {
            if (!map.ContainsKey(callType))
            {
                map.Add(callType, seat);
            }
        }

        private static bool CanWinOnDiscard(IReadOnlyList<MahjongTile> hand, MahjongTile discarded)
        {
            var temp = new List<MahjongTile>(hand.Count + 1);
            for (var i = 0; i < hand.Count; i++)
            {
                temp.Add(hand[i]);
            }

            temp.Add(discarded);
            return HandValidator.IsStandardWinningHand(temp);
        }

        private static bool CanPongOnDiscard(IReadOnlyList<MahjongTile> hand, MahjongTile discarded)
        {
            return CountSameKind(hand, discarded) >= 2;
        }

        private static bool CanKongOnDiscard(IReadOnlyList<MahjongTile> hand, MahjongTile discarded)
        {
            return CountSameKind(hand, discarded) >= 3;
        }

        private static bool CanChowOnDiscard(IReadOnlyList<MahjongTile> hand, MahjongTile discarded)
        {
            if (discarded.Suit == TileSuit.Honor)
            {
                return false;
            }

            var rank = discarded.Rank;
            var hasLeft = HasSameSuitRank(hand, discarded.Suit, rank - 2) && HasSameSuitRank(hand, discarded.Suit, rank - 1);
            var hasMid = HasSameSuitRank(hand, discarded.Suit, rank - 1) && HasSameSuitRank(hand, discarded.Suit, rank + 1);
            var hasRight = HasSameSuitRank(hand, discarded.Suit, rank + 1) && HasSameSuitRank(hand, discarded.Suit, rank + 2);
            return hasLeft || hasMid || hasRight;
        }

        private static bool HasSameSuitRank(IReadOnlyList<MahjongTile> hand, TileSuit suit, int rank)
        {
            if (rank < 1 || rank > 9)
            {
                return false;
            }

            for (var i = 0; i < hand.Count; i++)
            {
                var tile = hand[i];
                if (tile.Suit == suit && tile.Rank == rank)
                {
                    return true;
                }
            }

            return false;
        }

        private static int CountSameKind(IReadOnlyList<MahjongTile> hand, MahjongTile reference)
        {
            var count = 0;
            for (var i = 0; i < hand.Count; i++)
            {
                if (IsSameKind(hand[i], reference))
                {
                    count++;
                }
            }

            return count;
        }

        private static bool IsSameKind(MahjongTile a, MahjongTile b)
        {
            return a.Suit == b.Suit && a.Rank == b.Rank && a.Honor == b.Honor;
        }

        private static CallSummary IncrementCallSummary(CallSummary summary, CallType callType)
        {
            return callType switch
            {
                CallType.Win => new CallSummary(summary.WinCount + 1, summary.KongCount, summary.PongCount, summary.ChowCount),
                CallType.Kong => new CallSummary(summary.WinCount, summary.KongCount + 1, summary.PongCount, summary.ChowCount),
                CallType.Pong => new CallSummary(summary.WinCount, summary.KongCount, summary.PongCount + 1, summary.ChowCount),
                CallType.Chow => new CallSummary(summary.WinCount, summary.KongCount, summary.PongCount, summary.ChowCount + 1),
                _ => summary
            };
        }
    }

    public readonly struct DealResult
    {
        public DealResult(IReadOnlyList<MahjongTile>[] hands, int tilesConsumedFromWall)
        {
            Hands = hands ?? Array.Empty<IReadOnlyList<MahjongTile>>();
            TilesConsumedFromWall = tilesConsumedFromWall;
        }

        public IReadOnlyList<MahjongTile>[] Hands { get; }
        public int TilesConsumedFromWall { get; }
    }

    public readonly struct TurnLoopResult
    {
        public TurnLoopResult(
            int turnsPlayed,
            int wallIndex,
            IReadOnlyList<MahjongTile>[] hands,
            IReadOnlyList<MahjongTile>[] discardPiles,
            RoundEndReason roundEndReason,
            int winnerSeat,
            CallSummary callSummary,
            bool hasLastResolvedCall,
            CallType lastResolvedCall,
            int lastResolvedCaller,
            ReactionWindowSummary reactionWindowSummary)
        {
            TurnsPlayed = turnsPlayed;
            WallIndex = wallIndex;
            Hands = hands ?? Array.Empty<IReadOnlyList<MahjongTile>>();
            DiscardPiles = discardPiles ?? Array.Empty<IReadOnlyList<MahjongTile>>();
            RoundEndReason = roundEndReason;
            WinnerSeat = winnerSeat;
            CallSummary = callSummary;
            HasLastResolvedCall = hasLastResolvedCall;
            LastResolvedCall = lastResolvedCall;
            LastResolvedCaller = lastResolvedCaller;
            ReactionWindowSummary = reactionWindowSummary;
        }

        public int TurnsPlayed { get; }
        public int WallIndex { get; }
        public IReadOnlyList<MahjongTile>[] Hands { get; }
        public IReadOnlyList<MahjongTile>[] DiscardPiles { get; }
        public RoundEndReason RoundEndReason { get; }
        public int WinnerSeat { get; }
        public CallSummary CallSummary { get; }
        public bool HasLastResolvedCall { get; }
        public CallType LastResolvedCall { get; }
        public int LastResolvedCaller { get; }
        public ReactionWindowSummary ReactionWindowSummary { get; }
    }

    public readonly struct CallSummary
    {
        public CallSummary(int winCount, int kongCount, int pongCount, int chowCount)
        {
            WinCount = winCount;
            KongCount = kongCount;
            PongCount = pongCount;
            ChowCount = chowCount;
        }

        public int WinCount { get; }
        public int KongCount { get; }
        public int PongCount { get; }
        public int ChowCount { get; }
    }

    public readonly struct ReactionWindowSummary
    {
        public ReactionWindowSummary(int openedCount, int acceptedCount, int timedOutCount, int immediateResolveCount)
        {
            OpenedCount = openedCount;
            AcceptedCount = acceptedCount;
            TimedOutCount = timedOutCount;
            ImmediateResolveCount = immediateResolveCount;
        }

        public int OpenedCount { get; }
        public int AcceptedCount { get; }
        public int TimedOutCount { get; }
        public int ImmediateResolveCount { get; }
    }

    internal readonly struct ReactionResolution
    {
        public ReactionResolution(bool hasReaction, CallType resolvedCall, int callerSeat)
        {
            HasReaction = hasReaction;
            ResolvedCall = resolvedCall;
            CallerSeat = callerSeat;
        }

        public bool HasReaction { get; }
        public CallType ResolvedCall { get; }
        public int CallerSeat { get; }
    }
}

