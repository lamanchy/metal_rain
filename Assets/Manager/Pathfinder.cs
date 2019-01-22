using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Tile;
using UnityEngine;

namespace Manager {
    public class Pathfinder : MonoBehaviour {
        public Dictionary<Vector3Int, TileEntity> allTiles;
        public Dictionary<Vector3Int, TileEntity> AllTiles => allTiles ?? (allTiles = GetAllTiles());

        public int visibilityRange;
    
        public MovingEntity source => gameObject.GetComponent<SelectionManager>().CurrentTarget;
        private TileEntity target;
    
        private List<TileEntity> path = new List<TileEntity>();


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

        private void Start() {
            target = source.CurrentTile;
            OnEnter(source.CurrentTile);
        }

        public void OnEnter(TileEntity tile) {
            target = tile;
            path = source.DestinationTile.GetPathTo(target);
            RepaintHexColors();
        }

        public void OnExit(TileEntity tile) {
            target = null;
            path.Clear();
            RepaintTargetHex();
        }

        public void OnDown(bool isPrimary) {
            if (target == null
             || path.Count == 0
             || PathGoesThroughFog()
             || target.standingEntity == source) {
                return;
            }

            source.EnqueueInteraction(new List<TileEntity>(path), isPrimary);
            path.Clear();
        }

        public void OnBuild(GameObject prefab) {
            if (target == null
             || path.Count == 0
             || PathGoesThroughFog()
             || target.standingEntity == source) {
                return;
            }

            source.EnqueueBuild(new List<TileEntity>(path), prefab);
            path.Clear();
        }

        public void ClearHexColors() {
            foreach (var tile in AllTiles) {
                tile.Value.SetHexColor(HexColors.unseen);
            }
        }

        public Vector3 GetWorldPosition(Vector3Int position) => AllTiles[position].transform.position;
        
        private bool PathGoesThroughFog() {
            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(visibilityRange);
            return path.Any(tile => !visibleTiles.Contains(tile));
        }

        public void RepaintHexColors() {
            ClearHexColors();
            
            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(visibilityRange);

            // Highlight visible tiles
            foreach (var tile in visibleTiles) {
                tile.SetHexColor(HexColors.visible);
            }

            // Draw queued actions
            foreach (var action in source.ActionQueue) {
                action.SetHexColors();
            }
            
            // Draw selected path
            var wentOutOfVisible = false;
            foreach (var tile in path) {
                if (!visibleTiles.Contains(tile)) {
                    wentOutOfVisible = true;
                }
                tile.SetHexColor(wentOutOfVisible ? HexColors.blocked : HexColors.path);
            }
        }

        public void RepaintTargetHex() {
            if (target == null) {
                return;
            }

            if (path.Count == 0 || PathGoesThroughFog() || target.standingEntity == source) {
                target.SetHexColor(HexColors.blocked);
            } else if (target.standingEntity != null) {
                target.SetHexColor(HexColors.interaction);
            }
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                OnDown(true);
            } else if (Input.GetMouseButtonDown(1)) {
                OnDown(false);
            }
            RepaintTargetHex();
        }
    }
}
