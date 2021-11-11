using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private bool viewDebug;
    [SerializeField] private bool placeBehindPivot;
    [SerializeField] private bool obstacleRaycast;

    //obiekt wokol ktorego obraca sie kamera
    [SerializeField] private Transform pivot;
    [SerializeField] private Camera camera_;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] [Range(0, 10)] private float maxCameraDistance;
    //the speed at which camera moves back to max distance after encountering obstacle
    [SerializeField] [Range(0, 10)]private float cameraMoveBackSpeed;
    private float _currentCameraDistance;
    [SerializeField] [Range(0, 89)] private float startAngle = 45;
    //keep track of vertical angle for clamping
    private float _verticalAngle;
    // private float _horizontalAngle = 0;
    [SerializeField, Range(-45, 45)] private float lookAngle;

    [SerializeField] [Range(0, 90)] private float maxVerticalAngle = 80;
    [SerializeField] [Range(-90, 45)] private float minVerticalAngle = 10;
    
    [HideInInspector] public Vector3 pivotToCameraDir;
    [HideInInspector] public Vector3 rightAxis;
    
    private Vector3 CameraHalfExtends() {
        Vector3 halfExtends;
        halfExtends.y = camera_.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera_.fieldOfView);
        halfExtends.x = halfExtends.y * camera_.aspect;
        halfExtends.z = 0;
        return halfExtends;
    }

    private void FixedUpdate()
    {
        if (obstacleRaycast)
        {
            BoxcastForObstacles();
            UpdatePosition();
        }
    }

    private RaycastHit _cameraObstacleHit;

    private void BoxcastForObstacles()
    {
        Vector3 currentPivotCameraVector = pivot.position - cameraTransform.position;
        
        if (Physics.BoxCast(
            pivot.position, 
            CameraHalfExtends(), 
            pivotToCameraDir,
            out _cameraObstacleHit,
            Quaternion.LookRotation(-pivotToCameraDir, Vector3.Cross(pivotToCameraDir, rightAxis)),
            maxCameraDistance - camera_.nearClipPlane))
        {
            _currentCameraDistance = _cameraObstacleHit.distance - camera_.nearClipPlane;
        }
        else
        {
            // _currentCameraDistance = maxCameraDistance;
            _currentCameraDistance = Mathf.MoveTowards(_currentCameraDistance, maxCameraDistance,
                cameraMoveBackSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnValidate()
    {
        _currentCameraDistance = maxCameraDistance;
        if (pivot == null || cameraTransform == null) return;
        SetStartingPosition();
    }

    private void Awake()
    {
        _cameraObstacleHit = new RaycastHit();
        SetStartingPosition();
    }

    public void Rotate(float verticalAngleChange, float horizontalAngleChange)
    {
        RotateVertical(verticalAngleChange);
        RotateHorizontal(horizontalAngleChange);
        cameraTransform.LookAt(pivot.position);
        cameraTransform.Rotate(lookAngle, 0, 0);
    }

    private void RotateVertical(float verticalAngleChange)
    {
        float newAngle = _verticalAngle + verticalAngleChange;
        
        if (newAngle > maxVerticalAngle)
        {
            verticalAngleChange -= newAngle - maxVerticalAngle;
        } 
        else if (newAngle < minVerticalAngle)
        {
            verticalAngleChange += minVerticalAngle - newAngle;
        }
        
        _verticalAngle += verticalAngleChange;
        
        //not normalized;
        rightAxis = Vector3.Cross(pivotToCameraDir, pivot.up);
        
        if (viewDebug)
        {
            Debug.DrawLine(pivot.position, pivot.position + rightAxis.normalized * 2);
        }
        
        pivotToCameraDir =
            Quaternion.AngleAxis(verticalAngleChange, rightAxis) *
            pivotToCameraDir;
        
        cameraTransform.position = pivot.position + pivotToCameraDir * _currentCameraDistance;
    }

    private void RotateHorizontal(float horizontalAngleChange)
    {
        pivotToCameraDir = Quaternion.AngleAxis(horizontalAngleChange, pivot.up) * pivotToCameraDir;
        cameraTransform.position = pivot.position + pivotToCameraDir * _currentCameraDistance;
        
    }

    private void SetStartingPosition()
    {
        if (placeBehindPivot)
        {
            cameraTransform.position = pivot.position - pivot.forward * maxCameraDistance;
        }
        
        pivotToCameraDir = (cameraTransform.position - pivot.position).normalized;
        rightAxis = Vector3.Cross(pivotToCameraDir, pivot.up);

        _verticalAngle = 0;
        
        RotateVertical(startAngle);
    }

    private void OnDrawGizmos()
    {
        if (viewDebug)
        {
            if (pivot == null || cameraTransform == null) return;

            Vector3 pivotPosition = pivot.position;
            Vector3 cameraPosition = cameraTransform.position;
            
            Gizmos.DrawLine(pivotPosition, cameraPosition);
            Gizmos.DrawLine(pivotPosition, pivotPosition + pivot.forward * 2);
            Gizmos.DrawWireSphere(pivotPosition, maxCameraDistance);
            // Gizmos.DrawLine(pivotPosition, pivotPosition + _rotationAxis.normalized * 2);
        }
    }

    private void UpdatePosition()
    {
        cameraTransform.position = pivot.position + pivotToCameraDir * _currentCameraDistance;
    }
}