using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneSelect : MonoBehaviour
{
    [System.Serializable]
    public struct SceneNamePair {
        public SceneNamePair(string uiName, string buildName){
            this.uiName = uiName;
            this.buildName =  buildName;
        }
        
        public string uiName;
        public string buildName;

    }
    public List<SceneNamePair> sceneNames = new List<SceneNamePair>();

    private string[] uiNames;
    int sceneSelection = -1;
    GUIStyle header;

    void Start(){
        uiNames = sceneNames.Select(v => v.uiName).ToArray();
        header = new GUIStyle();
        header.fontStyle = FontStyle.Bold;
        header.fontSize = 16;
        header.normal.textColor = Color.white;
        header.alignment = TextAnchor.MiddleCenter;
    }

    void OnGUI(){
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.FlexibleSpace();

        GUILayout.Label("Select Mode", header);
        GUILayout.Label("");
        sceneSelection = GUILayout.SelectionGrid(sceneSelection, uiNames, uiNames.Length, GUILayout.MinHeight(200));
        if(sceneSelection != -1){
            SceneManager.LoadScene(sceneNames[sceneSelection].buildName);
        }
        GUILayout.FlexibleSpace();

        GUILayout.EndArea ();

    }
}
