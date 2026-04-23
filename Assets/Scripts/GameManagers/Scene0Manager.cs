using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene0Manager : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("Player", LoadSceneMode.Additive);
        SceneManager.LoadScene("LV1_Lights_postpro", LoadSceneMode.Additive);
    }
}
