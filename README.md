GarageKit for Unity
===================
You can start the project immediately in Unity using GarageKit.

![](http://media.tumblr.com/1ad50e485e709219816e2cc0057d65b1/tumblr_inline_ndw8fqGnIb1qzb8ql.png)

## What is it?

GaraegKit is Unity C# framework. provide the template scene, some manager scripts, state scripts, utility scripts and directory structure.

http://debuglog.tumblr.com/post/100742681354/garagekit-for-unity

## Require Modules

- TextMesh Pro

#### from AssetStore

- dotween
  - https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

#### from OpenUPM (https://openupm.com/)

- In-game Debug Console
  - `$ openupm add com.yasirkula.ingamedebugconsole`
- Graphy - Ultimate Stats Monitor
  - `$ openupm add com.tayx.graphy`
- UniTask
  - `$ openupm add com.cysharp.unitask`

#### /Editor (already in)

- Editor enhancements
  - https://github.com/sharkattack51/UnityEditorEnhancements
    - fork of
  https://bitbucket.org/Tenebrous/unityeditorenhancements/
  http://forum.unity3d.com/threads/free-editor-enhancements-extensions-in-project-window-components-in-heirarchy.149569/

#### /Plugins/Log4Net (already in)

- log4net  
  - http://logging.apache.org/log4net/

## How to use?

First change the API compatibility level to `.NET4.x` to fix the error.

1. open /Scenes/Main.unity
2. and press the play.
3. and customize your project.

## Examples

/Scenes/Examples

- ApplicationSettingExample.unity
- CameraControllExample.unity
- TimelinedStateExample.unity
- VRGuiExample.unity

## Main Scripts

- AppMain.cs

#### /Scripts/Managers

- DebugManager.cs
- SceneStateManager.cs
- SoundManager.cs
- TimeManager.cs
- UserInputManager.cs

#### /Scripts/States/Base

- AsyncStateBase.cs
- StateBase.cs
- TimelinedStateBase.cs
- VRSceneStateBase.cs

## Utility Scripts

#### /Editor

- AssetPostprocessUTF8Encode.cs

#### /Scripts/Utils

- Utils.cs
- AndroidUtil.cs

#### /Scripts/Utils/Application
  
- ApplicationSetting.cs
- ExecuteArgs.cs
- ExternalProcess.cs
- RemotePrefs.cs
- SetAntiAliasing.cs
- SetAppResoution.cs
- StandaloneResolutionHelper.cs

#### /Scripts/Utils/Button

- ButtonObjectBase.cs

#### /Scripts/Utils/CameraControl

- CameraShfter.cs
- FlyThroughCamera.cs
- GrabMove.cs
- ObjectOrbit.cs
- ObjectOrbit2.cs
- OrbitCamera.cs
- Panorama360Camera.cs
- PinchZoomCamera.cs

#### /Scripts/Utils/DataLoader

- ContentsDownLoader.cs
- CsvLoader.cs
- ImageLoader.cs

#### /Scripts/Utils/Debug

- CameraGizmoDrawer.cs
- EditorHide.cs
- FrameRateUtil.cs
- GizmoDrawer.cs
- LookAtGizmoDrawer.cs
- MemoryProfiler.cs
- VisibleMouseCursor.cs

#### /Scripts/Utils/Envelope

- AnimationCurveUtil.cs
- Envelope.cs

#### /Scripts/Utils/Event

- TimelineEventAction.cs
- TimelineEventActionList.cs
- AnimationEventDelegate.cs
- ColliderHandler.cs
- TimerEvent.cs

#### /Scripts/Utils/Input

- VirtualInput.cs

#### /Scripts/Utils/Network

- TinyHttpServer.cs
- UDPReciever.cs
- UDPSender.cs

#### /Scripts/Utils/Object

- AnimationByStep.cs
- AutoBrink.cs
- AutoRotate.cs
- AutoScale.cs
- AutoUVScroll.cs
- Billbord.cs
- Fader.cs
- FpsCounter.cs
- LineObject.cs
- ScreenAnchor.cs
- ScreenPositionFollower.cs
- TransformFollower.cs
- WebCamPlateObject.cs
- ZsortOrderGroup.cs

#### /Scripts/Utils/Render

- CompositLayer.cs
- GlLineRenderer.cs
- RenderScreenTexture.cs

#### /Scripts/Utils/Texture

- NumberTexture.cs
- SequenceSprite.cs
- SequenceTexture.cs

#### /Scripts/Utils/UI

- UIFadeGroupComponent.cs
- UIFadeTelop.cs
- UILineRenderer.cs

#### /Scripts/Utils/VR

- VRGazeGuideArrow.cs

...TBA
