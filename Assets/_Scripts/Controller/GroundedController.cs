using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GroundedController : MonoBehaviour
{
    [SerializeField] private bool viewDebug;
    [SerializeField] [Range(1, 90)] private float maxGroundAngle;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;

    [SerializeField] private float maxAcceleration;
    [SerializeField] private float maxAirAcceleration;
    
    private Rigidbody _rb;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _desiredVelocity;
    
    private float _minGroundDotProduct;
    private int _groundContactCount;
    private Vector3 _contactNormal;
    private bool Grounded => _groundContactCount >= 1;
    private bool _jumpQueued;

    void OnValidate () {
        _minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //ten tryb minimalizuje zwolnienie przy lądowaniu
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        OnValidate();
    }

    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();
        if (!Grounded)
        {
            ApplyGravity();
        }


        if (viewDebug)
        {
            if (Grounded)
            {
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.cyan);
            }
            else
            {
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.gray);
            }
            
            Debug.DrawLine(_rb.position, _rb.position + _desiredVelocity, Color.green);
            Debug.DrawLine(_rb.position, _rb.position + _velocity, Color.blue);
            Vector3 v = _velocity;
            v.y = 0;
            // Debug.Log(v.magnitude);
            Debug.DrawLine(_rb.position, _rb.position + v, Color.blue);
            Debug.DrawLine(_rb.position + _velocity, _rb.position + v, Color.blue);
            Debug.DrawLine(_rb.position, _rb.position + _contactNormal * 3, Color.yellow);
        }
        
        if (_jumpQueued)
        {
            _jumpQueued = false;
            Jump();
        }


        _rb.velocity = _velocity;
        ClearState();
    }

    private void ApplyGravity()
    {
        Vector3 newVelocity = _velocity;
        newVelocity.y -= gravity * Time.fixedDeltaTime;
        _velocity = newVelocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        EvaluateCollision(other);
    }

    private void OnCollisionStay(Collision other)
    {
        EvaluateCollision(other);
    }

    private void EvaluateCollision(Collision collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            
            if (normal.y >= _minGroundDotProduct) {
                _groundContactCount += 1;
                _contactNormal += normal;
            }
        }
    }
    
    void UpdateState () {
        _velocity = _rb.velocity;
        if (Grounded) {
            if (_groundContactCount > 1) {
                _contactNormal.Normalize();
            }
        }
        else {
            _contactNormal = Vector3.up;
        }
    }
    
    private void ClearState () {
        _groundContactCount = 0;
        _contactNormal = Vector3.zero;
    }

    public void QueueJump() => _jumpQueued = true;

    private void Jump()
    {
        if (Grounded)
        {
            float jumpSpeed = Mathf.Sqrt(-2f * -gravity * jumpHeight);
            float alignedSpeed = Vector3.Dot(_velocity, _contactNormal);
            if (alignedSpeed > 0f) {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }

            _velocity += _contactNormal * jumpSpeed;
        }
    }
    
    void AdjustVelocity ()
    {
        float acceleration = Grounded ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        Vector3 desiredVelocityProjection = Vector3.ProjectOnPlane(_desiredVelocity, _contactNormal);

        Vector3 currentVelocityProjection =
            Vector3.ProjectOnPlane(new Vector3(_velocity.x, 0, _velocity.z), _contactNormal);

        Vector3 desiredVelocityChange = desiredVelocityProjection - currentVelocityProjection;
        Vector3 velocityChange = 
            desiredVelocityChange.sqrMagnitude < maxSpeedChange * maxSpeedChange ? 
                desiredVelocityChange :
                desiredVelocityChange.normalized * maxSpeedChange;

        _velocity += velocityChange;
    }

    public void SetDesiredVelocity(Vector3 velocity) => _desiredVelocity = velocity;
}
