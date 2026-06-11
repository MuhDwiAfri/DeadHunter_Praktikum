using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] points; 
    public float speed = 3f;
    public float reachDistance = 0.1f;
    
    private int currentPointIndex = 0;
    private bool movingForward = true; // Penanda arah urutan waypoint

    void Update(){
    // Pastikan ada minimal 2 waypoint (Kiri dan Kanan)
        if (points == null || points.Length < 2) return;
        // 1. Ambil posisi target berdasarkan indeks saat ini
        Vector3 targetPosition = points[currentPointIndex].position;
        // 2. Gerakkan objek menuju target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        // 3. Cek apakah sudah sampai di waypoint
        if (Vector3.Distance(transform.position, targetPosition) < reachDistance){
            if (movingForward){
                currentPointIndex++;
                // Jika sampai di ujung kanan (waypoint terakhir), balik arah
                if (currentPointIndex >= points.Length)
                {
                    currentPointIndex = points.Length - 2; // Targetkan titik sebelumnya
                    movingForward = false;
                }
            }
            else
            {
                currentPointIndex--;
                // Jika sampai di ujung kiri (waypoint pertama), balik arah lagi
                if (currentPointIndex < 0)
                {
                    currentPointIndex = 1; // Targetkan titik setelahnya
                    movingForward = true;
                }
            }
        }
    }
}