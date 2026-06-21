using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishMenu : MonoBehaviour  // Menghapus huruf 'h' agar sama dengan nama file
{
    public void KembaliKeMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}