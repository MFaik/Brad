using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerDeathManager : MonoBehaviour
{
    public bool Invinsible = false;
    [SerializeField] GameObject PlayerDeathParticle;
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy") && !Invinsible){
            CameraManager.ShakeCamera(0.08f,0.1f);
            Instantiate(PlayerDeathParticle,transform.position,Quaternion.identity);
            Destroy(gameObject);
            GameObject.Find("Global Light 2D").GetComponent<Light2D>().intensity = .4f;
        }    
    }
}
