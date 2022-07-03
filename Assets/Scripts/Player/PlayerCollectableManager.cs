using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollectableManager : MonoBehaviour
{
    [SerializeField] GameObject CollectableParticle;
    int _collectableCount = -1;

    private void Start() {
        Time.timeScale = 1f;
        _collectableCount = GameObject.FindGameObjectsWithTag("Collectable").Length;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Collectable")){
            Instantiate(CollectableParticle, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            if(--_collectableCount == 0){
                CameraManager.ShakeCamera(0.08f,1f);
                Invoke(nameof(NextLevel),0.15f);
                Time.timeScale = .2f;
                GetComponent<PlayerDeathManager>().Invinsible = true;
            }
        }
    }

    private void NextLevel() {
        if(SceneManager.GetActiveScene().buildIndex == 25)
            SceneManager.LoadScene("Level Select");
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
