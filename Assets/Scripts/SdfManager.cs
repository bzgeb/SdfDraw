using UnityEngine;

public class SdfManager : MonoBehaviour
{
    const int MaxShapes = 128;
    int _numShapes;
    [SerializeField] Material _material;

    ComputeBuffer _sdfBuffer;
    ComputeBuffer _vertexBuffer;

    struct Sdf
    {
        public Vector3 Position;
        public float Size;
        public Vector2 Direction;
    }

    void Start()
    {
        _numShapes = Random.Range(32, MaxShapes);
        _sdfBuffer = new ComputeBuffer(_numShapes, sizeof(float) * 6, ComputeBufferType.Default);
        _vertexBuffer = new ComputeBuffer(6, sizeof(float) * 3, ComputeBufferType.Default);

        var sdfs = new Sdf[_numShapes];
        for (int i = 0; i < _numShapes; ++i)
        {
            Vector3 position = new Vector3(Random.Range(-4f, 4f), Random.Range(-2f, 2f), 0);
            sdfs[i] = new Sdf
            {
                Position = position, Size = Random.Range(0.1f, 0.5f),
                Direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * Random.Range(2f, 5f)
            };
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

        _material.SetBuffer("_SdfBuffer", _sdfBuffer);
        _material.SetBuffer("_VertexBuffer", _vertexBuffer);
        _material.SetInt("_NumSdfs", _numShapes);
    }

    void OnRenderObject()
    {
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, 1);
    }

    void OnDestroy()
    {
        _sdfBuffer.Release();
        _vertexBuffer.Release();
    }
}