using UnityEngine;
using UnityEngine.UI;

public class RecordingSettingsPanel : MonoBehaviour
{
    public Sample_Video sample;

    public GameObject settings;
    public RectTransform recordingPanel;

    public void ToggleImage(Toggle toggle)
    {
        sample.recordingSettings.imageData = toggle.isOn;
    }

    public void ToggleDepth(Toggle toggle)
    {
        sample.recordingSettings.depthData = toggle.isOn;
    }

    public void ToggleUserSpace(Toggle toggle)
    {
        sample.recordingSettings.userSpaceData = toggle.isOn;
    }

    public void ToggleBody(Toggle toggle)
    {
        sample.recordingSettings.bodyData = toggle.isOn;
    }

    public void ToggleSettingsPanel(Toggle toggle)
    {
        settings.SetActive(toggle.isOn);
        recordingPanel.sizeDelta = new Vector2(recordingPanel.sizeDelta.x, toggle.isOn ? 174 : 67);
    }
}