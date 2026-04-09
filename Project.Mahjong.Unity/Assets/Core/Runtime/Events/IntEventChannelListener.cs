using System;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectMahjong.Core.Events
{
    /// <summary>
    /// Observer component for <see cref="IntEventChannelSO"/>.
    /// </summary>
    public sealed class IntEventChannelListener : MonoBehaviour
    {
        [Serializable]
        public sealed class IntUnityEvent : UnityEvent<int>
        {
        }

        [Header("Observer Wiring")]
        [SerializeField] private IntEventChannelSO _channel;
        [SerializeField] private IntUnityEvent _onEventRaised;

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

        private void HandleEventRaised(int value)
        {
            _onEventRaised?.Invoke(value);
        }
    }
}

