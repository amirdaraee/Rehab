using System;
using UnityEngine;
using UnityEngine.UI;
using LightBuzz;
using LightBuzz.Vitruvius;

public class Sample_BodyMeasurements : MonoBehaviour
{
    SensorAdapter adapter = null;

    public SensorType sensorType = SensorType.Kinect2;

    Texture2D greenScreenViewTexture = null;
    public RawImage greenScreenViewRawImage = null;
    public Transform greenScreenViewTransform = null;
    GreenScreenFilter greenScreenFilter = null;

    public ScreenViewStickman screenViewStickman = null;

    public Text[] boneLengths;
    public Text[] boneWidthTexts;
    public LineRenderer[] boneLines;
    Vector3D uiPlaneScale;

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
                screenViewStickman.UpdateStickman(adapter, frame, body, greenScreenViewTransform, Visualization.Depth);

                uiPlaneScale = greenScreenViewTransform.localScale;
                uiPlaneScale.Set(
                    uiPlaneScale.X * ((RectTransform)greenScreenViewTransform).root.localScale.x * ((RectTransform)greenScreenViewTransform).rect.size.x,
                    uiPlaneScale.Y * ((RectTransform)greenScreenViewTransform).root.localScale.y * ((RectTransform)greenScreenViewTransform).rect.size.y, 1);

                float spineLengthWorld;
                float spineLengthScreen;
                MeasureBoneLength(body, JointType.SpineShoulder, JointType.SpineBase, boneLengths[0], 2, 4, out spineLengthWorld, out spineLengthScreen);

                float armLeftLengthWorld;
                float armLeftLengthScreen;
                MeasureBoneLength(body, JointType.ShoulderLeft, JointType.ElbowLeft, boneLengths[1], 5, 6, out armLeftLengthWorld, out armLeftLengthScreen);

                float forearmLeftLengthWorld;
                float forearmLeftLengthScreen;
                MeasureBoneLength(body, JointType.ElbowLeft, JointType.WristLeft, boneLengths[2], 6, 7, out forearmLeftLengthWorld, out forearmLeftLengthScreen);

                float armRightLengthWorld;
                float armRightLengthScreen;
                MeasureBoneLength(body, JointType.ShoulderRight, JointType.ElbowRight, boneLengths[3], 8, 9, out armRightLengthWorld, out armRightLengthScreen);

                float forearmRightLengthWorld;
                float forearmRightLengthScreen;
                MeasureBoneLength(body, JointType.ElbowRight, JointType.WristRight, boneLengths[4], 9, 10, out forearmRightLengthWorld, out forearmRightLengthScreen);

                float thighLeftLengthWorld;
                float thighLeftLengthScreen;
                MeasureBoneLength(body, JointType.HipLeft, JointType.KneeLeft, boneLengths[5], 11, 12, out thighLeftLengthWorld, out thighLeftLengthScreen);

                float calfLeftLengthWorld;
                float calfLeftLengthScreen;
                MeasureBoneLength(body, JointType.KneeLeft, JointType.AnkleLeft, boneLengths[6], 12, 13, out calfLeftLengthWorld, out calfLeftLengthScreen);

                float thighRightLengthWorld;
                float thighRightLengthScreen;
                MeasureBoneLength(body, JointType.HipRight, JointType.KneeRight, boneLengths[7], 14, 15, out thighRightLengthWorld, out thighRightLengthScreen);

                float calfRightLengthWorld;
                float calfRightLengthScreen;
                MeasureBoneLength(body, JointType.KneeRight, JointType.AnkleRight, boneLengths[8], 15, 16, out calfRightLengthWorld, out calfRightLengthScreen);

