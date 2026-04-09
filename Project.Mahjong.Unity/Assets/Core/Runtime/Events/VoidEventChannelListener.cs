using UnityEngine;
using UnityEngine.Events;

namespace ProjectMahjong.Core.Events
{
    /// <summary>
    /// Observer component for <see cref="VoidEventChannelSO"/>.
    /// </summary>
    public sealed class VoidEventChannelListener : MonoBehaviour
    {
        [Header("Observer Wiring")]
        [SerializeField] private VoidEventChannelSO _channel;
        [SerializeField] private UnityEvent _onEventRaised;

        private void OnEnable()
        {
            if (_channel != null)
            {
                _channel.Subscribe(HandleEventRaised);
            }
        }

        private void OnDisable()
        {
            if (_channel != null)
            {
                _channel.Unsubscribe(HandleEventRaised);
            }
        }

        private void HandleEventRaised()
        {
            _onEventRaised?.Invoke();
        }
    }
}

