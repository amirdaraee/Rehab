using UnityEngine;
using UnityEngine.UI;

public class PlaybackSettingsToggle : MonoBehaviour
{
    public Sample_Video sample;

    public void ToggleImage(Toggle toggle)
    {
        sample.playbackSettings.imageData = toggle.isOn;
    }

    public void ToggleDepth(Toggle toggle)
    {
        sample.playbackSettings.depthData = toggle.isOn;
    }

    public void ToggleUserSpace(Toggle toggle)
    {
        sample.playbackSettings.userSpaceData = toggle.isOn;
    }
}