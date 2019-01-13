using System;
using System.Collections.Generic;
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

        [SerializeField]
        private Color defaultHexColor = Color.black;

        [SerializeField]
        private Color pathHexColor = Color.green;

        [SerializeField]
        private Color selectedHexColor = Color.blue;

        [SerializeField]
        private Color visibleHexColor = Color.yellow;

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

        public void OnEnter(TileEntity obj) {
            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(visibilityRange);

            // Highlight visible tiles
            foreach (var tile in visibleTiles) {
                tile.SetHexColor(visibleHexColor);
            }

            if (obj == target) {
                return;
            }
            target = obj;

            if (source != null) {
                path = source.CurrentTile.GetPathTo(target);
                foreach (var tile in path) {
                    tile.SetHexColor(visibleTiles.Contains(tile) ? pathHexColor : selectedHexColor);
                }
            }

            if (target.standingEntity != null || path.Count == 0) {
                target.SetHexColor(selectedHexColor);
            }
        }

        public void OnExit(TileEntity tile) {
            target.SetHexColor(defaultHexColor);
            target = null;
            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(visibilityRange);
            foreach (var visibleTile in path) {
                visibleTile.SetHexColor(visibleTiles.Contains(visibleTile) ? visibleHexColor : defaultHexColor);
            }
            path.Clear();
        }

        public void OnDown() {
            if (target == null 
             || target.standingEntity != null 
             || path.Count == 0
             || !source.CurrentTile.GetVisibleSurroundings(visibilityRange).Contains(target)) {
                return;
            }

            source.MoveTo(target);

            ClearHexColors();
            OnEnter(source.CurrentTile);
            path.Clear();
        }

        public void ClearHexColors() {
            foreach (var tile in AllTiles) {
                tile.Value.SetHexColor(defaultHexColor);
            }
        }

        public Vector3 GetWorldPosition(Vector3Int position) => AllTiles[position].transform.position;

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                OnDown();
            }
        }
    }
}
