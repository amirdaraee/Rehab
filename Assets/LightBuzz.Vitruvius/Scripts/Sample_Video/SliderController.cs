using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    public GameObject contextPanel;

    protected bool outOfHandle = true;
    protected bool pointerActive = false;
    protected float value = 0;

    bool isSeeking = false;
    public bool IsSeeking
    {
        get
        {
            return isSeeking;
        }
        set
        {
            bool seekingStateChanged = isSeeking != value;

            isSeeking = value;

            if (seekingStateChanged)
            {
                OnSeekingStateChanged();
            }
        }
    }

    public virtual bool Interactable
    {
        get
        {
            return slider.interactable;
        }
        set
        {
            slider.interactable = value;
        }
    }

    void OnEnable()
    {
        value = slider.value;

        UpdateTexts();
    }

    public void OnPointerEnter()
    {
        outOfHandle = false;

        if (contextPanel != null)
        {
            contextPanel.SetActive(true);
        }
    }

    public void OnPointerExit()
    {
        outOfHandle = true;

        if (contextPanel != null && !pointerActive)
        {
            contextPanel.SetActive(false);
        }
    }

    public void OnPointerDown()
    {
        pointerActive = true;
    }

    public void OnPointerUp()
    {
        pointerActive = false;

        if (contextPanel != null && outOfHandle)
        {
            contextPanel.SetActive(false);
        }

        IsSeeking = false;
    }

    public void OnDrag()
    {
        ValidateValue(false);
    }

    public void OnValueChanged()
    {
        ValidateValue(!pointerActive);
    }

    void ValidateValue(bool externalChange)
    {
        if (!externalChange)
        {
            IsSeeking = true;
        }

        float value = slider.value;

        if (value != this.value)
        {
            this.value = value;

            if (!externalChange)
            {
                OnInternalValueChanged();
            }

            UpdateTexts();
        }
    }

    protected virtual void OnSeekingStateChanged()
    {
    }

    protected virtual void OnInternalValueChanged()
    {
    }

    protected virtual void UpdateTexts()
    {
    }
}