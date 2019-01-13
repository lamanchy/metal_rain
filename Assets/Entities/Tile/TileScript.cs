﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entities.Tile {
    public class TileScript : BaseEntity {
        private const int MAX_STEP = 1;
        private const int HEIGHT_OF_VISIBILITY = 1;
        public float elevation;
        private TileScript group;
        public float height = 30f;
        private Pathfinder pathfinder;
        private MaterialPropertyBlock propertyBlock;
        private new Renderer renderer;

        private void Awake() {
            renderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
        }

        private void Start() {
            pathfinder = FindObjectOfType<Pathfinder>();
        }

        public void MoveToPosition() {
            var diameter = transform.localScale.x;
            var smallestWidth = Mathf.Sqrt(3.0f) * 0.5f * diameter;
            var sideSize = smallestWidth / Mathf.Sqrt(3.0f);

            gameObject.transform.position = new Vector3(Position.x * ((diameter - sideSize) * 0.5f + sideSize), height, (Position.y - Position.x * 0.5f) * smallestWidth);
            var scale = transform.localScale;
            scale.Set(1, height / 2f, 1);
            transform.localScale = scale;
        }

        private void SetGroup(TileScript groupToSet) {
            if (group != null) {
                return;
            }

            group = groupToSet;

            foreach (var tile in GetNeighbours()) {
                tile.SetGroup(group);
            }
        }

        private void OnMouseEnter() {
            pathfinder.OnEnter(this);
        }

        private void OnMouseExit() {
            pathfinder.OnExit(this);
        }

        // Sets new color for this hexagon; alpha is not overriden
        public void SetHexColor(Color newColor) {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_ColorMask", newColor);
            renderer.SetPropertyBlock(propertyBlock);
        }

        // NOTE: Recommended to use only during map generation
        public void SetSideMaterial(Material newMaterial) {
            var mats = GetComponent<Renderer>().sharedMaterials.ToArray();
            mats[0] = newMaterial;
            GetComponent<Renderer>().sharedMaterials = mats;
        }

        // NOTE: Recommended to use only during map generation
        public void SetTopMaterial(Material newMaterial) {
            var mats = GetComponent<Renderer>().sharedMaterials.ToArray();
            mats[1] = newMaterial;
            GetComponent<Renderer>().sharedMaterials = mats;
        }

        public Vector3 GetTop() => transform.position + new Vector3(0, transform.localScale.y, 0);

        public List<TileScript> GetPathTo(TileScript target) {
            var parent = new Dictionary<TileScript, TileScript>();
            var traveled = new Dictionary<TileScript, int> { { this, 0 } };
            var result = new List<TileScript>();

            var toSearch = new SortedDictionary<int, List<TileScript>> { { Distance(target.Position), new List<TileScript> { this } } };

            SetGroup(this);
            target.SetGroup(target);

            while (toSearch.Count > 0) {
                var enumerator = toSearch.Keys.GetEnumerator();
                enumerator.MoveNext();
                var currentDistance = enumerator.Current;
                var randIndex = 0; // Random.Range(0, toSearch[currentDistance].Count);

                var current = toSearch[currentDistance][randIndex];
                toSearch[currentDistance].RemoveAt(randIndex);
                if (toSearch[currentDistance].Count == 0) {
                    toSearch.Remove(currentDistance);
                }

                if (current.group != null && target.group != null && current.group != target.group) {
                    break;
                }

                if (current == target) {
                    while (current != this) {
                        result.Add(current);
                        current = parent[current];
                    }
                    break;
                }

                foreach (var neighbour in current.GetNeighbours()) {
                    if (parent.ContainsKey(neighbour)) {
                        continue;
                    }
                    var distance = neighbour.Distance(target.Position) + traveled[current];
                    if (!toSearch.ContainsKey(distance)) {
                        toSearch[distance] = new List<TileScript>();
                    }
                    toSearch[distance].Add(neighbour);
                    parent[neighbour] = current;
                    traveled[neighbour] = traveled[current] + 1;
                }
            }

            return result;
        }

        public List<TileScript> GetSurroundings(int distance = 1) {
            var result = new List<TileScript>();

            for (var x = -distance; x <= distance; x++) {
                for (var y = -distance; y <= distance; y++) {
                    for (var z = -10; z < 11; z++) {
                        var key = new Vector3Int(Position.x + x, Position.y + y, z);
                        if (Distance(key) > distance || Distance(key) == 0) {
                            continue;
                        }
                        if (!pathfinder.allTiles.ContainsKey(key)) {
                            continue;
                        }
                        var neighbour = pathfinder.allTiles[key];
                        result.Add(neighbour);
                    }
                }
            }

            return result;
        }

        public List<TileScript> GetVisibleSurroundings(int distance) {
            var neigbours = GetSurroundings(distance);
            var result = new HashSet<TileScript>();

            RaycastHit hit;
            foreach (var target in neigbours) {
                var layerMask = 1 << 9;
                var source = GetTop() + new Vector3(0, HEIGHT_OF_VISIBILITY, 0);
                var destination = target.GetTop() + new Vector3(0, HEIGHT_OF_VISIBILITY, 0);
                var direction = destination - source;
                var dist = Vector3.Distance(source, destination);
                if (Physics.Raycast(source, direction, out hit, dist, layerMask)) {
                    result.Add(hit.collider.gameObject.GetComponent<TileScript>());
                } else {
                    result.Add(target);
                }
            }

            return result.ToList();
        }

        public List<TileScript> GetNeighbours() {
            var result = GetSurroundings(1);
            result.RemoveAll(tile => Mathf.Abs(GetTop().y - tile.GetTop().y) > 1);

            return result;
        }

        private int Distance(Vector3Int otherPos) => (
                                                         Mathf.Abs(Position.x - otherPos.x) +
                                                         Mathf.Abs(Position.y - otherPos.y) +
                                                         Mathf.Abs(Position.x - Position.y - (otherPos.x - otherPos.y))
                                                     ) / 2;
    }
}
