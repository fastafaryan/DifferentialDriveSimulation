using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPointer : MonoBehaviour
{
    private float spawnTime;
    [SerializeField]
    private float lifeTime = 2f;

    private void Start()
    {
        spawnTime = SimulationController.Instance.Timer;
    }
    // Update is called once per frame
    private void Update()
    {
        if (SimulationController.Instance.Timer  > spawnTime + lifeTime)
            Destroy(gameObject);
    }
}
