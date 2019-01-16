﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Tile {
    [RequireComponent(typeof(MeshCollider))]
    public class TileEntity : BaseEntity {
        private const int MaxStep = 1;
        private const int HeightOfVisibility = 1;

        public float elevation;
        public float height;

        public BaseEntity standingEntity;

        private TileEntity group;

        private MaterialPropertyBlock propertyBlock;
        private new Renderer renderer;

        private void Awake() {
            renderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
        }

        public new void AlignToGrid() {
            var localScale = transform.localScale;
            var diameter = localScale.x;
            var smallestWidth = Mathf.Sqrt(3.0f) * 0.5f * diameter;
            var sideSize = smallestWidth / Mathf.Sqrt(3.0f);

            transform.position = new Vector3(position.x * ((diameter - sideSize) * 0.5f + sideSize), elevation, (position.y - position.x * 0.5f) * smallestWidth);
            localScale.Set(1, height, 1);
            transform.localScale = localScale;
        }

        private void SetGroup(TileEntity groupToSet) {
            if (group != null) {
                return;
            }

            group = groupToSet;

            foreach (var tile in GetNeighbours()) {
                tile.SetGroup(group);
            }
        }

        private void OnMouseEnter() {
            Pathfinder.OnEnter(this);
        }

        private void OnMouseExit() {
            Pathfinder.OnExit(this);
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

        private Vector3 GetTop() => transform.position;

        public List<TileEntity> GetPathTo(TileEntity target) {
            var parent = new Dictionary<TileEntity, TileEntity>();
            var traveled = new Dictionary<TileEntity, int> { { this, 0 } };
            var result = new List<TileEntity>();

            var toSearch = new SortedDictionary<int, List<TileEntity>> { { Distance(target.position), new List<TileEntity> { this } } };

            SetGroup(this);
            target.SetGroup(target);

            while (toSearch.Count > 0) {
                using (var enumerator = toSearch.Keys.GetEnumerator()) {
                    enumerator.MoveNext();
                    var currentDistance = enumerator.Current;
                    const int randIndex = 0; // Random.Range(0, toSearch[currentDistance].Count);

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
                        var distance = neighbour.Distance(target.position) + traveled[current];
                        if (!toSearch.ContainsKey(distance)) {
                            toSearch[distance] = new List<TileEntity>();
                        }
                        toSearch[distance].Add(neighbour);
                        parent[neighbour] = current;
                        traveled[neighbour] = traveled[current] + 1;
                    }
                }
            }
            result.Reverse();
            return result;
        }

        private TileEntity GetVisibleTile(TileEntity tile) {
            const int layerMask = 1 << 9;
            var source = GetTop() + new Vector3(0, HeightOfVisibility, 0);
            var destination = tile.GetTop() + new Vector3(0, HeightOfVisibility, 0);
            var direction = destination - source;
            var dist = Vector3.Distance(source, destination);

            return Physics.Raycast(source, direction, out var hit, dist, layerMask) 
                ? hit.collider.gameObject.GetComponent<TileEntity>() 
                : tile;
        }

        private List<TileEntity> GetSurroundings(int distance = 1) {
            var result = new List<TileEntity>();

            for (var x = -distance; x <= distance; x++) {
                for (var y = -distance; y <= distance; y++) {
                    for (var z = -10; z < 11; z++) {
                        var key = new Vector3Int(position.x + x, position.y + y, z);
                        if (Distance(key) > distance) {
                            continue;
                        }
                        if (!Pathfinder.AllTiles.ContainsKey(key)) {
                            continue;
                        }
                        var neighbour = Pathfinder.AllTiles[key];
                        result.Add(neighbour);
                    }
                }
            }

            return result;
        }

        public List<TileEntity> GetVisibleSurroundings(int distance) {
            return GetSurroundings(distance).Aggregate(new HashSet<TileEntity>(), (tiles, tile) => {
                tiles.Add(GetVisibleTile(tile));
                return tiles;
            }).ToList();
        }

        private IEnumerable<TileEntity> GetNeighbours() {
            var result = GetSurroundings(1);
            result.RemoveAll(tile => Mathf.Abs(GetTop().y - tile.GetTop().y) > 1);

            return result;
        }

        private int Distance(Vector3Int otherPos) => (
                                                         Mathf.Abs(position.x - otherPos.x) +
                                                         Mathf.Abs(position.y - otherPos.y) +
                                                         Mathf.Abs(position.x - position.y - (otherPos.x - otherPos.y))
                                                     ) / 2;
    }
}
