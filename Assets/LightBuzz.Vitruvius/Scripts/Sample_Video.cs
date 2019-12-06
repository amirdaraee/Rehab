using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;

public class Sample_Video : MonoBehaviour
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

    public Model model = null;
    public ScreenViewStickface screenViewStickface = null;
    public PointCloudStickface pointCloudStickface = null;

    public RecordingSettings recordingSettings;
    public PlaybackSettings playbackSettings;

    public Text bufferQueueText;
    public Image bufferVisuals;
    int bufferFrameCount = 0;

    public PlaybackSeekSlider seekSlider;

    bool isAndroid = false;

    void Awake()
    {
        isAndroid = !Application.isEditor && Application.platform == RuntimePlatform.Android;
    }

    void Start()
    {
        recordingSettings.destinationFolder = Path.Combine(Application.persistentDataPath, "Video");
        playbackSettings.sourceFolder = recordingSettings.destinationFolder;

        if (!Directory.Exists(recordingSettings.destinationFolder))
        {
            Directory.CreateDirectory(recordingSettings.destinationFolder);
        }
    }

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

        model.Initialize();
    }

    void OnDisable()
    {
        if (VideoPlayer.IsPlaying)
        {
            VideoPlayer.Stop();
        }

        VideoPlayer.Dispose();

        if (VideoRecorder.IsRecording)
        {
            VideoRecorder.Stop();
        }

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

        model.Dispose();
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

        Frame frame = VideoPlayer.IsPlaying ? VideoPlayer.Load() : adapter.UpdateFrame();

        if (VideoPlayer.IsPlaying && !VideoPlayer.IsSeeking)
        {
            seekSlider.slider.value = (VideoPlayer.Seek + 1) / (float)VideoPlayer.FrameCount;
        }

        if (frame != null)
        {
            if (frame.ImageData != null)
            {
                imageViewTexture = ValidateTexture(imageViewTexture, frame.ImageWidth, frame.ImageHeight, imageViewMaterial, imageViewTransform);

                if (imageViewTexture != null)
                {
                    if (isAndroid && VideoPlayer.IsPlaying)
                    {
                        imageViewTexture.LoadImage(frame.ImageData);
                    }
                    else
                    {
                        imageViewTexture.LoadRawTextureData(frame.ImageData);
                        imageViewTexture.Apply(false);
                    }
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
                screenViewStickface.UpdateStickface(adapter, frame, body.Face, imageViewTransform, Visualization.Image);
                pointCloudStickface.UpdateStickface(body.Face);

                model.DoAvateering(body);
            }

            if (!VideoPlayer.IsPlaying)
            {
                VideoRecorder.Record(frame, recordingSettings);
            }
        }

        if (VideoRecorder.IsRecording)
        {
            bufferFrameCount = VideoRecorder.FrameCount;
        }

        if (bufferFrameCount > 0)
        {
            bufferVisuals.fillAmount = VideoRecorder.QueueLength / (float)bufferFrameCount;
            bufferQueueText.text = VideoRecorder.QueueLength.ToString();
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