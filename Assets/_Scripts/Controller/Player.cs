using UnityEngine;

public class Player : MonoBehaviour
{
    private InputMap _input;
    private OrbitCamera _camera;
    [SerializeField] private Transform cameraTransform;
    private Transform _playerTransform;
    private GroundedController _controller;
    private Vector2 _mouseDelta;
    private Vector2 _movementAxes;
    private bool _jump;

    [SerializeField] private float maxSpeed;
    [SerializeField] private bool invertMouseX;
    [SerializeField] private bool invertMouseY;
    [SerializeField] private float lookSensitivityX;
    [SerializeField] private float lookSensitivityY;
    private float sensitivityModifierX;
    private float sensitivityModifierY;

    private void Awake()
    {
        _input = new InputMap();
        _input.Enable();
        _camera = GetComponent<OrbitCamera>();
        _playerTransform = transform;
        _controller = GetComponent<GroundedController>();
        //TODO: move somewhere else
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        ReadMovementInput();
        
        _camera.Rotate(_mouseDelta.y * (invertMouseY ? -1 : 1) * lookSensitivityY,
            _mouseDelta.x * (invertMouseX ? -1 : 1) * lookSensitivityX);
        
        Vector3 cameraToPlayerDir = -1 * _camera.pivotToCameraDir;
        cameraToPlayerDir.y = 0;
        cameraToPlayerDir.Normalize();

        Vector3 rightCameraAxis = _camera.rightAxis.normalized;
        // rightCameraAxis.y = 0;
        
        Vector3 desiredDirection = cameraToPlayerDir * _movementAxes.y + rightCameraAxis * _movementAxes.x;
        
        // Debug.DrawLine(_playerTransform.position, _playerTransform.position + desiredDirection * 3, Color.red);
        Debug.DrawLine(_playerTransform.position, _playerTransform.position + cameraToPlayerDir * 2, Color.blue);
        
        _controller.SetDesiredVelocity(desiredDirection * maxSpeed);
        if (_jump)
        {
            _controller.QueueJump();
        }
        
    }

    private void ReadMovementInput()
    {
        _mouseDelta = _input.Player.Look.ReadValue<Vector2>();
        _movementAxes = _input.Player.MovementAxes.ReadValue<Vector2>();
        _jump = _input.Player.Jump.triggered;
    }

    private void CalculateSensitivity()
    {
        // float aspectRatio = (float)Screen.currentResolution.width / Screen.currentResolution.height;
    }
}
