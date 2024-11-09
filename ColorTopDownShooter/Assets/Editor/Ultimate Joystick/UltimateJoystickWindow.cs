/* Written by Kaz Crowe */
/* UltimateJoystickWindow.cs */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UltimateJoystickWindow : EditorWindow
{
	static string version = "2.1.5";// ALWAYS UDPATE
	static int importantChanges = 1;// UPDATE ON IMPORTANT CHANGES
	static string menuTitle = "Main Menu";

	// LAYOUT STYLES //
	int sectionSpace = 20;
	int itemHeaderSpace = 10;
	int paragraphSpace = 5;
	GUIStyle sectionHeaderStyle = new GUIStyle();
	GUIStyle itemHeaderStyle = new GUIStyle();
	GUIStyle paragraphStyle = new GUIStyle();

	GUILayoutOption[] buttonSize = new GUILayoutOption[] { GUILayout.Width( 200 ), GUILayout.Height( 35 ) }; 
	GUILayoutOption[] docSize = new GUILayoutOption[] { GUILayout.Width( 300 ), GUILayout.Height( 330 ) };
	GUISkin style;
	Texture2D scriptReference, positionVisual;
	Texture2D ubPromo, usbPromo;

	class PageInformation
	{
		public string pageName = "";
		public Vector2 scrollPosition = Vector2.zero;
		public delegate void TargetMethod();
		public TargetMethod targetMethod;
	}
	static PageInformation mainMenu = new PageInformation() { pageName = "Main Menu" };
	static PageInformation howTo = new PageInformation() { pageName = "How To" };
	static PageInformation overview = new PageInformation() { pageName = "Overview" };
	static PageInformation documentation = new PageInformation() { pageName = "Documentation" };
	static PageInformation extras = new PageInformation() { pageName = "Extras" };
	static PageInformation otherProducts = new PageInformation() { pageName = "Other Products" };
	static PageInformation feedback = new PageInformation() { pageName = "Feedback" };
	static PageInformation changeLog = new PageInformation() { pageName = "Change Log" };
	static PageInformation versionChanges = new PageInformation() { pageName = "Version Changes" };
	static PageInformation thankYou = new PageInformation() { pageName = "Thank You" };
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation currentPage = new PageInformation();
	

	[MenuItem( "Window/Tank and Healer Studio/Ultimate Joystick", false, 0 )]
	static void InitializeWindow ()
	{
		EditorWindow window = GetWindow<UltimateJoystickWindow>( true, "Tank and Healer Studio Asset Window", true );
		window.maxSize = new Vector2( 500, 500 );
		window.minSize = new Vector2( 500, 500 );
		window.Show();
	}

	void OnEnable ()
	{
		style = ( GUISkin ) EditorGUIUtility.Load( "Ultimate Joystick/UltimateJoystickEditorSkin.guiskin" );

		scriptReference = ( Texture2D )EditorGUIUtility.Load( "Ultimate Joystick/UJ_ScriptRef.jpg" );
		positionVisual = ( Texture2D )EditorGUIUtility.Load( "Ultimate Joystick/UJ_PosVisual.png" );
		ubPromo = ( Texture2D ) EditorGUIUtility.Load( "Ultimate UI/UB_Promo.png" );
		usbPromo = ( Texture2D ) EditorGUIUtility.Load( "Ultimate UI/USB_Promo.png" );

		if( !pageHistory.Contains( mainMenu ) )
			pageHistory.Insert( 0, mainMenu );

		mainMenu.targetMethod = MainMenu;
		howTo.targetMethod = HowTo;
		overview.targetMethod = OverviewPage;
		documentation.targetMethod = DocumentationPage;
		extras.targetMethod = Extras;
		otherProducts.targetMethod = OtherProducts;
		feedback.targetMethod = Feedback;
		changeLog.targetMethod = ChangeLog;
		versionChanges.targetMethod = VersionChanges;
		thankYou.targetMethod = ThankYou;

		if( pageHistory.Count == 1 )
			currentPage = mainMenu;
	}
	
	void OnGUI ()
	{
		if( style == null )
		{
			GUILayout.BeginVertical( "Box" );
			GUILayout.FlexibleSpace();
			ErrorScreen();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			return;
		}

		GUI.skin = style;

		paragraphStyle = GUI.skin.GetStyle( "ParagraphStyle" );
		itemHeaderStyle = GUI.skin.GetStyle( "ItemHeader" );
		sectionHeaderStyle = GUI.skin.GetStyle( "SectionHeader" );
		
		EditorGUILayout.Space();

		GUILayout.BeginVertical( "Box" );
		
		EditorGUILayout.LabelField( "Ultimate Joystick", GUI.skin.GetStyle( "WindowTitle" ) );

		GUILayout.Space( 3 );
		
		if( GUILayout.Button( "Version " + version, GUI.skin.GetStyle( "VersionNumber" ) ) && currentPage != changeLog )
			NavigateForward( changeLog );

		GUILayout.Space( 12 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 5 );
		if( pageHistory.Count > 1 )
		{
			if( GUILayout.Button( "", GUI.skin.GetStyle( "BackButton" ), GUILayout.Width( 80 ), GUILayout.Height( 40 ) ) )
				NavigateBack();
		}
		else
			GUILayout.Space( 80 );

		GUILayout.Space( 15 );
		EditorGUILayout.LabelField( menuTitle, GUI.skin.GetStyle( "MenuTitle" ) );
		GUILayout.FlexibleSpace();
		GUILayout.Space( 80 );
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if( currentPage.targetMethod != null )
			currentPage.targetMethod();

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.Space( 25 );

		EditorGUILayout.EndVertical();

		Repaint();
	}

	void ErrorScreen ()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUIStyle errorStyle = new GUIStyle( GUI.skin.label );
		errorStyle.fixedHeight = 55;
		errorStyle.fixedWidth = 175;
		errorStyle.fontSize = 48;
		errorStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
		EditorGUILayout.LabelField( "ERROR", errorStyle );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 50 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "Could not find the needed GUISkin located in the Editor Default Resources folder. Please ensure that the correct GUISkin, UltimateJoystickEditorSkin, is in the right folder( Editor Default Resources/Ultimate Joystick ) before trying to access the Ultimate Joystick Window.", EditorStyles.wordWrappedLabel );
		GUILayout.Space( 50 );
		EditorGUILayout.EndHorizontal();
	}

	static void NavigateBack ()
	{
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		menuTitle = pageHistory[ pageHistory.Count - 1 ].pageName;
		currentPage = pageHistory[ pageHistory.Count - 1 ];
	}

	static void NavigateForward ( PageInformation menu )
	{
		pageHistory.Add( menu );
		menuTitle = menu.pageName;
		currentPage = menu;
	}
	
	void MainMenu ()
	{
		mainMenu.scrollPosition = EditorGUILayout.BeginScrollView( mainMenu.scrollPosition, false, false, docSize );

		GUILayout.Space( 25 );
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "How To", buttonSize ) )
			NavigateForward( howTo );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Overview", buttonSize ) )
			NavigateForward( overview );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Documentation", buttonSize ) )
			NavigateForward( documentation );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Extras", buttonSize ) )
			NavigateForward( extras );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Other Products", buttonSize ) )
			NavigateForward( otherProducts );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Feedback", buttonSize ) )
			NavigateForward( feedback );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.EndScrollView();
	}

	void HowTo ()
	{
		howTo.scrollPosition = EditorGUILayout.BeginScrollView( howTo.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   To create a Ultimate Joystick in your scene, simply go up to GameObject / Ultimate UI / Ultimate Joystick. What this does is locates the Ultimate Joystick prefab that is located within the Editor Default Resources folder, and creates an Ultimate Joystick within the scene.\n\nThis method of adding an Ultimate Joystick to your scene ensures that the joystick will have a Canvas and an EventSystem so that it can work correctly.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   One of the great things about the Ultimate Joystick is how easy it is to reference to other scripts. The first thing that you will want to make sure to do is to name the joystick in the Script Reference section. After this is complete, you will be able to reference that particular joystick by it's name from a static function within the Ultimate Joystick script.\n\nAfter the joystick has been given a name in the Script Reference section, we can get that joystick's position by creating a Vector2 variable at run time and storing the result from the static function: 'GetPosition'. This Vector2 will be the joystick's position, and will contain float values between -1, and 1, with 0 being at the center.\n\nKeep in mind that the joystick's left and right ( horizontal ) movement is translated into this Vector2's X value, while the up and down ( vertical ) is the Vector2's Y value. This will be important when applying the Ultimate Joystick's position to your scripts.", paragraphStyle );
			
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 10 );
		GUILayout.Label( positionVisual, GUILayout.Width( 200 ), GUILayout.Height( 200 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Example", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   Let's assume that we want to use a joystick for a characters movement. The first thing to do is to assign the name \"Movement\" in the Joystick Name variable located in the Script Reference section of the Ultimate Joystick.", paragraphStyle );
		
		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( scriptReference );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "After that, we need to create a Vector2 variable to store the result of the joystick's position returned by the 'GetPosition' function. In order to get the \"Movement\" joystick's position, we need to pass in the name \"Movement\" as the argument.", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "C# Example:", itemHeaderStyle );
		EditorGUILayout.TextArea( "Vector2 joystickPosition = UltimateJoystick.GetPosition( \"Movement\" );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "Javascript Example:", itemHeaderStyle );
		EditorGUILayout.TextArea( "var joystickPosition : Vector2 = UltimateJoystick.GetPosition( \"Movement\" );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "The joystickPosition variable now contains the values of the position that the Movement joystick was in at the movement it was referenced. Now we can use this information in any way that is desired. For example, if you are wanting to put the joystick's position into a character movement script, you would create a Vector3 variable for movement direction, and put in the appropriate values of the Ultimate Joystick's position.", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "C# Example:", itemHeaderStyle );
		EditorGUILayout.TextArea( "Vector3 movementDirection = new Vector3( joystickPosition.x, 0, joystickPosition.y );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "Javascript Example:", itemHeaderStyle );
		EditorGUILayout.TextArea( "var movementDirection : Vector3 = new Vector3( joystickPosition.x, 0, joystickPosition.y );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "In the above example, the joystickPosition variable is used to give the movement direction values in the X and Z directions. This is because you generally don't want your character to move in the Y direction unless the user jumps. That is why we put the joystickPosition.y value into the Z value of the movementDirection variable.", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "Understanding how to use the values from any input is important when creating character controllers, so experiment with the values and try to understand how the mobile input can be used in different ways.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.EndScrollView();
	}
	
	void OverviewPage ()
	{
		overview.scrollPosition = EditorGUILayout.BeginScrollView( overview.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Assigned Variables", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   In the Assigned Variables section, there are a few components that should already be assigned if you are using one of the Prefabs that has been provided. If not, you will see error messages on the Ultimate Joystick inspector that will help you to see if any of these variables are left unassigned. Please note that these need to be assigned in order for the Ultimate Joystick to work properly.", paragraphStyle );

		GUILayout.Space( sectionSpace );
		
		/* //// --------------------------- < SIZE AND PLACEMENT > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Size And Placement", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Size and Placement section allows you to customize the joystick's size and placement on the screen, as well as determine where the user's touch can be processed for the selected joystick.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Scaling Axis
		EditorGUILayout.LabelField( "« Scaling Axis »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines which axis the joystick will be scaled from. If Height is chosen, then the joystick will scale itself proportionately to the Height of the screen.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Anchor
		EditorGUILayout.LabelField( "« Anchor »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines which side of the screen that the joystick will be anchored to.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Touch Size
		EditorGUILayout.LabelField( "« Touch Size »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Touch Size configures the size of the area where the user can touch. You have the options of either 'Default','Medium', 'Large' or 'Custom'. When the option 'Custom' is selected, an additional box will be displayed that allows for a more adjustable touch area.", paragraphStyle );
				
		GUILayout.Space( paragraphSpace );

		// Touch Size Customization
		EditorGUILayout.LabelField( "« Touch Size Customization »", itemHeaderStyle );
		EditorGUILayout.LabelField( "If the 'Custom' option of the Touch Size is selected, then you will be presented with the Touch Size Customization box. Inside this box are settings for the Width and Height of the touch area, as well as the X and Y position of the touch area on the screen.", paragraphStyle );
				
		GUILayout.Space( paragraphSpace );

		// Dynamic Positioning
		EditorGUILayout.LabelField( "« Dynamic Positioning »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Dynamic Positioning will make the joystick snap to where the user touches, instead of the user having to touch a direct position to get the joystick. The Touch Size option will directly affect the area where the joystick can snap to.", paragraphStyle );
				
		GUILayout.Space( paragraphSpace );

		// Joystick Size
		EditorGUILayout.LabelField( "« Joystick Size »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Joystick Size will change the scale of the joystick. Since everything is calculated out according to screen size, your joystick Touch Size and other properties will scale proportionately with the joystick's size along your specified Scaling Axis.", paragraphStyle );
				
		GUILayout.Space( paragraphSpace );

		// Radius
		EditorGUILayout.LabelField( "« Radius »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Radius determines how far away the joystick will move from center when it is being used, and will scale proportionately with the joystick.", paragraphStyle );
				
		GUILayout.Space( paragraphSpace );

		// Joystick Position
		EditorGUILayout.LabelField( "« Joystick Position »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Joystick Position will present you with two sliders. The X value will determine how far the Joystick is away from the Left and Right sides of the screen, and the Y value from the Top and Bottom. This will encompass 50% of your screen width, relevant to your Anchor selection.", paragraphStyle );
		/* \\\\ -------------------------- < END SIZE AND PLACEMENT > --------------------------- //// */

		GUILayout.Space( sectionSpace );

		/* //// ----------------------------- < STYLE AND OPTIONS > ----------------------------- \\\\ */
		EditorGUILayout.LabelField( "Style And Options", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Style and Options section contains options that affect how the joystick handles and is visually presented to the user.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// -----< VISUAL DISPLAY >----- //

		// Disable Visuals
		EditorGUILayout.LabelField( "« Disable Visuals »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Disable Visuals presents you with the option to disable the visuals of the joystick, whilst keeping all functionality. When paired with Dynamic Positioning and Throwable, this option can give you a very smooth camera control.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Use Fade
		EditorGUILayout.LabelField( "« Use Fade »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Use Fade option will present you with settings for the targeted alpha for the touched and untouched states, as well as the duration for the fade between the targeted alpha settings.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Use Animation
		EditorGUILayout.LabelField( "« Use Animation »", itemHeaderStyle );
		EditorGUILayout.LabelField( "If you would like the joystick to play an animation when being interacted with, then you will want to enable the Use Animation option.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Show Highlight
		EditorGUILayout.LabelField( "« Show Highlight »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Show Highlight will allow you to customize the set highlight images with a custom color. With this option, you will also be able to customize and set the highlight color at runtime using the UpdateHighlightColor function.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Show Tension
		EditorGUILayout.LabelField( "« Show Tension »", itemHeaderStyle );
		EditorGUILayout.LabelField( "With Show Tension enabled, the joystick will display it's position visually. This is done using custom colors and images that will display the direction and intensity of the joystick's current position. With this option enabled, you will be able to update the tension colors at runtime using the UpdateTensionColors function.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// -----< FUNCTIONALITY >----- //

		// Throwable
		EditorGUILayout.LabelField( "« Throwable »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Throwable option allows the joystick to smoothly transition back to center after being released. This can be used to give your input a smoother feel.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );
		
		// Draggable
		EditorGUILayout.LabelField( "« Draggable »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Draggable option will allow the joystick to move from it's default position when the user's input exceeds the set radius.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Axis
		EditorGUILayout.LabelField( "« Axis »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Axis determines which axis the joystick will snap to. By default it is set to Both, which means the joystick will use both the X and Y axis for movement. If either the X or Y option is selected, then the joystick will lock to the corresponding axis.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Boundary
		EditorGUILayout.LabelField( "« Boundary »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Boundary will allow you to decide if you want to have a square or circular edge to your joystick.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Dead Zone
		EditorGUILayout.LabelField( "« Dead Zone »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Dead Zone gives the option of setting one or two values that the joystick is constrained by. When selected, the joystick will be forced to a maximum value when it has past the set dead zone.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Tap Count
		EditorGUILayout.LabelField( "« Tap Count »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Tap Count option allows you to decide if you want to store the amount of taps that the joystick receives. The options provided with the Tap Count will allow you to customize the target amount of taps and the amount of time the user will have to accumulate these taps.", paragraphStyle );
		/* //// --------------------------- < END STYLE AND OPTIONS > --------------------------- \\\\ */

		GUILayout.Space( sectionSpace );

		/* //// ----------------------------- < SCRIPT REFERENCE > ------------------------------ \\\\ */
		EditorGUILayout.LabelField( "Script Reference", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Script Reference section contains fields for naming and helpful code snippets that you can copy and paste into your scripts. This section can also expose the horizontal and vertical values so that they can be referenced by certain game making plugins.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );
		
		// Joystick Name
		EditorGUILayout.LabelField( "« Joystick Name »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The unique name of your Ultimate Joystick. This name is what will be used to reference this particular joystick from the public static functions.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Joystick Use
		EditorGUILayout.LabelField( "« Joystick Use »", itemHeaderStyle );
		EditorGUILayout.LabelField( "This option will present you with a code snippet that is determined by your selection. This code can be copy and pasted into your custom scripts. Please note that the Joystick Use option does not actually determine what the joystick can do. Instead it only provides example code for you to use in your scripts.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Expose Values
		EditorGUILayout.LabelField( "« Expose Values »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Expose Values option will expose the horizontal and vertical values into public variables that can be accessed by certain game making plugins. This option can also be used for debugging the Ultimate Joystick to see what the values are when you are using them.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.EndScrollView();
	}
	
	void DocumentationPage ()
	{
		documentation.scrollPosition = EditorGUILayout.BeginScrollView( documentation.scrollPosition, false, false, docSize );

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		// Vector2 GetPosition
		EditorGUILayout.LabelField( "Vector2 GetPosition()", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the Ultimate Joystick's position in a Vector2 variable. The X value that is returned represents the Left and Right( Horizontal ) position of the joystick, whereas the Y value represents the Up and Down( Vertical ) position of the joystick. The values returned will always be between -1 and 1, with 0 being the center.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// float GetDistance
		EditorGUILayout.LabelField( "float GetDistance()", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the distance of the joystick from it's center in a float value. The value returned will always be a value between 0 and 1.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// UpdatePositioning()
		EditorGUILayout.LabelField( "UpdatePositioning()", itemHeaderStyle );
		EditorGUILayout.LabelField( "Updates the size and positioning of the Ultimate Joystick. This function can be used to update any options that may have been changed prior to Start().", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// ResetJoystick()
		EditorGUILayout.LabelField( "ResetJoystick()", itemHeaderStyle );
		EditorGUILayout.LabelField( "Resets the joystick back to it's neutral state. This function should only be used if the Ultimate Joystick gets stuck for some reason.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );

		// DisableJoystick()
		EditorGUILayout.LabelField( "DisableJoystick()", itemHeaderStyle );
		EditorGUILayout.LabelField( "This function will reset the Ultimate Joystick and disable the gameObject. Use this function when wanting to disable the Ultimate Joystick from being used.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );

		// EnableJoystick()
		EditorGUILayout.LabelField( "EnableJoystick()", itemHeaderStyle );
		EditorGUILayout.LabelField( "This function will ensure that the Ultimate Joystick is completely reset before enabling itself to be used again.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );

		// UpdateHighlightColor()
		EditorGUILayout.LabelField( "UpdateHighlightColor( Color targetColor )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Updates the colors of the assigned highlight images with the targeted color if the showHighlight variable is set to true. The targetColor variable will overwrite the current color setting for highlightColor and apply immediately to the highlight images.", paragraphStyle );
				
		GUILayout.Space( paragraphSpace );

		// UpdateTensionColors()
		EditorGUILayout.LabelField( "UpdateTensionColors( Color targetTensionNone, Color targetTensionFull )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Updates the tension accent image colors with the targeted colors if the showTension variable is true. The tension colors will be set to the targeted colors, and will be applied when the joystick is next used.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// bool GetTapCount
		EditorGUILayout.LabelField( "bool GetTapCount", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current state of the joystick's Tap Count according to the options set. The boolean returned will be true only after the Tap Count options have been achieved from the users input.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// float GetHorizontalAxis
		EditorGUILayout.LabelField( "float GetHorizontalAxis", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current horizontal value of the joystick's position. The value returned will always be between -1 and 1, with 0 being the neutral position.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// float GetVerticalAxis
		EditorGUILayout.LabelField( "float GetVerticalAxis", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current vertical value of the joystick's position. The value returned will always be between -1 and 1, with 0 being the neutral position.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// bool GetJoystickState
		EditorGUILayout.LabelField( "bool GetJoystickState", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the state that the joystick is currently in. This function will return true when the joystick is being interacted with, and false when not.", paragraphStyle );

		GUILayout.Space( sectionSpace );
		
		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		// UltimateJoystick.GetPosition
		EditorGUILayout.LabelField( "Vector2 UltimateJoystick.GetPosition( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the Ultimate Joystick's position in a Vector2 variable. This static function will return the same exact value as the local GetPosition function. See GetPosition for more information.", paragraphStyle );
		GUILayout.Space( paragraphSpace );

		// UltimateJoystick.GetDistance
		EditorGUILayout.LabelField( "float UltimateJoystick.GetDistance( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the distance of the joystick from it's center in a float value. This static function will return the same value as the local GetDistance function. See GetDistance for more information", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// UltimateJoystick.GetJoystickState
		EditorGUILayout.LabelField( "bool UltimateJoystick.GetJoystickState( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the state that the targeted joystick is currently in. This function will return true when the joystick is being interacted with, and false when not.", paragraphStyle );
				
		GUILayout.Space( paragraphSpace );

		// UltimateJoystick.GetJoystick()
		EditorGUILayout.LabelField( "UltimateJoystick UltimateJoystick.GetJoystick( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the instance of the Ultimate Joystick in the current scene with the targeted name. This function allows the user to call any public functions and modify any public variables of the returned Ultimate Joystick.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// UltimateJoystick.GetHorizontalAxis()
		EditorGUILayout.LabelField( "float UltimateJoystick.GetHorizontalAxis( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current horizontal value of the targeted joystick's position. The value returned will always be between -1 and 1, with 0 being the neutral position.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// UltimateJoystick.GetVerticalAxis()
		EditorGUILayout.LabelField( "float UltimateJoystick.GetVerticalAxis( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current vertical value of the targeted joystick's position. The value returned will always be between -1 and 1, with 0 being the neutral position.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// UltimateJoystick.GetTapCount()
		EditorGUILayout.LabelField( "bool UltimateJoystick.GetTapCount( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current state of the targeted joystick's Tap Count according to the options set. The boolean returned will be true only after the Tap Count options have been achieved from the users input.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.EndScrollView();
	}
	
	void Extras ()
	{
		extras.scrollPosition = EditorGUILayout.BeginScrollView( extras.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Videos", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The links below are to the collection of videos that we have made in connection with the Ultimate Joystick. The Tutorial Videos are designed to get the Ultimate Joystick implemented into your project as fast as possible, and give you a good understanding of what you can achieve using it in your projects, whereas the demonstrations are videos showing how we, and others in the Unity community, have used assets created by Tank & Healer Studio in our projects.", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Tutorials", buttonSize ) )
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TmWdbR_bklluPeElJ_xUdO9" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Demonstrations", buttonSize ) )
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TlkjepDAY_GnpA1CX-rFltz" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Example Scripts", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   Below is a link to a list of free example scripts that we have made available on our support website. Please feel free to use these as an example of how to get started on your own scripts. The scripts provided are fully commented to help you to grasp the concept behind the code. These scripts are by no means a complete solution, and are not intended to be used in finished projects.", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Example Scripts", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/uj-example-scripts.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}
	
	void OtherProducts ()
	{
		otherProducts.scrollPosition = EditorGUILayout.BeginScrollView( otherProducts.scrollPosition, false, false, docSize );

		/* -------------- < ULTIMATE BUTTON > -------------- */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 15 );
		GUILayout.Label( ubPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Ultimate Button", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   Buttons are a core element of UI, and as such they should be easy to customize and implement. The Ultimate Button is the embodiment of that very idea. This code package takes the best of Unity's Input and UnityEvent methods and pairs it with exceptional customization to give you the most versatile button for your mobile project. Are you in need of a button for attacking, jumping, shooting, or all of the above? With Ultimate Button's easy size and placement options, style options, script reference and button events, you'll have everything you need to create your custom buttons, whether they are simple or complex.", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-button.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* ------------ < END ULTIMATE BUTTON > ------------ */

		GUILayout.Space( 25 );

		/* ------------ < ULTIMATE STATUS BAR > ------------ */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 15 );
		GUILayout.Label( usbPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Ultimate Status Bar", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   The Ultimate Status Bar is a complete solution to display virtually any status for your game. With an abundance of options and customization available to you, as well as the simplest integration, the Ultimate Status Bar makes displaying any condition a cinch. Whether it’s health and energy for your player, the health of a target, or the progress of loading your scene, the Ultimate Status Bar can handle it with ease and style!", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-status-bar.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* -------------- < END STATUS BAR > --------------- */

		EditorGUILayout.EndScrollView();
	}
	
	void Feedback ()
	{
		feedback.scrollPosition = EditorGUILayout.BeginScrollView( feedback.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Having Problems?", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you experience any issues with the Ultimate Joystick, please contact us right away! We will lend any assistance that we can to resolve any issues that you have.\n\n<b>Support Email:</b>.", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( 25 );


		EditorGUILayout.LabelField( "Good Experiences?", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you have appreciated how easy the Ultimate Joystick is to get into your project, leave us a comment and rating on the Unity Asset Store. We are very grateful for all positive feedback that we get.", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Rate Us", buttonSize ) )
			Application.OpenURL( "https://www.assetstore.unity3d.com/en/#!/content/27695" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Show Us What You've Done!", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you have used any of the assets created by Tank & Healer Studio in your project, we would love to see what you have done. Contact us with any information on your game and we will be happy to support you in any way that we can!\n\n<b>Contact Us:</b>", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.EndScrollView();
	}

	void ChangeLog ()
	{
		changeLog.scrollPosition = EditorGUILayout.BeginScrollView( changeLog.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Version 2.1.5", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Improvements to the Documentation Window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Minor editor fixes.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.1.4", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Removed the Third Person Example folder and all it's contents because it was using Unity's scripts, which were causing errors for some who had the Standard Assets in their folders.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added two new functions to be used for Disabling and Enabling the Ultimate Joystick at runtime. See DisableJoystick() and EnableJoystick() in the Documentation section for more information.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the adding of the Ultimate Joystick Updater script while in the editor as it caused strange errors occasionally.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.1.3", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Updated Documentation Window with up-to-date information, as well as improving overall functionality of the Documentation Window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Minor editor fixes.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.1.2", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Removed Touch Actions section from the Editor. All options that were previously in the Touch Actions section are now located in the Style and Options section.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Fixed an issue with the Documentation Window not showing up as intended in some rare cases.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.1.1", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Improved functionality for the basic interaction of the Ultimate Joystick.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Minor change to the Ultimate Joystick editor.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.1", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Removed all example files from the Plugins folder.", paragraphStyle );
		EditorGUILayout.LabelField( "  • All example files have been placed in a new folder: Ultimate Joystick Examples.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new example scene.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Updated third-person example with more functionality.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added four new Ultimate Joystick textures.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Improved tension accent display functionality.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Further improved Ultimate Joystick Editor functionality.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed Ultimate Joystick PSD from the project files.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added four new functions to increase the efficiency of referencing the Ultimate Joystick.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed unneeded static functions.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Renamed some functions to better reflect their purpose.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added page to Documentation Window to show important changes.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.0.4", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Minor change to the editor script.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.0.3", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Attempt to upload package without any Thumbs.db files included.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.0.2", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Small fix to the editor window.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.0.1", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Fixed a small issue with the fade not working as intended.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.0", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Added a new In-Engine Documentation Window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed Javascript scripts to improve script reference functionality.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Reorganized folder structure.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Improved fade options within the Touch Actions section.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Improved overall performance.", paragraphStyle );

		EditorGUILayout.EndScrollView();
	}
	
	void ThankYou ()
	{
		thankYou.scrollPosition = EditorGUILayout.BeginScrollView( thankYou.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "    The two of us at Tank & Healer Studio would like to thank you for purchasing the Ultimate Joystick asset package from the Unity Asset Store. If you have any questions about the Ultimate Joystick that are not covered in this Documentation Window, please don't hesitate to contact us at: ", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "    We hope that the Ultimate Joystick will be a great help to you in the development of your game. After pressing the continue button below, you will be presented with helpful information on this asset to assist you in implementing Ultimate Joystick into your project.\n", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", paragraphStyle, GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", buttonSize ) )
			NavigateBack();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}

	void VersionChanges ()
	{
		versionChanges.scrollPosition = EditorGUILayout.BeginScrollView( versionChanges.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "  Thank you for downloading the most recent version of the Ultimate Joystick. There is some exciting new functionality as well as some changes that could affect any existing reference of the Ultimate Joystick. Please check out the sections below to see all the important changes that have been made. As always, if you run into any issues with the Ultimate Joystick, please contact us at:", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "GENERAL CHANGES", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  Some folder structure and existing functionality has been updated and improved. None of these changes should affect any existing use of the Ultimate Joystick.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed example files from the Plugins folder. All example files will now be in the folder named: Ultimate Joystick Examples.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new example scene: Asteroids.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Modified the Third Person Example scene to better implement the Ultimate Joystick and show more of its potential.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added four new Ultimate Joystick textures that can be used in your projects.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Modified the DisplayTensionAccents function to accept a limited number of tension directions. Now up, down, left, or right can be used as the only tension accents without causing any unwanted behavior.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added helpful text to the Ultimate Joystick Editor to help bring attention to variables that have not been assigned.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the Ultimate Joystick PSD from the project files.", paragraphStyle );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "NEW FUNCTIONS", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  Some new functions have been added to help reference the Ultimate Joystick more efficiently. For information on what each new function does, please refer to the Documentation section of this help window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • float GetHorizontalAxis()", paragraphStyle );
		EditorGUILayout.LabelField( "  • float GetVerticalAxis()", paragraphStyle );
		EditorGUILayout.LabelField( "  • UltimateJoystick GetJoystick()", paragraphStyle );
		EditorGUILayout.LabelField( "  • bool GetTapCount()", paragraphStyle );

		GUILayout.Space( 10 );
		
		EditorGUILayout.LabelField( "REMOVED FUNCTIONS", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  Only the public static functions have been removed. All functionality remains the same for the public version of each function. To replace any existing code that uses these removed functions, please use the GetJoystick function.", paragraphStyle );
		EditorGUILayout.LabelField( "  • UpdatePositioning()", paragraphStyle );
		EditorGUILayout.LabelField( "  • ResetJoystick()", paragraphStyle );
		EditorGUILayout.LabelField( "  • UpdateHighlightColor()", paragraphStyle );
		EditorGUILayout.LabelField( "  • UpdateTensionColors()", paragraphStyle );
		
		GUILayout.Space( 10 );
		
		EditorGUILayout.LabelField( "RENAMED FUNCTIONS", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  Some of the public functions have been renamed to match the public static functions used more commonly. If you are using any of these public functions, please be sure to update them to the corresponding renamed function.", paragraphStyle );
		EditorGUILayout.LabelField( "  • JoystickPosition has been renamed to GetPosition.", paragraphStyle );
		EditorGUILayout.LabelField( "  • JoystickDistance has been renamed to GetDistance.", paragraphStyle );
		EditorGUILayout.LabelField( "  • JoystickState has been renamed to GetJoystickState", paragraphStyle );

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Got it!", buttonSize ) )
			NavigateBack();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}
	
	[InitializeOnLoad]
	class UltimateJoystickInitialLoad
	{
		static UltimateJoystickInitialLoad ()
		{
			// If the user has a older version of UJ that used the bool for startup...
			if( EditorPrefs.HasKey( "UltimateJoystickStartup" ) && !EditorPrefs.HasKey( "UltimateJoystickVersion" ) )
			{
				// Set the new pref to 0 so that the pref will exist and the version changes will be shown.
				EditorPrefs.SetInt( "UltimateJoystickVersion", 0 );
			}

			// If this is the first time that the user has downloaded the Ultimate Joystick...
			if( !EditorPrefs.HasKey( "UltimateJoystickVersion" ) )
			{
				// Navigate to the Thank You page.
				NavigateForward( thankYou );

				// Set the version to current so they won't see these version changes.
				EditorPrefs.SetInt( "UltimateJoystickVersion", importantChanges );

				EditorApplication.update += WaitForCompile;
			}
			else if( EditorPrefs.GetInt( "UltimateJoystickVersion" ) < importantChanges )
			{
				// Navigate to the Version Changes page.
				NavigateForward( versionChanges );

				// Set the version to current so they won't see this page again.
				EditorPrefs.SetInt( "UltimateJoystickVersion", importantChanges );

				EditorApplication.update += WaitForCompile;
			}
		}

		static void WaitForCompile ()
		{
			if( EditorApplication.isCompiling )
				return;

			EditorApplication.update -= WaitForCompile;

			InitializeWindow();
		}
	}
}