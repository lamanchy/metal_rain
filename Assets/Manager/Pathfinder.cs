using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public Dictionary<Vector3, TileScript> allTiles = new Dictionary<Vector3, TileScript>();
    private TileScript source;
    private TileScript target;
    private List<TileScript> path = new List<TileScript>();

    void Start()
    {
        foreach (TileScript tile in FindObjectsOfType<TileScript>())
        {
            if (allTiles.ContainsKey(tile.pos)) {
                throw new System.Exception("Repeating position of tile..." + tile.pos.ToString());
            }
            allTiles[tile.pos] = tile;
        }
    }

    public void OnEnter(TileScript obj)
    {
        target = obj;

        foreach (TileScript tile in obj.GetVisibleSurroundings(10))
        {
            if (tile == source) continue;
            Renderer rend = tile.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.grey);
        }

        if (source != null)
        {
            path = source.GetPathTo(target);
            foreach(TileScript tile in path)
            {
                Renderer rend = tile.GetComponent<Renderer>();
                rend.material.SetColor("_Color", Color.white);
            }
        }

        if (obj != source)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.white);
        }
    }

    public void OnExit(TileScript obj)
    {
        target = null;
        if (obj != source)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);
        }
        if (path.Count > 0)
        {
            foreach (TileScript tile in path)
            {
                Renderer rend = tile.GetComponent<Renderer>();
                rend.material.SetColor("_Color", Color.red);
            }
            path = new List<TileScript>();
        }
        foreach (TileScript tile in obj.GetVisibleSurroundings(10))
        {
            if (tile == source) continue;
            Renderer rend = tile.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);
        }
    }

    public void OnDown()
    {
        Renderer rend;
        if (source)
        {
            rend = source.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);
        }
        if (path.Count > 0)
        {
            foreach (TileScript tile in path)
            {
                rend = tile.GetComponent<Renderer>();
                rend.material.SetColor("_Color", Color.red);
            }
            path = new List<TileScript>();
        }
        if (source == target) target = null;
        source = target;
        if (source)
        {
            rend = source.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.blue);
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnDown();
    }
}
