using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;

public class RecordButton : MonoBehaviour
{
    public Image buttonImage;
    public Sprite recordSprite;
    public Sprite stopSprite;

    public GameObject gaugeBackground;
    public GameObject bufferQueue;

    public Sample_Video sample;

    void OnEnable()
    {
        VideoRecorder.onSavingCompleted += VideoRecorder_onSavingCompleted;
    }

    void OnDisable()
    {
        VideoRecorder.onSavingCompleted -= VideoRecorder_onSavingCompleted;
    }

    void VideoRecorder_onSavingCompleted(int framesSaved)
    {
        gaugeBackground.SetActive(false);
        bufferQueue.SetActive(false);
    }

    public void ToggleRecord()
    {
        if (VideoRecorder.IsRecording)
        {
            VideoRecorder.Stop();

            if (VideoRecorder.FrameCount == 0)
            {
                gaugeBackground.SetActive(false);
                bufferQueue.SetActive(false);
            }
        }
        else if (!VideoPlayer.IsPlaying)
        {
            VideoRecorder.Start();

            if (VideoRecorder.IsRecording)
            {
                gaugeBackground.SetActive(true);
                bufferQueue.SetActive(true);
            }
        }

        buttonImage.sprite = VideoRecorder.IsRecording ? stopSprite : recordSprite;
    }

    public void DeleteFiles()
    {
        if (!VideoRecorder.IsRecording && !VideoPlayer.IsPlaying)
        {
            string folder = sample.recordingSettings.destinationFolder;

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                return;
            }
            
            if (Directory.GetFileSystemEntries(folder).Length != 0)
            {
                Directory.Delete(folder, true);
                Directory.CreateDirectory(folder);
            }
        }
    }
}