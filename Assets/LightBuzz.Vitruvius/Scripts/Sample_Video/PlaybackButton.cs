using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;

public class PlaybackButton : MonoBehaviour
{
    public Sample_Video sampleBehaviour;
    public PlaybackSeekSlider seekSlider;
    public Text clipTimeText;

    public Image buttonImage;
    public Sprite playSprite;
    public Sprite pauseSprite;

    void OnEnable()
    {
        VideoPlayer.onPaused += OnPaused;
        VideoPlayer.onUnpaused += OnUnpaused;
    }

    void OnDisable()
    {
        VideoPlayer.onPaused -= OnPaused;
        VideoPlayer.onUnpaused -= OnUnpaused;
    }

    public void TogglePlayback()
    {
        if (VideoPlayer.IsPlaying)
        {
            VideoPlayer.IsPaused = !VideoPlayer.IsPaused;
        }
        else if (!VideoRecorder.IsRecording && !VideoRecorder.IsBuffering)
        {
            VideoPlayer.Start(sampleBehaviour.playbackSettings);

            System.TimeSpan clipTime = VideoPlayer.ClipTime;
            clipTimeText.text = string.Format("{0:D2}:{1:D2}", clipTime.Minutes, clipTime.Seconds);

            if (VideoPlayer.IsPlaying)
            {
                buttonImage.sprite = VideoPlayer.IsPaused ? playSprite : pauseSprite;
            }
        }

        seekSlider.Interactable = VideoPlayer.IsPlaying;
    }

    public void StopPlayback()
    {
        if (VideoPlayer.IsPlaying)
        {
            VideoPlayer.Stop();

            seekSlider.Interactable = false;
            buttonImage.sprite = playSprite;
            clipTimeText.text = "00:00";
        }
    }

    void OnPaused()
    {
        buttonImage.sprite = playSprite;
    }

    void OnUnpaused()
    {
        buttonImage.sprite = pauseSprite;
    }
}