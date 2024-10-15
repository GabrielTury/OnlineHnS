using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public Vector3 WorldPosition { get; private set; }

    public Node parent { get; private set; }

    public Node[] neighbors { get; private set; }

    public int size { get; private set; }

    #region Costs
    /// <summary>
    /// Acumulated cost
    /// </summary>
    public int gCost {  get; private set; }
    /// <summary>
    /// Distance Cost
    /// </summary>
    public int hCost { get; private set; }
    /// <summary>
    /// Total cost (sum of gCost + hCost)
    /// </summary>
    public int fCost { get => gCost + hCost;}
    #endregion

    public Node(Vector3 pos, int s)
    {
        WorldPosition = pos;
        size = s;
    }

    public void SetHCost(int cost)
    {
        hCost = cost;
    }

    public void SetGCost(int cost)
    {
        gCost = cost;
    }

    public void SetParent(Node p)
    {
        parent = p;
    }

    public void SetNeighbors(Node[] n)
    {
        neighbors = n;
    }
}
