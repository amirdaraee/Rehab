using UnityEngine;

[ExecuteInEditMode]
public class AngleArc : MonoBehaviour
{
    #region Variables with Properties

    public MeshFilter meshFilter = null;
    Mesh mesh = null;

    Vector3[] vertices = null;
    int[] triangles = null;
    Vector2[] uv = null;

    const int MIN_POSIBLE_QUALITY = 5;
    const int MAX_POSIBLE_QUALITY = 100;

    [Range(MIN_POSIBLE_QUALITY, MAX_POSIBLE_QUALITY), SerializeField]
    int minQuality = MIN_POSIBLE_QUALITY;
#if UNITY_EDITOR
    int prevMinQuality = MIN_POSIBLE_QUALITY;
#endif
    public int MinQuality
    {
        get
        {
            return minQuality;
        }
        set
        {
            if (minQuality != value)
            {
                minQuality = Mathf.Clamp(value, MIN_POSIBLE_QUALITY, MAX_POSIBLE_QUALITY);
#if UNITY_EDITOR
                prevMinQuality = minQuality;
#endif
                if (maxQuality < minQuality)
                {
                    MaxQuality = minQuality;
                }
                else
                {
                    RefreshArc();
                }
            }
        }
    }

    [Range(MIN_POSIBLE_QUALITY, MAX_POSIBLE_QUALITY), SerializeField]
    int maxQuality = MAX_POSIBLE_QUALITY;
#if UNITY_EDITOR
    int prevMaxQuality = MAX_POSIBLE_QUALITY;
#endif
    public int MaxQuality
    {
        get
        {
            return maxQuality;
        }
        set
        {
            if (maxQuality != value)
            {
                maxQuality = Mathf.Clamp(value, minQuality, MAX_POSIBLE_QUALITY);
#if UNITY_EDITOR
                prevMaxQuality = maxQuality;
#endif
                RefreshArc();
            }
        }
    }

    public int Quality
    {
        get
        {
            return (int)Mathf.Lerp(minQuality, maxQuality, angle / 360f);
        }
    }

    [SerializeField]
    float angle = 0;
#if UNITY_EDITOR
    float prevAngle = 0;
#endif
    public float Angle
    {
        get
        {
            return angle;
        }
        set
        {
            if (angle != value)
            {
                angle = value % 360;
#if UNITY_EDITOR
                prevAngle = angle;
#endif
                RefreshArc();
            }
        }
    }

    [SerializeField]
    float minDist = 0;
#if UNITY_EDITOR
    float prevMinDist = 0;
#endif
    public float MinDistance
    {
        get
        {
            return minDist;
        }
        set
        {
            if (minDist != value)
            {
                minDist = value;
#if UNITY_EDITOR
                prevMinDist = value;
#endif
                RefreshArc();
            }
        }
    }

    [SerializeField]
    float maxDist = 0.5f;
#if UNITY_EDITOR
    float prevMaxDist = 0.5f;
#endif
    public float MaxDistance
    {
        get
        {
            return maxDist;
        }
        set
        {
            if (maxDist != value)
            {
                maxDist = value;
#if UNITY_EDITOR
                prevMaxDist = value;
#endif
                RefreshArc();
            }
        }
    }

    #endregion

    #region Reserved methods // Awake - OnDestroy - Update(Editor) - Reset

    void Awake()
    {
        RefreshArc();
    }

    void OnDestroy()
    {
        if (mesh != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(mesh);
#else
            Destroy(mesh);
#endif
        }
    }

    #region Update(Editor)

#if UNITY_EDITOR
    void Update()
    {
        if (prevMinQuality != minQuality)
        {
            minQuality = Mathf.Clamp(minQuality, MIN_POSIBLE_QUALITY, MAX_POSIBLE_QUALITY);
            prevMinQuality = minQuality;

            if (maxQuality < minQuality)
            {
                MaxQuality = minQuality;
            }
            else
            {
                RefreshArc();
            }
        }

        if (prevMaxQuality != maxQuality)
        {
            maxQuality = Mathf.Clamp(maxQuality, minQuality, MAX_POSIBLE_QUALITY);
            prevMaxQuality = maxQuality;

            RefreshArc();
        }

        if (prevAngle != angle)
        {
            angle = angle % 360;
            prevAngle = angle;

            RefreshArc();
        }


        if (prevMinDist != minDist)
        {
            prevMinDist = minDist;

            RefreshArc();
        }

        if (prevMaxDist != maxDist)
        {
            prevMaxDist = maxDist;

            RefreshArc();
        }
    }
#endif

    #endregion

    void Reset()
    {
        RefreshArc();
    }

    #endregion

    #region RefreshArc

    void RefreshArc()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                return;
            }
        }

        if (mesh == null)
        {
            mesh = new Mesh();
        }
        else
        {
            mesh.Clear();
        }

        int quality = Quality;
        float angle_delta = -angle / quality;

        float angle_curr = angle;
        float angle_next = angle_delta + angle;

        Vector3 pos_curr_min = Vector3.zero;
        Vector3 pos_curr_max = Vector3.zero;

        Vector3 pos_next_min = Vector3.zero;
        Vector3 pos_next_max = Vector3.zero;

        int vertexCount = 4 * quality;
        if (vertices == null || vertices.Length != vertexCount)
        {
            vertices = new Vector3[vertexCount];
            triangles = new int[6 * quality];
            uv = new Vector2[vertexCount];
        }

        for (int i = 0, c = 0, t = 0; i < quality; i++)
        {
            Vector3 sphere_curr = new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * (angle_curr)),
            Mathf.Cos(Mathf.Deg2Rad * (angle_curr)),
            0);

            Vector3 sphere_next = new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * (angle_next)),
            Mathf.Cos(Mathf.Deg2Rad * (angle_next)),
            0);

            angle_curr += angle_delta;
            angle_next += angle_delta;

            pos_curr_min = sphere_curr * minDist;
            pos_curr_max = sphere_curr * maxDist;

            pos_next_min = sphere_next * minDist;
            pos_next_max = sphere_next * maxDist;

            c = 4 * i;
            vertices[c] = pos_curr_min;
            vertices[c + 1] = pos_curr_max;
            vertices[c + 2] = pos_next_max;
            vertices[c + 3] = pos_next_min;

            t = 6 * i;
            triangles[t] = c;
            triangles[t + 1] = c + 3;
            triangles[t + 2] = c + 2;
            triangles[t + 3] = c + 2;
            triangles[t + 4] = c + 1;
            triangles[t + 5] = c;
        }

        for (int i = 0, c = 0; i < quality; i++)
        {
            c = 4 * i;
            uv[c] = vertices[c] - new Vector3(-0.5f, -0.5f, 0);
            uv[c + 1] = vertices[c + 1] - new Vector3(-0.5f, -0.5f, 0);
            uv[c + 2] = vertices[c + 2] - new Vector3(-0.5f, -0.5f, 0);
            uv[c + 3] = vertices[c + 3] - new Vector3(-0.5f, -0.5f, 0);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();

        if (meshFilter.sharedMesh != mesh)
        {
            meshFilter.sharedMesh = mesh;
        }
    }

    #endregion

    #region ArcVisibility

    public void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    #endregion
}