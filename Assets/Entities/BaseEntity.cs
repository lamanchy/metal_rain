using Manager;
using UnityEngine;

namespace Entities {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BaseEntity : MonoBehaviour {
        public Vector3Int Position;

        private Pathfinder pathfinder;
        protected Pathfinder Pathfinder => pathfinder ?? (pathfinder = FindObjectOfType<Pathfinder>());
        
        [ContextMenu("Align to grid")]
        public void AlignToGrid() => transform.position = Pathfinder.GetWorldPosition(Position);
    }
}
