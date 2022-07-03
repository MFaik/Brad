using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int LevelNumber = -1;
    
    public void LoadLevel() {
        SceneManager.LoadScene("Level " + LevelNumber);
    }
}
