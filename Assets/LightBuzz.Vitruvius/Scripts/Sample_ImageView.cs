using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;

public class Sample_ImageView : MonoBehaviour
{
    SensorAdapter adapter = null;

    public SensorType sensorType = SensorType.Kinect2;

    Texture2D imageViewTexture = null;
    public RawImage imageViewRawImage = null;
    public Transform imageViewTransform = null;

    public ScreenViewStickman screenViewStickman = null;

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
    }

    void OnDisable()
    {
        if (adapter != null)
        {
            adapter.Close();
            adapter = null;
        }

        Destroy(imageViewTexture);
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
            if (frame.ImageData != null)
            {
                imageViewTexture = ValidateTexture(imageViewTexture, frame.ImageWidth, frame.ImageHeight, imageViewRawImage);

                if (imageViewTexture != null)
                {
                    imageViewTexture.LoadRawTextureData(frame.ImageData);
                    imageViewTexture.Apply(false);
                }
            }

            Body body = frame.GetClosestBody();

            if (body != null)
            {
                screenViewStickman.UpdateStickman(adapter, frame, body, imageViewTransform, Visualization.Image);
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