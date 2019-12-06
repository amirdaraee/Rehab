using UnityEngine;
using UnityEngine.UI;

public class RecordingPathField : MonoBehaviour
{
    public Sample_Video sample;

    public InputField folderInputField = null;
    public InputField fileInputField = null;

    void Awake()
    {
        folderInputField.text = sample.recordingSettings.destinationFolder;
        fileInputField.text = sample.recordingSettings.fileName;
    }

    public void SetFolderPath()
    {
        sample.recordingSettings.destinationFolder = folderInputField.text;
    }

    public void SetFilePath()
    {
        sample.recordingSettings.fileName = fileInputField.text;
    }
}