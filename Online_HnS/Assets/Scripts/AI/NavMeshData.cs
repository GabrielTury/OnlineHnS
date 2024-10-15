using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelMeshData", menuName = "NavMesh/NavMeshData")]
public class NavMeshData : ScriptableObject
{
    public Node[] mesh;
}
