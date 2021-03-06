﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementation of A* pathfinding algorithm.
/// </summary>
class AStarPathfinding : IPathfinding
{
   // private List<Cell> safePath;

    public override List<T> FindPath<T>(Dictionary<T, Dictionary<T, int>> edges, T originNode, T destinationNode)
    {
        IPriorityQueue<T> frontier = new HeapPriorityQueue<T>();
        frontier.Enqueue(originNode, 0);

        Dictionary<T, T> cameFrom = new Dictionary<T, T>();
        cameFrom.Add(originNode, default(T));
        Dictionary<T, int> costSoFar = new Dictionary<T, int>();
        costSoFar.Add(originNode, 0);

        while (frontier.Count != 0)
        {
            var current = frontier.Dequeue();
            if (current.Equals(destinationNode)) break;

            var neighbours = GetNeigbours(edges, current);
            foreach (var neighbour in neighbours)
            {
                var newCost = costSoFar[current] + edges[current][neighbour];
                if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                {
                    costSoFar[neighbour] = newCost;
                    cameFrom[neighbour] = current;
                    var priority = newCost + Heuristic(destinationNode, neighbour);
                    frontier.Enqueue(neighbour, priority);
                }
            }
        }

        List<T> path = new List<T>();
        if (!cameFrom.ContainsKey(destinationNode))
            return path;

        path.Add(destinationNode);
        var temp = destinationNode;
//        Debug.Log(temp + " - " + destinationNode + " - " + originNode);
        try
        {
            while (!cameFrom[temp].Equals(originNode))
            {
                var currentPathElement = cameFrom[temp];
                path.Add(currentPathElement);

                temp = currentPathElement;
            }
            if (path.Count != 0 && path[0] is Cell)
            {
              //  safePath = new List<Cell>();
               // foreach (T elem in path)
                //    safePath.Add(elem as Cell);
            }
        }
        catch (System.NullReferenceException e)
        {
            return path; //safePath;
        }

        return path;
    }
    private int Heuristic<T>(T a, T b) where T : IGraphNode
    {
        return a.GetDistance(b);
    }
}



