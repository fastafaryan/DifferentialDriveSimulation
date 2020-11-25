using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour
{
    #region Making Singleton
    public static UserInterfaceController Instance { get; private set; }

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
    public GameObject poseX = null;
    [SerializeField]
    public GameObject poseY = null;
    [SerializeField]
    public GameObject poseTheta = null;
    [SerializeField]
    public GameObject leftAngularVelocity = null;
    [SerializeField]
    public GameObject rightAngularVelocity = null;
    [SerializeField]
    public GameObject seconds = null;
    [SerializeField]
    public GameObject message = null;
    private float messageDisplayStartTime = 0;
    #endregion

    private void Start()
    {
        message.SetActive(false);
    }

    private void Update()
    {
        if (SimulationController.Instance.Timer > messageDisplayStartTime + 2)
        {
            message.SetActive(false);
        }

        if (SimulationController.Instance.isSimulating)
        {
            UpdateUI();
        }
    }

    public void DisplayMessage(string message)
    {
        Debug.Log("Display Message..");
        messageDisplayStartTime = SimulationController.Instance.Timer;
        this.message.GetComponent<TextMeshProUGUI>().text = message;
        this.message.SetActive(true);
    }

    public void UpdateUI()
    {
        poseX.GetComponent<InputField>().text = SimulationController.Instance.Robot.transform.position.x.ToString("F2");
        poseY.GetComponent<InputField>().text = SimulationController.Instance.Robot.transform.position.z.ToString("F2");
        poseTheta.GetComponent<InputField>().text = (((SimulationController.Instance.Robot.transform.eulerAngles.y * -1) + 360) % 360).ToString("F2");
        leftAngularVelocity.GetComponent<InputField>().text = SimulationController.Instance.Robot.GetComponent<RobotController>().LeftWheelAngularVelocity.ToString("F2");
        rightAngularVelocity.GetComponent<InputField>().text = SimulationController.Instance.Robot.GetComponent<RobotController>().RightWheelAngularVelocity.ToString("F2");
        seconds.GetComponent<InputField>().text = SimulationController.Instance.Timer.ToString("F2");
    }

    public void ToggleSimulationButton()
    {
        GameObject.Find("Button.ToggleSimulation/Text").GetComponent<TextMeshProUGUI>().text = SimulationController.Instance.isSimulating ? "Stop Simulation" : "Start Simulation";
       
    }
}
