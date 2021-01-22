using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    #region Making Singleton
    public static RobotController Instance { get; private set; }

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
    #endregion
    #region Properties
    [SerializeField]
    private float robotDiameter = 0.3f; // in meters(R in hw)
    [SerializeField]
    private float wheelDiameter = 0.1f; //meters
    [SerializeField]
    private float rightWheelAngularVelocity = 0f;
    public float RightWheelAngularVelocity { 
        get { return rightWheelAngularVelocity; } 
        set { rightWheelAngularVelocity = value; } 
    }
    [SerializeField]
    private float leftWheelAngularVelocity = 0f;
    public float LeftWheelAngularVelocity
    {
        get { return leftWheelAngularVelocity; }
        set { leftWheelAngularVelocity = value; }
    }
    [SerializeField]
    private GameObject iccPointerPrefab = null;
    private GameObject iccPointer = null;
    [SerializeField]
    private GameObject trajectoryPointerPrefab = null;
    private List<GameObject> trajectoryPointers = new List<GameObject>();

    private Vector2 icc = Vector2.zero;
    #endregion

    #region Functions

    // Calculate robot's angular velocity
    float AngularVelocity()
    {
        return (RightWheelVelocity() - LeftWheelVelocity()) / robotDiameter;
    }

    // Convert angular velocity to velocity
    float LeftWheelVelocity()
    {
        return leftWheelAngularVelocity * wheelDiameter / 2;
    }

    // Convert angular velocity to velocity
    float RightWheelVelocity()
    {
        return rightWheelAngularVelocity * wheelDiameter / 2;
    }

    Vector2 ICC()
    {
        Vector2 newIcc = new Vector2(transform.position.x - IccDistance() * Mathf.Sin(-1 * transform.eulerAngles.y / 57.2957795f), transform.position.z + IccDistance() * Mathf.Cos(-1 * transform.eulerAngles.y / 57.2957795f)); // Calculate ICC point

        if (icc != newIcc)
        {
            icc = newIcc;
            if (iccPointer != null)
                Destroy(iccPointer);
            iccPointer = Instantiate(iccPointerPrefab);
            iccPointer.transform.position = new Vector3(icc.x, 0, icc.y); // Draw a point to display ICC coordinates.
        }

        return newIcc;
    }

    float IccDistance()
    {
        return (robotDiameter / 2) * ((RightWheelVelocity() + LeftWheelVelocity()) / (RightWheelVelocity() - LeftWheelVelocity())); // Calculate robot's distance to ICC
    }

    public void SetNextPose()
    {
        // If robot has same velocity on both wheels use below equation as other equation causes problem due to infinite R (Distance to ICC)
        if (LeftWheelVelocity() == RightWheelVelocity())
        {
            // Set coordinates of the robot
            transform.position += new Vector3(LeftWheelVelocity() * Time.deltaTime * Mathf.Cos(-1 * transform.eulerAngles.y / 57.2957795f), 0, LeftWheelVelocity() * Time.deltaTime * Mathf.Sin(-1 * transform.eulerAngles.y / 57.2957795f));
            // Draw trajectory of the robot
            trajectoryPointers.Add(Instantiate(trajectoryPointerPrefab, transform.position, Quaternion.identity));
        }
        else
        {
            // Matrix 1 in equation row by row.
            Vector3 matrix1Row1 = new Vector3(Mathf.Cos(AngularVelocity() * Time.deltaTime), -1 * Mathf.Sin(AngularVelocity() * Time.deltaTime), 0);
            Vector3 matrix1Row2 = new Vector3(Mathf.Sin(AngularVelocity() * Time.deltaTime), Mathf.Cos(AngularVelocity() * Time.deltaTime), 0);
            Vector3 matrix1Row3 = new Vector3(0, 0, 1);

            // Matrix 2 in equation.
            Vector3 matrix2 = new Vector3(transform.position.x - ICC().x, transform.position.z - ICC().y, -1 * transform.eulerAngles.y / 57.2957795f);

            // Matrix 3 in equation.
            Vector3 matrix3 = new Vector3(ICC().x, ICC().y, AngularVelocity() * Time.deltaTime);

            // Multiply Matrix 1 and Matrix 2
            float temp1Row1 = matrix1Row1.x * matrix2.x + matrix1Row1.y * matrix2.y + matrix1Row1.z * matrix2.z;
            float temp1Row2 = matrix1Row2.x * matrix2.x + matrix1Row2.y * matrix2.y + matrix1Row2.z * matrix2.z;
            float temp1Row3 = matrix1Row3.x * matrix2.x + matrix1Row3.y * matrix2.y + matrix1Row3.z * matrix2.z;
            Vector3 temp1 = new Vector3(temp1Row1, temp1Row2, temp1Row3);

            // Add Matrix 3 with multiplication result of Matrix 1 and Matrix 2
            Vector3 result = matrix3 + temp1;

            // Set rotation of the robot (convert radian to degree by multipling with 57.2957795f)
            transform.eulerAngles = new Vector3(0, -1 * result.z * 57.2957795f, 0);
            // Set coordinates of the robot
            transform.position = new Vector3(result.x, 0, result.y);
            // Draw trajectory of the robot
            trajectoryPointers.Add(Instantiate(trajectoryPointerPrefab, transform.position, Quaternion.identity));
        }
        //Debug.Log((transform.position.x) + " : " + (transform.position.z) + " : " + (transform.eulerAngles.y / 57.2957795f));

    }

    public void Reset()
    {
        // Set robot position
        transform.position = new Vector3(0, 0, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);

        // Reset velocity
        rightWheelAngularVelocity = 0f;
        leftWheelAngularVelocity = 0f;

        // Set icc to origin
        if (iccPointer != null)
            Destroy(iccPointer);

        // Clear pointers
        foreach (GameObject trajectoryPointer in trajectoryPointers)
            Destroy(trajectoryPointer);
        trajectoryPointers.Clear();
    }

    public void SetPose(float x, float y, float theta)
    {
        transform.position = new Vector3(x, 0, y);
        transform.eulerAngles = new Vector3(0, -1 * theta, 0);
    }

    public void SetWheelAngularVelocities(float wL, float wR)
    {
        this.leftWheelAngularVelocity = wL;
        this.rightWheelAngularVelocity = wR;
    }

    public void SetParameters()
    {
        if (SimulationController.Instance.isSimulating)
        {
            UserInterfaceController.Instance.DisplayMessage("Stop simulation to submit values.");
            return;
        }
        // Get pose inputs & convert to float
        float x = float.Parse(UserInterfaceController.Instance.poseX.GetComponent<InputField>().text != "" ? UserInterfaceController.Instance.poseX.GetComponent<InputField>().text : "0");
        float y = float.Parse(UserInterfaceController.Instance.poseY.GetComponent<InputField>().text != "" ? UserInterfaceController.Instance.poseY.GetComponent<InputField>().text : "0");
        float theta = float.Parse(UserInterfaceController.Instance.poseTheta.GetComponent<InputField>().text != "" ? UserInterfaceController.Instance.poseTheta.GetComponent<InputField>().text : "0");
        // Set new pose
        SetPose(x, y, theta);
        Debug.Log(x + " " + y + " " + theta);

        // Get wheel angular velocities and convert to float
        float wL = float.Parse(UserInterfaceController.Instance.leftAngularVelocity.GetComponent<InputField>().text != "" ? UserInterfaceController.Instance.leftAngularVelocity.GetComponent<InputField>().text : "0");
        float wR = float.Parse(UserInterfaceController.Instance.rightAngularVelocity.GetComponent<InputField>().text != "" ? UserInterfaceController.Instance.rightAngularVelocity.GetComponent<InputField>().text : "0");
        // Set new wheel angular velocities
        SetWheelAngularVelocities(wL, wR);
    }
    #endregion

}
