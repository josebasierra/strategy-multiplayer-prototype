using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] string _sceneName;

    private void Start()
    {
        SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
    }
}
