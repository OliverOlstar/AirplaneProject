using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform _CameraTansform;
    private Transform _ParentTransform;
    private Vector3 _LocalRotation;
    private Vector3 _TargetLocalPosition;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask cameraCollisionLayers;
    [SerializeField] private float cameraCollisionDampening = 20;
    [SerializeField] [Range(0, 1)] private float cameraCollisionMinDisPercent = 0.1f;

    [Space]
    public float MouseSensitivity = 4f;
    public float TurnDampening = 10f;
    [SerializeField] private float OffSetLeft = 0f;
    [SerializeField] private float CameraDistance = 6f;
    [SerializeField] private float CameraMinHeight = -20f;
    [SerializeField] private float CameraMaxHeight = 90f;

    public bool CameraDisabled = false;
    
    void Start()
    {
        //Getting Transforms
        _CameraTansform = transform;
        _ParentTransform = transform.parent;

        //Maintaining Starting Rotation
        _LocalRotation.x = _ParentTransform.eulerAngles.y;
        _LocalRotation.y = _ParentTransform.eulerAngles.x;

        //Locking cursor
        //Cursor.lockState = CursorLockMode.Locked;

        //Setting camera distance
        _TargetLocalPosition = new Vector3(-OffSetLeft, 0f, CameraDistance * -1f);
        _CameraTansform.localPosition = _TargetLocalPosition;
    }

    void Update()
    {
        //Getting Mouse Movement
        if (!CameraDisabled)
        {
            DefaultCameraMovement();
            CameraCollision();
        }

        //Actual Camera Transformations
        Quaternion TargetQ = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        _ParentTransform.rotation = Quaternion.Lerp(_ParentTransform.rotation, TargetQ, Time.deltaTime * TurnDampening);

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(lockMouseDelay());
        }
    }

    private IEnumerator lockMouseDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void DefaultCameraMovement()
    {
        //Rotation of the camera based on mouse movement
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

            //Clamping the y rotation to horizon and not flipping over at the top
            if (_LocalRotation.y < CameraMinHeight)
            {
                _LocalRotation.y = CameraMinHeight;
            }
            else if (_LocalRotation.y > CameraMaxHeight)
            {
                _LocalRotation.y = CameraMaxHeight;
            }
        }
    }

    void CameraCollision()
    {
        //Camera Collision
        RaycastHit hit;
        Physics.Raycast(_ParentTransform.position, (_CameraTansform.position - _ParentTransform.position).normalized, out hit, CameraDistance, cameraCollisionLayers);

        if (hit.point != Vector3.zero)
        {
            hit.point -= _ParentTransform.position;
            //Debug.Log(hit.point.magnitude / _TargetLocalPosition.magnitude * 2 * 100 + "%");
            //Debug.DrawLine(_ParentTransform.position, hit.point + _ParentTransform.position, Color.red, 0.1f);
            _CameraTansform.localPosition = Vector3.Lerp(_CameraTansform.localPosition, _TargetLocalPosition * Mathf.Clamp((hit.point.magnitude / _TargetLocalPosition.magnitude), cameraCollisionMinDisPercent, 0.5f), Time.deltaTime * cameraCollisionDampening);
        }
        else
        {
            _CameraTansform.localPosition = Vector3.Lerp(_CameraTansform.localPosition, _TargetLocalPosition * 0.5f, Time.deltaTime * cameraCollisionDampening);
        }
    }



    // Camera Transition //////////////

    public void ChangePlayerCamera(float pOffSetLeft, float pMouseSensitivity, float pTurnDampening, float pCameraDistance, float pCameraMinHeight, float pCameraMaxHeight, float pTransitionSpeed)
    {
        StopCoroutine("OtherCameraVarsTransition");
        StartCoroutine(OtherCameraVarsTransition(pOffSetLeft, pMouseSensitivity, pTurnDampening, pCameraDistance, pCameraMinHeight, pCameraMaxHeight, pTransitionSpeed));
    }

    public IEnumerator OtherCameraVarsTransition(float pOffSetLeft, float pMouseSensitivity, float pTurnDampening, float pCameraDistance, float pCameraMinHeight, float pCameraMaxHeight, float pTransitionSpeed)
    {
        while (MouseSensitivity != pMouseSensitivity || TurnDampening != pTurnDampening || CameraDistance != pCameraDistance
                || CameraMinHeight != pCameraMinHeight || CameraMaxHeight != pCameraMaxHeight || OffSetLeft != pOffSetLeft)
        {
            //Lerping all of the values
            MouseSensitivity = Mathf.Lerp(MouseSensitivity, pMouseSensitivity, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(MouseSensitivity - pMouseSensitivity) <= 0.01f)
                MouseSensitivity = pMouseSensitivity;

            TurnDampening = Mathf.Lerp(TurnDampening, pTurnDampening, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(TurnDampening - pTurnDampening) <= 0.01f)
                TurnDampening = pTurnDampening;

            CameraDistance = Mathf.Lerp(CameraDistance, pCameraDistance, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(CameraDistance - pCameraDistance) <= 0.01f)
                CameraDistance = pCameraDistance;

            CameraMinHeight = Mathf.Lerp(CameraMinHeight, pCameraMinHeight, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(CameraMinHeight - pCameraMinHeight) <= 0.01f)
                CameraMinHeight = pCameraMinHeight;

            CameraMaxHeight = Mathf.Lerp(CameraMaxHeight, pCameraMaxHeight, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(CameraMaxHeight - pCameraMaxHeight) <= 0.01f)
                CameraMaxHeight = pCameraMaxHeight;

            OffSetLeft = Mathf.Lerp(OffSetLeft, pOffSetLeft, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(OffSetLeft - pOffSetLeft) <= 0.01f)
                OffSetLeft = pOffSetLeft;

            //Setting camera distance
            _TargetLocalPosition = new Vector3(-OffSetLeft, 0f, CameraDistance * -1f);

            yield return null;
        }
    }
}
