using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorGenerator : MonoBehaviour
{
    public GameObject meteor;
    TimeManager timeManager;
    Pathfinder pathfinder;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        pathfinder = FindObjectOfType<Pathfinder>();
        for (int i = 0; i < 10; i++)
            GenerateMeteor();
    }

    void GenerateMeteor()
    {
        GameObject instance = Instantiate(meteor, transform);
        int distance = 30;
        instance.transform.position = new Vector3(Random.Range(-20f, 20f) + distance, distance, Random.Range(-20f, 20f) - distance);
        float sizeFactor = Mathf.Sqrt(Random.Range(0.1f, 4f));

        instance.transform.localScale *= sizeFactor;
        MeteorScript meteorScript = instance.GetComponent<MeteorScript>();
        
        meteorScript.velocity = new Vector3(
            -2 + Random.Range(-0.1f, 0.1f), 
            -2 + Random.Range(-0.1f, 0.1f),
            2 + Random.Range(-0.1f, 0.1f)
            ) / sizeFactor;
        meteorScript.angularVelocity = new Vector3(
            2 * Random.Range(0,2) * 2 - 1 + Random.Range(-1f, 1f),
            2 * Random.Range(0, 2) * 2 - 1 + Random.Range(-1f, 1f),
            2 * Random.Range(0, 2) * 2 - 1 + Random.Range(-1f, 1f)
            ) / sizeFactor;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeManager.running && 40 > Random.Range(0, 100))
        {
            GenerateMeteor();
        }
    }
}
