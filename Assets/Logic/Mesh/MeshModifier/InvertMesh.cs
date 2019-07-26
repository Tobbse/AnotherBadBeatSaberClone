using UnityEngine;

public class InvertMesh : MonoBehaviour
{
    private Mesh _mesh;

    // Start is called before the first frame update
    void Start()
    {
        _mesh = this.GetComponent<MeshFilter>().mesh;

        _invertNormals();
        _invertVertices();
    }

    private void _invertNormals()
    {
        Vector3[] meshNormals = _mesh.normals;
        for (int i = 0; i < meshNormals.Length; i++)
        {
            meshNormals[i] *= -1;
        }
        _mesh.normals = meshNormals;
    }

    private void _invertVertices()
    {
        for (int i = 0; i < _mesh.subMeshCount; i++)
        {
            int[] triangles = _mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j+=3)
            {
                int tempVertex = triangles[j];
                triangles[j] = triangles[j + 1];
                triangles[j + 1] = tempVertex;
            }

            _mesh.SetTriangles(triangles, i);
        }
    }

}
