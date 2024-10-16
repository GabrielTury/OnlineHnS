using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node 
{
    [field: SerializeField] public Vector3 WorldPosition { get; private set; }

    [field: SerializeField] public int parentId { get; private set; }

    [field: SerializeField] public int[] neighborsIds { get; private set; }

    [field: SerializeField] public int size { get; private set; }

    [field: SerializeField] public int id {  get; private set; }

    [field: SerializeField] public int originId { get; private set; }

    [field: SerializeField] public bool isBlocked { get; private set; }

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

    public Node(Vector3 pos, int s, int id)
    {
        WorldPosition = pos;
        size = s;
        this.id = id;
    }

    public void SetHCost(int cost)
    {
        hCost = cost;
    }

    public void SetGCost(int cost)
    {
        gCost = cost;
    }

    public void SetParent(int pId)
    {
        parentId = pId;
    }

    public void SetNeighbors(int[] nId)
    {
        neighborsIds = nId;
    }

    public void SetOriginNode(int id)
    {
        originId = id;
    }
}
