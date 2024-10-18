using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NavOperations
{
    #if UNITY_EDITOR
    public static bool debug;
#endif

    public static Node[] checkMesh;
    public static Vector3[] CalculatePath(Node target, Node start, Node[] navMesh)
    {
        checkMesh = NavMesh.allNodes;
        List<Node> openNodeList = new List<Node>();
        List<Node> closedNodeList = new List<Node>();

        openNodeList.Add(start);
        Node checkNode = start;
        checkNode.SetGCost(0);

        int iterations = 0;

        while (openNodeList.Count > 0)
        {
            openNodeList.Sort((n1, n2) =>
            {
                int ret = (n1.fCost).CompareTo(n2.fCost);
                return ret != 0 ? ret : n1.gCost.CompareTo(n2.gCost);
            });

            iterations++;
            checkNode = openNodeList[0];

            if (checkNode == target)
            {
                List<Node> finalNodes = ReconstructPath(target);
                List<Vector3> path = new List<Vector3>();
                foreach (Node node in finalNodes)
                {
                    path.Add(node.WorldPosition);
                }
                Debug.Log("Created Path");
                Debug.LogWarning("Iterations: " + iterations);
                return path.ToArray();
            }

            openNodeList.Remove(checkNode);
            closedNodeList.Add(checkNode);

            List<Node> neighbors = new List<Node>();

            for(int i =0;i < checkNode.neighborsIds.Length;i++)
            {                
                neighbors.Add(navMesh[checkNode.neighborsIds[i]]);
            }
            

            foreach (Node n in neighbors)
            {
                if (n.isBlocked)
                {
                    continue;
                }
                if (closedNodeList.Contains(n))
                    continue;
                if (!closedNodeList.Contains(n))
                {
                    // Calculate tentative gCost (cost from start to this tile)
                    int tentativeGCost = checkNode.gCost+ 1;

                    if (!openNodeList.Contains(n) || tentativeGCost < n.gCost)
                    {
                        n.SetGCost(tentativeGCost + n.hCost);
                        n.SetOriginNode(checkNode.id);

                        if (!openNodeList.Contains(n))
                        {
                            openNodeList.Add(n);
                            CalculateListHeuristicCost(target.WorldPosition, n);
                        }
                    }
                }
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
        Debug.Log("Reconstructed path size: " + path.Count);

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

    private static void CalculateListHeuristicCost(Vector3 target, Node node)
    {

        float totalDistance = Mathf.Abs(target.x - node.WorldPosition.x) + Mathf.Abs(target.z - node.WorldPosition.z) + Mathf.Abs(target.y - node.WorldPosition.y);
        node.SetHCost((int)totalDistance);

    }
}
