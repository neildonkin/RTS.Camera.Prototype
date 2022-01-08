using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*
     * This code is based on this excellent tutorial:
     *
     * https://youtu.be/rnqF6S7PfFA
     *
     * It does not include the camera follow feature,
     * the zoom level is clamped and the tutorial code
     * has been adapted to improve performance.
     */
    private float _movementSpeed;

    public Transform cameraTransform;

    public float normalSpeed;
    public float fastSpeed;
    public float movementTime;
    public float rotationAmount;
    public float zoomAmount;
    public float minZoom;
    public float maxZoom;

    private Vector3 _zoomAmount;
    private Vector3 _newPosition;
    private Quaternion _newRotation;
    private Vector3 _newZoom;

    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Vector3 _rotateStartPosition;
    private Vector3 _rotateCurrentPosition;

    private Camera _mainCam;

    // Start is called before the first frame update
    private void Start()
    {
        // Accessing Camera.main is expensive, so only do it once
        _mainCam = Camera.main;

        _zoomAmount = new Vector3(0, -zoomAmount, zoomAmount);
        var camTransform = transform;
        _newPosition = camTransform.position;
        _newRotation = camTransform.rotation;

        _newZoom = cameraTransform.localPosition;
    }

    private void LateUpdate()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    private void HandleMouseInput()
    {
        const int leftMouseButton = 0;
        const int middleMouseButton = 2;

        // Scroll
        if (Input.mouseScrollDelta.y != 0)
        {
            _newZoom += Input.mouseScrollDelta.y * _zoomAmount;
        }

        var wasLeftMouseButtonPressedThiFrame = Input.GetMouseButtonDown(leftMouseButton);
        var isLeftMouseButtonDown = Input.GetMouseButton(leftMouseButton);

        if (wasLeftMouseButtonPressedThiFrame || isLeftMouseButtonDown)
        {
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            var isRayCast = plane.Raycast(ray, out var entry);

            if (isRayCast)
            {
                if (wasLeftMouseButtonPressedThiFrame)
                {
                    // Mouse drag has begun
                    _dragStartPosition = ray.GetPoint(entry);
                }

                if (isLeftMouseButtonDown)
                {
                    // Mouse drag has ended
                    _dragCurrentPosition = ray.GetPoint(entry);
                    _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
                }
            }
        }

        // Rotation
        if (Input.GetMouseButtonDown(middleMouseButton))
        {
            _rotateStartPosition = Input.mousePosition;
        }

        if (!Input.GetMouseButton(middleMouseButton)) return;
        _rotateCurrentPosition = Input.mousePosition;

        var difference = _rotateStartPosition - _rotateCurrentPosition;
        _rotateStartPosition = _rotateCurrentPosition;
        _newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
    }

    private void HandleMovementInput()
    {
        _movementSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        // Camera panning
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _newPosition += transform.forward * _movementSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _newPosition += transform.forward * -_movementSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _newPosition += transform.right * _movementSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _newPosition += transform.right * -_movementSpeed;
        }

        // Camera rotation
        if (Input.GetKey(KeyCode.Q))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        // Camera zoom
        if (Input.GetKey(KeyCode.R))
        {
            _newZoom -= _zoomAmount;
        }

        if (Input.GetKey(KeyCode.F))
        {
            _newZoom += _zoomAmount;
        }

        // Ensure we don't zoom too far in or out
        _newZoom.y = Mathf.Clamp(_newZoom.y, minZoom, maxZoom);
        _newZoom.z = Mathf.Clamp(_newZoom.z, minZoom, maxZoom);

        Transform tempTrans;
        (tempTrans = transform).position =
            Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(tempTrans.rotation, _newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition =
            Vector3.Lerp(cameraTransform.localPosition, _newZoom, Time.deltaTime * movementTime);
    }
}