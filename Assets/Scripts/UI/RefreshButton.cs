using UnityEngine;
using UnityEngine.UI;

public class RefreshButton : MonoBehaviour
{
    public HexGridGenerator terrainGenerator;

    void Start()
    {
        Button refreshButton = GetComponent<Button>();
        refreshButton.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        terrainGenerator.Randomize = true;
        terrainGenerator.UpdateMesh = true;
    }
}
