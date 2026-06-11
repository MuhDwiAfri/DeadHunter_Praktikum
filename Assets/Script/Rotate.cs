using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float speedX;
    [SerializeField] private float speedY;
    [SerializeField] private float speedZ;

    void Update()
    {
        // Menghitung rotasi berdasarkan kecepatan dikalikan satu putaran penuh (360 derajat)
        // Time.deltaTime digunakan agar kecepatan rotasi konsisten di setiap frame
        transform.Rotate(
            360 * speedX * Time.deltaTime, 
            360 * speedY * Time.deltaTime, 
            360 * speedZ * Time.deltaTime
        );
    }
}