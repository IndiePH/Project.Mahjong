using System.Collections.Generic;
using ProjectMahjong.Features.Mahjong.Data.Configs;

namespace ProjectMahjong.Features.Mahjong.Runtime.Gameplay.Dealing
{
    /// <summary>
    /// Validates the standard Mahjong win condition: 4 melds + 1 pair.
    /// </summary>
    public static class HandValidator
    {
        private const int TileKindCount = 34;

        public static bool IsStandardWinningHand(IReadOnlyList<MahjongTile> tiles)
        {
            if (tiles == null)
            {
                return false;
            }

            // Standard hand must have 3n + 2 tiles.
            if (tiles.Count % 3 != 2)
            {
                return false;
            }

            var counts = new int[TileKindCount];
            for (var i = 0; i < tiles.Count; i++)
            {
                var kind = GetTileKindIndex(tiles[i]);
                if (kind < 0 || kind >= TileKindCount)
                {
                    return false;
                }

                counts[kind]++;
            }

            // Try each possible pair, then check if the rest can be decomposed into melds.
            for (var pairKind = 0; pairKind < TileKindCount; pairKind++)
            {
                if (counts[pairKind] < 2)
                {
                    continue;
                }

                counts[pairKind] -= 2;
                var canWin = CanDecomposeMelds(counts);
                counts[pairKind] += 2;

                if (canWin)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CanDecomposeMelds(int[] counts)
        {
            var first = FindFirstNonZero(counts);
            if (first == -1)
            {
                return true;
            }

            // Option 1: triplet
            if (counts[first] >= 3)
            {
                counts[first] -= 3;
                if (CanDecomposeMelds(counts))
                {
                    counts[first] += 3;
                    return true;
                }

                counts[first] += 3;
            }

            // Option 2: sequence (only for suit tiles rank 1-7 start positions)
            if (IsSuitKind(first))
            {
                var rank = first % 9;
                if (rank <= 6 && counts[first + 1] > 0 && counts[first + 2] > 0)
                {
                    counts[first]--;
                    counts[first + 1]--;
                    counts[first + 2]--;

                    if (CanDecomposeMelds(counts))
                    {
                        counts[first]++;
                        counts[first + 1]++;
                        counts[first + 2]++;
                        return true;
                    }

                    counts[first]++;
                    counts[first + 1]++;
                    counts[first + 2]++;
                }
            }

            return false;
        }

        private static int FindFirstNonZero(int[] counts)
        {
            for (var i = 0; i < counts.Length; i++)
            {
                if (counts[i] > 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool IsSuitKind(int kindIndex)
        {
            // 0..26 = three suits (9 each), 27..33 = honors
            return kindIndex >= 0 && kindIndex < 27;
        }

        private static int GetTileKindIndex(MahjongTile tile)
        {
            return tile.Suit switch
            {
                TileSuit.Dots => tile.Rank - 1,
                TileSuit.Bamboo => 9 + (tile.Rank - 1),
                TileSuit.Characters => 18 + (tile.Rank - 1),
                TileSuit.Honor => 27 + ((int)tile.Honor - 1),
                _ => -1
            };
        }
    }
}

