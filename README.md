Volumetric Performance Toolbox
=====

> How can artists create new live performances during the time of COVID-19? Volumetric Performance Toolbox empowers creators to perform from their own living spaces for a virtual audience. Movement artist Valencia James performed publicly with the Toolbox for the first time on September 24, 2020 in the Mozilla Hubs virtual social space. The project is a collaboration with Valencia James as part of Eyebeam’s Rapid Response for a Better Digital Future fellowship.

More information on the Volumetric Performance Toolbox here:

https://valenciajames.com/volumetric-performance/
https://www.glowbox.io/work/volumetric-performance/

This is a fork of the AKFVX project, heavily modified to stream either a RGB+D texture to ffmpeg -or- a pointcloud seen to Spout.

# **AKVFX - Stream**

![gif](Documentation/masking.gif)

![gif](Documentation/orientation.gif)

This GitHub repository contains middleware for generating perspective-correct combined depth and color maps from the Azure Kinect that can be easily streamed with RTMP. Used in conjunction with [our fork of DepthKit.js](https://github.com/glowbox/Depthkit.js), artists and developers can stream volumetric video to render on the web with three.js.

Glowbox made this tool from a fork of Keijiro’s [AKVFX ](https://github.com/keijiro/Akvfx)(Azure Kinect plugin for Unity VFX Graph) which exposes color and world position maps from the Azure Kinect to Unity’s VFX graph. We use Unity's VFX graph in their HD Render Pipeline to post process data for streaming using Spout and FFmpeg. These allow us to encode Spout Textures and RTMP streams for things like Zoom calls, volumetric performances, and integration in other tools like OBS.

## **Overview**
---

This Unity application connects an Azure Kinect RGB and Depth stream and visualizes this as a pointcloud. It outputs the visual of the point cloud to a virtual webcam (SpoutCam). It can also instruct ffmpeg to stream the virtaul webcam output to an RTMP end point like Twitch.

#### **System Requirements**

To Run:

* [Windows 10](https://www.microsoft.com/en-us/software-download/windows10)

* [Azure Kinect DK](https://azure.microsoft.com/en-us/services/kinect-dk/)

* [Azure Kinect SDK](https://docs.microsoft.com/en-us/azure/kinect-dk/sensor-sdk-download)

* [SpoutCam](https://github.com/leadedge/SpoutCam/)

* [FFmpeg](https://ffmpeg.org/download.html#build-windows)

To build:

* [Unity 2020.1.1f1](https://unity.com/releases/2020-1)

* [High Definition Render Pipeline 8.2.0](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@8.2/manual/index.html)

* [Visual Effect Graph 8.2.0](https://docs.unity3d.com/Packages/com.unity.visualeffectgraph@8.2/manual/index.html)


#### **Installation**

* Download and install the [Azure Kinect SDK](https://docs.microsoft.com/en-us/azure/kinect-dk/sensor-sdk-download) 
* Test your Azure Kinect device by running the [Azure Kinect Viewer](https://docs.microsoft.com/en-us/azure/kinect-dk/azure-kinect-viewer) 
* Download [SpoutCam] (https://github.com/leadedge/SpoutCam/releases)
* Register SpoutCam
* Download [ffmpeg](https://www.gyan.dev/ffmpeg/builds/ffmpeg-git-essentials.7z)
* Unzip ffmpeg and copy it into a dedicated folder. For example: Documents\ffmpeg

##### **Verify Installation**

* Open a Powershell or Command line and execute the following:
```ffmpeg -list_devices true -f dshow -i dummy```

* Make sure "Azure Kinect 4K Camera" and "SpoutCam" is in the list of results. 
```
[dshow @ 0000026bafa6dd80] DirectShow video devices (some may be both video and audio devices)
[dshow @ 0000026bafa6dd80]  "Azure Kinect 4K Camera"
[dshow @ 0000026bafa6dd80]     Alternative name "@device_pnp_\\?\usb#vid_045e&pid_097d&mi_00#7&39a1c4d2&1&0000#{65e8773d-8f56-11d0-a3b9-00a0c9223196}\global"
[dshow @ 0000026bafa6dd80]  "SpoutCam"
[dshow @ 0000026bafa6dd80]     Alternative name "@device_sw_{860BB310-5D01-11D0-BD3B-00A0C911CE86}\{8E14549A-DB61-4309-AFA1-3578E927E933}"
[dshow @ 0000026bafa6dd80] DirectShow audio devices
[dshow @ 0000026bafa6dd80]  "Microphone Array (4- Azure Kinect Microphone Array)"
[dshow @ 0000026bafa6dd80]     Alternative name "@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{28A95263-5320-4C08-BC56-6A4552FDF3B7}"
```

* If the system can'f find ffmpeg, add it to the path or change your working director to the folder ffpmeg is installed in.

##### Adding ffmpeg to the system path (optional):
* In the start menu search: Edit the system environment variables
* Choose "Environment Variables"
* Choose "Path"
* Add the folder that contains ffmpeg.exe and ffplay.exe

## **Quick Start**
---

### **Mode Select**

There are two modes available:

* **Orthographic (RGB+D)** mode by default creates a 640x960 combined orthographic depth (encoded in hue) and color map. This is designed to be easy to load into a three.js scene using our [corresponding fork](https://github.com/volumetricperformance/Depthkit.js) of Depthkit.js for hosting volumetric streams.
* **Perspective (RGB)** mode by default creates a 1920x1080 color image. This affords fast input into creative tools like Open Broadcast Studio, MadMapper or TouchDesigner.

### **Calibrate the Image**

Once you’ve chosen a mode for outputting your data, calibrate your image by clipping out extraneous features with a bounding box and correcting the orientation of your pointcloud.

### **Set the Bounding Box**

![gif](Documentation/masking.gif)

* Check “**edit box mask bounds**.”
* Adjust transform sliders to fit the performance area within bounds.
* Uncheck “edit box mask bounds.”

### **Set the Pointcloud Orientation**

![gif](Documentation/orientation.gif)

* Identify a flat surface.
* Check “**edit pointcloud transform**.”
* Switch to an orthographic editor view by clicking on a directional button in the menu for clearer overview of the pointcloud.
* Adjust orientation sliders to align pointcloud with the flat surface.
* Uncheck “edit pointcloud transform.”
* Click Main Camera or Reset Camera to return to Main Camera.

### **Select a Visual Effect for Rendering the Scene**

There are three different VFX Graphs usable in the project: Point_Masked, Pixel_Masked and Spike_Masked:

* **Point_Masked** draws points at a constant scale.
* **Pixel_Masked** scales point deeper in space to compensate for the decrease of their density in depth.
* **Spike_Masked** fills holes between points even at extreme angles. It’s the slowest of the three methods.

Different modes have different effects available: Perspective (RGB) uses Point_Masked, and Spike_Masked, Orthographic (RGB+D) uses Point_Masked and Pixel_Masked, due to respective differences in the camera formats.

Read more about these effects in more detail here: ___vfx.md___

### **Configure Streaming**

To configure this tool to stream to three.js using our fork of Depthkit.js:

* Ensure that you’re using the Orthographic (RGB+D) mode.
* Set the RTMP endpoint beneath “Preview Stream.”
* Ensure that your output resolution matches your intended receiver by adjusting Output Width and Output Height.
* Press start stream!

For more information on the three.js implementation see our fork of Depthkit.js here: [https://github.com/glowbox/Depthkit.js](https://github.com/glowbox/Depthkit.js)
