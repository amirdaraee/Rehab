using UnityEngine;

public class OpenURLButton : MonoBehaviour
{
    public void OpenURL()
    {
        Application.OpenURL(@"http://lightbuzz.com");
    }
}