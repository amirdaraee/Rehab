using UnityEngine.UI;
using LightBuzz.Vitruvius;

public class PlaybackSpeedSlider : SliderController
{
    public Text contextText;

    protected override void OnInternalValueChanged()
    {
        base.OnInternalValueChanged();

        VideoPlayer.Speed = value;
    }

    protected override void UpdateTexts()
    {
        contextText.text = value.ToString("0.0");
    }
}