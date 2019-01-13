using System;
using System.Collections.Generic;
using Entities;
using Entities.Tile;
using UnityEngine;

namespace Manager {
    public class Pathfinder : MonoBehaviour {
        public Dictionary<Vector3Int, TileScript> allTiles;
        public Dictionary<Vector3Int, TileScript> AllTiles => allTiles ?? (allTiles = GetAllTiles());

        public int visibilityRange;
    
        public MovingEntity source => gameObject.GetComponent<SelectionManager>().CurrentTarget;
        private TileScript target;
    
        private List<TileScript> path = new List<TileScript>();

        [SerializeField]
        private Color defaultHexColor = Color.black;

        [SerializeField]
        private Color pathHexColor = Color.green;

        [SerializeField]
        private Color selectedHexColor = Color.blue;

        [SerializeField]
        private Color visibleHexColor = Color.yellow;

        private Dictionary<Vector3Int, TileScript> GetAllTiles() {
            var tiles = new Dictionary<Vector3Int, TileScript>();
            foreach (var tile in FindObjectsOfType<TileScript>()) {
                if (tiles.ContainsKey(tile.Position)) {
                    throw new Exception("Repeating position of tile..." + tile.Position);
                }
                tiles[tile.Position] = tile;
            }
            return tiles;
        }

        public void OnEnter(TileScript obj) {
            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(visibilityRange);

            // Highlight visible tiles
            foreach (var tile in visibleTiles) {
                if (tile == source.CurrentTile) {
                    continue;
                }
                tile.SetHexColor(visibleHexColor);
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

        public void OnExit(TileScript obj) {
            target = null;
            if (obj != source.CurrentTile) {
                obj.SetHexColor(defaultHexColor);
            }
            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(visibilityRange);
            foreach (var tile in path) {
                tile.SetHexColor(visibleTiles.Contains(tile) ? visibleHexColor : defaultHexColor);
            }
            path.Clear();
        }

        public void OnDown() {
            if (target == null || target.standingEntity != null || path.Count == 0) {
                return;
            }

            var visibleTiles = source.CurrentTile.GetVisibleSurroundings(visibilityRange);
            if (!visibleTiles.Contains(target)) {
                return;
            }

            source.CurrentTile.SetHexColor(defaultHexColor);
            foreach (var tile in visibleTiles) {
                tile.SetHexColor(defaultHexColor);
            }

            source.MoveTo(target);
            source.CurrentTile.SetHexColor(selectedHexColor);
            path.Clear();
        }

        public Vector3 GetWorldPosition(Vector3Int position) => AllTiles[position].transform.position;

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                OnDown();
            }
        }
    }
}
