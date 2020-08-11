using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    Process ffplay;
    Process ffmpg;

    StreamWriter messageStream;

    private void Awake()
    {
       

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (GUILayout.Button("Raw Stream"))
        {
            PreviewStream();
        }
    }

    void PreviewStream()
    {
        if( ffplay != null)
        {
            ffplay.OutputDataReceived -= new DataReceivedEventHandler(DataReceived);
            ffplay.ErrorDataReceived -= new DataReceivedEventHandler(ErrorReceived);
            ffplay.Kill();
            ffplay = null;
        }
        else
        {
            ffplay = new Process();
            ffplay.StartInfo.FileName = Path.Combine(Application.dataPath, "..", "ffplay.exe");
            ffplay.StartInfo.Arguments = "-f dshow -video_size 640x960 -vf \"format = yuv420p\" -i video=\"Unity Video Capture\"";
            //ffplay.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

            ffplay.StartInfo.UseShellExecute = false;
            ffplay.StartInfo.RedirectStandardOutput = true;
            ffplay.StartInfo.RedirectStandardInput = true;
            ffplay.StartInfo.RedirectStandardError = true;
            ffplay.OutputDataReceived += new DataReceivedEventHandler(DataReceived);
            ffplay.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);

            ffplay.Start();
            ffplay.BeginOutputReadLine();
            messageStream = ffplay.StandardInput;
        }

    }

    void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        // Handle it
        UnityEngine.Debug.LogError(eventArgs.Data);
    }


    void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.LogError(eventArgs.Data);
    }


    void OnApplicationQuit()
    {
        if (ffplay != null && !ffplay.HasExited )
        {
            ffplay.Kill();
        }
    }
}
