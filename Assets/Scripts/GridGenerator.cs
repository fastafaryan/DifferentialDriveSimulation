using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GridGenerator : MonoBehaviour
{
    public static GridGenerator Instance { get; private set; }

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
    [SerializeField]
    private int gridSize = 10; // Grid size in meters.
    [SerializeField]
    private Vector2 areaSize = new Vector2(100, 100); // Ground size to place grid lines along.
    [SerializeField]
    private GameObject linePrefab = null; // Lines to spawn for creating grids.
    private List<GameObject> gridLines = new List<GameObject>(); // Pointer list to spawned grid lines.
    #endregion

    #region Functions
    private void Start()
    {
        DrawGrids();
    }
    public void DrawGrids()
    {
        GameObject currentLine;
        LineRenderer lineRenderer;
        int numberOfGridsX = Mathf.RoundToInt(areaSize.x / gridSize);
        int numberOfGridsZ = Mathf.RoundToInt(areaSize.y / gridSize);

        // Draw on x axis
        for (int x = -(numberOfGridsX / 2); x <= (numberOfGridsX / 2); x++)
        {
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, new Vector3(x * gridSize + (gridSize / 2), 0.01f, -(areaSize.x / 2)));
            lineRenderer.SetPosition(1, new Vector3(x * gridSize + (gridSize / 2), 0.01f, (areaSize.x / 2)));
            gridLines.Add(currentLine);
        }

        // Draw on z axis
        for (int z = -(numberOfGridsZ / 2); z <= (numberOfGridsZ / 2); z++)
        {
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, new Vector3(-(areaSize.x / 2), 0.01f, z * gridSize + (gridSize / 2)));
            lineRenderer.SetPosition(1, new Vector3((areaSize.x / 2), 0.01f, z * gridSize + (gridSize / 2)));
            gridLines.Add(currentLine);
        }
    }
    public void ClearGrids()
    {
        foreach (GameObject line in gridLines)
        {
            Destroy(line);
        }
        gridLines.Clear();
    }
    #endregion
}
