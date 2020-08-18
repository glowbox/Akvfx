using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityTemplateProjects;
using UnityEngine.VFX;

public class UIManager : MonoBehaviour
{
    //todo: clean up, create a process class and a dictionary or array to hold them
    Process ffplay;
    Process ffmpeg;

    public GameObject Bounds;
    public SimpleCameraController CameraPivot;
    public VisualEffect PointCloudVFXGraph;

    bool streaming;
    bool previewing;
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

        if (previewing)
        {
            if (GUILayout.Button("Stop Preview"))
            {
                PreviewStream(false);
            }
        }
        else
        {
            if (GUILayout.Button("Preview Stream"))
            {
                PreviewStream(true);
            }
        }
        

        config.rtmp_path = GUILayout.TextField(config.rtmp_path.Length < 1 ? "rtmp url" : config.rtmp_path, 255);
        config.local_mediaserver_path = Config.CurrentAppConfig.local_mediaserver_path;
        config.ffmpeg_path = Config.CurrentAppConfig.ffmpeg_path;


        if (streaming)
        {
            if (GUILayout.Button("Stop Stream"))
            {
                ToggleStream(false);
            }
        }
        else
        {
            if (GUILayout.Button("Start Stream"))
            {
                ToggleStream(true);
            }

        }

        //postion
        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Left:", GUILayout.Width(50));
        config.mask.pos_x = GUILayout.HorizontalScrollbar(config.mask.pos_x, 1.0f, -3.0f, 3.0f);
        GUILayout.Label("Right", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Up", GUILayout.Width(50));
        config.mask.pos_y = GUILayout.HorizontalScrollbar(config.mask.pos_y, 1.0f, -2.0f, 4.0f);
        GUILayout.Label("Down", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Close", GUILayout.Width(50));
        config.mask.pos_z = GUILayout.HorizontalScrollbar(config.mask.pos_z, 1.0f, 0.0f, 4.0f);
        GUILayout.Label("Far", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        
        //rotation
        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Pitch:", GUILayout.Width(50));
        config.mask.rot_x = GUILayout.HorizontalScrollbar(config.mask.rot_x, 1.0f, 0, 180f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Roll", GUILayout.Width(50));
        config.mask.rot_y = GUILayout.HorizontalScrollbar(config.mask.rot_y, 1.0f, 0, 180f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Yaw", GUILayout.Width(50));
        config.mask.rot_z = GUILayout.HorizontalScrollbar(config.mask.rot_z, 1.0f, 0, 180f);
        GUILayout.EndHorizontal();

        //scale
        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Width:", GUILayout.Width(50));
        config.mask.scale_x = GUILayout.HorizontalScrollbar(config.mask.scale_x, 1.0f, 1.0f, 4.0f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Height", GUILayout.Width(50));
        config.mask.scale_y = GUILayout.HorizontalScrollbar(config.mask.scale_y, 1.0f, 1.0f, 4.0f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Depth", GUILayout.Width(50));
        config.mask.scale_z = GUILayout.HorizontalScrollbar(config.mask.scale_z, 1.0f, 1.0f, 4.0f);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Bounds", GUILayout.Width(120)))
        {
            config.mask.pos_x = config.mask.pos_y = 0.0f;
            config.mask.pos_z = 0;
            config.mask.rot_x = config.mask.rot_y= config.mask.rot_z= 0.0f;
            config.mask.scale_x = config.mask.scale_y = config.mask.scale_z = 4.0f;
        }


        if (GUILayout.Button("Reset Camera", GUILayout.Width(120)))
        {
            CameraPivot.Reset();
        }

        Bounds.transform.localScale = new Vector3(config.mask.scale_x, config.mask.scale_y, config.mask.scale_z);

        Bounds.transform.position = new Vector3(config.mask.pos_x, config.mask.pos_y, config.mask.scale_z/2.0f + config.mask.pos_z);

        Bounds.transform.rotation = Quaternion.Euler(config.mask.rot_x, config.mask.rot_y, config.mask.rot_z);


        //rotation
        GUILayout.BeginHorizontal("Pointcloud", GUILayout.Width(300));
        GUILayout.Label("Pitch:", GUILayout.Width(50));
        config.pointcloud.rot_x = GUILayout.HorizontalScrollbar(config.pointcloud.rot_x, 1.0f, 0, 360f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Pointcloud", GUILayout.Width(300));
        GUILayout.Label("Roll", GUILayout.Width(50));
        config.pointcloud.rot_y = GUILayout.HorizontalScrollbar(config.pointcloud.rot_y, 1.0f, 0, 360f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("Pointcloud", GUILayout.Width(300));
        GUILayout.Label("Yaw", GUILayout.Width(50));
        config.pointcloud.rot_z = GUILayout.HorizontalScrollbar(config.pointcloud.rot_z, 1.0f, 0, 360f);
        GUILayout.EndHorizontal();

        PointCloudVFXGraph.transform.rotation = Quaternion.Euler(config.pointcloud.rot_x, config.pointcloud.rot_y, config.pointcloud.rot_z);


        //save changes
        Config.CurrentAppConfig = config;
    }

    void ToggleStream(bool stream)
    {

        if(stream)
        {
            if( ffmpeg == null)
            {
                ffmpeg = new Process();
                ffmpeg.StartInfo.FileName = Path.Combine(Config.CurrentAppConfig.ffmpeg_path, "ffmpeg.exe");
                ffmpeg.StartInfo.Arguments = $"-f dshow -video_size 640x960 -i video=\"Unity Video Capture\" -c:v libx264 -preset veryfast -b:v 1984k -maxrate 1984k -bufsize 3968k -vf \"format = yuv420p\" -g 60 -f flv {Config.CurrentAppConfig.rtmp_path}";
                ffmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

                ffmpeg.StartInfo.UseShellExecute = true;


                ffmpeg.Start();
                streaming = true;
            }

        }
        else
        {
            if (ffmpeg != null)
            {
                ffmpeg.Kill();
                ffmpeg = null;
            }
            streaming = false;
        }
    }

    void PreviewStream( bool show)
    {

        if(show)
        {
            if (ffplay == null)
            {
                ffplay = new Process();
                ffplay.StartInfo.FileName = Path.Combine(Config.CurrentAppConfig.ffmpeg_path, "ffplay.exe");
                ffplay.StartInfo.Arguments = "-f dshow -video_size 640x960 -vf \"format = yuv420p\" -i video=\"Unity Video Capture\"";
                ffplay.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

                ffplay.StartInfo.UseShellExecute = true;

                ffplay.Start();
                previewing = true;
            }
        }
        else
        {
            if( ffplay != null)
            {
                ffplay.Kill();
                ffplay = null;
            }
            previewing = false;
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
