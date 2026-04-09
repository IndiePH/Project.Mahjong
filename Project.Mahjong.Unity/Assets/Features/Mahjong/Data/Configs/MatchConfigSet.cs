using System;
using UnityEngine;

namespace ProjectMahjong.Features.Mahjong.Data.Configs
{
    [CreateAssetMenu(
        menuName = "Project Mahjong/Configs/Match Config Set",
        fileName = "MatchConfigSet")]
    public sealed class MatchConfigSet : ScriptableObject
    {
        [Header("Identity")]
        [Min(1)]
        [SerializeField] private int _schemaVersion = 1;
        [SerializeField] private string _configSetId = "match.default";
        [SerializeField] private string _displayName = "Default Match Config";

        [Header("Config References")]
        [SerializeField] private RuleSetConfig _rules;
        [SerializeField] private ScoringConfig _scoring;
        [SerializeField] private AiTuningConfig[] _aiProfilesBySeatOrDifficulty = Array.Empty<AiTuningConfig>();

        [Header("Metadata")]
        [SerializeField] private string _notes;

        public int SchemaVersion => _schemaVersion;
        public string ConfigSetId => _configSetId;
        public string DisplayName => _displayName;
        public RuleSetConfig Rules => _rules;
        public ScoringConfig Scoring => _scoring;
        public AiTuningConfig[] AiProfilesBySeatOrDifficulty => _aiProfilesBySeatOrDifficulty;
        public string Notes => _notes;

        private void OnValidate()
        {
            if (_schemaVersion < 1)
            {
                _schemaVersion = 1;
            }
        }
    }
}

