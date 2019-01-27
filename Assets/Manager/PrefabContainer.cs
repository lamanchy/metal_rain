using System.Collections.Generic;
using UnityEngine;

namespace Manager {
    public class PrefabContainer : MonoBehaviour {
        public GameObject LightningPrefab;

        [Header("Wreckage meshes")]
        public List<Mesh> HighEnergyMeshes;
        public List<Mesh> MediumEnergyMeshes;
        public List<Mesh> LowEnergyMeshes;

        [Header("Action icons")]
        public Sprite MoveAction;
        public Sprite InteractAction;
        public Sprite BuildAction;

        public Mesh GetWreckageMesh(float energy) {
            if (energy < Constants.HighEnergyValue / 3) {
                return LowEnergyMeshes[Random.Range(0, LowEnergyMeshes.Count)];
            }
            if (energy < Constants.HighEnergyValue / 3 * 2) {
                return MediumEnergyMeshes[Random.Range(0, MediumEnergyMeshes.Count)];
            }
            return HighEnergyMeshes[Random.Range(0, HighEnergyMeshes.Count)];
        }

        public static PrefabContainer Instance => GameObject.FindObjectOfType<PrefabContainer>();
    }
}
