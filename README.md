Includes scripts to log the following:
- All gaze and eye related data exposed by [the Varjo SDK](https://developer.varjo.com/docs/unity-xr-sdk/getting-started-with-varjo-xr-plugin-for-unity). 
- (Optional) 0 or 1 indicating if the current (combined) gaze vector intersects specific objects in the scene.
- (Optional) 0 or 1 if specific objects in the scene are held (depends on [the XR interaction toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/installation.html)) 
- (Optional) Position and rotation of specific objects in the scene.

By default, all these fields will be logged at the maximum possible capture rate (for the Varjo VR3 200 fps). Since the update rate of the unity engine is lower some values (all marked as optional above) will be duplicated for the capture frames in-between each update. 

Usage:
  - Open Unity project and scene with something to look at.
  	- If you start a new project, install the XR plugin management  by Project Settings > XR plugin management (the bottom item)
  - Import dependencies:
	- [the Varjo SDK](https://developer.varjo.com/docs/unity-xr-sdk/getting-started-with-varjo-xr-plugin-for-unity) and select Varjo as XR provider (Project Settings > XR plugin management).
  		- Update to the latest version to match the latest firmware on the device (check in Varjo Base under the Support tab)
  		- Follow the steps up to and including converting the main camera in the unity scene to an XR rig. 	
	- [The XR interaction toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/installation.html)
  - If necessary (not already done), convert main camera in scene to XR rig (right click camera > XR)
  - Download and import the [HumLabVRLogger Unity package](https://github.com/lucsdjango/HumLabVRLogger/blob/main/HumLabVRLogger.unitypackage)
  - Add the ETLogger PreFab to the scene
  - Specify the camera representing the (center) in the "XR camera" field in the ETLogger component (inspector window when ETLogger object is selected).
  - Add Component "GazeGrabLoggedObject" to any object with a collider component.
  - Run scene in editor or via Build and Run.
  	- Make sure eyetracking is wnabled and calibrated on the Varjo device.
  - Logs will indicate whether it is looked at or held at each update frame. (As long as an object with an enabled ETLogger component is present in the scene and activated.)
  - Logs will be saved 
	- as text files with tab separated values
	- named by a specified prefix and the current time and date
	- to the Assets/Logs folder (if run from the Unity editor)
