#if ENABLE_WINMD_SUPPORT
using Windows.Storage;
using Windows.Storage.Pickers;
using System;
#endif
using UnityEngine;

public class GPXFileSaver : MonoBehaviour
{
    private GPXMovementTracker tracker;

    void Start()
    {
        FindTracker();
    }

    private void FindTracker()
    {
        tracker = FindAnyObjectByType<GPXMovementTracker>(); // Find the tracker dynamically

        if (tracker == null)
        {
            Debug.LogError("GPXFileSaver: No GPXMovementTracker found in the scene.");
        }
    }

    public void SaveGPXFileUWP()
    {
#if ENABLE_WINMD_SUPPORT
        Debug.Log("UWP File Save initiated...");

        // Generate GPX data on the Unity main thread
        if (tracker == null)
        {
            tracker = FindAnyObjectByType<GPXMovementTracker>();
            if (tracker == null)
            {
                Debug.LogError("GPXFileSaver: No GPXMovementTracker found in the scene.");
                return;
            }
        }
        string characterGpx = tracker.GenerateCharacterGPXData();
        string realLifeGpx = tracker.GenerateRealLifeGPXData();

        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
            try
            {
                // Create a file picker
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("GPX File", new[] { ".gpx" });
                string timestamp = System.DateTime.Now.ToString("ddMMyy_HHmmss"); // Format: DDMMYY_HHMMSS
                savePicker.SuggestedFileName = $"fitmaze{timestamp}";

                Debug.Log("Displaying file picker...");
                // Show the save picker
                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    Debug.LogError($"Base file selected: {file.Path}");

                    string folderPath = System.IO.Path.GetDirectoryName(file.Path);
                    string baseName = System.IO.Path.GetFileNameWithoutExtension(file.Name);

                    StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);

                    StorageFile characterFile = await folder.CreateFileAsync(
                        baseName + "_character.gpx",
                        CreationCollisionOption.ReplaceExisting);

                    StorageFile realLifeFile = await folder.CreateFileAsync(
                        baseName + "_reallife.gpx",
                        CreationCollisionOption.ReplaceExisting);

                    await FileIO.WriteTextAsync(characterFile, characterGpx);
                    await FileIO.WriteTextAsync(realLifeFile, realLifeGpx);

                    Debug.LogError("Both GPX files saved successfully.");
                }
                else
                {
                    Debug.LogError("Save operation was canceled by the user.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}\n{ex.StackTrace}");
            }

            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }, false);
        }, false);
#else
        Debug.LogError("This file save method only works on UWP.");
#endif
    }
}
