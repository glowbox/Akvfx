using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityTemplateProjects;

public class UIManager : MonoBehaviour
{
    //todo: clean up, create a process class and a dictionary or array to hold them
    Process ffplay;
    Process ffmpeg;

    public GameObject Bounds;
    public SimpleCameraController CameraPivot;
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
        //load
        Config.AppConfig config = Config.CurrentAppConfig;

        if (GUILayout.Button("Preview Stream"))
        {
            PreviewStream();
        }

        config.rtmp_path = GUILayout.TextField(config.rtmp_path.Length < 1 ? "rtmp url" : config.rtmp_path, 255);
        config.local_mediaserver_path = Config.CurrentAppConfig.local_mediaserver_path;
        config.ffmpeg_path = Config.CurrentAppConfig.ffmpeg_path;
        
        if (GUILayout.Button("Start Stream"))
        {
            StartStream();
        }

        //postion
        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Left:", GUILayout.Width(50));
        config.pos_x = GUILayout.HorizontalScrollbar(config.pos_x, 1.0f, -3.0f, 3.0f);
        GUILayout.Label("Right", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Up", GUILayout.Width(50));
        config.pos_y = GUILayout.HorizontalScrollbar(config.pos_y, 1.0f, -2.0f, 4.0f);
        GUILayout.Label("Down", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Close", GUILayout.Width(50));
        config.pos_z = GUILayout.HorizontalScrollbar(config.pos_z, 1.0f, 0.0f, 4.0f);
        GUILayout.Label("Far", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        
        //rotation
        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Pitch:", GUILayout.Width(50));
        config.rot_x = GUILayout.HorizontalScrollbar(config.rot_x, 1.0f, 0, 180f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Roll", GUILayout.Width(50));
        config.rot_y = GUILayout.HorizontalScrollbar(config.rot_y, 1.0f, 0, 180f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Yaw", GUILayout.Width(50));
        config.rot_z = GUILayout.HorizontalScrollbar(config.rot_z, 1.0f, 0, 180f);
        GUILayout.EndHorizontal();

        //scale
        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Width:", GUILayout.Width(50));
        config.scale_x = GUILayout.HorizontalScrollbar(config.scale_x, 1.0f, 1.0f, 4.0f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Height", GUILayout.Width(50));
        config.scale_y = GUILayout.HorizontalScrollbar(config.scale_y, 1.0f, 1.0f, 4.0f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Depth", GUILayout.Width(50));
        config.scale_z = GUILayout.HorizontalScrollbar(config.scale_z, 1.0f, 1.0f, 4.0f);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Bounds", GUILayout.Width(120)))
        {
            config.pos_x = config.pos_y = 0; config.pos_z = 2.0f;
            config.rot_x = config.rot_y= config.rot_z= 0.0f;
            config.scale_x = config.scale_y = config.scale_z = 4.0f;
        }


        if (GUILayout.Button("Reset Camera", GUILayout.Width(120)))
        {
            CameraPivot.Reset();
        }

        Bounds.transform.localScale = new Vector3(config.scale_x, config.scale_y, config.scale_z);

        Bounds.transform.position = new Vector3(config.pos_x, config.pos_y, config.pos_z);

        Bounds.transform.rotation = Quaternion.Euler(config.rot_x, config.rot_y, config.rot_z);


        //save changes
        Config.CurrentAppConfig = config;
    }

    void StartStream()
    {
        if (ffmpeg != null)
        {           
            ffmpeg.Kill();
            ffmpeg = null;
        }
        else
        {
            ffmpeg = new Process();
            ffmpeg.StartInfo.FileName = Path.Combine(Config.CurrentAppConfig.ffmpeg_path, "ffmpeg.exe");
            ffmpeg.StartInfo.Arguments = $"-f dshow -video_size 640x960 -i video=\"Unity Video Capture\" -c:v libx264 -preset veryfast -b:v 1984k -maxrate 1984k -bufsize 3968k -vf \"format = yuv420p\" -g 60 -f flv {Config.CurrentAppConfig.rtmp_path}";
            ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

            ffmpeg.StartInfo.UseShellExecute = true;
        

            ffmpeg.Start();
        }

    }
    void PreviewStream()
    {
        if( ffplay != null)
        {       
            ffplay.Kill();
            ffplay = null;
        }
        else
        {
            ffplay = new Process();
            ffplay.StartInfo.FileName = Path.Combine(Config.CurrentAppConfig.ffmpeg_path, "ffplay.exe");
            ffplay.StartInfo.Arguments = "-f dshow -video_size 640x960 -vf \"format = yuv420p\" -i video=\"Unity Video Capture\"";
            ffplay.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

            ffplay.StartInfo.UseShellExecute = true;
         
            ffplay.Start();
            
        }

    }


    void OnApplicationQuit()
    {
        if (ffplay != null && !ffplay.HasExited )
        {
            ffplay.Kill();
        }
        if (ffmpeg != null && !ffmpeg.HasExited)
        {
            ffmpeg.Kill();
        }
    }
}
