using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    private int jamur = 0;

    [Header("UI & Audio Settings")]
    [SerializeField] private Text jamurText;
    [SerializeField] private AudioClip collectionSound; 

    // TAMBAHAN: Variabel target yang bisa diubah-ubah angkanya di tiap level lewat Inspector
    [Header("Level Target")]
    [SerializeField] private int totalJamurYangDibutuhkan = 2; 

    private void Start()
    {
        UpdateUIJamur();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Jamur"))
        {
            Destroy(other.gameObject);
            jamur++;
            UpdateUIJamur(); // Menggunakan fungsi biar rapi

            if (collectionSound != null)
            {
                AudioSource.PlayClipAtPoint(collectionSound, transform.position);
            }
        }
    }

    // TAMBAHAN: Fungsi pembantu untuk mengupdate UI agar teksnya informatif (contoh, Jamur: 1 / 2)
    private void UpdateUIJamur()
    {
        if (jamurText != null)
        {
            jamurText.text = "Jamur: " + jamur + " / " + totalJamurYangDibutuhkan;
        }
    }

    // KUNCI UNTUK FINISH: Fungsi publik agar bisa dicek oleh skrip Finish.cs
    public bool ApakahSemuaJamurSudahDiambil()
    {
        return jamur >= totalJamurYangDibutuhkan;
    }
}