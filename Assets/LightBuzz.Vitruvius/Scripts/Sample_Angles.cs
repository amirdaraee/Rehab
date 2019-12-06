using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using LightBuzz;
using LightBuzz.Vitruvius;

public class Sample_Angles : MonoBehaviour
{
    SensorAdapter adapter = null;

    public SensorType sensorType = SensorType.Kinect2;

    Texture2D imageViewTexture = null;
    public Material imageViewMaterial = null;
    public Transform imageViewTransform = null;
    public ScreenViewStickman screenViewStickman = null;

    public bool flipView = false;

    [Space(5)]

    public Transform leftElbowParent;
    public AngleArc leftElbowArc;
    public TextMesh leftElbowAngleText;

    [Space(5)]

    public Transform rightElbowParent;
    public AngleArc rightElbowArc;
    public TextMesh rightElbowAngleText;

    [Space(5)]

    public Transform leftKneeParent;
    public AngleArc leftKneeArc;
    public TextMesh leftKneeAngleText;

    [Space(5)]

    public Transform rightKneeParent;
    public AngleArc rightKneeArc;
    public TextMesh rightKneeAngleText;

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
        imageViewMaterial.mainTexture = null;
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

            Body body = frame.GetClosestBody();

            if (body != null)
            {
                screenViewStickman.UpdateStickman(adapter, frame, body, imageViewTransform, Visualization.Image);

                UpdateArc(
                    screenViewStickman.jointPoints[5].position,
                    screenViewStickman.jointPoints[6].position,
                    screenViewStickman.jointPoints[7].position,
                    body.Joints[JointType.ShoulderLeft].WorldPosition,
                    body.Joints[JointType.ElbowLeft].WorldPosition,
                    body.Joints[JointType.WristLeft].WorldPosition,
                   leftElbowParent, leftElbowArc, leftElbowAngleText);

                UpdateArc(
                    screenViewStickman.jointPoints[9].position,
                    screenViewStickman.jointPoints[10].position,
                    screenViewStickman.jointPoints[11].position,
                    body.Joints[JointType.ShoulderRight].WorldPosition,
                    body.Joints[JointType.ElbowRight].WorldPosition,
                    body.Joints[JointType.WristRight].WorldPosition,
                   rightElbowParent, rightElbowArc, rightElbowAngleText);

                UpdateArc(
                    screenViewStickman.jointPoints[13].position,
                    screenViewStickman.jointPoints[14].position,
                    screenViewStickman.jointPoints[15].position,
                    body.Joints[JointType.HipLeft].WorldPosition,
                    body.Joints[JointType.KneeLeft].WorldPosition,
                    body.Joints[JointType.AnkleLeft].WorldPosition,
                   leftKneeParent, leftKneeArc, leftKneeAngleText);

                UpdateArc(
                    screenViewStickman.jointPoints[17].position,
                    screenViewStickman.jointPoints[18].position,
                    screenViewStickman.jointPoints[19].position,
                    body.Joints[JointType.HipRight].WorldPosition,
                    body.Joints[JointType.KneeRight].WorldPosition,
                    body.Joints[JointType.AnkleRight].WorldPosition,
                   rightKneeParent, rightKneeArc, rightKneeAngleText);
            }
        }
    }

    void UpdateArc(
        Vector2 start2D, Vector2 center2D, Vector2 end2D,
        Vector3D start3D, Vector3D center3D, Vector3D end3D,
        Transform arcParent, AngleArc arc, TextMesh arcText)
    {
        Vector2 direction1 = (start2D - center2D).normalized;
        Vector2 direction2 = (end2D - center2D).normalized;

        float angle = Vector2.Angle(direction1, direction2);
        arc.Angle = angle;
        arc.transform.up = Quaternion.Euler(0, 0, angle) *
            (Vector2.Dot(Quaternion.Euler(0, 0, 90) * direction1, direction2) > 0 ? direction1 : direction2);

        angle = Calculations.Angle(start3D, center3D, end3D);
        arcText.text = angle.ToString("N0") + '°';

        arcParent.position = center2D;
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