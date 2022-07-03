using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NunchuckEditor : MonoBehaviour
{
    

    Vector3 _lastScale = new Vector3(1,1,1);

    void Update() {
        if(transform.localScale != _lastScale){
            foreach(Transform child in transform){
                child.localScale = new Vector3(1,transform.localScale.x/transform.localScale.y,1);
            }
        }
        if (!Application.IsPlaying(gameObject))
            return;
    }
}
