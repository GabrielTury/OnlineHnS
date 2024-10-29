using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;


public class NavOperations
{
    #if UNITY_EDITOR
    public static bool debug;
#endif

    public static Node[] checkMesh;
    public static Vector3[] CalculatePath(Node target, Node start, Node[] navMesh, bool use3DMesh)
    {
        checkMesh = NavMesh.allNodes;
        SortedSet<Node> openNodeList = new SortedSet<Node>(new NodeComparer());
        HashSet<Node> closedNodeList = new HashSet<Node>();

        openNodeList.Add(start);
        Node checkNode = start;
        checkNode.SetGCost(0);

        int iterations = 0;

        while (openNodeList.Count > 0)
        {
            if(iterations > 5000)
            {
                Debug.LogError("MORE THAN ITERATIONS");
                break;
            }
            iterations++;
            checkNode = openNodeList.Min;

            if (checkNode.id == target.id)
            {
                List<Node> finalNodes = ReconstructPath(target);
                List<Vector3> path = new List<Vector3>();
                foreach (Node node in finalNodes)
                {
                    path.Add(node.WorldPosition);
                }
                //Debug.Log("Created Path");
                //Debug.LogWarning("Iterations: " + iterations);
                foreach(Node n in openNodeList)
                {
                    n.SetOriginNode(0);
                }
                openNodeList.Clear();

                foreach(Node n in closedNodeList)
                {
                    n.SetOriginNode(0);
                }
                closedNodeList.Clear();

                return path.ToArray();
            }

            if(!openNodeList.Remove(checkNode))
            {
                Debug.Log("Couldn't Remove");
            }
            closedNodeList.Add(checkNode);

            List<Node> neighbors = new List<Node>();
            if(use3DMesh)
            {
                for(int i =0;i < checkNode.neighborsIds.Length;i++)
                {                    
                    neighbors.Add(navMesh[checkNode.neighborsIds[i]]);                                  
                }
            }
            else if (!use3DMesh)
            {
                for (int i = 0; i < checkNode.planarNeighborsIds.Length; i++)
                {
                    neighbors.Add(navMesh[checkNode.planarNeighborsIds[i]]);
                }
            }
            

            foreach (Node n in neighbors)
            {
                if (n.isBlocked)
                {
                    continue;
                }
                if (closedNodeList.Contains(n))
                    continue;
                    // Calculate tentative gCost (cost from start to this tile)
                    float tentativegCost = checkNode.gCost + Vector3.Distance(n.WorldPosition, checkNode.WorldPosition);
                
                if(!openNodeList.Contains(n))
                {
                    n.SetGCost(tentativegCost);
                    n.SetOriginNode(checkNode.id);
                    CalculateListHeuristicCost(target.WorldPosition, n);
                }
                else if (checkNode.fCost < n.fCost)
                {
                    openNodeList.Remove(n);
                    n.SetGCost(tentativegCost + n.hCost);
                    n.SetOriginNode(checkNode.id);
                }

                if(!openNodeList.Contains(n))
                    openNodeList.Add(n);

            }
        }
        return null;
    }


    private static List<Node> ReconstructPath(Node current)
    {
        List<Node> path = new List<Node>();
        while (current != null)
        {            
            path.Add(current);
            if (current.originId == 0) break;
            current = checkMesh[current.originId];
        }
        path.Reverse(); // Reverse the path to get it from start to end
        //Debug.Log("Reconstructed path size: " + path.Count);

        //Debug.Log("Path successfully reconstructed");

        return path;
    }

    public static Node GetNearestNode(Vector3 pos, Node[] mesh)
    {
        Node nearest = null;
        float distance = 100;
        foreach (Node node in mesh)
        {
            float currentDistance = Vector3.Distance(pos, node.WorldPosition);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                nearest = node;
            }
        }
        return nearest;
    }

    public static Node GetNearestNodeInPlane(Vector3 pos, Node[] mesh, float y)
    {
        Node nearest = null;
        float distance = 100;
        foreach (Node node in mesh)
        {
            if (node.WorldPosition.y != y)
                continue;

            if (node.isBlocked)
                continue;

            float currentDistance = Vector3.Distance(pos, node.WorldPosition);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                nearest = node;
            }
        }
        return nearest;
    }

    private static void CalculateListHeuristicCost(Vector3 target, Node node)
    {

        float totalDistance = Mathf.Abs(target.x - node.WorldPosition.x) + Mathf.Abs(target.z - node.WorldPosition.z) + Mathf.Abs(target.y - node.WorldPosition.y);
        node.SetHCost((int)totalDistance);

    }

    public static bool CheckForPathDiff(Vector3[] path1, Vector3[] path2)
    {
        if (path1[path1.Length -1] != path2[path2.Length  - 1])
        {
            return true;
        }
        return false;
    }
}
public class NodeComparer : IComparer<Node>
{
    public int Compare(Node x, Node y)
    {
        int result = x.fCost.CompareTo(y.fCost);
        if (result == 0)
        {
            result = x.gCost.CompareTo(y.gCost);
        }
        if (result == 0)
        {
            result = x.hCost.CompareTo(y.hCost);
        }
        if(result == 0)
        {
            result = x.id.CompareTo(y.id);
        }
        return result;
    }
}
