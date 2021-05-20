using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class MatCapWindow : EditorWindow{
    public string textureName = "MatCapTexture";
    public static int size = 512;

    const int height = 70;
    string basePathTextures = "Assets/Resources/Textures";
    GameObject gameObject;
    Camera cam;
 
    static MatCapWindow window;
 
    [MenuItem("MatCapTool/MatCapTexture")]
    static void showWindow(){
        window = GetWindow<MatCapWindow>(false, "GenerateTexture", true);
        window.resizeWindow();
        window.Show();
        window.position = new Rect(0, 0, size, height);
        window.init();
    }

    void resizeWindow(){
        Vector2 dim = new Vector2(size, size + height);
        window.minSize = dim;
    }
 
    public void init(){
        gameObject = new GameObject("Editor Camera");
        GameObject realCamera = GameObject.FindGameObjectWithTag("MainCamera");
        gameObject.transform.position = realCamera.transform.position;
        gameObject.AddComponent<Camera>();

        cam = gameObject.GetComponent<Camera>();
        cam.backgroundColor = Color.green;
        cam.enabled = false;
        cam.orthographic = false;
        cam.orthographicSize = realCamera.GetComponent<Camera>().orthographicSize;
    }
 
    void OnGUI(){
        textureName = EditorGUILayout.TextField("Texture Name", textureName);
        size = EditorGUILayout.IntField("Image Size", size);
        window.resizeWindow();

        cam.targetTexture = new RenderTexture(
            size,
            size, 
            24,
            RenderTextureFormat.ARGB32
        );        

        cam.Render();
        GUI.DrawTexture(new Rect(0, height, size, size), cam.targetTexture);

        if (GUILayout.Button("Save")){
            textureName = string.IsNullOrEmpty(textureName)? DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") : textureName;
            generateTexture();
        }       
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

        cam.targetTexture = new RenderTexture(
            size,
            size, 
            24,
            RenderTextureFormat.ARGB32
        );
        cam.Render();

        Texture2D result = toTexture2D(cam.targetTexture);
        File.WriteAllBytes(path, result.EncodeToPNG());
        UnityEditor.AssetDatabase.Refresh();
    }

    Texture2D toTexture2D(RenderTexture rTex){
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    void OnInspectorUpdate(){
        Repaint();
    }
 
    void OnDestroy(){
        Resources.UnloadUnusedAssets();
        DestroyImmediate(gameObject);
    }
}