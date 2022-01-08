using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float _movementSpeed;
    
    public Transform cameraTransform;
    
    public float normalSpeed;
    public float fastSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Vector3 _rotateStartPosition;
    private Vector3 _rotateCurrentPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        var camTransform = transform;
        newPosition = camTransform.position;
        newRotation = camTransform.rotation;

        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    void HandleMouseInput()
    {
        const int leftMouseButton = 0;
        const int middleMouseButton = 2;
        var mainCam = Camera.main;
        
        // Scroll
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }
        
        if (mainCam != null)
        {
            // Drag
            if (Input.GetMouseButtonDown(leftMouseButton))
            {
                // Mouse drag has begun
                var plane = new Plane(Vector3.up, Vector3.zero);

                var ray = mainCam.ScreenPointToRay(Input.mousePosition);

                if (plane.Raycast(ray, out var entry))
                {
                    _dragStartPosition = ray.GetPoint(entry);
                }
            }

            if (Input.GetMouseButton(leftMouseButton))
            {
                // Mouse drag has ended
                var plane = new Plane(Vector3.up, Vector3.zero);

                var ray = mainCam.ScreenPointToRay(Input.mousePosition);

                if (plane.Raycast(ray, out var entry))
                {
                    _dragCurrentPosition = ray.GetPoint(entry);
                    newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
                }
            }

            // Rotation
            if (Input.GetMouseButtonDown(middleMouseButton))
            {
                _rotateStartPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(middleMouseButton))
            {
                _rotateCurrentPosition = Input.mousePosition;

                var difference = _rotateStartPosition - _rotateCurrentPosition;
                _rotateStartPosition = _rotateCurrentPosition;
                newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
            }
        }
    }
    
    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _movementSpeed = fastSpeed;
        }
        else
        {
            _movementSpeed = normalSpeed;
        }

        // Camera panning
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * _movementSpeed);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -_movementSpeed);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * _movementSpeed);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -_movementSpeed);
        }

        // Camera rotation
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        
        // Camera zoom
        if (Input.GetKey(KeyCode.R))
        {
            newZoom -= zoomAmount;
        }

        if (Input.GetKey((KeyCode.F)))
        {
            newZoom += zoomAmount;
        }

        Transform tempTrans;
        (tempTrans = transform).position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(tempTrans.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}