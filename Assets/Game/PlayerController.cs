using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float force = 12f;
    [SerializeField] private float maxSpeed = 8f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _rb.AddForce(GameLogic.MoveDirection(input) * force, ForceMode.Force);
        _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, maxSpeed);
    }
}
