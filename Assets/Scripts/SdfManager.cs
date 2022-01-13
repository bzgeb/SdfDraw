using UnityEngine;

public class SdfManager : MonoBehaviour
{
    const int MaxShapes = 64;
    int _numShapes;
    [SerializeField] Material _materialPrototype;
    Material _material;
    [SerializeField] float _growSpeed = 100f;
    [SerializeField] int _numStartingShapes;

    Vector4[] _sdfPositions;
    float[] _sdfSizes;
    Vector4[] _sdfDirections;
    float[] _sdfStartTimes;

    Camera _camera;

    void OnEnable()
    {
        var meshRenderer = FindObjectOfType<MeshRenderer>();
        _material = new Material(_materialPrototype);
        _material.name = "Instance";
        meshRenderer.material = _material;
        _camera = FindObjectOfType<Camera>();

        _numShapes = _numStartingShapes;

        _sdfPositions = new Vector4[MaxShapes];
        _sdfSizes = new float[MaxShapes];
        _sdfDirections = new Vector4[MaxShapes];
        _sdfStartTimes = new float[MaxShapes];

        for (int i = 0; i < MaxShapes; ++i)
        {
            _sdfStartTimes[i] = -1;
        }
        
        for (int i = 0; i < _numShapes; ++i)
        {
            Vector4 position = new Vector4(Random.Range(-1, 1), Random.Range(-1, 1), 0.0f, 1.0f);
            _sdfPositions[i] = position;
            _sdfSizes[i] = Random.Range(0.1f, 0.5f);
            _sdfDirections[i] =
                new Vector4(Random.Range(-1, 1), Random.Range(-1, 1) * Random.Range(2f, 5f), 0.0f, 0.0f);
            _sdfStartTimes[i] = Time.time;
        }

        _material.SetVectorArray("_SdfPositions", _sdfPositions);
        _material.SetFloatArray("_SdfSizes", _sdfSizes);
        _material.SetFloatArray("_SdfStartTimes", _sdfStartTimes);
        _material.SetVectorArray("_SdfDirections", _sdfDirections);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                var position = hit.textureCoord * 2.0f - Vector2.one;
                AddCircle(position);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            int index = _numShapes - 1;
            ResizeCircle(index, _sdfSizes[index] + (Time.deltaTime * _growSpeed));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < _numShapes; ++i)
            {
                _sdfStartTimes[i] = Time.time;
            }

            _material.SetFloatArray("_SdfStartTimes", _sdfStartTimes);

            var isMovementEnabled = _material.GetFloat("_EnableMovement");
            _material.SetFloat("_EnableMovement", 1 - isMovementEnabled);
        }
    }

    void AddCircle(Vector3 position)
    {
        if (_numShapes < MaxShapes)
        {
            _sdfPositions[_numShapes] = position;
            _sdfDirections[_numShapes] =
                new Vector4(Random.Range(-1, 1), Random.Range(-1, 1) * Random.Range(2f, 5f), 0.0f, 0.0f);
            _sdfSizes[_numShapes] = 0.01f;
            _sdfStartTimes[_numShapes] = Time.time;
            ++_numShapes;

            _material.SetVectorArray("_SdfPositions", _sdfPositions);
            _material.SetFloatArray("_SdfSizes", _sdfSizes);
            _material.SetFloatArray("_SdfStartTimes", _sdfStartTimes);
            _material.SetVectorArray("_SdfDirections", _sdfDirections);
        }
    }

    void ResizeCircle(int index, float size)
    {
        _sdfSizes[index] = size;
        _sdfStartTimes[index] = Time.time;

        _material.SetFloatArray("_SdfSizes", _sdfSizes);
        _material.SetFloatArray("_SdfStartTimes", _sdfStartTimes);
    }
}