                MeasureBoneWidth(frame, body, JointType.SpineShoulder, JointType.SpineBase, boneWidthTexts[0], boneLines[0], spineLengthWorld, spineLengthScreen);
                MeasureBoneWidth(frame, body, JointType.ShoulderLeft, JointType.ElbowLeft, boneWidthTexts[1], boneLines[1], armLeftLengthWorld, armLeftLengthScreen);
                MeasureBoneWidth(frame, body, JointType.ElbowLeft, JointType.WristLeft, boneWidthTexts[2], boneLines[2], forearmLeftLengthWorld, forearmLeftLengthScreen);
                MeasureBoneWidth(frame, body, JointType.ShoulderRight, JointType.ElbowRight, boneWidthTexts[3], boneLines[3], armRightLengthWorld, armRightLengthScreen);
                MeasureBoneWidth(frame, body, JointType.ElbowRight, JointType.WristRight, boneWidthTexts[4], boneLines[4], forearmRightLengthWorld, forearmRightLengthScreen);
                MeasureBoneWidth(frame, body, JointType.HipLeft, JointType.KneeLeft, boneWidthTexts[5], boneLines[5], thighLeftLengthWorld, thighLeftLengthScreen);
                MeasureBoneWidth(frame, body, JointType.KneeLeft, JointType.AnkleLeft, boneWidthTexts[6], boneLines[6], calfLeftLengthWorld, calfLeftLengthScreen);
                MeasureBoneWidth(frame, body, JointType.HipRight, JointType.KneeRight, boneWidthTexts[7], boneLines[7], thighRightLengthWorld, thighRightLengthScreen);
                MeasureBoneWidth(frame, body, JointType.KneeRight, JointType.AnkleRight, boneWidthTexts[8], boneLines[8], calfRightLengthWorld, calfRightLengthScreen);
            }
        }
    }

    void MeasureBoneLength(Body body, JointType joint1, JointType joint2, Text text, int stickmanJointIndex1, int stickmanJointIndex2, out float worldLength, out float screenLength)
    {
        worldLength = (body.Joints[joint1].WorldPosition - body.Joints[joint2].WorldPosition).Length.ToCentimeters();
        screenLength = (adapter.WorldToDepthSpace(body.Joints[joint1].WorldPosition) - adapter.WorldToDepthSpace(body.Joints[joint2].WorldPosition)).Length;

        text.text = worldLength.ToString("N0") + "cm";
    }

    void MeasureBoneWidth(Frame frame, Body body, JointType joint1, JointType joint2, Text text, LineRenderer line, float boneLengthWorld, float boneLengthScreen)
    {
        int width = adapter.DepthWidth;
        int height = adapter.DepthHeight;
        int searchLength = 100;

        Vector2D p1_2D = adapter.WorldToDepthSpace(body.Joints[joint1].WorldPosition);
        Vector2D p2_2D = adapter.WorldToDepthSpace(body.Joints[joint2].WorldPosition);
        Vector2D center_2D = Vector2D.Lerp(p1_2D, p2_2D, 0.5f);
        center_2D.Set(Convert.ToInt32(center_2D.X), Convert.ToInt32(center_2D.Y));

        Vector2D from = FindEndPixel(frame, center_2D, (LightBuzz.Quaternion.Euler(0, 0, 90) * (p2_2D - p1_2D)).Normalized, searchLength, true);
        Vector2D to = FindEndPixel(frame, center_2D, (LightBuzz.Quaternion.Euler(0, 0, -90) * (p2_2D - p1_2D)).Normalized, searchLength, false);

        float boneWidthScreen = (from - to).Length;
        float worldToScreenRatio = boneLengthWorld / boneLengthScreen;
        float boneWidthWorld = boneWidthScreen * worldToScreenRatio;
        text.text = boneWidthWorld.ToString("N0") + "cm";
        
        from = from.GetPositionOnPlane(width, height, greenScreenViewTransform.position, greenScreenViewTransform.rotation, uiPlaneScale);
        to = to.GetPositionOnPlane(width, height, greenScreenViewTransform.position, greenScreenViewTransform.rotation, uiPlaneScale);
        line.SetPosition(0, from);
        line.SetPosition(1, to);
    }

    Vector2D FindEndPixel(Frame frame, Vector2D center, Vector2D direction, int searchLength, bool leftSide)
    {
        // Exit offset is required for the end point when the end pixel is found.
        // It is required at the right side, when the index shouldn't get a minus 1 offset
        // as a zero based indexing.
        float exitOffset = leftSide ? 1 : 0;

        for (int i = 0, x = 0, y = 0, width = frame.DepthWidth, height = frame.DepthHeight; i < searchLength; i++)
        {
            x = Convert.ToInt32(center.X + direction.X * i);
            y = Convert.ToInt32(center.Y + direction.Y * i);

            if (x < 0 || x >= width || y < 0 || y >= height || frame.UserSpaceData[x + y * width] == 0xff || i + 1 == searchLength)
            {
                return new Vector2D(
                    Convert.ToInt32(center.X + direction.X * (i - exitOffset)),
                    Convert.ToInt32(center.Y + direction.Y * (i - exitOffset)));
            }
        }

        return center;
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