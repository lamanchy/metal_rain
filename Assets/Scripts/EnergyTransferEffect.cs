using System;
using DigitalRuby.LightningBolt;
using Manager;
using UnityEngine;

public class EnergyTransferEffect : IDisposable {
    public readonly LightningBoltScript Lightning;
    private static GameObject prefab;

    public EnergyTransferEffect(GameObject start, GameObject end) {
        if (prefab == null) {
            prefab = GameObject.FindObjectOfType<PrefabContainer>().LightningPrefab;
        }
        Lightning = GameObject.Instantiate(prefab, start.transform).GetComponent<LightningBoltScript>();
        Lightning.StartObject = start;
        Lightning.EndObject = end;
    }

    public void Dispose() {
        if (Lightning != null) {
            GameObject.Destroy(Lightning.gameObject);
        }
    }
}
