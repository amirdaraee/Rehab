using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    public Toggle toggle;
    public GameObject speedSlider;

    public ColorBlock enabledColorBlock;
    ColorBlock disabledColorBlock;

    void Awake()
    {
        disabledColorBlock = toggle.colors;
    }

    public void OnToggled()
    {
        speedSlider.SetActive(toggle.isOn);
        toggle.colors = toggle.isOn ? enabledColorBlock : disabledColorBlock;
    }

    public void ToggleSpeedSlider()
    {
        speedSlider.SetActive(!speedSlider.activeSelf);
    }
}