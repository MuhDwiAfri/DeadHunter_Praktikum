using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private ItemCollector playerCollector;

    private void Start()
    {
        // Mencari skrip ItemCollector yang menempel di karakter utama secara otomatis saat game mulai
        playerCollector = FindObjectOfType<ItemCollector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah yang menyentuh gerbang adalah Player
        if (other.CompareTag("Player"))
        {
            // Cek apakah jumlah jamur di skrip ItemCollector player sudah memenuhi target
            if (playerCollector != null && playerCollector.ApakahSemuaJamurSudahDiambil())
            {
                // Jika sudah cukup, gas pindah ke level selanjutnya (buildIndex + 1)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                Debug.Log("Jamur belum cukup, cari lagi di sekitar hutan!");
            }
        }
    }
}