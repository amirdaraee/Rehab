using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;

public class LoopButton : MonoBehaviour
{
    bool initialized = false;

    public ColorBlock enabledColorBlock;
    ColorBlock disabledColorBlock;

    public void ToggleLoop(Toggle toggle)
    {
        if (!initialized)
        {
            initialized = true;

            disabledColorBlock = toggle.colors;
        }

        VideoPlayer.Loop = toggle.isOn;
        toggle.colors = toggle.isOn ? enabledColorBlock : disabledColorBlock;
    }
}