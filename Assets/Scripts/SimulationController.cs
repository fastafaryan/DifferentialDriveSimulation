using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject robot = null;
    public GameObject Robot { get { return robot; } }

    private float timer = 0.0f;
    public float Timer { get { return timer; } }

    public bool isSimulating = false;
    #endregion


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
        // Check is simulation is finished or paused.

        // Increase time by delta time
        timer += Time.deltaTime;
        // Calculates and sets robot's pose.
        robot.GetComponent<RobotController>().SetNextPose();
    }

    public void Submit()
    {
        /*
        // Convert inputs to floats, set to 0 if null  
        float poseXFloat = (poseX.GetComponent<Text>().text != "") ? float.Parse(poseX.GetComponent<Text>().text) : 0;
        float poseYFloat = (poseX.GetComponent<Text>().text != "") ? float.Parse(poseY.GetComponent<Text>().text) : 0;
        float poseThetaFloat = (poseX.GetComponent<Text>().text != "") ? float.Parse(poseTheta.GetComponent<Text>().text) : 0;
        float leftAngularVelocityFloat = (leftAngularVelocity.GetComponent<Text>().text != "") ? float.Parse(leftAngularVelocity.GetComponent<Text>().text) : 0;
        float rightAngularVelocityFloat = (rightAngularVelocity.GetComponent<Text>().text != "") ? float.Parse(rightAngularVelocity.GetComponent<Text>().text) : 0;

        // Set robot position
        robot.transform.position = new Vector3(poseXFloat, 0, poseYFloat);
        // Set robot rotation
        robot.transform.eulerAngles = new Vector3(0, poseThetaFloat, 0);

        // Set robot angular velocities
        robot.GetComponent<RobotController>().rightWheelAngularVelocity = rightAngularVelocityFloat;
        robot.GetComponent<RobotController>().leftWheelAngularVelocity = leftAngularVelocityFloat;

        // Disable default simulation
        //robot.GetComponent<Robot>().isDefaultSimulation = false;
        */
    }

    public void ResetSimulation()
    {
        // Reset timer
        timer = 0;
        // Reset robot
        robot.GetComponent<RobotController>().Reset();
        UserInterfaceController.Instance.UpdateUI();
    }

    public void ToggleSimulation()
    {
        isSimulating = !isSimulating;
        UserInterfaceController.Instance.ToggleSimulationButton();
    }
}
