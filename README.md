# Introduction to VR in the Humanities Lab

## Equipment
The Humanities Lab provides a range of VR equipment, both tethered and standalone headsets and peripherals/accessories.

- Tethered VR setups include
	+ One PC dedicated to VR data collection
	+ One PC dedicated to VR development
	+ Varjo headsets with integrated Eyetracking
	+ SteamVR tracking system
	+ SteamVR compatible hand controllers
	+ Vive Trackers (6DOF)
	+ Force- and haptic-feedback enabled gloves
- Tethered VR setups include
	+ Meta Quest 2 VR headsets
	+ Pico 2 Eye headset with integrated eye-tracking

	
## Software


### Unity
The Lab uses Unity as the main development platform for VR. Note that users may install other software and development tools on the development PC in consultation with the lab staff.

We provide and maintain Unity packages for logging Eyetracking data locally.

- [Tethered Varjo] (https://github.com/lucsdjango/HumLabVRLogger)
  - Add Component "GazeGrabLogger" to any object, and logs will indicated whether it is looked at or held at each update frame. (As long as an object with an enabled ETLogger component is present in the scene and activated.)
- Stand-alone Quest Pro


### Cognitive3D

The custom logging solution logs raw eye tracking data per time frame. The lab also provides access to Cognitive3D, an online tool to extract and visualize higher-level eye-tracking measures such as fixations and heat-maps. [Cognitive3D's documentation](https://docs.cognitive3d.com/unity/minimal-setup-guide/) explains how to integrate it with Unity. Ask the lab staff for login information! We recommend using the custom logging solution in parallell with Cognitive3D to also have access to the raw data.


 
