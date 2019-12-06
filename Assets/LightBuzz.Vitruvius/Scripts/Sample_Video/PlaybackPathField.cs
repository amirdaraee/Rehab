using UnityEngine;
using UnityEngine.UI;

public class PlaybackPathField : MonoBehaviour
{
    public Sample_Video sample;

    public InputField folderInputField = null;
    public InputField fileInputField = null;

    void Awake()
    {
        folderInputField.text = sample.playbackSettings.sourceFolder;
        fileInputField.text = sample.playbackSettings.fileName;
    }

    public void SetFolderPath()
    {
        sample.playbackSettings.sourceFolder = folderInputField.text;
    }

    public void SetFilePath()
    {
        sample.playbackSettings.fileName = fileInputField.text;
    }
}