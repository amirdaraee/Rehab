using UnityEngine;
using LightBuzz;
using LightBuzz.Vitruvius;

public class ScreenViewStickface : MonoBehaviour
{
    public StickfaceSettings settings;
    public FaceStick[] faceSticks;

    bool initialized = false;

    void Initialize()
    {
        for (int i = 0; i < faceSticks.Length; i++)
        {
            faceSticks[i].pointIndices = new int[faceSticks[i].indices.Length];

            for (int j = 0; j < faceSticks[i].indices.Length; j++)
            {
                faceSticks[i].pointIndices[j] = int.Parse(faceSticks[i].indices[j].name);
            }
        }

        initialized = true;
    }

    public void UpdateStickface(SensorAdapter adapter, Frame frame, Face face, Transform viewPlane, Visualization visualization)
    {
        bool isPlaybackFrame = frame != null && frame.IsPlaybackFrame;
        if (face == null || viewPlane == null || (!isPlaybackFrame && adapter == null)) return;

        if (!initialized)
        {
            Initialize();
        }

        Vector3 viewPlanePosition = viewPlane.position;
        LightBuzz.Quaternion viewPlaneRotation = viewPlane.rotation;
        Vector3D viewPlaneScale = viewPlane.localScale;

        if (viewPlane.GetType() == typeof(RectTransform))
        {
            viewPlaneScale.Set(viewPlaneScale.X * ((RectTransform)viewPlane).root.localScale.x * ((RectTransform)viewPlane).rect.size.x,
                viewPlaneScale.Y * ((RectTransform)viewPlane).root.localScale.y * ((RectTransform)viewPlane).rect.size.y, 1);
        }

        float smoothness = Mathf.Lerp(1f, Time.deltaTime, settings.updateSmoothness);

        faceSticks[0].show = settings.leftEye;
        faceSticks[1].show = settings.rightEye;
        faceSticks[2].show = settings.leftEyebrow;
        faceSticks[3].show = settings.rightEyebrow;
        faceSticks[4].show = settings.nose;
        faceSticks[5].show = settings.mouth;
        faceSticks[6].show = settings.jaw;

        Vector2Int resolution = visualization == Visualization.Image ?
            new Vector2Int(frame.ImageWidth, frame.ImageHeight) :
            new Vector2Int(frame.DepthWidth, frame.DepthHeight);

        for (int i = 0; i < faceSticks.Length; i++)
        {
            if (!faceSticks[i].show)
            {
                if (faceSticks[i].indicesParent.activeSelf)
                {
                    faceSticks[i].indicesParent.SetActive(false);
                }

                if (faceSticks[i].pointsParent.activeSelf)
                {
                    faceSticks[i].pointsParent.SetActive(false);
                }
            }
            else
            {
                if (settings.showPointIds != faceSticks[i].indicesParent.activeSelf)
                {
                    faceSticks[i].indicesParent.SetActive(settings.showPointIds);
                }

                if (settings.showPointIds == faceSticks[i].pointsParent.activeSelf)
                {
                    faceSticks[i].pointsParent.SetActive(!settings.showPointIds);
                }
            }

            Vector3D position;

            for (int j = 0, index = 0; j < faceSticks[i].indices.Length; j++)
            {
                index = faceSticks[i].pointIndices[j];

                position = (visualization == Visualization.Image ? face.Points2D[index] : face.WorldToDepthSpace(index, adapter)).
                    GetPositionOnPlane(resolution.x, resolution.y, viewPlanePosition, viewPlaneRotation, viewPlaneScale);
                position = Vector3D.Lerp(faceSticks[i].points[j].position, position, smoothness);

                faceSticks[i].points[j].position = position;
                faceSticks[i].indices[j].position = position;
            }

            if (settings.drawLines != faceSticks[i].lineParent.activeSelf)
            {
                faceSticks[i].lineParent.SetActive(settings.drawLines);
            }
        }

        // left eye
        FaceStick faceStick = faceSticks[0];
        UpdateLineVisibility(faceStick);
        faceStick.line.SetPosition(0, faceStick.GetPosition(0));
        faceStick.line.SetPosition(1, faceStick.GetPosition(1));
        faceStick.line.SetPosition(2, faceStick.GetPosition(2));
        faceStick.line.SetPosition(3, faceStick.GetPosition(3));
        faceStick.line.SetPosition(4, faceStick.GetPosition(4));
        faceStick.line.SetPosition(5, faceStick.GetPosition(5));
        faceStick.line.SetPosition(6, faceStick.GetPosition(0));

        // right eye
        faceStick = faceSticks[1];
        UpdateLineVisibility(faceStick);
        faceStick.line.SetPosition(0, faceStick.GetPosition(0));
        faceStick.line.SetPosition(1, faceStick.GetPosition(1));
        faceStick.line.SetPosition(2, faceStick.GetPosition(2));
        faceStick.line.SetPosition(3, faceStick.GetPosition(3));
        faceStick.line.SetPosition(4, faceStick.GetPosition(4));
        faceStick.line.SetPosition(5, faceStick.GetPosition(5));
        faceStick.line.SetPosition(6, faceStick.GetPosition(0));

        // left eyebrow
        faceStick = faceSticks[2];
        UpdateLineVisibility(faceStick);
        faceStick.line.SetPosition(0, faceStick.GetPosition(0));
        faceStick.line.SetPosition(1, faceStick.GetPosition(1));
        faceStick.line.SetPosition(2, faceStick.GetPosition(2));
        faceStick.line.SetPosition(3, faceStick.GetPosition(3));
        faceStick.line.SetPosition(4, faceStick.GetPosition(4));

        // right eyebrow
        faceStick = faceSticks[3];
        UpdateLineVisibility(faceStick);
        faceStick.line.SetPosition(0, faceStick.GetPosition(0));
        faceStick.line.SetPosition(1, faceStick.GetPosition(1));
        faceStick.line.SetPosition(2, faceStick.GetPosition(2));
        faceStick.line.SetPosition(3, faceStick.GetPosition(3));
        faceStick.line.SetPosition(4, faceStick.GetPosition(4));

        // nose
        faceStick = faceSticks[4];
        UpdateLineVisibility(faceStick);
        faceStick.line.SetPosition(0, faceStick.GetPosition(0));
        faceStick.line.SetPosition(1, faceStick.GetPosition(1));
        faceStick.line.SetPosition(2, faceStick.GetPosition(2));
        faceStick.line.SetPosition(3, faceStick.GetPosition(3));
        faceStick.line.SetPosition(4, faceStick.GetPosition(4));
        faceStick.line.SetPosition(5, faceStick.GetPosition(5));
        faceStick.line.SetPosition(6, faceStick.GetPosition(6));
        faceStick.line.SetPosition(7, faceStick.GetPosition(7));
        faceStick.line.SetPosition(8, faceStick.GetPosition(8));
        faceStick.line.SetPosition(9, faceStick.GetPosition(3));

        // mouth
        faceStick = faceSticks[5];
        UpdateLineVisibility(faceStick);
        faceStick.line.SetPosition(0, faceStick.GetPosition(0));
        faceStick.line.SetPosition(1, faceStick.GetPosition(1));
        faceStick.line.SetPosition(2, faceStick.GetPosition(2));
        faceStick.line.SetPosition(3, faceStick.GetPosition(3));
        faceStick.line.SetPosition(4, faceStick.GetPosition(4));
        faceStick.line.SetPosition(5, faceStick.GetPosition(5));
        faceStick.line.SetPosition(6, faceStick.GetPosition(6));
        faceStick.line.SetPosition(7, faceStick.GetPosition(7));
        faceStick.line.SetPosition(8, faceStick.GetPosition(8));
        faceStick.line.SetPosition(9, faceStick.GetPosition(9));
        faceStick.line.SetPosition(10, faceStick.GetPosition(10));
        faceStick.line.SetPosition(11, faceStick.GetPosition(11));
        faceStick.line.SetPosition(12, faceStick.GetPosition(0));
        faceStick.line.SetPosition(13, faceStick.GetPosition(12));
        faceStick.line.SetPosition(14, faceStick.GetPosition(13));
        faceStick.line.SetPosition(15, faceStick.GetPosition(14));
        faceStick.line.SetPosition(16, faceStick.GetPosition(15));
        faceStick.line.SetPosition(17, faceStick.GetPosition(16));
        faceStick.line.SetPosition(18, faceStick.GetPosition(17));
        faceStick.line.SetPosition(19, faceStick.GetPosition(18));
        faceStick.line.SetPosition(20, faceStick.GetPosition(19));
        faceStick.line.SetPosition(21, faceStick.GetPosition(12));

        // jaw
        faceStick = faceSticks[6];
        UpdateLineVisibility(faceStick);
        faceStick.line.SetPosition(0, faceStick.GetPosition(0));
        faceStick.line.SetPosition(1, faceStick.GetPosition(1));
        faceStick.line.SetPosition(2, faceStick.GetPosition(2));
        faceStick.line.SetPosition(3, faceStick.GetPosition(3));
        faceStick.line.SetPosition(4, faceStick.GetPosition(4));
        faceStick.line.SetPosition(5, faceStick.GetPosition(5));
        faceStick.line.SetPosition(6, faceStick.GetPosition(6));
        faceStick.line.SetPosition(7, faceStick.GetPosition(7));
        faceStick.line.SetPosition(8, faceStick.GetPosition(8));
        faceStick.line.SetPosition(9, faceStick.GetPosition(9));
        faceStick.line.SetPosition(10, faceStick.GetPosition(10));
        faceStick.line.SetPosition(11, faceStick.GetPosition(11));
        faceStick.line.SetPosition(12, faceStick.GetPosition(12));
        faceStick.line.SetPosition(13, faceStick.GetPosition(13));
        faceStick.line.SetPosition(14, faceStick.GetPosition(14));
        faceStick.line.SetPosition(15, faceStick.GetPosition(15));
        faceStick.line.SetPosition(16, faceStick.GetPosition(16));
    }

