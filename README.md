Includes scripts to log the following:
- All gaze and eye related data exposed by [the Varjo SDK](https://developer.varjo.com/docs/unity-xr-sdk/eye-tracking-with-varjo-xr-plugin). 
- (Optional) 0 or 1 indicating if the current (combined) gaze vector intersects specific objects in the scene.
- (Optional) 0 or 1 if specific objects in the scene are held (depends on [the XR interaction toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.5/manual/index.html)) 
- (Optional) Position and rotation of specific objects in the scene.

By default, all these fields will be logged at the maximum possible capture rate (for the Varjo VR3 200 fps). Since the update rate of the unity engine is lower some values (all marked as optional above) will be duplicated for the capture frames in-between each update. 

Usage:
  - Open Unity project and scene with something to look at.
  - Import dependencies:
	- [the Varjo SDK](https://developer.varjo.com/docs/unity-xr-sdk/eye-tracking-with-varjo-xr-plugin) and select Varjo as XR provider (Project Settings > XR plugin management).
	- [The XR interaction toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.5/manual/installation.html)
  - If necessary (not already done), convert main camera in scene to XR rig (right click camera > XR)
  - Download and import the [HumLabVRLogger Unity package](https://github.com/lucsdjango/HumLabVRLogger/blob/main/HumLabVRLogger.unitypackage)
  -	Add the ETLogger PreFab to the scene
  - Specify the camera representing the (center) 
  - Add Component "GazeGrabLogger" to any object, and logs will indicate whether it is looked at or held at each update frame. (As long as an object with an enabled ETLogger component is present in the scene and activated.)
  - Logs will be saved 
	- as text files with tab separated values
	- named by a specified prefix and the current time and date
	- to the Assets/Logs folder (if run from the Unity editor)
