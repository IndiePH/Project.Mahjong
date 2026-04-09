using System;
using UnityEngine;

namespace ProjectMahjong.Core.Events
{
    /// <summary>
    /// Base ScriptableObject event channel with payload support.
    /// Observers subscribe/unsubscribe explicitly to receive raised values.
    /// </summary>
    /// <typeparam name="T">Payload type carried by this channel.</typeparam>
    public abstract class EventChannelSO<T> : ScriptableObject
    {
        private event Action<T> EventRaised;

        /// <summary>
        /// Notify all subscribed observers.
        /// </summary>
        public void Raise(T payload)
        {
            EventRaised?.Invoke(payload);
        }

        /// <summary>
        /// Register a callback to observe this channel.
        /// </summary>
        public void Subscribe(Action<T> listener)
        {
            EventRaised += listener;
        }

        /// <summary>
        /// Remove a callback from this channel.
        /// </summary>
        public void Unsubscribe(Action<T> listener)
        {
            EventRaised -= listener;
        }

        protected virtual void OnDisable()
        {
            // Avoid stale delegates when domain/scene reloads.
            EventRaised = null;
        }
    }
}

