using UnityEngine;

namespace ProjectMahjong.Core.Composition
{
    /// <summary>
    /// Scene-wired composition root for shared services.
    /// Avoids statics/service locator patterns and keeps dependencies explicit and inspector-traceable.
    /// </summary>
    public sealed class GameContext : MonoBehaviour
    {
        [Header("Shared services (scene-wired)")]
        [SerializeField] private MonoBehaviour[] _services = new MonoBehaviour[0];

        /// <summary>
        /// Returns the first service in <see cref="_services"/> that implements <typeparamref name="T"/>.
        /// This is instance-scoped (not global/static) and reflection-free (interface cast only).
        /// </summary>
        public bool TryGetService<T>(out T service) where T : class
        {
            foreach (var s in _services)
            {
                if (s is T typed)
                {
                    service = typed;
                    return true;
                }
            }

            service = null;
            return false;
        }
    }
}

