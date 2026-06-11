using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Trigger : MonoBehaviour{
private Vector3 startingPosition;
private Rigidbody rb;
void Start(){
startingPosition = transform.position;
rb = GetComponent<Rigidbody>();
}
private void OnTriggerEnter(Collider other){
if (other.CompareTag("Obstacle")){
ResetPlayer();
}
}
void ResetPlayer()
{
transform.position = startingPosition;
if (rb != null)
{
rb.velocity = Vector3.zero;
rb.angularVelocity = Vector3.zero;
}
Debug.Log("Awas, jalan yang benar dong");
}
}

//

//

