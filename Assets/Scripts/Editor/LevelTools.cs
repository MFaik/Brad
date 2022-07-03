using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LevelTools : MonoBehaviour
{
    [MenuItem("LevelTools/Create Level Sprites")]
    public static void CreateSpritesOfLevels() {
        EditorSceneManager.SaveOpenScenes();
        string currentScene = EditorSceneManager.GetActiveScene().path;

        int totalLevelCount = AssetDatabase.FindAssets("t:SceneAsset", new string[] {"Assets/Scenes/Levels"}).Length;
        for(int i = 1; i <= totalLevelCount; i++){
            EditorSceneManager.OpenScene("Assets/Scenes/Levels/Level " + i + ".unity");

            GameObject tilemapObject = GameObject.FindGameObjectWithTag("Ground");
            Tilemap tilemap = tilemapObject ? GameObject.FindGameObjectWithTag("Ground").GetComponent<Tilemap>() : null;
            if(!tilemap){
                Debug.LogError("Couldn't find tilemap in Level" + i);
                continue;
            }

            Texture2D texture = new Texture2D(64, 36);

            BoundsInt bounds = (new BoundsInt(-32, -18, 0, 64, 36, 1));
            TileBase[] a = tilemap.GetTilesBlock(bounds);
            for(int y = 0; y < bounds.size.y; y++){
                for(int x = 0; x < bounds.size.x; x++){
                    if(a[x+y*bounds.size.x])
                        texture.SetPixel(x, y, Color.white);
                    else
                        texture.SetPixel(x, y, Color.black);
                }
            }
            texture.Apply();

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 36), new Vector2(64/2, 36/2),4);
            sprite.name = "Level " + i;

            SaveSpriteAsAsset(sprite, "Resources/LevelSprites/Level " + i + ".png");
        }

        EditorSceneManager.OpenScene(currentScene);
    }

    //https://forum.unity.com/threads/issues-dynamically-creating-sprites-and-saving-them.318126/
    static void SaveSpriteAsAsset(Sprite sprite, string proj_path) {
        var abs_path = Path.Combine(Application.dataPath, proj_path);
        proj_path = Path.Combine("Assets", proj_path);
    
        Directory.CreateDirectory(Path.GetDirectoryName(abs_path));
        File.WriteAllBytes(abs_path, ImageConversion.EncodeToPNG(sprite.texture));
    
        //AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(proj_path);
    
        var ti = AssetImporter.GetAtPath(proj_path) as TextureImporter;
        ti.spritePixelsPerUnit = sprite.pixelsPerUnit;
        ti.mipmapEnabled = false;
        ti.textureType = TextureImporterType.Sprite;
        ti.filterMode = FilterMode.Point;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
    
        EditorUtility.SetDirty(ti);
        ti.SaveAndReimport();
    }
}
