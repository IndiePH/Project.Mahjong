using UnityEngine;

namespace ProjectMahjong.Features.Mahjong.Data.Configs
{
    [CreateAssetMenu(
        menuName = "Project Mahjong/Configs/AI Tuning",
        fileName = "AiTuningConfig")]
    public sealed class AiTuningConfig : ScriptableObject
    {
        [Header("Identity")]
        [Min(1)]
        [SerializeField] private int _schemaVersion = 1;
        [SerializeField] private string _aiProfileId = "ai.default";
        [SerializeField] private string _displayName = "Default AI";
        [SerializeField] private DifficultyLevel _difficulty = DifficultyLevel.Medium;
        [SerializeField] private AiBehaviorStyle _behaviorStyle = AiBehaviorStyle.Balanced;

        [Header("Decision Weights")]
        [Range(0f, 1f)]
        [SerializeField] private float _discardRandomness = 0.2f;
        [Min(0f)]
        [SerializeField] private float _offenseWeight = 1f;
        [Min(0f)]
        [SerializeField] private float _defenseWeight = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _callAggression = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float _riskTolerance = 0.4f;
        [Min(0)]
        [SerializeField] private int _lookaheadDepth = 1;
        [SerializeField] private bool _tileTrackingEnabled = true;

        [Header("Timing")]
        [Min(0)]
        [SerializeField] private int _reactionDelayMinMs = 350;
        [Min(0)]
        [SerializeField] private int _reactionDelayMaxMs = 900;

        [Header("Metadata")]
        [SerializeField] private string _notes;

        public int SchemaVersion => _schemaVersion;
        public string AiProfileId => _aiProfileId;
        public string DisplayName => _displayName;
        public DifficultyLevel Difficulty => _difficulty;
        public AiBehaviorStyle BehaviorStyle => _behaviorStyle;
        public float DiscardRandomness => _discardRandomness;
        public float OffenseWeight => _offenseWeight;
        public float DefenseWeight => _defenseWeight;
        public float CallAggression => _callAggression;
        public float RiskTolerance => _riskTolerance;
        public int LookaheadDepth => _lookaheadDepth;
        public bool TileTrackingEnabled => _tileTrackingEnabled;
        public int ReactionDelayMinMs => _reactionDelayMinMs;
        public int ReactionDelayMaxMs => _reactionDelayMaxMs;
        public string Notes => _notes;

        private void OnValidate()
        {
            if (_schemaVersion < 1)
            {
                _schemaVersion = 1;
            }

            _discardRandomness = Mathf.Clamp01(_discardRandomness);
            _callAggression = Mathf.Clamp01(_callAggression);
            _riskTolerance = Mathf.Clamp01(_riskTolerance);

            if (_offenseWeight < 0f)
            {
                _offenseWeight = 0f;
            }

            if (_defenseWeight < 0f)
            {
                _defenseWeight = 0f;
            }

            if (_lookaheadDepth < 0)
            {
                _lookaheadDepth = 0;
            }

            if (_reactionDelayMinMs < 0)
            {
                _reactionDelayMinMs = 0;
            }

            if (_reactionDelayMaxMs < _reactionDelayMinMs)
            {
                _reactionDelayMaxMs = _reactionDelayMinMs;
            }
        }
    }
}

