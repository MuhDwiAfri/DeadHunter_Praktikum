using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangerScene : MonoBehaviour
{
    public float changeTime;
    public string sceneName;
    private bool sceneLoaded = false; 
    private void Update()
    {
        if (sceneLoaded) return;

        changeTime -= Time.deltaTime;
        if (changeTime <= 0)
        {
            sceneLoaded = true;
            SceneManager.LoadScene(sceneName);
        }   
    }
}