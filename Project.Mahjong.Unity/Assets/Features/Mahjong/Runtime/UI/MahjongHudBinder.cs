using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectMahjong.Features.Mahjong.Runtime.UI
{
    /// <summary>
    /// Simple UI Toolkit binder for prototype telemetry.
    /// </summary>
    public sealed class MahjongHudBinder : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private Label _seatCountValue;
        private Label _wallRemainingValue;
        private Label _turnsPlayedValue;
        private Label _activeSeatValue;
        private Label _roundEndValue;
        private Label _winnerSeatValue;
        private Label _callsValue;
        private Label _windowsValue;
        private Label _lastCallValue;
        private Label _discardsValue;
        private Label _winningHandsValue;

        private void Awake()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning("MahjongHudBinder: UIDocument/rootVisualElement is missing.", this);
                return;
            }

            var root = _uiDocument.rootVisualElement;
            _seatCountValue = root.Q<Label>("seat-count-value");
            _wallRemainingValue = root.Q<Label>("wall-remaining-value");
            _turnsPlayedValue = root.Q<Label>("turns-played-value");
            _activeSeatValue = root.Q<Label>("active-seat-value");
            _roundEndValue = root.Q<Label>("round-end-value");
            _winnerSeatValue = root.Q<Label>("winner-seat-value");
            _callsValue = root.Q<Label>("calls-value");
            _windowsValue = root.Q<Label>("windows-value");
            _lastCallValue = root.Q<Label>("last-call-value");
            _discardsValue = root.Q<Label>("discards-value");
            _winningHandsValue = root.Q<Label>("winning-hands-value");
        }

        public void Apply(MahjongHudViewState state)
        {
            SetLabel(_seatCountValue, state.SeatCount.ToString());
            SetLabel(_wallRemainingValue, state.WallRemaining.ToString());
            SetLabel(_turnsPlayedValue, state.TurnsPlayed.ToString());
            SetLabel(_activeSeatValue, $"S{state.CurrentActiveSeat}");
            SetLabel(_roundEndValue, state.RoundEnd ?? "N/A");
            SetLabel(_winnerSeatValue, state.WinnerSeat >= 0 ? $"S{state.WinnerSeat}" : "None");
            SetLabel(_callsValue, state.Calls ?? "N/A");
            SetLabel(_windowsValue, state.Windows ?? "N/A");
            SetLabel(_lastCallValue, state.LastCall ?? "N/A");
            SetLabel(_discardsValue, state.Discards ?? "N/A");
            SetLabel(_winningHandsValue, state.WinningHands ?? "N/A");
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

