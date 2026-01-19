using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject followTarget;
    [SerializeField] private float offSet;
    [SerializeField] private float height;
    [SerializeField] private float angle;
    
    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        transform.Rotate(Vector3.right, angle);
    }
    
    void Update()
    {
        transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y + height, followTarget.transform.position.z + offSet);
    }
}
