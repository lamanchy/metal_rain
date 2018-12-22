using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public Vector3Int pos;
    public float elevation = 0;
    public float height = 30f;
    private Pathfinder pathfinder;

    void Start()
    {
        pathfinder = FindObjectOfType<Pathfinder>();
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


    // fns
    
    public List<TileScript> GetPathTo(TileScript target)
    {
        Dictionary<TileScript, TileScript> parent = new Dictionary<TileScript, TileScript>();
        Dictionary<TileScript, int> traveled = new Dictionary<TileScript, int> {
            {this, 0 }
        };
        List<TileScript> result = new List<TileScript>();
        SortedDictionary<int, List<TileScript>> toSearch = new SortedDictionary<int, List<TileScript>>
        {
            { this.Distance(target), new List<TileScript>() { this } }
        };

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
                int distance = neighbour.Distance(target) + traveled[current];
                if (!toSearch.ContainsKey(distance)) toSearch[distance] = new List<TileScript>();
                toSearch[distance].Add(neighbour);
                parent[neighbour] = current;
                traveled[neighbour] = traveled[current] + 1;
            }
        }
        
        return result;
    }

    List<TileScript> GetNeighbours()
    {
        List<TileScript> result = new List<TileScript>();
        List<Vector2Int> moves = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1)
        };


        foreach (Vector2Int move in moves)
        {
            for (int z = -10; z < 11; z++)
            {
                Vector3Int key = new Vector3Int(pos.x+move.x, pos.y+move.y, z);
                if (!pathfinder.allTiles.ContainsKey(key)) continue;
                TileScript neighbour = pathfinder.allTiles[key];
                if (Mathf.Abs(neighbour.elevation - elevation) > 1) continue;
                result.Add(neighbour);
            }
        }

        return result;
    }


    int Distance(TileScript other)
    {
        return (
            Mathf.Abs(pos.x - other.pos.x) +
            Mathf.Abs(pos.y - other.pos.y) +
            Mathf.Abs((pos.x - pos.y) - (other.pos.x - other.pos.y))
            ) / 2;
    }
}
