using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityTemplateProjects;
using UnityEngine.VFX;
using UnityEngine.Animations;

public class UIManager : MonoBehaviour
{
    //todo: clean up, create a process class and a dictionary or array to hold them
    Process ffplay;
    Process ffmpeg;

    public Config.Mode mode = Config.Mode.fixedView;
    public GameObject Bounds;
    public Camera PreviewCamera;
    public SimpleCameraController CameraPivot;
    public VisualEffect PointCloudVFXGraph;
    public PositionConstraint VFXPivot;

    public List<OrthographicCameraController> orthographicCameras;

    public List<VisualEffectAsset> VFXGraphs = new List<VisualEffectAsset>();
    public RenderTexture Output;
    public RenderTextureBroadcaster outputTextureBroadcaster;

    public Config.TextureSettings defaultOutputSettings;

    bool streaming;
    bool previewing;

    bool editingMask;
    bool editingPointcloud;

    string[] vfxNames;

    string[] cameraNames;
    int currentCamera = -1;
    GUIStyle header;
    GUIStyle description;

    void Start()
    {
        vfxNames = VFXGraphs.Select(v => v.name).ToArray();
        cameraNames = orthographicCameras.Select(v => v.name).ToArray();
        VFXPivot.constraintActive = true;

        header = new GUIStyle();
        header.fontStyle = FontStyle.Bold;
        header.fontSize = 16;
        header.normal.textColor = Color.white;

        description = new GUIStyle();
        description.fontStyle = FontStyle.Normal;
        description.fontSize = 14;
        description.normal.textColor = Color.grey;
        description.wordWrap = true;
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
        int initialWidth = config.output.width;
        int initialHeight = config.output.height;
        if((config.output.height == 0 && config.output.width == 0) || config.mode != mode){
            config.output.width = defaultOutputSettings.width;
            config.output.height = defaultOutputSettings.height;
        }
        GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
        GUILayout.Label("Output Width:", GUILayout.Width(50));
        int.TryParse(GUILayout.TextField(config.output.width.ToString()), out config.output.width);
        GUILayout.Label("Output Height", GUILayout.Width(50));
        int.TryParse(GUILayout.TextField(config.output.height.ToString()), out config.output.height);
        GUILayout.EndHorizontal();
        if(initialWidth != config.output.width || initialHeight != config.output.height){
            outputTextureBroadcaster.Resize(config.output.width, config.output.height);
        }

        int vfxSelection = GUILayout.SelectionGrid(config.vfxSelection, vfxNames, 3);
        if(config.vfxSelection != vfxSelection){
            PointCloudVFXGraph.visualEffectAsset = VFXGraphs[vfxSelection];
            config.vfxSelection = vfxSelection;
        }
        if(GUILayout.Button("Main Camera") && currentCamera != -1){
            currentCamera = -1;
            CameraPivot.enabled = true;
            PreviewCamera.gameObject.SetActive(true);
        }
        int cameraSelection = GUILayout.SelectionGrid(currentCamera, cameraNames, 2);
        if(currentCamera != cameraSelection){
            if(currentCamera == -1){
                CameraPivot.enabled = false;
                PreviewCamera.gameObject.SetActive(false);
            } else {
                orthographicCameras[currentCamera].gameObject.SetActive(false);
            }
            currentCamera = cameraSelection;
            orthographicCameras[currentCamera].gameObject.SetActive(true);
            orthographicCameras[currentCamera].Reset();
        }
        
        editingMask = GUILayout.Toggle(editingMask, "Edit box mask bounds");
        if(editingMask){
            //postion
            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Left:", GUILayout.Width(50));
            config.mask.pos_x = GUILayout.HorizontalScrollbar(config.mask.pos_x, 1.0f, -3.0f, 3.0f, GUILayout.MinWidth(200));
            GUILayout.Label("Right", GUILayout.Width(50));
            float.TryParse(GUILayout.TextField(config.mask.pos_x.ToString()), out config.mask.pos_x);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Up", GUILayout.Width(50));
            config.mask.pos_y = GUILayout.HorizontalScrollbar(config.mask.pos_y, 1.0f, -2.0f, 4.0f, GUILayout.MinWidth(200));
            GUILayout.Label("Down", GUILayout.Width(50));
            float.TryParse(GUILayout.TextField(config.mask.pos_y.ToString()), out config.mask.pos_y);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Close", GUILayout.Width(50));
            config.mask.pos_z = GUILayout.HorizontalScrollbar(config.mask.pos_z, 1.0f, 0.0f, 4.0f, GUILayout.MinWidth(200));
            GUILayout.Label("Far", GUILayout.Width(50));
            float.TryParse(GUILayout.TextField(config.mask.pos_z.ToString()), out config.mask.pos_z);
            GUILayout.EndHorizontal();

            
            //rotation
            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Pitch:", GUILayout.Width(50));
            config.mask.rot_x = GUILayout.HorizontalScrollbar(config.mask.rot_x, 1.0f, 0, 180f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.mask.rot_x.ToString()), out config.mask.rot_x);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Roll", GUILayout.Width(50));
            config.mask.rot_y = GUILayout.HorizontalScrollbar(config.mask.rot_y, 1.0f, 0, 180f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.mask.rot_y.ToString()), out config.mask.rot_y);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Yaw", GUILayout.Width(50));
            config.mask.rot_z = GUILayout.HorizontalScrollbar(config.mask.rot_z, 1.0f, 0, 180f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.mask.rot_z.ToString()), out config.mask.rot_z);
            GUILayout.EndHorizontal();

            //scale
            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Width:", GUILayout.Width(50));
            config.mask.scale_x = GUILayout.HorizontalScrollbar(config.mask.scale_x, 1.0f, 1.0f, 4.0f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.mask.scale_x.ToString()), out config.mask.scale_x);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Height", GUILayout.Width(50));
            config.mask.scale_y = GUILayout.HorizontalScrollbar(config.mask.scale_y, 1.0f, 1.0f, 4.0f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.mask.scale_y.ToString()), out config.mask.scale_y);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Depth", GUILayout.Width(50));
            config.mask.scale_z = GUILayout.HorizontalScrollbar(config.mask.scale_z, 1.0f, 1.0f, 4.0f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.mask.scale_z.ToString()), out config.mask.scale_z);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Reset Bounds", GUILayout.Width(120)))
            {
                config.mask.pos_x = config.mask.pos_y = 0.0f;
                config.mask.pos_z = 0;
                config.mask.rot_x = config.mask.rot_y= config.mask.rot_z= 0.0f;
                config.mask.scale_x = config.mask.scale_y = config.mask.scale_z = 4.0f;
            }
        }

        Bounds.transform.localScale = new Vector3(config.mask.scale_x, config.mask.scale_y, config.mask.scale_z);

        Bounds.transform.position = new Vector3(config.mask.pos_x, config.mask.pos_y, config.mask.scale_z/2.0f + config.mask.pos_z);

        Bounds.transform.rotation = Quaternion.Euler(config.mask.rot_x, config.mask.rot_y, config.mask.rot_z);
        
        editingPointcloud = GUILayout.Toggle(editingPointcloud, "Edit pointcloud transform");

        Vector3 startRotation = Vector3.zero;
        bool pointcloudRotationChanged = false;
        if(editingPointcloud){
            startRotation = new Vector3(config.pointcloud.rot_x, config.pointcloud.rot_y, config.pointcloud.rot_z);
            //rotation
            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Pitch:", GUILayout.Width(50));
            config.pointcloud.rot_x = GUILayout.HorizontalScrollbar(config.pointcloud.rot_x, 1.0f, 0, 360f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.pointcloud.rot_x.ToString()), out config.pointcloud.pos_z);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Roll", GUILayout.Width(50));
            config.pointcloud.rot_y = GUILayout.HorizontalScrollbar(config.pointcloud.rot_y, 1.0f, 0, 360f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.pointcloud.rot_y.ToString()), out config.pointcloud.pos_z);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box", GUILayout.Width(300));
            GUILayout.Label("Yaw", GUILayout.Width(50));
            config.pointcloud.rot_z = GUILayout.HorizontalScrollbar(config.pointcloud.rot_z, 1.0f, 0, 360f, GUILayout.MinWidth(200));
            float.TryParse(GUILayout.TextField(config.pointcloud.rot_z.ToString()), out config.pointcloud.pos_z);
            GUILayout.EndHorizontal();
            Vector3 endRotation = new Vector3(config.pointcloud.rot_x, config.pointcloud.rot_y, config.pointcloud.rot_z);
            pointcloudRotationChanged = startRotation != endRotation;
        } 

        VFXPivot.constraintActive = pointcloudRotationChanged;
        VFXPivot.transform.rotation = Quaternion.Euler(config.pointcloud.rot_x, config.pointcloud.rot_y, config.pointcloud.rot_z);


        if (GUILayout.Button("Reset Camera", GUILayout.Width(120)))
        {
            CameraPivot.Reset();
        }

        
        GUILayout.BeginArea(new Rect(Screen.width - 320, 20, 300, 500));
        GUILayout.Box(Output,GUIStyle.none, GUILayout.MaxWidth(300), GUILayout.MaxHeight(500));
        GUILayout.EndArea ();


        GUILayout.BeginArea(new Rect(Screen.width - 300, Screen.height - 100, 300, 100));
        GUILayout.Label(
            currentCamera == -1 ? 
            "Use W and S to move the camera forward and back, A and D to move left and right. Hold ctrl, left click, and move mouse to rotate view." 
            : "Pan camera with WASD keys or by holding middle mouse and moving mouse. Scroll to zoom in and out.", description, GUILayout.MaxWidth(250));
        GUILayout.EndArea ();
        PointCloudVFXGraph.SetInt("ShowUnmasked", editingPointcloud || editingMask ? 1 : 0);

        config.mode = mode;

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
