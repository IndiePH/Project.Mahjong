using ProjectMahjong.Features.Mahjong.Runtime.Bootstrap;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectMahjong.Features.Mahjong.Runtime.UI
{
    /// <summary>
    /// Binds RoundState summary fields into the Results panel.
    /// </summary>
    public sealed class MahjongResultsBinder : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private RoundRuntimeController _roundRuntimeController;

        private Label _endReasonValue;
        private Label _winnerValue;
        private Label _turnsValue;
        private Label _callsValue;
        private Label _windowsValue;
        private Label _discardsValue;

        private void Awake()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning("MahjongResultsBinder: UIDocument/rootVisualElement is missing.", this);
                return;
            }

            var root = _uiDocument.rootVisualElement;
            _endReasonValue = root.Q<Label>("results-endreason-value");
            _winnerValue = root.Q<Label>("results-winner-value");
            _turnsValue = root.Q<Label>("results-turns-value");
            _callsValue = root.Q<Label>("results-calls-value");
            _windowsValue = root.Q<Label>("results-windows-value");
            _discardsValue = root.Q<Label>("results-discards-value");
        }

        private void OnEnable()
        {
            if (_roundRuntimeController != null)
            {
                _roundRuntimeController.RoundStateChanged += OnRoundStateChanged;
                if (_roundRuntimeController.HasRound)
                {
                    Apply(_roundRuntimeController.CurrentState);
                }
            }
        }

        private void OnDisable()
        {
            if (_roundRuntimeController != null)
            {
                _roundRuntimeController.RoundStateChanged -= OnRoundStateChanged;
            }
        }

        private void OnRoundStateChanged(RoundState state)
        {
            Apply(state);
        }

        private void Apply(RoundState state)
        {
            SetLabel(_endReasonValue, state.TurnResult.RoundEndReason.ToString());
            SetLabel(_winnerValue, state.TurnResult.WinnerSeat >= 0 ? $"S{state.TurnResult.WinnerSeat}" : "None");
            SetLabel(_turnsValue, state.TurnResult.TurnsPlayed.ToString());
            SetLabel(_callsValue, state.CallSummary);
            SetLabel(_windowsValue, state.WindowSummary);
            SetLabel(_discardsValue, state.DiscardSummary);
        }

        private static void SetLabel(Label label, string text)
        {
            if (label != null)
            {
                label.text = text;
            }
        }
    }
}

