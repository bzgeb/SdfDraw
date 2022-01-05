using UnityEngine;

public class SdfManager : MonoBehaviour
{
    const int MaxShapes = 512;
    int _numShapes;
    Material material;

    ComputeBuffer _sdfBuffer;
    ComputeBuffer _vertexBuffer;

    struct Sdf
    {
        public Vector3 Position;
        public float Size;
    }

    void Start()
    {
        material = new Material(Shader.Find("SdfShape"));

        _numShapes = Random.Range(128, MaxShapes);
        _sdfBuffer = new ComputeBuffer(_numShapes, sizeof(float) * 4, ComputeBufferType.Default);
        _vertexBuffer = new ComputeBuffer(6, sizeof(float) * 3, ComputeBufferType.Default);

        var sdfs = new Sdf[_numShapes];
        for (int i = 0; i < _numShapes; ++i)
        {
            Vector3 position = new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), 0);
            sdfs[i] = new Sdf { Position = position, Size = Random.Range(0.2f, 0.6f) };
        }

        _sdfBuffer.SetData(sdfs);

        var vertexBuffer = new Vector3[]
        {
            new Vector3(-1.0f, -1.0f, 0.0f),
            new Vector3(1.0f, -1.0f, 0.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(1.0f, -1.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),
        };
        _vertexBuffer.SetData(vertexBuffer);

        material.SetBuffer("_SdfBuffer", _sdfBuffer);
        material.SetBuffer("_VertexBuffer", _vertexBuffer);
        material.SetInt("_NumSdfs", _numShapes);
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, 1);
    }

    void OnDestroy()
    {
        _sdfBuffer.Release();
        _vertexBuffer.Release();
    }
}