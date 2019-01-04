﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileScript : MonoBehaviour
{
    public Vector3Int pos;
    public float elevation = 0;
    public float height = 30f;
    private Pathfinder pathfinder;
    private TileScript group = null;

    const int MAX_STEP = 1;
    const int HEIGHT_OF_VISIBILITY = 1;

    void Start()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    void SetGroup(TileScript groupToSet)
    {
        if (group != null) return;

        group = groupToSet;

        foreach (TileScript tile in GetNeighbours())
            tile.SetGroup(group);
    }

    public void MoveToPosition()
    {
        gameObject.transform.position = new Vector3(pos.x*Mathf.Sqrt(1-0.25f), elevation-height/2f, pos.y - pos.x/2f);
        Vector3 scale = transform.localScale;
        scale.Set(1, height/2f, 1);
        transform.localScale = scale;
    }

    private void OnMouseEnter()
    {
        pathfinder.OnEnter(this);
    }

    private void OnMouseExit()
    {
        pathfinder.OnExit(this);
    }

    public Vector3 GetTop()
    {
        return transform.position + new Vector3(0, transform.localScale.y, 0);
    }
    
    public List<TileScript> GetPathTo(TileScript target)
    {
        Dictionary<TileScript, TileScript> parent = new Dictionary<TileScript, TileScript>();
        Dictionary<TileScript, int> traveled = new Dictionary<TileScript, int> {
            {this, 0 }
        };
        List<TileScript> result = new List<TileScript>();

        SortedDictionary<int, List<TileScript>> toSearch = new SortedDictionary<int, List<TileScript>>
        {
            { this.Distance(target.pos), new List<TileScript>() { this } }
        };

        SetGroup(this);
        target.SetGroup(target);

        while (toSearch.Count > 0)
        {
            SortedDictionary<int, List<TileScript>>.KeyCollection.Enumerator enumerator = toSearch.Keys.GetEnumerator();
            enumerator.MoveNext();
            int currentDistance = enumerator.Current;
            int randIndex = 0; // Random.Range(0, toSearch[currentDistance].Count);

            TileScript current = toSearch[currentDistance][randIndex];
            toSearch[currentDistance].RemoveAt(randIndex);
            if (toSearch[currentDistance].Count == 0)
                toSearch.Remove(currentDistance);
            
            if (current.group != null && target.group != null && current.group != target.group)
                break;

            if (current == target)
            {
                while (current != this)
                {
                    result.Add(current);
                    current = parent[current];
                }
                break;
            }

            foreach (TileScript neighbour in current.GetNeighbours())
            {
                if (parent.ContainsKey(neighbour)) continue;
                int distance = neighbour.Distance(target.pos) + traveled[current];
                if (!toSearch.ContainsKey(distance)) toSearch[distance] = new List<TileScript>();
                toSearch[distance].Add(neighbour);
                parent[neighbour] = current;
                traveled[neighbour] = traveled[current] + 1;
            }
        }
        
        return result;
    }

    public List<TileScript> GetSurroundings(int distance = 1)
    {
        List<TileScript> result = new List<TileScript>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                for (int z = -10; z < 11; z++)
                {
                    Vector3Int key = new Vector3Int(pos.x + x, pos.y + y, z);
                    if (Distance(key) > distance || Distance(key) == 0) continue;
                    if (!pathfinder.allTiles.ContainsKey(key)) continue;
                    TileScript neighbour = pathfinder.allTiles[key];
                    result.Add(neighbour);
                }
            }
        }

        return result;
    }

    public List<TileScript> GetVisibleSurroundings(int distance)
    {
        List<TileScript> neigbours = GetSurroundings(distance);
        HashSet<TileScript> result = new HashSet<TileScript>();

        RaycastHit hit;
        foreach (TileScript target in neigbours)
        {
            int layerMask = 1 << 9;
            Vector3 source = GetTop() + new Vector3(0, HEIGHT_OF_VISIBILITY, 0);
            Vector3 destination = target.GetTop() + new Vector3(0, HEIGHT_OF_VISIBILITY, 0);
            Vector3 direction = destination - source;
            float dist = Vector3.Distance(source, destination);
            if (Physics.Raycast(source, direction, out hit, dist, layerMask))
            {
                result.Add(hit.collider.gameObject.GetComponent<TileScript>());
            } else
            {
                result.Add(target);
            }
        }

        return result.ToList();
    }

    public List<TileScript> GetNeighbours()
    {
        List<TileScript> result = GetSurroundings(distance: 1);
        result.RemoveAll(tile => Mathf.Abs(GetTop().y - tile.GetTop().y) > 1);

        return result;
    }


    int Distance(Vector3Int otherPos)
    {
        return (
            Mathf.Abs(pos.x - otherPos.x) +
            Mathf.Abs(pos.y - otherPos.y) +
            Mathf.Abs((pos.x - pos.y) - (otherPos.x - otherPos.y))
            ) / 2;
    }
}
