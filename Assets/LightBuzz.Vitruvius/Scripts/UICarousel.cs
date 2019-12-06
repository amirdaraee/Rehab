using UnityEngine;
using UnityEngine.UI;

public class UICarousel : MonoBehaviour
{
    public Sample_Carousel sample;
    public Text selectionText;

    [Space(5)]

    public Transform carouselPivot;
    public float carouselSpeed = 2;
    public float modelDistance = 2;

    int modelCount = 0;
    float angleStep = 1;

    void Awake()
    {
        modelCount = sample.models.Length;
        angleStep = 360f / modelCount;

        for (int i = 0; i < modelCount; i++)
        {
            sample.models[i].AvatarRoot.transform.localPosition = Quaternion.Euler(0, angleStep * i, 0) * Vector3.back * modelDistance;
        }

        UpdateSelectionText();
    }
    
    void LateUpdate()
    {
        carouselPivot.rotation = Quaternion.RotateTowards(carouselPivot.rotation, Quaternion.Euler(0, -angleStep * sample.Selected, 0), Time.deltaTime * carouselSpeed);

        for (int i = 0; i < modelCount; i++)
        {
            sample.models[i].AvatarRoot.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    
    public void GoToPrevious()
    {
        sample.Selected--;
        UpdateSelectionText();
    }

    public void GoToNext()
    {
        sample.Selected++;
        UpdateSelectionText();
    }

    void UpdateSelectionText()
    {
        selectionText.text = (sample.Selected + 1) + "/" + sample.models.Length;
    }
}