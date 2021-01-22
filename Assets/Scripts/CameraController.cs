using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    #region Properties

    #region Movement Properties
    [SerializeField]
    private float keyboardMovementSpeed = 20f;
    [SerializeField]
    private float mouseMovementSpeed = 20f;
    [SerializeField]
    private float mouseMovementBorderThickness = 10f;
    #endregion

    #region Zooming & Height Properties
    [SerializeField]
    private float mouseZoomSensivity = 250f;
    [SerializeField]
    private float minHeight = 1f;
    [SerializeField]
    private float maxHeight = 10f;
    [SerializeField]
    private float defaultHeight = 2f;
    private float currentHeight;
    #endregion

    #endregion

    #region Unity Methods
    void Start()
    {
        currentHeight = defaultHeight;
    }

    void Update()
    {
        KeyboardMovement();
        MouseMovement();
        MouseZoom();
    }
    #endregion

    #region Custom Methods

    #region Keyboard Movement 
    void KeyboardMovement()
    {
        // Get keyboard input values
        Vector3 keyboardMovementInput = new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0, UnityEngine.Input.GetAxis("Vertical"));
        // Move object if input value is different then zero
        if (keyboardMovementInput != Vector3.zero)
        {
            keyboardMovementInput *= keyboardMovementSpeed; // Multiply with movement speed
            keyboardMovementInput *= transform.position.y / 10; // Multiply with 10th of current height
            keyboardMovementInput *= UnityEngine.Time.deltaTime; // Multiply with deltaTime
            keyboardMovementInput = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * keyboardMovementInput; // Multiply movement input with Y axis rotation.
            keyboardMovementInput = transform.InverseTransformDirection(keyboardMovementInput); // Convert world space to local space 
            transform.Translate(keyboardMovementInput, Space.Self); // Apply movement
        }
    }
    #endregion

    #region Mouse Movement 
    void MouseMovement()
    {
        Vector3 movementVector = new Vector3(0, 0, 0);

        // Check mouse positions 
        if (UnityEngine.Input.mousePosition.x >= Screen.width - mouseMovementBorderThickness)
            movementVector.x = 1;
        if (UnityEngine.Input.mousePosition.x <= mouseMovementBorderThickness)
            movementVector.x = -1;
        if (UnityEngine.Input.mousePosition.y >= Screen.height - mouseMovementBorderThickness)
            movementVector.z = 1;
        if (UnityEngine.Input.mousePosition.y <= mouseMovementBorderThickness)
            movementVector.z = -1;

        if (movementVector != Vector3.zero)
        {
            movementVector *= mouseMovementSpeed;
            movementVector *= transform.position.y / 10; // Multiply with 10th of current height
            movementVector *= UnityEngine.Time.deltaTime;
            movementVector = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * movementVector;
            movementVector = transform.InverseTransformDirection(movementVector);

            transform.Translate(movementVector, Space.Self);
        }
    }
    #endregion

    #region Mouse Zoom
    void MouseZoom()
    {
        float mouseScroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0)
        {
            Vector3 zoomDirection = transform.forward * mouseScroll * UnityEngine.Time.deltaTime * mouseZoomSensivity;
            float newHeight = transform.position.y + zoomDirection.y;

            if (newHeight >= minHeight && newHeight <= maxHeight)
            {
                currentHeight = newHeight;
                transform.Translate(zoomDirection, Space.World);
            }
            else if (newHeight < minHeight)
            {
                currentHeight = minHeight;
                transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
            }
            else if (newHeight > maxHeight)
            {
                currentHeight = maxHeight;
                transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
            }
        }
    }
    #endregion

    public void ResetCamera()
    {
        Debug.Log("[INFO] Camera position is reset.");
        CameraController.Instance.transform.position = new Vector3(0, defaultHeight, 0);
    }


    // Set camera position to above of Robot. (Robot is accesed from SimulationController.Robot) 
    public void GoToRobot()
    {
        Debug.Log("[INFO] Camera is alligned to robot.");
       CameraController.Instance.transform.position = new Vector3(SimulationController.Instance.Robot.transform.position.x, defaultHeight, SimulationController.Instance.Robot.transform.position.z);
    }
    #endregion
}