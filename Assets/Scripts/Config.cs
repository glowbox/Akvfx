using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Config
{
 
    [System.Serializable]
    public enum Mode { fixedView, freeformView };
    public static string app_config = "config.json";
    [System.Serializable]
    public struct Transformation {
        public float pos_x;
        public float pos_y;
        public float pos_z;

        public float rot_x;
        public float rot_y;
        public float rot_z;

        public float scale_x;
        public float scale_y;
        public float scale_z;
    }

    [System.Serializable]
    public struct TextureSettings {
        public int width;
        public int height;
    }

    [System.Serializable]
    public struct AppConfig
    {
        public string rtmp_path;
        public string ffmpeg_path;
        public string ffmpeg_args;
        public string ffplay_args;

        public Transformation mask;

        public Transformation pointcloud;

        public TextureSettings output;

        public Mode mode;
        public int vfxSelection;

        [System.NonSerialized]
        public bool loaded;

        public static bool operator ==(AppConfig c1, AppConfig c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(AppConfig c1, AppConfig c2)
        {
            return !c1.Equals(c2);
        }
        public static AppConfig DefaultConfig()
        {
            return new AppConfig
            {
                rtmp_path = String.Empty,
                ffmpeg_path = AppConfig.ffmpegPath(),
                ffmpeg_args = "-f dshow -video_size 640x960 -rtbufsize 10M -i video=\"SpoutCam\" -c:v libx264 -preset veryfast -b:v 1984k -c:a aac -b:a 160k -ar 44100 -maxrate 1984k -bufsize 3968k -vf \"format = yuv420p\" -g 60 -f flv",
                ffplay_args = "-f dshow -video_size 640x960 -vf \"format = yuv420p\" -i video=\"SpoutCam\""
            };
        }

        public void Save(string path)
        {
            try
            {
                string json = JsonUtility.ToJson(this, true);
                System.IO.File.WriteAllText(path, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving App Config:{app_config} {e}");
            }
        }

        public static string ffmpegPath()
        {
            var enviromentPath = System.Environment.GetEnvironmentVariable("PATH");

            var paths = enviromentPath.Split(';');
            var exePath = paths.Select(x => Path.Combine(x, "ffmpeg.exe"))
                               .Where(x => File.Exists(x))
                               .FirstOrDefault();
            
            return Path.GetDirectoryName(exePath);
        }
    }

    private static AppConfig _CurrentAppConfig;
    public static AppConfig CurrentAppConfig
    {
        get
        {
            if (!_CurrentAppConfig.loaded)
            {

                if (System.IO.File.Exists(app_config))
                {
                    try
                    {
                        string json = System.IO.File.ReadAllText(app_config);
                        _CurrentAppConfig = JsonUtility.FromJson<AppConfig>(json);

                        string ffmpegLocation = Path.Combine(_CurrentAppConfig.ffmpeg_path, "ffmpeg.exe");

                        if (!File.Exists(ffmpegLocation))
                        {
                            _CurrentAppConfig.ffmpeg_path = AppConfig.ffmpegPath();
                        }

                        _CurrentAppConfig.loaded = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error loading App Config {app_config} {e}");
                    }
                }
                else
                {
                    _CurrentAppConfig = AppConfig.DefaultConfig();
                    _CurrentAppConfig.Save(app_config);
                    _CurrentAppConfig.loaded = true;
                }
            }
            return _CurrentAppConfig;
        }

        set
        {
            if(_CurrentAppConfig != value)
            {
                _CurrentAppConfig = value;
                _CurrentAppConfig.Save(app_config);
            }        
        }
    }

}
