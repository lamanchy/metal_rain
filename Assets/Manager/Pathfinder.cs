using System;
using System.Collections.Generic;
using Entities;
using Entities.Tile;
using UnityEngine;

namespace Manager {
    public class Pathfinder : MonoBehaviour {
        public Dictionary<Vector3Int, TileScript> allTiles;
        public Dictionary<Vector3Int, TileScript> AllTiles => allTiles ?? (allTiles = GetAllTiles());
    
        public MovingEntity source;
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
            target = obj;

            foreach (var tile in obj.GetVisibleSurroundings(4)) {
                if (tile == source.CurrentTile) {
                    continue;
                }
                tile.SetHexColor(visibleHexColor);
            }

            if (source != null) {
                path = source.CurrentTile.GetPathTo(target);
                foreach (var tile in path) {
                    tile.SetHexColor(pathHexColor);
                }
            }

            if (obj != source.CurrentTile) {
                obj.SetHexColor(pathHexColor);
            }
        }

        public void OnExit(TileScript obj) {
            target = null;
            if (obj != source.CurrentTile) {
                obj.SetHexColor(defaultHexColor);
            }
            if (path.Count > 0) {
                foreach (var tile in path) {
                    tile.SetHexColor(defaultHexColor);
                }
                path.Clear();
            }
            foreach (var tile in obj.GetVisibleSurroundings(10)) {
                if (tile == source.CurrentTile) {
                    continue;
                }
                tile.SetHexColor(defaultHexColor);
            }
        }

        public void OnDown() {
            if (source != null) {
                source.CurrentTile.SetHexColor(defaultHexColor);
            }
            if (path.Count > 0) {
                foreach (var tile in path) {
                    tile.SetHexColor(defaultHexColor);
                }
                path = new List<TileScript>();
            }
            if (source.CurrentTile == target) {
                target = null;
            }
            source.MoveTo(target);
            if (source != null) {
                source.CurrentTile.SetHexColor(selectedHexColor);
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
