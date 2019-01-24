using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Tile;
using UnityEngine;

namespace Entities {
    public interface IUnitAction {
        bool HasBeenInterrupted { get; set; }
        Color color { get; }
        IEnumerator Execute();

        void SetHexColors();
    }

    public class MoveAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        public readonly List<TileEntity> Path;

        public Color color => HexColors.Movement;

        public bool HasBeenInterrupted { get; set; }

        public MoveAction(MovingEntity movingEntity, List<TileEntity> path) {
            this.movingEntity = movingEntity;
            this.Path = path;
        }

        public IEnumerator Execute() {
            foreach (var tile in Path) {
                if (tile.standingEntity != null) {
                    Debug.Log("Path blocked.");
                    HasBeenInterrupted = true;
                    yield break;
                }
                tile.standingEntity = movingEntity;
                var startingPosition = movingEntity.transform.position;
                var destinationPosition = tile.transform.position;
                var moveSpeed = movingEntity.MoveSpeedModifier / 1000f;
                for (var i = 0f; i < 1f; i += moveSpeed) {
                    while (!movingEntity.IsPowered) {
                        yield return null;
                    }
                    if (HasBeenInterrupted) {
                        // TODO better solve cancelling of movement mid transfer
                        movingEntity.transform.position = startingPosition;
                        tile.standingEntity = null;
                        yield break;
                    }
                    movingEntity.transform.position = Vector3.Lerp(startingPosition, destinationPosition, i);
                    yield return null;
                }
                movingEntity.Pathfinder.AllTiles[movingEntity.Position].standingEntity = null;
                movingEntity.Position = tile.Position;
            }
        }

        public void SetHexColors() {
            foreach (var tile in Path) {
                tile.SetHexColor(color);
            }
        }
    }

    public class InteractAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        private readonly TileEntity target;
        private readonly bool isPrimary;

        public Color color => HexColors.Interaction;
        
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
            target.SetHexColor(color);
        }
    }

    public class BuildAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        private readonly TileEntity target;
        private readonly GameObject buildPrefab;

        public Color color => HexColors.Build;

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
            // target.standingEntity = building;
            yield return null;
        }

        public void SetHexColors() {
            target.SetHexColor(color);
        }
    }
}
