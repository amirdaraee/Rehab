using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;

public class Sample_GreenScreen : MonoBehaviour
{
    SensorAdapter adapter = null;

    public SensorType sensorType = SensorType.Kinect2;

    Texture2D greenScreenViewTexture = null;
    public RawImage greenScreenViewRawImage = null;
    public Transform greenScreenViewTransform = null;
    GreenScreenFilter greenScreenFilter = null;

    public ScreenViewStickman screemViewStickman = null;

    void OnEnable()
    {
        if (GlobalSensorController.WasSetFromLoader)
        {
            sensorType = GlobalSensorController.StartWithSensor;
        }

        adapter = new SensorAdapter(sensorType)
        {
            OnChangedAvailabilityEventHandler = (sender, args) =>
            {
                Debug.Log(args.SensorType + " is connected: " + args.IsConnected);
            }
        };

        greenScreenFilter = new GreenScreenFilter(adapter);
    }

    void OnDisable()
    {
        if (adapter != null)
        {
            adapter.Close();
            adapter = null;
        }

        greenScreenFilter.Dispose();

        Destroy(greenScreenViewTexture);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

            return;
        }

        if (adapter == null) return;

        if (adapter.SensorType != sensorType)
        {
            adapter.SensorType = sensorType;
        }

        Frame frame = adapter.UpdateFrame();

        if (frame != null)
        {
            if (frame.ImageData != null && frame.UserSpaceData != null)
            {
                greenScreenFilter.UpdateFilter(frame);
                if (greenScreenFilter.Result != null)
                {
                    greenScreenViewTexture = ValidateTexture(greenScreenViewTexture, frame.DepthWidth, frame.DepthHeight, greenScreenViewRawImage);

                    if (greenScreenViewTexture != null)
                    {
                        greenScreenViewTexture.LoadRawTextureData(greenScreenFilter.Result);
                        greenScreenViewTexture.Apply(false);
                    }
                }
            }

            Body body = frame.GetClosestBody();

            if (body != null)
            {
                screemViewStickman.UpdateStickman(adapter, frame, body, greenScreenViewTransform, Visualization.Depth);
            }
        }
    }

    Texture2D ValidateTexture(Texture2D texture, int width, int height, RawImage rawImage)
    {
        if (width == 0 || height == 0) return texture;

        if (texture == null)
        {
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        }
        else if (texture.width != width || texture.height != height)
        {
            texture.Resize(width, height, TextureFormat.RGBA32, false);
        }

        rawImage.texture = texture;

        return texture;
    }
}