using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class NavMesh : MonoBehaviour
{
    [Header("When creating the NavMesh,\nincrease its size by changing\nthe size field at the Box Collider,\ndo not change scale")]
    [SerializeField]
    BoxCollider limits;

    [SerializeField]
    NavMeshData data;

    [SerializeField, Range(1, 8)]
    private int nodeSize = 1;

    public static Node[] allNodes {  get; private set; }

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

    [ButtonMono("Create Nav Mesh")]
    private void CreateNavMesh()
    {
        SetLimits();

        allNodes = new Node[xAmount * yAmount * zAmount];

        int currentIndex = 0;
        for(int i = 0; i < xAmount; i++)
        {
        //Parallel.For(0, xAmount, i =>
        //{
            for (int j = 0; j < yAmount; j++)
            {
                for (int k = 0; k < zAmount; k++)
                {
                    Node n = new Node(new Vector3(minX + (i * nodeSize), minY + (j * nodeSize), minZ + (k * nodeSize)), nodeSize, currentIndex);
                    //Debug.Log(n.WorldPosition);
                    allNodes[currentIndex] = n;
                    currentIndex++;
                }
            }
        //});
        }

        foreach (Node n in allNodes)
        {
            Node[] neighbors = GetNeighbors(n);
            int[] ids = new int[neighbors.Length];
            for(int i = 0; i < neighbors.Length; i++)
            {
                ids[i] = neighbors[i].id;
            }
            n.SetNeighbors(ids);
            CheckForObstacle(n);
        }

        data.mesh = allNodes;

#if UNITY_EDITOR
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //once = false;
#endif
    }

    private bool CheckForObstacle(Node n)
    {
        Collider[] hit = Physics.OverlapBox(n.WorldPosition, Vector3.one * n.size/2);
        if(hit != null)
        {
            foreach (Collider c in hit)
            {
                if (c.CompareTag("NavObstacle"))
                {
                    n.SetObstacle(true);
                    return true;
                }
            }
        }

        return false;
    }

    private Node[] GetNeighbors(Node checkNode)
    {
        if (checkNode == null)
        {
            Debug.Log("CheckNodeNull");
            return new Node[0];
        }

        List<Node> neighbors = new List<Node>();

        // Calculate the grid position of the checkNode
        int nodeX = Mathf.RoundToInt((checkNode.WorldPosition.x - minX) / nodeSize);
        int nodeY = Mathf.RoundToInt((checkNode.WorldPosition.y - minY) / nodeSize);
        int nodeZ = Mathf.RoundToInt((checkNode.WorldPosition.z - minZ) / nodeSize);

        // Iterate through potential neighbors in a 3x3x3 grid
        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int zOffset = -1; zOffset <= 1; zOffset++)
                {
                    // Skip the node itself
                    if (xOffset == 0 && yOffset == 0 && zOffset == 0)
                        continue;

                    int neighborX = nodeX + xOffset;
                    int neighborY = nodeY + yOffset;
                    int neighborZ = nodeZ + zOffset;

                    // Check if the neighbor is within bounds
                    if (neighborX >= 0 && neighborX < xAmount &&
                        neighborY >= 0 && neighborY < yAmount &&
                        neighborZ >= 0 && neighborZ < zAmount)
                    {
                        // Calculate the index of the neighbor in the flat array
                        int neighborIndex = neighborX * yAmount * zAmount + neighborY * zAmount + neighborZ;

                        Node neighborNode = allNodes[neighborIndex];
                        if (neighborNode != null)
                        {
                            float dx = Mathf.Abs(neighborNode.WorldPosition.x - checkNode.WorldPosition.x);
                            float dz = Mathf.Abs(neighborNode.WorldPosition.z - checkNode.WorldPosition.z);
                            float dy = Mathf.Abs(neighborNode.WorldPosition.y - checkNode.WorldPosition.y);

                            // Check distance constraints
                            if ((dx <= nodeSize + 0.01f || dz <= nodeSize + 0.01f || dy <= nodeSize + 0.01f) &&
                                (dx < 2 && dz < 2 && dy < 2))
                            {
                                neighbors.Add(neighborNode);
                            }
                        }
                    }
                }
            }
        }

        return neighbors.ToArray();
    }
    /*private Node[] GetNeighbors(Node checkNode)
    {
        List<Node> neighbors = new List<Node>();
        int xNeighbors = 0;
        int yNeighbors = 0;
        int zNeighbors = 0;
        if (checkNode != null)
        {
            foreach (Node t in allNodes)
            {
                float dx = Mathf.Abs(t.WorldPosition.x - checkNode.WorldPosition.x);
                float dz = Mathf.Abs(t.WorldPosition.z - checkNode.WorldPosition.z);
                float dy = Mathf.Abs(t.WorldPosition.y - checkNode.WorldPosition.y);
                if ((dx <= nodeSize + 0.01f || dz <= nodeSize + 0.01f || dy <= nodeSize + 0.01f) && (dx < 2 && dz < 2 && dy < 2))
                {
                    if (t != checkNode) // Exclude the Node itself
                    {
                        neighbors.Add(t);
                    }

                    if (dx < nodeSize) xNeighbors++;
                    if (dy < nodeSize) yNeighbors++;
                    if (dz < nodeSize) zNeighbors++;
                }
            }
        }
        else
        {
            Debug.Log("CheckNodeNull");
        }

            return neighbors.ToArray();
    }*/

#if UNITY_EDITOR
    // bool once = false;
#endif
    private void OnDrawGizmosSelected()
    {
        //if (once) return;

        if (allNodes == null && data.mesh != null)
        {
            allNodes = data.mesh;
            //Debug.Log("NavMeshSet");
            //Debug.Log(allNodes.Length);
            //Debug.Log(data.mesh.Length);
        }
        else if (allNodes == null)
        {
            //Debug.Log("Return");
            return;
        }
        
        foreach(Node n in data.mesh)
        {
            //Debug.Log("Draw");
            Gizmos.color = Color.yellow;

            if (n.isBlocked)
                Gizmos.color = Color.red;

            Gizmos.DrawWireCube(n.WorldPosition, n.size * Vector3.one);
        }
        //once = true;
    }
}
