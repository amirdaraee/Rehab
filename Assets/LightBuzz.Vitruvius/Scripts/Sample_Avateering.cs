using UnityEngine;
using LightBuzz.Vitruvius;

public class Sample_Avateering : MonoBehaviour
{
    SensorAdapter adapter = null;

    public SensorType sensorType = SensorType.Kinect2;

    Texture2D imageViewTexture = null;
    public Material imageViewMaterial = null;
    public Transform imageViewTransform = null;
    public ScreenViewStickman imageViewStickman = null;

    Texture2D depthViewTexture = null;
    public Material depthViewMaterial = null;
    public Transform depthViewTransform = null;
    public ScreenViewStickman depthViewStickman = null;
    DepthFilter depthFilter = null;

    public bool flipView = false;

    public bool updateModels = true;
    public Model model1 = null;
    public Model model2 = null;
    public Model model3 = null;

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

        depthFilter = new DepthFilter();

        model1.Initialize();
        model2.Initialize();
        model3.Initialize();
    }

    void OnDisable()
    {
        if (adapter != null)
        {
            adapter.Close();
            adapter = null;
        }

        depthFilter.Dispose();

        Destroy(imageViewTexture);
        imageViewMaterial.mainTexture = null;

        Destroy(depthViewTexture);
        depthViewMaterial.mainTexture = null;

        model1.Dispose();
        model2.Dispose();
        model3.Dispose();
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
                imageViewTexture = ValidateTexture(imageViewTexture, frame.ImageWidth, frame.ImageHeight, imageViewMaterial, imageViewTransform);

                if (imageViewTexture != null)
                {
                    imageViewTexture.LoadRawTextureData(frame.ImageData);
                    imageViewTexture.Apply(false);
                }
            }

            depthFilter.UpdateFilter(frame);
            if (depthFilter.Result != null)
            {
                depthViewTexture = ValidateTexture(depthViewTexture, frame.DepthWidth, frame.DepthHeight, depthViewMaterial, depthViewTransform);

                if (depthViewTexture != null)
                {
                    depthViewTexture.LoadRawTextureData(depthFilter.Result);
                    depthViewTexture.Apply(false);
                }
            }

            Body body = frame.GetClosestBody();

            if (body != null)
            {
                imageViewStickman.UpdateStickman(adapter, frame, body, imageViewTransform, Visualization.Image);
                depthViewStickman.UpdateStickman(adapter, frame, body, depthViewTransform, Visualization.Depth);

                if (updateModels)
                {
                    model1.DoAvateering(body);
                    model2.DoAvateering(body);
                    model3.DoAvateering(body);
                }
            }
        }
    }

    Texture2D ValidateTexture(Texture2D texture, int width, int height, Material material, Transform transform)
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

        material.mainTexture = texture;
        transform.localScale = new Vector3(7 * (flipView ? -1 : 1), height / (float)width * -7, 1);

        return texture;
    }
}