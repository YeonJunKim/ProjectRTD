using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public AudioSource clickSound;

    public void OnGameStart()
    {
        clickSound.Play();
        SceneManager.LoadScene(1);  // GameScene
    }

    public void OnPath()
    {
        clickSound.Play();
        SceneManager.LoadScene(2);  // GameScene
    }
}
