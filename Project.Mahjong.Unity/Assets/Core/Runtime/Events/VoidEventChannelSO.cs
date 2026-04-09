using System;
using UnityEngine;

namespace ProjectMahjong.Core.Events
{
    /// <summary>
    /// ScriptableObject event channel without payload.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Project Mahjong/Events/Void Event Channel",
        fileName = "VoidEventChannel")]
    public sealed class VoidEventChannelSO : ScriptableObject
    {
        private event Action EventRaised;

        /// <summary>
        /// Notify all subscribed observers.
        /// </summary>
        public void Raise()
        {
            EventRaised?.Invoke();
        }

        /// <summary>
        /// Register an observer callback.
        /// </summary>
        public void Subscribe(Action listener)
        {
            EventRaised += listener;
        }

        /// <summary>
        /// Remove an observer callback.
        /// </summary>
        public void Unsubscribe(Action listener)
        {
            EventRaised -= listener;
        }

        private void OnDisable()
        {
            EventRaised = null;
        }
    }
}

