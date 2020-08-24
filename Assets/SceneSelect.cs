using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelect : MonoBehaviour
{
    public string[] sceneNames;
    int sceneSelection = -1;

    void OnGUI(){
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

        sceneSelection = GUILayout.SelectionGrid(sceneSelection, sceneNames, 1);
        if(sceneSelection != -1){
            SceneManager.LoadScene(sceneNames[sceneSelection]);
        }

        GUILayout.EndArea ();

    }
}
