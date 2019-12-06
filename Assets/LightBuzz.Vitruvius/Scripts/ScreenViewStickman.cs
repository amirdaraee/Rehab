using UnityEngine;
using LightBuzz;
using LightBuzz.Vitruvius;

public class ScreenViewStickman : MonoBehaviour
{
    public Transform[] jointPoints;
    public LineRenderer[] jointLines;
    public float updateSmoothness = 0.5f;

    public JointType[] JointTypes { get; private set; }

    void Initialize()
    {
        JointTypes = new JointType[jointPoints.Length];

        for (int i = 0; i < JointTypes.Length; i++)
        {
            JointTypes[i] = (JointType)System.Enum.Parse(typeof(JointType), jointPoints[i].name);
        }
    }

    public void UpdateStickman(SensorAdapter adapter, Frame frame, Body body, Transform viewPlane, Visualization visualization)
    {
        bool isPlaybackFrame = frame != null && frame.IsPlaybackFrame;
        if ((!isPlaybackFrame && adapter == null) || frame == null || body == null || viewPlane == null) return;

        if (JointTypes == null)
        {
            Initialize();
        }

        Vector3D viewPlanePosition = viewPlane.position;
        LightBuzz.Quaternion viewPlaneRotation = viewPlane.rotation;
        Vector3D viewPlaneScale = viewPlane.localScale;

        if (viewPlane.GetType() == typeof(RectTransform))
        {
            viewPlaneScale.Set(viewPlaneScale.X * ((RectTransform)viewPlane).root.localScale.x * ((RectTransform)viewPlane).rect.size.x,
                viewPlaneScale.Y * ((RectTransform)viewPlane).root.localScale.y * ((RectTransform)viewPlane).rect.size.y, 1);
        }

        float smoothness = Mathf.Lerp(1f, Time.deltaTime, updateSmoothness);

        Vector3D currPosition;

        for (int i = 0; i < JointTypes.Length; i++)
        {
            if (visualization == Visualization.Image)
            {
                if (isPlaybackFrame)
                {

                    currPosition = VideoPlayer.WorldToImageSpace(JointTypes[i], frame, body).GetPositionOnPlane(
                        frame.ImageWidth, frame.ImageHeight, viewPlanePosition, viewPlaneRotation, viewPlaneScale);
                }
                else
                {
                    currPosition = adapter.WorldToImageSpace(body.Joints[JointTypes[i]].WorldPosition).GetPositionOnPlane(
                        frame.ImageWidth, frame.ImageHeight, viewPlanePosition, viewPlaneRotation, viewPlaneScale);
                }
            }
            else
            {
                if (isPlaybackFrame)
                {

                    currPosition = VideoPlayer.WorldToDepthSpace(JointTypes[i], frame, body).GetPositionOnPlane(
                        frame.DepthWidth, frame.DepthHeight, viewPlanePosition, viewPlaneRotation, viewPlaneScale);
                }
                else
                {
                    currPosition = adapter.WorldToDepthSpace(body.Joints[JointTypes[i]].WorldPosition).GetPositionOnPlane(
                        frame.DepthWidth, frame.DepthHeight, viewPlanePosition, viewPlaneRotation, viewPlaneScale);
                }
            }

            jointPoints[i].position = Vector3D.Lerp(jointPoints[i].position, currPosition, smoothness);
        }

        jointLines[0].SetPosition(0, jointPoints[0].position);
        jointLines[0].SetPosition(1, jointPoints[1].position);
        jointLines[0].SetPosition(2, jointPoints[2].position);
        jointLines[0].SetPosition(3, jointPoints[3].position);
        jointLines[0].SetPosition(4, jointPoints[4].position);

        jointLines[1].SetPosition(0, jointPoints[8].position);
        jointLines[1].SetPosition(1, jointPoints[7].position);
        jointLines[1].SetPosition(2, jointPoints[6].position);
        jointLines[1].SetPosition(3, jointPoints[5].position);
        jointLines[1].SetPosition(4, jointPoints[2].position);
        jointLines[1].SetPosition(5, jointPoints[9].position);
        jointLines[1].SetPosition(6, jointPoints[10].position);
        jointLines[1].SetPosition(7, jointPoints[11].position);
        jointLines[1].SetPosition(8, jointPoints[12].position);

        jointLines[2].SetPosition(0, jointPoints[16].position);
        jointLines[2].SetPosition(1, jointPoints[15].position);
        jointLines[2].SetPosition(2, jointPoints[14].position);
        jointLines[2].SetPosition(3, jointPoints[13].position);
        jointLines[2].SetPosition(4, jointPoints[4].position);
        jointLines[2].SetPosition(5, jointPoints[17].position);
        jointLines[2].SetPosition(6, jointPoints[18].position);
        jointLines[2].SetPosition(7, jointPoints[19].position);
        jointLines[2].SetPosition(8, jointPoints[20].position);
    }
}