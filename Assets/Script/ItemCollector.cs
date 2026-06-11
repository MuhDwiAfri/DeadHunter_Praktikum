using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemCollector : MonoBehaviour
{
    private int coins = 0;
    [Header("UI & Audio Settings")]
    [SerializeField] private Text coinsText;
    [SerializeField] private AudioClip collectionSound; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            coins++;
            if (coinsText != null)
            {
                coinsText.text = "Coins: " + coins;
            }
            if (collectionSound != null)
            {
                AudioSource.PlayClipAtPoint(collectionSound, transform.position);
            }
        }
    }
}