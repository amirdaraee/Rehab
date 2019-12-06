using UnityEngine;
using LightBuzz;
using LightBuzz.Vitruvius;

public class PointCloudStickface : MonoBehaviour
{
    public StickfaceSettings settings;
    public FaceStick[] faceSticks;
    public float faceScaleMultiplier = 1;

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

    public void UpdateStickface(Face face)
    {
        if (face == null) return;

        if (!initialized)
        {
            Initialize();
        }

        float smoothness = Mathf.Lerp(1f, Time.deltaTime, settings.updateSmoothness);

        faceSticks[0].show = settings.leftEye;
        faceSticks[1].show = settings.rightEye;
        faceSticks[2].show = settings.leftEyebrow;
        faceSticks[3].show = settings.rightEyebrow;
        faceSticks[4].show = settings.nose;
        faceSticks[5].show = settings.mouth;
        faceSticks[6].show = settings.jaw;
        
        Vector3D nosePosition = face.Nose(LightBuzz.Vitruvius.Space.World);
        Vector3D currentPosition;

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

            for (int j = 0; j < faceSticks[i].indices.Length; j++)
            {
                currentPosition = Vector3D.Lerp(
                    faceSticks[i].points[j].localPosition,
                    (face.Points3D[faceSticks[i].pointIndices[j]] - nosePosition) * faceScaleMultiplier,
                    smoothness);

                faceSticks[i].points[j].localPosition = currentPosition;
                faceSticks[i].indices[j].localPosition = currentPosition;
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
}