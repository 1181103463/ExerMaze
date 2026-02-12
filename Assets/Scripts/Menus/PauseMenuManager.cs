using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PauseMenuManager : BaseMenuManager
{
    protected override string GetMenuSceneName()
    {
        return "PauseMenu";
    }

    public void OnResumeButtonPressed()
    {
        if (levelManager != null)
        {
            levelManager.ResumeGame();
        }
        else
        {
            SceneManager.UnloadSceneAsync("PauseMenu"); // Fallback if LevelManager is not found
            Time.timeScale = 1f; // Resume time
            BLEManager.Instance?.bleConnect?.UpdateSensorStateOnBLE("start");
        }
    }

    public void SaveMazeDetails()
    {
        AutoMG3D_1010 maze = FindObjectOfType<AutoMG3D_1010>();

        if (maze == null)
        {
            Debug.LogError("Maze not found!");
            return;
        }

        MazeSaveData data = maze.GetMazeSaveData();

        string json = JsonUtility.ToJson(data, true);

        string path = Application.persistentDataPath + "/maze_save.json";

        File.WriteAllText(path, json);

        Debug.Log("Maze saved at: " + path);
    }
}
