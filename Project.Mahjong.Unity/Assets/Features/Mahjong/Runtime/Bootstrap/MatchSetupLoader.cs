using System;
using UnityEngine;
using UnityEngine.Events;
using ProjectMahjong.Features.Mahjong.Data.Configs;

namespace ProjectMahjong.Features.Mahjong.Runtime.Bootstrap
{
    /// <summary>
    /// Loads a <see cref="MatchConfigSet"/> and resolves it into runtime-ready match setup data.
    /// </summary>
    public sealed class MatchSetupLoader : MonoBehaviour
    {
        [Serializable]
        public sealed class MatchSetupLoadedEvent : UnityEvent
        {
        }

        [Serializable]
        public sealed class MatchSetupFailedEvent : UnityEvent<string>
        {
        }

        [Header("Config Source")]
        [SerializeField] private MatchConfigSet _matchConfigSet;

        [Header("Default Setup")]
        [Range(1, 4)]
        [SerializeField] private int _seatCount = 4;
        [SerializeField] private DifficultyLevel _defaultAiDifficulty = DifficultyLevel.Medium;
        [SerializeField] private bool _loadOnAwake = true;

        [Header("Events")]
        [SerializeField] private MatchSetupLoadedEvent _onSetupLoaded;
        [SerializeField] private MatchSetupFailedEvent _onSetupFailed;

        private MatchSetup _currentSetup;
        private bool _hasSetup;

        /// <summary>
        /// True when a valid setup has been resolved from <see cref="_matchConfigSet"/>.
        /// </summary>
        public bool HasSetup => _hasSetup;

        /// <summary>
        /// Latest resolved match setup. Valid only when <see cref="HasSetup"/> is true.
        /// </summary>
        public MatchSetup CurrentSetup => _currentSetup;
        public MatchConfigSet CurrentConfigSet => _matchConfigSet;

        private void Awake()
        {
            if (_loadOnAwake)
            {
                LoadDefaultSetup();
            }
        }

        /// <summary>
        /// Resolves setup using serialized defaults.
        /// </summary>
        public bool LoadDefaultSetup()
        {
            return TryLoadSetup(_seatCount, _defaultAiDifficulty);
        }

        /// <summary>
        /// Resolves setup for a specific seat count and AI difficulty.
        /// </summary>
        public bool TryLoadSetup(int seatCount, DifficultyLevel aiDifficulty)
        {
            if (!TryBuildSetup(_matchConfigSet, seatCount, aiDifficulty, out var setup, out var error))
            {
                _hasSetup = false;
                _onSetupFailed?.Invoke(error);
                Debug.LogError(error, this);
                return false;
            }

            _currentSetup = setup;
            _hasSetup = true;
            _onSetupLoaded?.Invoke();
            return true;
        }

        /// <summary>
        /// Resolves setup using a runtime-selected config set plus seat count and difficulty.
        /// </summary>
        public bool TryLoadSetup(MatchConfigSet configSet, int seatCount, DifficultyLevel aiDifficulty)
        {
            if (configSet == null)
            {
                _hasSetup = false;
                const string error = "Match setup failed: selected MatchConfigSet is null.";
                _onSetupFailed?.Invoke(error);
                Debug.LogError(error, this);
                return false;
            }

            _matchConfigSet = configSet;
            return TryLoadSetup(seatCount, aiDifficulty);
        }

        /// <summary>
        /// Stateless setup builder for callers that do not want to mutate this loader.
        /// </summary>
        public static bool TryBuildSetup(
            MatchConfigSet configSet,
            int seatCount,
            DifficultyLevel aiDifficulty,
            out MatchSetup setup,
            out string error)
        {
            if (configSet == null)
            {
                setup = default;
                error = "Match setup failed: MatchConfigSet reference is null.";
                return false;
            }

            if (configSet.Rules == null)
            {
                setup = default;
                error = $"Match setup failed: Rules config is missing in set '{configSet.name}'.";
                return false;
            }

            if (configSet.Scoring == null)
            {
                setup = default;
                error = $"Match setup failed: Scoring config is missing in set '{configSet.name}'.";
                return false;
            }

            if (seatCount < 1 || seatCount > 4)
            {
                setup = default;
                error = $"Match setup failed: Seat count must be in [1..4], got {seatCount}.";
                return false;
            }

            var aiProfile = ResolveAiProfile(configSet.AiProfilesBySeatOrDifficulty, aiDifficulty);
            if (aiProfile == null)
            {
                setup = default;
                error = $"Match setup failed: No AI profile found for difficulty '{aiDifficulty}'.";
                return false;
            }

            var aiProfilesPerSeat = new AiTuningConfig[seatCount];
            for (var i = 0; i < aiProfilesPerSeat.Length; i++)
            {
                aiProfilesPerSeat[i] = aiProfile;
            }

            setup = new MatchSetup(
                configSet,
                configSet.Rules,
                configSet.Scoring,
                aiProfilesPerSeat,
                seatCount,
                aiDifficulty);

            error = string.Empty;
            return true;
        }

        private static AiTuningConfig ResolveAiProfile(AiTuningConfig[] candidates, DifficultyLevel difficulty)
        {
            if (candidates == null || candidates.Length == 0)
            {
                return null;
            }

            AiTuningConfig firstValid = null;
            for (var i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];
                if (candidate == null)
                {
                    continue;
                }

                firstValid ??= candidate;
                if (candidate.Difficulty == difficulty)
                {
                    return candidate;
                }
            }

            // Fallback to the first available profile so setup can still proceed.
            return firstValid;
        }
    }

    /// <summary>
    /// Runtime-resolved setup consumed by turn/game/bootstrap systems.
    /// </summary>
    public readonly struct MatchSetup
    {
        public MatchSetup(
            MatchConfigSet configSet,
            RuleSetConfig rules,
            ScoringConfig scoring,
            AiTuningConfig[] aiProfilesPerSeat,
            int seatCount,
            DifficultyLevel aiDifficulty)
        {
            ConfigSet = configSet;
            Rules = rules;
            Scoring = scoring;
            AiProfilesPerSeat = aiProfilesPerSeat ?? Array.Empty<AiTuningConfig>();
            SeatCount = seatCount;
            AiDifficulty = aiDifficulty;
        }

        public MatchConfigSet ConfigSet { get; }
        public RuleSetConfig Rules { get; }
        public ScoringConfig Scoring { get; }
        public AiTuningConfig[] AiProfilesPerSeat { get; }
        public int SeatCount { get; }
        public DifficultyLevel AiDifficulty { get; }
    }
}

