using UnityEngine;
using System.Collections.Generic;

namespace ProjectMahjong.Features.Mahjong.Data.Configs
{
    [CreateAssetMenu(
        menuName = "Project Mahjong/Configs/Rule Set",
        fileName = "RuleSetConfig")]
    public sealed class RuleSetConfig : ScriptableObject
    {
        [Header("Identity")]
        [Min(1)]
        [SerializeField] private int _schemaVersion = 1;
        [SerializeField] private string _ruleSetId = "rules.default";
        [SerializeField] private string _displayName = "Default Rules";

        [Header("Players and Tiles")]
        [Range(1, 4)]
        [SerializeField] private int _supportedPlayerCount = 4;
        [SerializeField] private TileSetMode _tileSetMode = TileSetMode.Full136;
        [SerializeField] private SuitFlags _customIncludedSuits = SuitFlags.All;
        [SerializeField] private int _wallTileCountOverride;

        [Header("Calls")]
        [SerializeField] private bool _allowChow = true;
        [SerializeField] private bool _allowPong = true;
        [SerializeField] private bool _allowKong = true;
        [SerializeField] private CallType[] _callPriority =
        {
            CallType.Win,
            CallType.Kong,
            CallType.Pong,
            CallType.Chow
        };

        [Header("Turn and Win")]
        [Min(1)]
        [SerializeField] private int _startingHandTileCount = 13;
        [Min(1)]
        [SerializeField] private int _drawPerTurn = 1;
        [SerializeField] private bool _enableReactionWindow = true;
        [Min(0)]
        [SerializeField] private int _reactionWindowMs = 4000;
        [SerializeField] private WinPatternProfile _winPatternProfile = WinPatternProfile.Standard4Sets1Pair;
        [SerializeField] private bool _allowSelfDrawBonus = true;

        [Header("Metadata")]
        [SerializeField] private string _notes;

        public int SchemaVersion => _schemaVersion;
        public string RuleSetId => _ruleSetId;
        public string DisplayName => _displayName;
        public int SupportedPlayerCount => _supportedPlayerCount;
        public TileSetMode TileSetMode => _tileSetMode;
        public SuitFlags CustomIncludedSuits => _customIncludedSuits;
        public int WallTileCountOverride => _wallTileCountOverride;
        public bool AllowChow => _allowChow;
        public bool AllowPong => _allowPong;
        public bool AllowKong => _allowKong;
        public CallType[] CallPriority => _callPriority;
        public int StartingHandTileCount => _startingHandTileCount;
        public int DrawPerTurn => _drawPerTurn;
        public bool EnableReactionWindow => _enableReactionWindow;
        public int ReactionWindowMs => _reactionWindowMs;
        public WinPatternProfile WinPatternProfile => _winPatternProfile;
        public bool AllowSelfDrawBonus => _allowSelfDrawBonus;
        public string Notes => _notes;

        private void OnValidate()
        {
            if (_schemaVersion < 1)
            {
                _schemaVersion = 1;
            }

            if (_supportedPlayerCount < 1)
            {
                _supportedPlayerCount = 1;
            }
            else if (_supportedPlayerCount > 4)
            {
                _supportedPlayerCount = 4;
            }

            if (_startingHandTileCount < 1)
            {
                _startingHandTileCount = 1;
            }

            if (_drawPerTurn < 1)
            {
                _drawPerTurn = 1;
            }

            if (!_enableReactionWindow)
            {
                _reactionWindowMs = 0;
            }
            else if (_reactionWindowMs <= 0)
            {
                _reactionWindowMs = 1000;
            }
        }
    }

    [CreateAssetMenu(
        menuName = "Project Mahjong/Definitions/Tile Set",
        fileName = "TileSetDefinition")]
    public sealed class TileSetDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _tileSetId = "tileset.default";
        [SerializeField] private string _displayName = "Default Tile Set";

        [Header("Suit Toggles")]
        [SerializeField] private bool _includeDots = true;
        [SerializeField] private bool _includeBamboo = true;
        [SerializeField] private bool _includeCharacters = true;

        [Header("Honor Toggles")]
        [SerializeField] private bool _includeWinds = true;
        [SerializeField] private bool _includeDragons = true;

        [Header("Counts")]
        [Range(1, 4)]
        [SerializeField] private int _copiesPerTile = 4;

        public string TileSetId => _tileSetId;
        public string DisplayName => _displayName;
        public int CopiesPerTile => _copiesPerTile;

        public List<MahjongTile> BuildTiles(SuitFlags suitOverride, bool useSuitOverride)
        {
            var tiles = new List<MahjongTile>(136);
            var tileId = 0;

            var includeDots = useSuitOverride ? (suitOverride & SuitFlags.Dots) != 0 : _includeDots;
            var includeBamboo = useSuitOverride ? (suitOverride & SuitFlags.Bamboo) != 0 : _includeBamboo;
            var includeCharacters = useSuitOverride ? (suitOverride & SuitFlags.Characters) != 0 : _includeCharacters;

            if (includeDots)
            {
                AddSuitTiles(tiles, ref tileId, TileSuit.Dots);
            }

            if (includeBamboo)
            {
                AddSuitTiles(tiles, ref tileId, TileSuit.Bamboo);
            }

            if (includeCharacters)
            {
                AddSuitTiles(tiles, ref tileId, TileSuit.Characters);
            }

            if (_includeWinds)
            {
                AddHonorTiles(tiles, ref tileId, HonorType.East, HonorType.North);
            }

            if (_includeDragons)
            {
                AddHonorTiles(tiles, ref tileId, HonorType.Red, HonorType.White);
            }

            return tiles;
        }

        private void AddSuitTiles(List<MahjongTile> tiles, ref int tileId, TileSuit suit)
        {
            for (var rank = 1; rank <= 9; rank++)
            {
                for (var copy = 0; copy < _copiesPerTile; copy++)
                {
                    tiles.Add(new MahjongTile(tileId++, suit, rank, HonorType.None));
                }
            }
        }

        private void AddHonorTiles(List<MahjongTile> tiles, ref int tileId, HonorType start, HonorType end)
        {
            for (var honor = (int)start; honor <= (int)end; honor++)
            {
                for (var copy = 0; copy < _copiesPerTile; copy++)
                {
                    tiles.Add(new MahjongTile(tileId++, TileSuit.Honor, 0, (HonorType)honor));
                }
            }
        }

        private void OnValidate()
        {
            if (_copiesPerTile < 1)
            {
                _copiesPerTile = 1;
            }
            else if (_copiesPerTile > 4)
            {
                _copiesPerTile = 4;
            }
        }
    }
}

