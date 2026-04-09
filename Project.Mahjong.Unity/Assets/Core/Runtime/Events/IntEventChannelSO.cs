using UnityEngine;

namespace ProjectMahjong.Core.Events
{
    /// <summary>
    /// ScriptableObject event channel with int payload.
    /// </summary>
    [CreateAssetMenu(
        menuName = "Project Mahjong/Events/Int Event Channel",
        fileName = "IntEventChannel")]
    public sealed class IntEventChannelSO : EventChannelSO<int>
    {
    }
}

