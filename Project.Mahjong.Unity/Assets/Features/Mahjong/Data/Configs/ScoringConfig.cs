using System;
using UnityEngine;

namespace ProjectMahjong.Features.Mahjong.Data.Configs
{
    [CreateAssetMenu(
        menuName = "Project Mahjong/Configs/Scoring",
        fileName = "ScoringConfig")]
    public sealed class ScoringConfig : ScriptableObject
    {
        [Serializable]
        public sealed class BonusPatternEntry
        {
            [SerializeField] private string _patternId = "pattern.id";
            [SerializeField] private string _displayName = "Pattern";
            [SerializeField] private int _points;
            [SerializeField] private bool _enabled = true;

            public string PatternId => _patternId;
            public string DisplayName => _displayName;
            public int Points => _points;
            public bool Enabled => _enabled;
        }

        [Header("Identity")]
        [Min(1)]
        [SerializeField] private int _schemaVersion = 1;
        [SerializeField] private string _scoringProfileId = "scoring.default";
        [SerializeField] private string _displayName = "Default Scoring";

        [Header("Core Points")]
        [Min(1)]
        [SerializeField] private int _baseWinPoints = 10;
        [Min(0)]
        [SerializeField] private int _selfDrawBonusPoints = 2;
        [Min(0)]
        [SerializeField] private int _kongBonusPoints = 1;

        [Header("Optional Pattern Bonuses")]
        [SerializeField] private BonusPatternEntry[] _bonusPatternEntries = Array.Empty<BonusPatternEntry>();

        [Header("Bounds and Flow")]
        [SerializeField] private int _clampMinScore;
        [SerializeField] private int _clampMaxScore = 9999;
        [SerializeField] private bool _roundEndOnFirstWin = true;

        [Header("Metadata")]
        [SerializeField] private string _notes;

        public int SchemaVersion => _schemaVersion;
        public string ScoringProfileId => _scoringProfileId;
        public string DisplayName => _displayName;
        public int BaseWinPoints => _baseWinPoints;
        public int SelfDrawBonusPoints => _selfDrawBonusPoints;
        public int KongBonusPoints => _kongBonusPoints;
        public BonusPatternEntry[] BonusPatternEntries => _bonusPatternEntries;
        public int ClampMinScore => _clampMinScore;
        public int ClampMaxScore => _clampMaxScore;
        public bool RoundEndOnFirstWin => _roundEndOnFirstWin;
        public string Notes => _notes;

        private void OnValidate()
        {
            if (_schemaVersion < 1)
            {
                _schemaVersion = 1;
            }

            if (_baseWinPoints < 1)
            {
                _baseWinPoints = 1;
            }

            if (_selfDrawBonusPoints < 0)
            {
                _selfDrawBonusPoints = 0;
            }

            if (_kongBonusPoints < 0)
            {
                _kongBonusPoints = 0;
            }

            if (_clampMaxScore < _clampMinScore)
            {
                _clampMaxScore = _clampMinScore;
            }
        }
    }
}

