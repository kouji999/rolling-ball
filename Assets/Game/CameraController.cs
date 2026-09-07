using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -8f);
    [SerializeField] private float smoothTime = 0.3f;

    private Vector3 _velocity;

    private void LateUpdate()
    {
        if (target == null) return;
        var desired = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        transform.LookAt(target);
    }
}
