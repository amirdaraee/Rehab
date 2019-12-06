using UnityEngine;

[System.Serializable]
public class FaceStick
{
    public GameObject pointsParent;
    public Transform[] points;
    public GameObject indicesParent;
    public Transform[] indices;
    public GameObject lineParent;
    public LineRenderer line;

    [HideInInspector]
    public bool show = true;

    [System.NonSerialized]
    public int[] pointIndices = null;

    public Vector3 GetPosition(int index)
    {
        return points[index].position;
    }
}