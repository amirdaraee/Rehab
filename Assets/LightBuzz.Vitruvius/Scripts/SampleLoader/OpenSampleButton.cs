using LightBuzz.Vitruvius;
using UnityEngine;

public class OpenSampleButton : MonoBehaviour
{
    public int sceneId = 0;

    public void OpenScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
    }
}