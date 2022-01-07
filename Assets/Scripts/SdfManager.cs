using UnityEngine;

public class SdfManager : MonoBehaviour
{
    const int MaxShapes = 256;
    int _numShapes;
    [SerializeField] Material _material;
    [SerializeField] float _growSpeed = 100f;
    [SerializeField] int _numStartingShapes;

    ComputeBuffer _sdfBuffer;
    ComputeBuffer _vertexBuffer;
    Sdf[] _sdfs;

    Camera _camera;

    struct Sdf
    {
        public Vector3 Position;
        public float Size;
        public Vector2 Direction;
        public float StartTime;
    }

    void Start()
    {
        _camera = FindObjectOfType<Camera>();

        _numShapes = _numStartingShapes;
        _sdfBuffer = new ComputeBuffer(MaxShapes, sizeof(float) * 7, ComputeBufferType.Default);
        _vertexBuffer = new ComputeBuffer(6, sizeof(float) * 3, ComputeBufferType.Default);

        _sdfs = new Sdf[MaxShapes];
        
        var aspectRatio = Screen.width / (float)Screen.height;
        
        for (int i = 0; i < _numShapes; ++i)
        {
            Vector3 position = new Vector3(Random.Range(-aspectRatio, aspectRatio), Random.Range(-1, 1), 0);
            _sdfs[i] = new Sdf
            {
                Position = position, Size = Random.Range(0.1f, 0.5f),
                Direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * Random.Range(2f, 5f)
            };
        }

        _sdfBuffer.SetData(_sdfs);

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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var aspectRatio = Screen.width / (float)Screen.height;
            var viewportPosition = _camera.ScreenToViewportPoint(Input.mousePosition) * 2 - Vector3.one;
            viewportPosition.y = -viewportPosition.y;
            viewportPosition.x *= aspectRatio;
            AddCircle(viewportPosition);
        }
        else if (Input.GetMouseButton(0))
        {
            int index = _numShapes - 1;
            ResizeCircle(index, _sdfs[index].Size + (Time.deltaTime * _growSpeed));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < _numShapes; ++i)
            {
                _sdfs[i].StartTime = Time.time;
            }
            _sdfBuffer.SetData(_sdfs);

            var isMovementEnabled = _material.GetFloat("_EnableMovement");
            _material.SetFloat("_EnableMovement", 1 - isMovementEnabled);
        }
    }

    void AddCircle(Vector3 position)
    {
        _sdfs[_numShapes] = new Sdf
        {
            Position = position, Size = 0.01f,
            Direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * Random.Range(2f, 5f),
            StartTime = Time.time
        };
        _sdfBuffer.SetData(_sdfs);

        ++_numShapes;
        _material.SetInt("_NumSdfs", _numShapes);
    }

    void ResizeCircle(int index, float size)
    {
        _sdfs[index].Size = size;
        _sdfs[index].StartTime = Time.time;
        _sdfBuffer.SetData(_sdfs);
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