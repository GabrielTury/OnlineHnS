using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMesh : MonoBehaviour
{
    [Header("When creating the NavMesh,\nincrease its size by changing\nthe size field at the Box Collider,\ndo not change scale")]
    [SerializeField]
    BoxCollider limits;

    [SerializeField]
    NavMeshData data;

    [SerializeField, Range(1, 8)]
    private int nodeSize = 1;

    public Node[] allNodes {  get; private set; }

    private int xAmount, yAmount, zAmount;
    private int minX, minY, minZ;
    private int maxX, maxY, maxZ;

    private void Awake()
    {
        allNodes = data.mesh;
    }
    private void SetLimits()
    {
        Vector3 size = limits.size;        
        Vector3 worldCenter = limits.transform.TransformPoint(limits.center);
        minX = (int)(worldCenter.x - size.x *0.5f);
        minY = (int)(worldCenter.y - size.y * 0.5f);
        minZ = (int)(worldCenter.z - size.z * 0.5f);

        maxX = (int)(worldCenter.x + size.x * 0.5f);
        maxY = (int)(worldCenter.y + size.y * 0.5f);
        maxZ = (int)(worldCenter.z + size.z * 0.5f);
        
        xAmount = Mathf.Abs((maxX - minX) / nodeSize);
        yAmount = Mathf.Abs((maxY - minY) / nodeSize);
        zAmount = Mathf.Abs((maxZ - minZ) / nodeSize);
    }

    [Button("Create Nav Mesh")]
    private void CreateNavMesh()
    {
        SetLimits();

        allNodes = new Node[xAmount * yAmount * zAmount];

        int currentIndex = 0;
        for(int i = 0; i < xAmount; i++)
        {
            for (int j = 0; j < yAmount; j++)
            {
                for(int k = 0; k < zAmount; k++)
                {
                    Node n = new Node(new Vector3(minX + i, minY + j, minZ + k), nodeSize);
                    Debug.Log(n.WorldPosition);
                    allNodes[currentIndex] = n;
                    currentIndex++;
                }
            }
        }

        data.mesh = allNodes;
    }

    private void OnDrawGizmosSelected()
    {
        if (allNodes == null && data.mesh != null)
            allNodes = data.mesh;
        else if (allNodes == null)
            return;

        foreach(Node n in allNodes)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(n.WorldPosition, n.size * Vector3.one);
        }
    }
}
