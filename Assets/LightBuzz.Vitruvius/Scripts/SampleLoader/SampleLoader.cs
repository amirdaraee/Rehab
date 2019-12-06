using LightBuzz.Vitruvius;
using UnityEngine;
using UnityEngine.UI;

public class SampleLoader : MonoBehaviour
{
    public Dropdown sensorSelection;

    void OnEnable()
    {
        sensorSelection.value = (int)GlobalSensorController.StartWithSensor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void SelectSensor()
    {
        GlobalSensorController.StartWithSensor = (SensorType)sensorSelection.value;
    }
}