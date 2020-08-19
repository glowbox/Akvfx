using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class RenderTextureEvent : UnityEvent<RenderTexture>
{
}
public class RenderTextureBroadcaster : MonoBehaviour
{
    public RenderTexture texture;

    [Header("Subscribers")]
    public List<Camera> cameraTargetTextures;
    public List<UIManager> uIManagerOutputs;
    public List<UnityCapture> unityCaptureSources;

    public void UpdateTexture(RenderTexture texture){
        texture.Release();
        this.texture = texture;
        foreach(Camera camera in cameraTargetTextures){
            camera.targetTexture = texture;
        }
        foreach(UIManager uIManager in uIManagerOutputs){
            uIManager.Output = texture;
        }
        foreach(UnityCapture unityCapture in unityCaptureSources){
            unityCapture.source = texture;
        }
    }

    public void Resize(int width, int height){
        RenderTexture newTexture = new RenderTexture(texture);
        if(width == 0 || height == 0){
            print("width and height must not be zero");
            return;
        }
        newTexture.width = width;
        newTexture.height = height;
        UpdateTexture(newTexture);
    }
}
