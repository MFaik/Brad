using UnityEngine;
using UnityEngine.UI;
#if UnityEditor //HACK: I couldn;t figure this out srry :p
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public class LevelButtonCreator : MonoBehaviour {
    [SerializeField] GameObject LevelButton;
    public void CreateLevelButtons() {
        #if UnityEditor
        while(transform.childCount > 0){
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        int totalLevelCount = AssetDatabase.FindAssets("t:SceneAsset", new string[] {"Assets/Scenes/Levels"}).Length;
        for(int i = 1; i <= totalLevelCount; i++){
            GameObject button = PrefabUtility.InstantiatePrefab(LevelButton) as GameObject;
            button.transform.SetParent(transform);
            button.GetComponent<LevelButton>().LevelNumber = i;
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>("LevelSprites/Level "+ i);
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        #endif
    }
}