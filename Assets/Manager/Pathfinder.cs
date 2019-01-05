using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public Dictionary<Vector3, TileScript> allTiles = new Dictionary<Vector3, TileScript>();
    private TileScript source;
    private TileScript target;
    private List<TileScript> path = new List<TileScript>();

    [SerializeField]
    private Color defaultHexColor = Color.black;

    [SerializeField]
    private Color selectedHexColor = Color.blue;

    [SerializeField]
    private Color visibleHexColor = Color.yellow;

    [SerializeField]
    private Color pathHexColor = Color.green;

    void Start()
    {
        foreach (TileScript tile in FindObjectsOfType<TileScript>())
        {
            if (allTiles.ContainsKey(tile.pos)) {
                throw new System.Exception("Repeating position of tile..." + tile.pos.ToString());
            }
            allTiles[tile.pos] = tile;

            tile.SetHexColor(defaultHexColor);
        }
    }

    public void OnEnter(TileScript obj)
    {
        target = obj;

        foreach (TileScript tile in obj.GetVisibleSurroundings(4))
        {
            if (tile == source) continue;
            tile.SetHexColor(visibleHexColor);
        }

        if (source != null)
        {
            path = source.GetPathTo(target);
            foreach(TileScript tile in path)
            {
                tile.SetHexColor(pathHexColor);
            }
        }

        if (obj != source)
        {
            obj.SetHexColor(pathHexColor);
        }
    }

    public void OnExit(TileScript obj)
    {
        target = null;
        if (obj != source)
        {
            obj.SetHexColor(defaultHexColor);
        }
        if (path.Count > 0)
        {
            foreach (TileScript tile in path)
            {
               tile.SetHexColor(defaultHexColor);
            }
            path.Clear();
        }
        foreach (TileScript tile in obj.GetVisibleSurroundings(10))
        {
            if (tile == source) continue;
            tile.SetHexColor(defaultHexColor);
        }
    }

    public void OnDown()
    {
        if (source)
        {
            source.SetHexColor(defaultHexColor);
        }
        if (path.Count > 0)
        {
            foreach (TileScript tile in path)
            {
                tile.SetHexColor(defaultHexColor);
            }
            path = new List<TileScript>();
        }
        if (source == target) target = null;
        source = target;
        if (source)
        {
            source.SetHexColor(selectedHexColor);
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnDown();
    }
}
