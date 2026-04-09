using System;
using System.Collections.Generic;

namespace ProjectMahjong.Features.Mahjong.Data.Configs
{
    public enum TileSetMode
    {
        Full136 = 0,
        RemoveCharacters = 1,
        Custom = 2
    }

    [Flags]
    public enum SuitFlags
    {
        None = 0,
        Dots = 1 << 0,
        Bamboo = 1 << 1,
        Characters = 1 << 2,
        All = Dots | Bamboo | Characters
    }

    public enum CallType
    {
        Win = 0,
        Kong = 1,
        Pong = 2,
        Chow = 3
    }

    public enum WinPatternProfile
    {
        Standard4Sets1Pair = 0,
        Custom = 1
    }

    public enum DifficultyLevel
    {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    public enum AiBehaviorStyle
    {
        Offensive = 0,
        Defensive = 1,
        Balanced = 2
    }

    public enum TileSuit
    {
        Dots = 0,
        Bamboo = 1,
        Characters = 2,
        Honor = 3
    }

    public enum HonorType
    {
        None = 0,
        East = 1,
        South = 2,
        West = 3,
        North = 4,
        Red = 5,
        Green = 6,
        White = 7
    }

    public readonly struct MahjongTile
    {
        public MahjongTile(int tileId, TileSuit suit, int rank, HonorType honor)
        {
            TileId = tileId;
            Suit = suit;
            Rank = rank;
            Honor = honor;
        }

        public int TileId { get; }
        public TileSuit Suit { get; }
        public int Rank { get; }
        public HonorType Honor { get; }
    }

    public static class WallBuilder
    {
        public static bool TryBuildShuffledWall(
            RuleSetConfig rules,
            TileSetDefinition full136Definition,
            TileSetDefinition removeCharactersDefinition,
            int? seed,
            out List<MahjongTile> wall,
            out string error)
        {
            if (rules == null)
            {
                wall = null;
                error = "Wall build failed: RuleSetConfig is null.";
                return false;
            }

            var definition = ResolveDefinition(rules, full136Definition, removeCharactersDefinition);
            if (definition == null)
            {
                wall = null;
                error = $"Wall build failed: Missing tile set definition for mode '{rules.TileSetMode}'.";
                return false;
            }

            var useSuitOverride = rules.TileSetMode == TileSetMode.Custom;
            wall = definition.BuildTiles(rules.CustomIncludedSuits, useSuitOverride);
            if (wall.Count == 0)
            {
                error = "Wall build failed: Generated tile set is empty.";
                return false;
            }

            if (rules.WallTileCountOverride > 0 && rules.WallTileCountOverride < wall.Count)
            {
                wall.RemoveRange(rules.WallTileCountOverride, wall.Count - rules.WallTileCountOverride);
            }

            ShuffleInPlace(wall, seed);
            error = string.Empty;
            return true;
        }

        private static TileSetDefinition ResolveDefinition(
            RuleSetConfig rules,
            TileSetDefinition full136Definition,
            TileSetDefinition removeCharactersDefinition)
        {
            return rules.TileSetMode switch
            {
                TileSetMode.Full136 => full136Definition,
                TileSetMode.RemoveCharacters => removeCharactersDefinition,
                TileSetMode.Custom => full136Definition,
                _ => full136Definition
            };
        }

        private static void ShuffleInPlace(List<MahjongTile> tiles, int? seed)
        {
            var random = seed.HasValue ? new Random(seed.Value) : new Random();
            for (var i = tiles.Count - 1; i > 0; i--)
            {
                var j = random.Next(0, i + 1);
                var temp = tiles[i];
                tiles[i] = tiles[j];
                tiles[j] = temp;
            }
        }
    }
}

