﻿using System.Collections;
using System.Collections.Generic;
using Entities.Tile;
using Manager;
using UnityEngine;

namespace Entities {
    public interface IUnitAction {
        bool HasBeenInterrupted { get; set; }
        Color Color { get; }
        Sprite Sprite { get; }
        IEnumerator Execute();

        void SetHexColors();
    }

    public class MoveAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        public readonly List<TileEntity> Path;

        public Color Color => HexColors.Movement;
        public Sprite Sprite => PrefabContainer.Instance.MoveAction;

        public bool HasBeenInterrupted { get; set; }

        public MoveAction(MovingEntity movingEntity, List<TileEntity> path) {
            this.movingEntity = movingEntity;
            this.Path = path;
        }

        public IEnumerator Execute() {
            while (Path.Count > 0) {
                if (HasBeenInterrupted) {
                    yield break;
                }
                var tile = Path[0];
                if (tile.standingEntity != null) {
                    Debug.Log("Path blocked.");
                    HasBeenInterrupted = true;
                    yield break;
                }
                tile.standingEntity = movingEntity;
                var transform = movingEntity.transform;
                var startingPosition = transform.position;
                var startingRotation = transform.rotation;
                var destinationPosition = tile.transform.position;
                var direction = destinationPosition - startingPosition;
                direction.y = 0;
                var destinationRotation = Quaternion.LookRotation(direction);
                destinationRotation.Normalize();
                var middle = (startingPosition + destinationPosition) / 2;
                middle.y = Mathf.Min(startingPosition.y, destinationPosition.y) - 1;
                var middlePosition = (startingPosition + destinationPosition) / 2;
                middlePosition.y = Mathf.Max(startingPosition.y, destinationPosition.y) + (float) 0.1;
                startingPosition -= middle;
                destinationPosition -= middle;
                middlePosition -= middle;
                var moveSpeed = movingEntity.MoveSpeedModifier / 1000f;
                for (var i = 0f; i < 1f; i += moveSpeed) {
                    while (!movingEntity.IsPowered) {
                        yield return null;
                    }

                    movingEntity.transform.position = i < 0.5
                        ? middle + Vector3.Slerp(startingPosition, middlePosition, 2 * i)
                        : middle + Vector3.Slerp(middlePosition, destinationPosition, 2 * i - 1);
                    movingEntity.transform.rotation = 
                        Quaternion.Lerp(startingRotation, destinationRotation, 2*i);
                    yield return null;
                }
                movingEntity.Pathfinder.AllTiles[movingEntity.Position].standingEntity = null;
                movingEntity.Position = tile.Position;
                Path.Remove(tile);
                movingEntity.Pathfinder.RepaintHexColors();
            }
        }

        public void SetHexColors() {
            foreach (var tile in Path) {
                tile.SetHexColor(Color);
            }
        }
    }

    public class InteractAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        private readonly TileEntity target;
        private readonly bool isPrimary;

        public Color Color => HexColors.Interaction;
        public Sprite Sprite => PrefabContainer.Instance.InteractAction;
        
        public bool HasBeenInterrupted { get; set; }

        public InteractAction(MovingEntity movingEntity, TileEntity target, bool isPrimary) {
            this.movingEntity = movingEntity;
            this.target = target;
            this.isPrimary = isPrimary;
        }

        public IEnumerator Execute() {
            while (!movingEntity.IsPowered) {
                yield return null;
            }
            if (target.standingEntity == null) {
                Debug.Log("Target is gone.");
                HasBeenInterrupted = true;
                yield break;
            }
            yield return movingEntity.Interact(target.standingEntity, isPrimary);
        }

        public void SetHexColors() {
            target.SetHexColor(Color);
        }
    }

    public class BuildAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        private readonly TileEntity target;
        private readonly GameObject buildPrefab;

        public Color Color => HexColors.Build;
        public Sprite Sprite => PrefabContainer.Instance.BuildAction;

        public bool HasBeenInterrupted { get; set; }

        public BuildAction(MovingEntity movingEntity, TileEntity target, GameObject buildPrefab) {
            this.movingEntity = movingEntity;
            this.target = target;
            this.buildPrefab = buildPrefab;
        }

        public IEnumerator Execute() {
            if (target.standingEntity != null) {
                Debug.Log("Can't build on occupied tile.");
                HasBeenInterrupted = true;
                yield break;
            }
            var building = GameObject.Instantiate(buildPrefab).GetComponent<StaticEntity>();
            building.Position = target.Position;
            building.AlignToGrid();
            yield return null;
        }

        public void SetHexColors() {
            target.SetHexColor(Color);
        }
    }
}
