using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class Manager : MonoBehaviour
{

    public ImageTarget imageTarget;
    ObjectTracker mTracker;

    void Update()
    {
        
    }

    public void EndTrial()
    {
        Debug.Log("Prueba finalizada");
        //SceneManager.LoadScene("Game Escene");
    }

    public void EndGame()
    {
        Debug.Log("Prueba finalizada");
        SceneManager.LoadScene("Start Menu");
    }
}
