using UnityEngine;
using LightBuzz;
using LightBuzz.Vitruvius;

public class Sample_VirtualMirror : MonoBehaviour
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

    public Model model1 = null;
    public Model model2 = null;
    bool hadBody = false;

    float model1Size = 0;
    float model2Size = 0;
    float bodyImageSize = 0;
    float bodyDepthSize = 0;

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
        model1.AvatarRoot.SetActive(false);
        model1Size = Vector3.Distance(model1.Bones[(int)JointType.SpineBase].Transform.position, model1.Bones[(int)JointType.Head].Transform.position);

        model2.Initialize();
        model2.AvatarRoot.SetActive(false);
        model2Size = Vector3.Distance(model2.Bones[(int)JointType.SpineBase].Transform.position, model2.Bones[(int)JointType.Head].Transform.position);
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

                if (!hadBody)
                {
                    hadBody = true;

                    model1.AvatarRoot.SetActive(true);
                    model2.AvatarRoot.SetActive(true);
                }

                model1.DoAvateering(body);

                model1.Bones[(int)JointType.SpineBase].Transform.position =
                    adapter.WorldToImageSpace(body.Joints[JointType.SpineBase].WorldPosition).
                    GetPositionOnPlane(frame.ImageWidth, frame.ImageHeight,
                    imageViewTransform.position, imageViewTransform.rotation, imageViewTransform.localScale);

                model2.DoAvateering(body);
                model2.Bones[(int)JointType.SpineBase].Transform.position =
                    adapter.WorldToDepthSpace(body.Joints[JointType.SpineBase].WorldPosition).
                    GetPositionOnPlane(frame.DepthWidth, frame.DepthHeight,
                    depthViewTransform.position, depthViewTransform.rotation, depthViewTransform.localScale);

                if (Vector3D.Angle(Vector3D.Up, body.Joints[JointType.Neck].WorldPosition - body.Joints[JointType.SpineBase].WorldPosition) < 10)
                {
                    bodyImageSize = (imageViewStickman.jointPoints[0].position - imageViewStickman.jointPoints[4].position).magnitude;
                    bodyDepthSize = (depthViewStickman.jointPoints[0].position - depthViewStickman.jointPoints[4].position).magnitude;

                    float scale = bodyImageSize / model1Size;
                    model1.AvatarRoot.transform.localScale = new Vector3(scale, scale, scale);

                    scale = bodyDepthSize / model2Size;
                    model2.AvatarRoot.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
            else if (hadBody)
            {
                hadBody = false;

                model1.AvatarRoot.SetActive(false);
                model2.AvatarRoot.SetActive(false);
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