using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Tile;
using Entities.Wreckage;
using UnityEngine;

namespace Manager {
    public class Pathfinder : MonoBehaviour {
        public Dictionary<Vector3Int, TileEntity> allTiles;

        private List<TileEntity> path = new List<TileEntity>();
        private TileEntity target;

        public int VisibilityRange;
        public Dictionary<Vector3Int, TileEntity> AllTiles => allTiles ?? (allTiles = GetAllTiles());

        private bool IsTargetBlocked => target == null || (path.Count == 0 && target.standingEntity == null) || target.standingEntity == source || target.standingEntity is FallenWreckage;
                                                       //  || PathGoesThroughFog()

        public MovingEntity source => gameObject.GetComponent<SelectionManager>().CurrentTarget;

        private Dictionary<Vector3Int, TileEntity> GetAllTiles() {
            var tiles = new Dictionary<Vector3Int, TileEntity>();
            foreach (var tile in FindObjectsOfType<TileEntity>()) {
                if (tiles.ContainsKey(tile.Position)) {
                    throw new Exception("Repeating position of tile..." + tile.Position);
                }
                tiles[tile.Position] = tile;
            }
            return tiles;
        }

        private void Update() {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, ~LayerMask.NameToLayer("Tiles"))) {
                var newTarget = hit.collider.GetComponent<TileEntity>();
                Debug.Assert(newTarget != null, "Ray should not hit anything else than TileEntity");
                if (newTarget != target) {
                    target = newTarget;
                    path = source.DestinationTile.GetPathTo(target);
                }
            } else {
                target = null;
                path.Clear();
            }

            if (Input.GetMouseButtonDown(0)) {
                OnDown(true);
            } else if (Input.GetMouseButtonDown(1)) {
                OnDown(false);
            }
            RepaintHexColors();
            RepaintTargetHex();
        }

        public void OnDown(bool isPrimary) {
            if (IsTargetBlocked) {
                return;
            }

            if (path.Count == 0) {
                path.Add(target);
            }
            
            source.EnqueueInteraction(new List<TileEntity>(path), isPrimary);
            path.Clear();
        }

        public void OnBuild(GameObject prefab) {
            if (IsTargetBlocked || source is Mothership) {
                return;
            }

            source.EnqueueBuild(new List<TileEntity>(path), prefab);
            path.Clear();
        }

        public void ClearHexColors() {
            foreach (var tile in AllTiles) {
                tile.Value.SetHexColor(HexColors.Unseen);
            }
        }

        public Vector3 GetWorldPosition(Vector3Int position) => AllTiles[position].transform.position;

        private bool PathGoesThroughFog() {
            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(VisibilityRange);
            return path.Any(tile => !visibleTiles.Contains(tile));
        }

        public void RepaintHexColors() {
            ClearHexColors();

            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(VisibilityRange);

            // Highlight visible tiles
            foreach (var tile in visibleTiles) {
                tile.SetHexColor(HexColors.Visible);
            }

            // Draw queued actions
            foreach (var action in source.ActionQueue) {
                action.SetHexColors();
            }

            // Draw selected path
            var wentOutOfVisible = false;
            foreach (var tile in path) {
                if (!visibleTiles.Contains(tile)) {
                    // disabled for better playability
                    // wentOutOfVisible = true;
                }
                tile.SetHexColor(wentOutOfVisible ? HexColors.Blocked : HexColors.Path);
            }
        }

        public void RepaintTargetHex() {
            if (IsTargetBlocked) {
                target?.SetHexColor(HexColors.Blocked);
            } else if (target?.standingEntity != null) {
                target?.SetHexColor(HexColors.Interaction);
            }
        }
    }
}
