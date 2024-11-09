Thank you for purchasing the Ultimate Button UnityPackage!

/* ------- < IMPORTANT INFORMATION > ------- */
Within Unity, please go to Window / Tank and Healer Studio / Ultimate Button to access important information on how to get
started using the Ultimate Button. There is a ton of information available to help you get the Ultimate Button into your
project as fast as possible. However, if you can't view the in-engine documentation window, please see the information below.
/* ----- < END IMPORTANT INFORMATION > ----- */

// --- IF YOU CAN'T VIEW THE ULTIMATE BUTTON WINDOW, READ THIS SECTION --- //

	// --- HOW TO CREATE --- //
To create a Ultimate Button in your scene, simply go up to GameObject / UI / Ultimate UI / Ultimate Button. What this does
is locates the Ultimate Button prefab that is located within the Editor Default Resources folder, and creates an Ultimate Button
within the scene. Alternatively, you can locate the Prefabs folder within the Ultimate Button files and simply drag and drop
Prefab out into the Hierarchy window. This will create an Ultimate Button, and create a Canvas and EventSystem if one is not
already present.

	// --- HOW TO REFERENCE --- //
The Ultimate Button is extremely easy to integrate into your project. There are 2 separate ways that you can do this. The
Script Reference section of the Ultimate Button in the Inspector will allow you to name the particlular Ultimate Button and
reference it through code to get it's states. The other option is to use the Button Events section which will allow you to 
use Unity Events to call functions or modify variables using these events. So let's look at both options and see which one
will be the best for your project.

  ------------
| Button Events |
  ------------
To reference the Ultimate Button using Unity Events, all you need to do is make the function that you are trying to reference
Public. This will allow the Ultimate Button to see those functions and call them correctly. Next you will want to make a Event
for the Button by creating a On Button Down () or On Button Up () event. These are located in the Button Events section of the
inspector.

  -----------------
| Script Reference |
  -----------------
The Script Reference section will allow you to use the Ultimate Button very much like Input system already used in Unity. First,
you will want to create an Ultimate Button within your scene and assign a name within the Script Reference section. If you are
using the Ultimate Button to replace Standalone code, then you will want to find anything in your code with the Input.GetButton,
Input.GetButtonDown, or Input.GetButtonUp functions. Then replace them with UltimateButton.GetButton, UltimateButton.GetButtonDown,
or UltimateButton.GetButtonUp. These functions are available to copy for your code once you assign a name to your Ultimate Button
within your scene. If you have the Ultimate Button for use in your scripts from scratch, then all you need to do is create an
Ultimate Button within your scene and assign it a name so that you can use the Ultimate Button state functions.

/* - IMPORTANT NOTE - */
------------------------
If you have any issues getting this product implemented into your project, please contact us at TankAndHealerStudio@outlook.com
and we will try to help you out as much as we can!

	- Support Email: TankAndHealerStudio@outlook.com

Thank you,
	- Tank And Healer Studio


/* ------------------< CHANGE >------------------ */
/* --------------------< LOG >------------------- */

| Version 2.0.0 |
	• Added In-Engine Documentation Window to help get the Ultimate Button implemented as quick as possible.
	• Updated example scene to use some of Unity's free assets.
	• Removed old example files.
	• Added new example files for the truck example scene.

| Version 2.0.2 |
	• Removed the Touch Actions section. All options previously located in the Touch Actions section are now located in the Style and
		Options seciton.
	• Expanded the functionality of using the Ultimate Button in your scripts. Added a new section titled Button Events. Now you can
		use either the Script Reference or the Button Events section to implement into your scripts.
	• Removed example files from the Plugins folder. All example files will now be in the folder named: Ultimate Button Examples.
	• Added four new Ultimate Button textures that can be used in your projects.
	• Removed the Ultimate Button PSD from the project files.
	• Added new function: UltimateButton GetUltimateButton()