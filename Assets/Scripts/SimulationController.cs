using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationController : MonoBehaviour
{
    #region Making Singleton
    public static SimulationController Instance { get; private set; }

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
    private GameObject robotToSpawn = null;

    private GameObject robot;
    public GameObject Robot { get { return robot; } }

    private float timer = 0.0f;
    public float Timer { get { return timer; } }

    public bool isSimulating = false;
    #endregion

    #region
    private void Start()
    {
        // Spawn robot.
        Debug.Log("[INFO] Robot spawned!");
        robot = Instantiate<GameObject>(robotToSpawn, Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSimulating)
        {
            Simulate();
        }
    }

    void Simulate()
    {
        // Increase time by delta time
        timer += Time.deltaTime;
        // Calculates and sets robot's pose.
        SimulationController.Instance.Robot.GetComponent<RobotController>().SetNextPose();
    }

    public void Submit()
    {
        // Return if simulation is ongoing.
        if(SimulationController.Instance.isSimulating)
        {
            UserInterfaceController.Instance.DisplayMessage("Simulation must be stopped to submit new values!");
            return;
        }

        // Convert inputs to floats, set to 0 if null  
        float poseXFloat = (UserInterfaceController.Instance.poseX.GetComponent<InputField>().text != "") ? 
            float.Parse(UserInterfaceController.Instance.poseX.GetComponent<InputField>().text) : 0;
        float poseYFloat = (UserInterfaceController.Instance.poseX.GetComponent<InputField>().text != "") ? 
            float.Parse(UserInterfaceController.Instance.poseY.GetComponent<InputField>().text) : 0;
        float poseThetaFloat = (UserInterfaceController.Instance.poseX.GetComponent<InputField>().text != "") ? 
            float.Parse(UserInterfaceController.Instance.poseTheta.GetComponent<InputField>().text) : 0;
        float leftAngularVelocityFloat = (UserInterfaceController.Instance.leftAngularVelocity.GetComponent<InputField>().text != "") ? 
            float.Parse(UserInterfaceController.Instance.leftAngularVelocity.GetComponent<InputField>().text) : 0;
        float rightAngularVelocityFloat = (UserInterfaceController.Instance.rightAngularVelocity.GetComponent<InputField>().text != "") ? 
            float.Parse(UserInterfaceController.Instance.rightAngularVelocity.GetComponent<InputField>().text) : 0;

        // Set robot position
        SimulationController.Instance.Robot.transform.position = new Vector3(poseXFloat, 0, poseYFloat);
        // Set robot rotation
        SimulationController.Instance.Robot.transform.eulerAngles = new Vector3(0, -1*poseThetaFloat, 0);

        // Set robot angular velocities
        SimulationController.Instance.Robot.GetComponent<RobotController>().RightWheelAngularVelocity = rightAngularVelocityFloat;
        SimulationController.Instance.Robot.GetComponent<RobotController>().LeftWheelAngularVelocity = leftAngularVelocityFloat;
    }

    public void ResetSimulation()
    {
        // Reset timer
        SimulationController.Instance.timer = 0;
        // Reset robot
        SimulationController.Instance.Robot.GetComponent<RobotController>().Reset();
        UserInterfaceController.Instance.UpdateUI();
    }

    public void ToggleSimulation()
    {
        Debug.Log("[INFO] Simulation is toggled.");

        isSimulating = !isSimulating;
        if (isSimulating)
            Debug.Log("[INFO] Simulation is started.");
        else 
            Debug.Log("[INFO] Simulation is stopped.");

        UserInterfaceController.Instance.ToggleSimulationButton();
    }
    #endregion
}
