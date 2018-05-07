GarageKit for Unity
===================
You can start the project immediately in Unity using GarageKit.

![](http://media.tumblr.com/1ad50e485e709219816e2cc0057d65b1/tumblr_inline_ndw8fqGnIb1qzb8ql.png)

## What is it?

GaraegKit is framework that uses the Unity C#. I provide the template scene, some manager scripts, state scripts, utility scripts and directory structure.

http://debuglog.tumblr.com/post/100742681354/garagekit-for-unity

## How to use?

1. open Main.unity
2. and press the play.
3. and customize your project.

you can add or delete the `SceneState` class using the `StateGenerator` of the editor script.

`Menu -> EditorScript -> StateGenerator`

## Require Modules

/Editor

- Editor enhancements  
http://forum.unity3d.com/threads/free-editor-enhancements-extensions-in-project-window-components-in-heirarchy.149569/
https://bitbucket.org/Tenebrous/unityeditorenhancements/commits/all

/Plugins/Log4Net

- log4net  
http://logging.apache.org/log4net/

/Scripts/Utils

- DebugConsole  
https://gist.github.com/darktable/1412228

- iTween  
http://itween.pixelplacement.com/index.php

- JPGEncoder  
https://code.google.com/p/unity-jpeg-encoder/source/browse/tags/Version1/UnityProject/Assets/JPGEncoder.cs?spec=svn21&r=21

- TouchScript  
https://www.assetstore.unity3d.com/jp/#!/content/7394

- MiniJson  
https://gist.github.com/darktable/1411710

## Utility Scripts

/Editor

- AssetPostprocessUTF8Encode.cs
- CreateCuntomPlane.cs
- StateGenerator.cs

/Scripts/Utils

- Utils.cs

/Scripts/Utils/Application
  
- ApplicationSetting.cs
- ExecuteArgs.cs
- ExternalProcess.cs
- SetAntiAliasing.cs
- SetAppResoution.cs
- Unity4ResolutionHelper.cs

/Scripts/Utils/Button

- ButtonObjectBase.cs

/Scripts/Utils/CameraControl

- CameraShfter.cs
- FlyThroughCamera.cs
- GrabMove.cs
- ObjectOrbit.cs
- ObjectOrbit2.cs
- OrbitCamera.cs
- PinchZoomCamera.cs

/Scripts/Utils/DataLoader

- ContentsDownLoader.cs
- CsvLoader.cs
- ImageLoader.cs

/Scripts/Utils/Debug

- CameraGizmoDrawer.cs
- EditorHide.cs
- FrameRateUtil.cs
- GizmoDrawer.cs
- MemoryProfiler.cs
- VisibleMouseCursor.cs

/Scripts/Utils/Envelope

- AnimationCurveUtil.cs
- Envelope.cs

/Scripts/Utils/Event

- AnimationEventDelegate.cs
- TimerEvent.cs

/Scripts/Utils/Input

- VirtualInput.cs

/Scripts/Utils/Network

- TinyHttpServer.cs
- UDPReciever.cs
- UDPSender.cs

/Scripts/Utils/Object

- AutoBrink.cs
- AutoRotate.cs
- AutoScale.cs
- AutoUVScroll.cs
- Billbord.cs
- Fader.cs
- FpsCounter.cs
- LineObject.cs
- MeterObject.cs
- NineGridMesh.cs
- ScreenAnchor.cs
- ScreenPositionFollower.cs
- ScreenSaverObject.cs
- TransformFollower.cs
- WebCamPlateObject.cs

/Scripts/Utils/Render

- CompositLayer.cs
- GlLineRenderer.cs
- RenderScreenTexture.cs

/Scripts/Utils/Texture

- NumberTexture.cs
- SequenceTexture.cs

...TBA
