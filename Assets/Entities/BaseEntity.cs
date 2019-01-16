using Manager;
using UnityEngine;

namespace Entities {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BaseEntity : MonoBehaviour {
        public Vector3Int position;

        private Pathfinder pathfinder;
        public Pathfinder Pathfinder => pathfinder ? pathfinder : pathfinder = FindObjectOfType<Pathfinder>();
                                        // viz https://github.com/JetBrains/resharper-unity/wiki/Possible-unintended-bypass-of-lifetime-check-of-underlying-Unity-engine-object

        [ContextMenu("Align to grid")]
        public void AlignToGrid() => transform.position = Pathfinder.GetWorldPosition(position);

        public virtual void Interact(BaseEntity otherEntity) {}
    }
}
