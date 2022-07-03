using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour //a.k.a. The Doom Bringer
{
    void Update() {
        if(Input.anyKeyDown){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }    
    }
}
