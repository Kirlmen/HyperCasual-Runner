using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject startReference, endReference;
    public BoxCollider hiddenPlatform;

    private void Start()
    {
        Vector3 direction = endReference.transform.position - startReference.transform.position; //iki referans arasındaki yön vektörünü alıyoruz.
        float distance = direction.magnitude; //yön vektörümüzün ağırlığı. 2 referans noktası arasındaki mesafeyi ölçebiliyoruz.
        direction = direction.normalized;
        hiddenPlatform.transform.forward = direction; //colliderı da referans noktalarına göre yönünü eşitliyor.(rotation olarak)
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance); //hiddenplatform z ekseninde aradaki mesafe kadar genişler.


        hiddenPlatform.transform.position = startReference.transform.position + (direction * distance / 2) + (new Vector3(0, -direction.z, direction.y) * hiddenPlatform.size.y / 2); //hiddenplatform colliderı iki referans noktasının en ortasına gelmesi için.

    }
}
