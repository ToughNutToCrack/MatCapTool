using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureWindow : EditorWindow{
    public string textureName = "MatCapTexture";

    string basePathTextures = "Assets/Resources/Textures";

    [MenuItem("MatCapTool/GenerateTexture")]
    public static void showWindow(){
        UnityEditor.EditorWindow window = GetWindow<TextureWindow>(false, "GenerateTexture", true);
        const int width = 500;
        const int height = 50;
        Vector2 size = new Vector2(width, height);
        window.minSize = size;
        window.position = new Rect(0, 0, width, height);
        window.Show();
    }

    void OnGUI(){
        textureName = EditorGUILayout.TextField("Texture Name", textureName);
       
        if (GUILayout.Button("Generate")){
            textureName = string.IsNullOrEmpty(textureName)? DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") : textureName;
            generateTexture();
        }
    }

    void OnInspectorUpdate(){
        Repaint();
    }

    void OnDestroy(){
        EditorUtility.ClearProgressBar();
        Resources.UnloadUnusedAssets();
    }

    void generateTexture(){
        if(!Directory.Exists(basePathTextures)){
            Directory.CreateDirectory(basePathTextures);
        }

        string path = Path.Combine(basePathTextures, textureName + ".png");
        if(File.Exists(path)){
            textureName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            path = Path.Combine(basePathTextures, textureName + ".png");
        }
        ScreenCapture.CaptureScreenshot(path);
        UnityEditor.AssetDatabase.Refresh();
    }
}