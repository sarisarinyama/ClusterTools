using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureCounter : EditorWindow
{
    [MenuItem("Window/Texture Counter")]
    public static void Init()
    {
        var window = EditorWindow.GetWindow<TextureCounter>("Texture Counter");
        DontDestroyOnLoad(window);
        }

    private void OnGUI()
    {
//        using (var verticalArea = new EditorGUILayout.VerticalScope())
//        {
//            GUILayout.Label("These");
//            GUILayout.Label("Labels");
//            GUILayout.Label("Will be shown");
//            GUILayout.Label("On top of each other");
//        }
//        
//        using (var verticalArea = new EditorGUILayout.VerticalScope())
//        {
//            var buttonClicked = GUILayout.Button("Click me!");
//            if (buttonClicked)
//            {
//                Debug.Log("The custom window's " + "button was clicked!");
//            }
//        }
//        
//        GUI.Label(
//        new Rect(50,50,100,20),
//        "This is a Label!");

        using (var verticalArea = new EditorGUILayout.VerticalScope())
        {
            var paths = AssetDatabase.FindAssets("t:texture");
            var count = paths.Length;
            EditorGUILayout.LabelField("Texture COunt",count.ToString());
        }
    }
}
    
    
   
