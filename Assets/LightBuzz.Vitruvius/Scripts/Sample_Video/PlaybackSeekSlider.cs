using UnityEngine.UI;
using LightBuzz.Vitruvius;

public class PlaybackSeekSlider : SliderController
{
    public Image fillImage;
    public Text elapsedTimeText;

    public override bool Interactable
    {
        get
        {
            return base.Interactable;
        }
        set
        {
            if (base.Interactable != value)
            {
                base.Interactable = value;

                this.value = 0;
                fillImage.enabled = value;
                elapsedTimeText.text = "00:00";
            }
        }
    }

    protected override void OnSeekingStateChanged()
    {
        base.OnSeekingStateChanged();

        VideoPlayer.IsSeeking = IsSeeking;
    }

    protected override void OnInternalValueChanged()
    {
        base.OnInternalValueChanged();
        
        VideoPlayer.Seek = (int)((VideoPlayer.FrameCount - 1) * value);
    }

    protected override void UpdateTexts()
    {
        base.UpdateTexts();

        System.TimeSpan elapsedTime = VideoPlayer.ElapsedTime;
        elapsedTimeText.text = string.Format("{0:D2}:{1:D2}", elapsedTime.Minutes, elapsedTime.Seconds);
    }
}