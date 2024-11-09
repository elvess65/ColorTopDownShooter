Thank you for purchasing the Ultimate Joystick UnityPackage!

/* ------- < IMPORTANT INFORMATION > ------- */
Within Unity, please go to Window / Tank and Healer Studio / Ultimate Joystick to access important information on how to get started using the Ultimate Joystick. There is
a ton of information available to help you get the Ultimate Joystick into your project as fast as possible. However, if you can't view the in-engine documentation
window, please see the information below.
/* ----- < END IMPORTANT INFORMATION > ----- */


// --- IF YOU CAN'T VIEW THE ULTIMATE JOYSTICK WINDOW, READ THIS SECTION --- //

	// --- HOW TO CREATE --- //
To create a Ultimate Joystick in your scene, simply go up to GameObject / UI / Ultimate UI / Ultimate Joystick. What this does is locates the Ultimate Joystick
prefab that is located within the Editor Default Resources folder, and creates an Ultimate Joystick within the scene. This method of adding an Ultimate Joystick
to your scene ensures that the joystick will have a Canvas and an EventSystem so that it can work correctly.

	// --- HOW TO REFERENCE --- //
One of the great things about the Ultimate Joystick is how easy it is to reference to other scripts. The first thing that you will want to make sure to do is to
name the joystick in the Script Reference section. After this is complete, you will be able to reference that particular joystick by it's name from a static function
within the Ultimate Joystick script. After the joystick has been given a name in the Script Reference section, we can get that joystick's position by creating a
Vector2 variable at run time and storing the result from the static function: 'GetPosition'. This Vector2 will be the joystick's position, and will contain float
values between -1, and 1, with 0 being at the center. Keep in mind that the joystick's left and right ( horizontal ) movement is translated into this Vector2's X
value, while the up and down ( vertical ) is the Vector2's Y value. This will be important when applying the Ultimate Joystick's position to your scripts.

Let's assume that we want to use a joystick for a characters movement. The first thing to do is to assign the name "Movement" in the Joystick Name variable located
in the Script Reference section of the Ultimate Joystick. After that, we need to create a Vector2 variable to store the result of the joystick's position returned
by the 'GetPosition' function. In order to get the "Movement" joystick's position, we need to pass in the name "Movement" as the argument.

	// C# EXAMPLE //
		Vector2 joystickPosition = UltimateJoystick.GetPosition( "Movement" );

	// JAVASCRIPT EXAMPLE //
		var joystickPosition : Vector2 = UltimateJoystick.GetPosition( "Movement" );

The joystickPosition variable now contains the values of the position that the Movement joystick was in at the movement it was referenced. Now we can use this
information in any way that is desired. For example, if you are wanting to put the joystick's position into a character movement script, you would create a Vector3
variable for movement direction, and put in the appropriate values of the Ultimate Joystick's position.

	// C# EXAMPLE //
		Vector3 movementDirection = new Vector3( joystickPosition.x, 0, joystickPosition.y );

	// JAVASCRIPT EXAMPLE //
		var movementDirection : Vector3 = new Vector3( joystickPosition.x, 0, joystickPosition.y );

In the above example, the joystickPosition variable is used to give the movement direction values in the X and Z directions. This is because you generally don't
want your character to move in the Y direction unless the user jumps. That is why we put the joystickPosition.y value into the Z value of the movementDirection
variable. Understanding how to use the values from any input is important when creating character controllers, so experiment with the values and try to understand
how the mobile input can be used in different ways.