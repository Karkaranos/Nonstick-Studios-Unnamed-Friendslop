using UnityEngine;
using FishNet.Object;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private float speed;

     private void Awake() 
     {
        Debug.Log($"{gameObject.name} spawned");
     }

     private void Update() 
     {
        Vector3 movementThisFrame = Vector3.forward * speed * Time.deltaTime;
        transform.Translate(movementThisFrame);
        //Debug.Log($"{gameObject.name} moved {movementThisFrame.magnitude} units");
     }
}
