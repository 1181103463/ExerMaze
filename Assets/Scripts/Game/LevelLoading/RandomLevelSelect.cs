using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandomLevelSelect : MonoBehaviour
{
    [SerializeField] private Button level12x12Button;
    [SerializeField] private Button level24x24Button;
    [SerializeField] private Button level36x36Button;
    [SerializeField] private Button customRandomLevelButton;
    [SerializeField] private Button loadMazeButton;

    public string levelName;

    void Awake()
    {
        if (level12x12Button != null)
            level12x12Button.onClick.AddListener(() => SelectLevel(12, 12));
        else
            Debug.LogWarning("Level 10x10 button not assigned");

        if (level24x24Button != null)
            level24x24Button.onClick.AddListener(() => SelectLevel(24, 24));
        else
            Debug.LogWarning("Level 20x20 button not assigned");

        if (level36x36Button != null)
            level36x36Button.onClick.AddListener(() => SelectLevel(36, 36));
        if (customRandomLevelButton != null)
        {
            customRandomLevelButton.onClick.AddListener(() => ToCustomRandomLevelSelectScene());
        }
        else
            Debug.LogWarning("Level 30x30 button not assigned");
        
        if (loadMazeButton != null)
            loadMazeButton.onClick.AddListener(() => LoadMaze());
        else
            Debug.LogWarning("LoadMazeButton button not assigned");
    }

    private void SelectLevel(int width, int depth)
    {
        GameManager.Instance.SetMazeSize(width, depth);
        SceneManager.LoadScene("RandomLevel");
    }

    private void ToCustomRandomLevelSelectScene()
    {
        SceneManager.LoadSceneAsync("CustomRandomLevelSelect");
    }

    public void LoadMaze()
    {
        string path = Application.persistentDataPath + "/maze_save.json";

        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found!");
            return;
        }

        string json = File.ReadAllText(path);
        SaveMazeData data = JsonUtility.FromJson<SaveMazeData>(json);

        MazeSaveHolder.LoadedData = data;
        MazeSaveHolder.HasLoadedData = true;

        SceneManager.LoadScene("RandomLevel");
    }
}