    void UpdateLineVisibility(FaceStick faceStick)
    {
        if (faceStick.show != faceStick.line.gameObject.activeSelf)
        {
            faceStick.line.gameObject.SetActive(faceStick.show);
        }
    }

    public static void UpdateFacePoint(SensorAdapter adapter, Frame frame, Vector2D facePoint, Transform facePointTransform, Transform viewPlane, Visualization visualization, float updateSmoothness)
    {
        bool ignoreAdapter = VideoPlayer.IsPlaying || !Application.isEditor && Application.platform == RuntimePlatform.Android;
        if ((!ignoreAdapter && adapter == null) || facePointTransform == null || viewPlane == null) return;

        Vector3 viewPlanePosition = viewPlane.position;
        LightBuzz.Quaternion viewPlaneRotation = viewPlane.rotation;
        Vector3D viewPlaneScale = viewPlane.localScale;

        if (viewPlane.GetType() == typeof(RectTransform))
        {
            viewPlaneScale.Set(viewPlaneScale.X * ((RectTransform)viewPlane).root.localScale.x * ((RectTransform)viewPlane).rect.size.x,
                viewPlaneScale.Y * ((RectTransform)viewPlane).root.localScale.y * ((RectTransform)viewPlane).rect.size.y, 1);
        }

        float smoothness = Mathf.Lerp(1f, Time.deltaTime, updateSmoothness);

        Vector3D position = visualization == Visualization.Image ? facePoint : VideoPlayer.IsPlaying ? default(Vector2D) :
            !Application.isEditor && Application.platform == RuntimePlatform.Android ? facePoint : adapter.ImageToDepthSpace(facePoint);

        Vector2Int resolution = visualization == Visualization.Image ?
            new Vector2Int(frame.ImageWidth, frame.ImageHeight) :
            new Vector2Int(frame.DepthWidth, frame.DepthHeight);

        facePointTransform.position = Vector3D.Lerp(facePointTransform.position,
            position.GetPositionOnPlane(resolution.x, resolution.y, viewPlanePosition, viewPlaneRotation, viewPlaneScale), smoothness);
    }
}