using LightBuzz.Vitruvius;

public static class GlobalSensorController
{
    static SensorType startWithSensor = SensorType.Kinect2;
    public static SensorType StartWithSensor
    {
        get
        {
            return startWithSensor;
        }
        set
        {
            startWithSensor = value;
            setFromLoader = true;
        }
    }

    static bool setFromLoader = false;
    public static bool WasSetFromLoader
    {
        get
        {
            return setFromLoader;
        }
    }
}