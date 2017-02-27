using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using System.Collections;

public class SplashScreenManager : MonoBehaviour {

    void Start()
    {
        SceneManager.LoadScene("MainScreen");
    }
}
