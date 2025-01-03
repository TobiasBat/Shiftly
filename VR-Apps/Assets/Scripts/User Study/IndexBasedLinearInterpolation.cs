using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class IndexBasedLinearInterpolation : MonoBehaviour
{
    public string nameMesh0; 
    public Mesh mesh0;
    public string nameMesh1;
    public Mesh mesh1;
    [Range(0.0f,1.0f)]
    public float interValue = 0.0f;
    [HideInInspector]
    public MeshFilter meshFilter;
    [HideInInspector]
    public Mesh interpolatedMesh;

    private float lastInterpolatedValue = -0.1f; 

    // Start is called before the first frame update
    void Start()
    {
        
        
        // updateInterpolateMesh(interValue); 

    }

    // Update is called once per frame
    void Update()
    {
        if (lastInterpolatedValue != interValue)
        {
            updateInterpolateMesh(interValue);
        }
        
    }

    public void Init()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        interpolatedMesh = new Mesh();
        gameObject.GetComponent<MeshFilter>().mesh = interpolatedMesh;
        interpolatedMesh.name = "Interpolated Mesh";

        if (mesh0.vertices.Length != mesh1.vertices.Length ||
            mesh0.normals.Length != mesh1.normals.Length)
        {
            Debug.LogError("Meshes have to have same topology");
        }

        updateInterpolateMesh(interValue);
    }

    public void updateInterpolateMesh(float t)
    {
        interValue = t; 
        interpolatedMesh.Clear();
        Vector3[] vertices = new Vector3[mesh0.vertices.Length];
        Vector3[] normals = new Vector3[mesh0.normals.Length]; 
        for (int i = 0; i < mesh0.vertices.Length && i < mesh1.vertices.Length; i++)
        {
            Vector3 v0 = mesh0.vertices[i];
            Vector3 v1 = mesh1.vertices[i];
            Vector3 v = (1.0f - interValue) * v0 + interValue * v1;
            vertices[i] = v;

            Vector3 n0 = mesh0.normals[i];
            Vector3 n1 = mesh1.normals[i];
            Vector3 n = (1.0f - interValue) * n0 + interValue * n1;
            normals[i] = n; 
        }

        interpolatedMesh.vertices = vertices;
        interpolatedMesh.normals = normals;
        interpolatedMesh.triangles = mesh0.triangles;

        lastInterpolatedValue = t; 
    }
    
}
