using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config
{
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
    public struct AppConfig
    {
        public string rtmp_path;
        public string ffmpeg_path;
        public string local_mediaserver_path;

        public Transformation mask;

        public Transformation pointcloud;


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
                        _CurrentAppConfig.loaded = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error loading App Config {app_config} {e}");
                    }
                }
                else
                {
                    _CurrentAppConfig = new AppConfig();
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
