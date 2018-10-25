/* Written by Kaz Crowe */
/* UltimateButtonWindow.cs */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UltimateButtonWindow : EditorWindow
{
	static string version = "2.1.1";// ALWAYS UDPATE
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
	Texture2D scriptReference;
	Texture2D ujPromo, usbPromo;
	
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
	static PageInformation otherProducts = new PageInformation() { pageName = "Other Products" };
	static PageInformation feedback = new PageInformation() { pageName = "Feedback" };
	static PageInformation changeLog = new PageInformation() { pageName = "Change Log" };
	static PageInformation versionChanges = new PageInformation() { pageName = "Version Changes" };
	static PageInformation thankYou = new PageInformation() { pageName = "Thank You" };
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation currentPage = new PageInformation();
	

	[MenuItem( "Window/Tank and Healer Studio/Ultimate Button", false, 20 )]
	static void InitializeWindow ()
	{
		EditorWindow window = GetWindow<UltimateButtonWindow>( true, "Tank and Healer Studio Asset Window", true );
		window.maxSize = new Vector2( 500, 500 );
		window.minSize = new Vector2( 500, 500 );
		window.Show();
	}

	void OnEnable ()
	{
		style = ( GUISkin )EditorGUIUtility.Load( "Ultimate Button/UltimateButtonEditorSkin.guiskin" );

		scriptReference = ( Texture2D )EditorGUIUtility.Load( "Ultimate Button/UltimateButtonScriptReference.jpg" );
		ujPromo = ( Texture2D )EditorGUIUtility.Load( "Ultimate UI/UJ_Promo.png" );
		usbPromo = ( Texture2D )EditorGUIUtility.Load( "Ultimate UI/USB_Promo.png" );

		if( !pageHistory.Contains( mainMenu ) )
			pageHistory.Insert( 0, mainMenu );

		mainMenu.targetMethod = MainMenu;
		howTo.targetMethod = HowTo;
		overview.targetMethod = Overview;
		documentation.targetMethod = Documentation;
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
		
		EditorGUILayout.LabelField( "Ultimate Button", GUI.skin.GetStyle( "WindowTitle" ) );

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
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "ERROR", EditorStyles.boldLabel );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "Could not find the needed GUISkin located in the Editor Default Resources folder. Please ensure that the correct GUISkin, UltimateButtonEditorSkin, is in the right folder( Editor Default Resources/Ultimate Button ) before trying to access the Ultimate Button Window.", paragraphStyle );
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

		EditorGUILayout.LabelField( "   To create a Ultimate Button in your scene, simply go up to GameObject / UI / Ultimate UI / Ultimate Button. What this does is locates the Ultimate Button prefab that is located within the Editor Default Resources folder, and creates an Ultimate Button within the scene. Alternatively, you can locate the Prefabs folder within the Ultimate Button files and simply drag and drop Prefab out into the Hierarchy window. This will create an Ultimate Button, and create a Canvas and EventSystem if one is not already present.", paragraphStyle );

		EditorGUILayout.LabelField( "This method of adding an Ultimate Button to your scene ensures that the button will have a Canvas and an EventSystem so that it can work correctly.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   One of the great things about the Ultimate Button is how easy it is to reference to other scripts. The first thing that you will want to make sure to do is determine how you want to use the Ultimate Button within your scripts. If you are used to using the Events that are used in Unity's default UI buttons, then you may want to use the Unity Events options located within the Button Events section of the Ultimate Button inspector. However, if you are used to using Unity's Input system for getting input, then the Script Reference section would probably suit you better.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For this example, we'll go over how to use the Script Reference section. First thing to do is assign the Button Name within the Script Reference section. After this is complete, you will be able to reference that particular button by it's name from a static function within the Ultimate Button script.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Example", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you are going to use the Ultimate Button for making a player jump, then you will need to check the button's state to determine when the user has touched the button and is wanting the player to jump. So for this example, let's assign the name \"Jump\" in the Script Reference section of the Ultimate Button.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( scriptReference );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "There are several functions that allow you to check the different states that the Ultimate Button is in. For more information on all the functions that you have available to you, please see the documentation section of this Help Window.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For this example we will be using the GetButtonDown function to see if the user has pressed down on the button. It is worth noting that this function is useful when wanting to make the player start the jump action on the exact frame that the user has pressed down on the button, and not after at all.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "C# and Javascript Example:", itemHeaderStyle );
		EditorGUILayout.TextArea( "if( UltimateButton.GetButtonDown( \"Jump\" ) )\n{\n	// Call player jump function.\n}", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Feel free to experiment with the different functions of the Ultimate Button to get it working exactly the way you want to. Additionally, if you are curious about how the Ultimate Button has been implemented into an Official Tank and Healer Studio example, then please see the README.txt that is included with the example files for the project.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.EndScrollView();
	}
	
	void Overview ()
	{
		overview.scrollPosition = EditorGUILayout.BeginScrollView( overview.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Assigned Variables", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   In the Assigned Variables section, there are a few components that should already be assigned if you are using one of the Prefabs that has been provided. If not, you will see error messages on the Ultimate Button inspector that will help you to see if any of these variables are left unassigned. Please note that these need to be assigned in order for the Ultimate Button to work properly.", paragraphStyle );

		GUILayout.Space( sectionSpace );
		
		/* //// --------------------------- < SIZE AND PLACEMENT > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Size And Placement", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Size and Placement section allows you to customize the button's size and placement on the screen, as well as determine where the user's touch can be processed for the selected button.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Scaling Axis
		EditorGUILayout.LabelField( "« Scaling Axis »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines which axis the button will be scaled from. If Height is chosen, then the button will scale itself proportionately to the Height of the screen.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Anchor
		EditorGUILayout.LabelField( "« Anchor »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines which side of the screen that the button will be anchored to.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Touch Size
		EditorGUILayout.LabelField( "« Touch Size »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Touch Size configures the size of the area where the user can touch. You have the options of either 'Default','Medium', or 'Large'.", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		
		// Button Size
		EditorGUILayout.LabelField( "« Button Size »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Button Size will change the scale of the button. Since everything is calculated out according to screen size, your Touch Size option and other properties will scale proportionately with the button's size along your specified Scaling Axis.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Button Position
		EditorGUILayout.LabelField( "« Button Position »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Button Position will present you with two sliders. The X value will determine how far the button is away from the Left and Right sides of the screen, and the Y value from the Top and Bottom. This will encompass 50% of your screen, relevant to your Anchor selection.", paragraphStyle );
		/* \\\\ -------------------------- < END SIZE AND PLACEMENT > --------------------------- //// */

		GUILayout.Space( sectionSpace );

		/* //// ----------------------------- < STYLE AND OPTIONS > ----------------------------- \\\\ */
		EditorGUILayout.LabelField( "Style And Options", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Style and Options section contains options that affect how the button is visually presented to the user.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Image Style
		EditorGUILayout.LabelField( "« Image Style »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines whether the input range should be circular or square. This option affects how the Input Range and Track Input options function.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Input Range
		EditorGUILayout.LabelField( "« Input Range »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The range that the Ultimate Button will react to when initiating and dragging the input.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Track Input
		EditorGUILayout.LabelField( "« Track Input »", itemHeaderStyle );
		EditorGUILayout.LabelField( "If the Track Input option is enabled, then the Ultimate Button will reflect it's state according to where the user's input currently is. This means that if the input moves off of the button, then the button state will turn to false. When the input returns to the button the state will return to true. If the Track Input option is disabled, then the button will reflect the state of only pressing and releasing the button.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Transmit Input
		EditorGUILayout.LabelField( "« Transmit Input »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Transmit Input option will allow you to send the input data to another script that uses Unity's EventSystem. For example, if you are using the Ultimate Joystick package, you could place the Ultimate Button on top of the Ultimate Joystick, and still have the Ultimate Button and Ultimate Joystick function correctly when interacted with.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Tap Count Option
		EditorGUILayout.LabelField( "« Tap Count Option »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Tap Count option allows you to decide if you want to store the amount of taps that the button receives. The options provided with the Tap Count will allow you to customize the target amount of taps, the tap time window, and the event to be called when the tap count has been achieved.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Base Color
		EditorGUILayout.LabelField( "« Base Color »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Base Color option determines the color of the button base images.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Show Highlight
		EditorGUILayout.LabelField( "« Show Highlight »", itemHeaderStyle );
		EditorGUILayout.LabelField( "Show Highlight will allow you to customize the set highlight images with a custom color. With this option, you will also be able to customize and set these images at runtime using the UpdateHighlightColor function. See the Documentation section for more details.", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		
		// Show Tension
		EditorGUILayout.LabelField( "« Show Tension »", itemHeaderStyle );
		EditorGUILayout.LabelField( "With Show Tension enabled, the button will display interactions visually using custom colors and images to display the intensity of the press. With this option enabled, you will be able to update the tension colors at runtime using the UpdateTensionColors function. See the Documentation section for more information.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Use Animation
		EditorGUILayout.LabelField( "« Use Animation »", itemHeaderStyle );
		EditorGUILayout.LabelField( "If you would like the button to play an animation when being interacted with, then you will want to enable the Use Animation option.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Use Fade
		EditorGUILayout.LabelField( "« Use Fade »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Use Fade option will present you with settings for the targeted alpha for the touched and untouched states, as well as the duration for the fade between the targeted alpha settings.", paragraphStyle );
		/* //// --------------------------- < END STYLE AND OPTIONS > --------------------------- \\\\ */

		GUILayout.Space( sectionSpace );

		/* //// ----------------------------- < SCRIPT REFERENCE > ------------------------------ \\\\ */
		EditorGUILayout.LabelField( "Script Reference", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Script Reference section contains fields for naming and helpful code snippets that you can copy and paste into your scripts. Be sure to refer to the Documentation Window for information about the functions that you have available to you.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );
		
		// Button Name
		EditorGUILayout.LabelField( "« Button Name »", itemHeaderStyle );
		EditorGUILayout.LabelField( "The unique name of your Ultimate Button. This name is what will be used to reference this particular button from the public static functions.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		// Example Code
		EditorGUILayout.LabelField( "« Example Code »", itemHeaderStyle );
		EditorGUILayout.LabelField( "This section will present you with code snippets that are determined by your selection. This code can be copy and pasted into your custom scripts. Please note that this section is only designed to help you get the Ultimate Button working in your scripts quickly. Any options within this section do have affect the actual functionality of the button.", paragraphStyle );
		/* //// --------------------------- < END SCRIPT REFERENCE > ---------------------------- \\\\ */

		GUILayout.Space( sectionSpace );

		/* //// ------------------------------- < BUTTON EVENTS > ------------------------------- \\\\ */
		EditorGUILayout.LabelField( "Button Events", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Button Events section contains Unity Events that can be created for when the Ultimate Button is pressed and released. Also, if you have the Tap Count Option set, then you can assign a Unity Event for the Tap Count Event option.", paragraphStyle );
		
		GUILayout.Space( itemHeaderSpace );
		/* //// ----------------------------- < END BUTTON EVENTS > ----------------------------- \\\\ */
		EditorGUILayout.EndScrollView();
	}
	
	void Documentation ()
	{
		documentation.scrollPosition = EditorGUILayout.BeginScrollView( documentation.scrollPosition, false, false, docSize );
		
		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		// UpdatePositioning()
		EditorGUILayout.LabelField( "UpdatePositioning()", itemHeaderStyle );
		EditorGUILayout.LabelField( "Updates the size and positioning of the Ultimate Button. This function can be used to update any options that may have been changed prior to Start().", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// UpdateBaseColor()
		EditorGUILayout.LabelField( "UpdateBaseColor( Color targetColor )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Updates the color of the assigned button base images with the targeted color. The targetColor option will overwrite the current setting for base color.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// UpdateHighlightColor()
		EditorGUILayout.LabelField( "UpdateHighlightColor( Color targetColor )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Updates the colors of the assigned highlight images with the targeted color if the showHighlight variable is set to true. The targetColor variable will overwrite the current color setting for highlightColor and apply immediately to the highlight images.", paragraphStyle );
				
		GUILayout.Space( paragraphSpace );

		// UpdateTensionColors()
		EditorGUILayout.LabelField( "UpdateTensionColors( Color targetTensionNone, Color targetTensionFull )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Updates the tension accent image colors with the targeted colors if the showTension variable is true. The tension colors will be set to the targeted colors, and will be applied when the button is next used.", paragraphStyle );

		GUILayout.Space( sectionSpace );
		
		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		// UltimateButton.GetUltimateButton
		EditorGUILayout.LabelField( "UltimateButton UltimateButton.GetUltimateButton( string buttonName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the Ultimate Button registered with the buttonName string. This function can be used to call local functions on the Ultimate Button to apply color changes or position updates at runtime.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );

		// UltimateButton.GetButtonDown
		EditorGUILayout.LabelField( "bool UltimateButton.GetButtonDown( string buttonName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns true on the frame that the targeted Ultimate Button is pressed down.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );
		
		// UltimateButton.GetButtonUp
		EditorGUILayout.LabelField( "bool UltimateButton.GetButtonUp( string buttonName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns true on the frame that the targeted Ultimate Button is released.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );
		
		// UltimateButton.GetButton
		EditorGUILayout.LabelField( "bool UltimateButton.GetButton( string buttonName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns true on the frames that the targeted Ultimate Button is being interacted with.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );
				
		// UltimateButton.GetTapCount
		EditorGUILayout.LabelField( "bool UltimateButton.GetTapCount( string buttonName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns true on the frame that the Tap Count option has been achieved.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.EndScrollView();
	}
	
	void OtherProducts ()
	{
		otherProducts.scrollPosition = EditorGUILayout.BeginScrollView( otherProducts.scrollPosition, false, false, docSize );

		/* -------------- < ULTIMATE JOYSTICK > -------------- */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 15 );
		GUILayout.Label( ujPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Ultimate Joystick", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   The Ultimate Joystick is a simple, yet powerful tool for the development of your mobile games. The Ultimate Joystick was created with the goal of giving Developers an incredibly versatile joystick solution, while being extremely easy to implement into existing, or new scripts. You don't need to be a programmer to work with the Ultimate Joystick, and it is very easy to implement into any type of character controller that you need. Additionally, Ultimate Joystick's source code is extremely well commented, easy to modify, and features a complete in-engine documentation window, making it ideal for game-specific adjustments. In its entirety, Ultimate Joystick is an elegant and easy to utilize mobile joystick solution.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-joystick.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* ------------ < END ULTIMATE JOYSTICK > ------------ */

		GUILayout.Space( sectionSpace );

		/* ------------ < ULTIMATE STATUS BAR > ------------ */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 15 );
		GUILayout.Label( usbPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Ultimate Status Bar", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   The Ultimate Status Bar is a complete solution to display virtually any status for your game. With an abundance of options and customization available to you, as well as the simplest integration, the Ultimate Status Bar makes displaying any condition a cinch. Whether it’s health and energy for your player, the health of a target, or the progress of loading your scene, the Ultimate Status Bar can handle it with ease. ", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-status-bar.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* -------------- < END ULTIMATE STATUS BAR > --------------- */

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.EndScrollView();
	}
	
	void Feedback ()
	{
		feedback.scrollPosition = EditorGUILayout.BeginScrollView( feedback.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Having Problems?", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you experience any issues with the Ultimate Button, please send us an email right away! We will lend any assistance that we can to resolve any issues that you have.\n\n<b>Support Email:</b>", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Good Experiences?", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you have appreciated how easy the Ultimate Button is to get into your project, leave us a comment and rating on the Unity Asset Store. We are very grateful for all positive feedback that we get.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Rate Us", buttonSize ) )
			Application.OpenURL( "https://www.assetstore.unity3d.com/en/#!/content/28824" );
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
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", paragraphStyle, GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.EndScrollView();
	}
	
	void ChangeLog ()
	{
		changeLog.scrollPosition = EditorGUILayout.BeginScrollView( changeLog.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Version 2.1.1", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Drastically improved the functionality of the Ultimate Button Documentation Window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Minor editor fixes.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.1", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Some folder structure and existing functionality has been updated and improved. ( None of these changes should affect any existing use of the Ultimate Button ).", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the Touch Actions section. All options previously located in the Touch Actions section are now located in the Style and Options section.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Expanded the functionality of using the Ultimate Button in your scripts. Added a new section titled Button Events. Now you can use either the Script Reference or the Button Events section to implement into your scripts.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed example files from the Plugins folder. All example files will now be in the folder named: Ultimate Button Examples.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added four new Ultimate Button textures that can be used in your projects.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the Ultimate Button PSD from the project files.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Improved Tension Accent functionality.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.0.2", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Minor changes to the editor script.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.0.1", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Minor changes to editor window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Small modifications to example scene.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 2.0", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Added a new in-engine documentation window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed old files from the previous example scene.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new 2D example assets for the new example scene.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Created new scripts to help show how to use the Ultimate Button more effectively.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Overall improvement to performance.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Improved overall performance.", paragraphStyle );

		EditorGUILayout.EndScrollView();
	}

	void ThankYou ()
	{
		thankYou.scrollPosition = EditorGUILayout.BeginScrollView( thankYou.scrollPosition, false, false, docSize );

		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "We here at Tank & Healer Studio would like to thank you for purchasing the Ultimate Button asset package from the Unity Asset Store. If you have any questions about this product please don't hesitate to contact us at: ", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "    We hope that the Ultimate Button will be a great help to you in the development of your game. After pressing the continue button below, you will be presented with helpful information on this asset to assist you in implementing Ultimate Button into your project.\n", paragraphStyle );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", GUILayout.Height( 30 ) );
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
		
		EditorGUILayout.LabelField( "  Thank you for downloading the most recent version of the Ultimate Button. This most recent update was huge, and there were many things that were updated, and some things were completely removed! Please check out the sections below to see all the important changes that have been made. As always, if you run into any issues with the Ultimate Button, please contact us at:", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "GENERAL CHANGES", itemHeaderStyle );
		EditorGUILayout.LabelField( "  Some folder structure and existing functionality has been updated and improved. None of these changes should affect any existing use of the Ultimate Button.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the Touch Actions section. All options previously located in the Touch Actions section are now located in the Style and Options section.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Expanded the functionality of using the Ultimate Button in your scripts. Added a new section titled Button Events. Now you can use either the Script Reference or the Button Events section to implement into your scripts.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed example files from the Plugins folder. All example files will now be in the folder named: Ultimate Button Examples.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added four new Ultimate Button textures that can be used in your projects.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the Ultimate Button PSD from the project files.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Improved Tension Accent functionality.", paragraphStyle );
		
		GUILayout.Space( 10 );

		EditorGUILayout.LabelField( "NEW FUNCTIONS", itemHeaderStyle );
		EditorGUILayout.LabelField( "  Some new functions have been added to help reference the Ultimate Button more efficiently. For information on what each new function does, please refer to the Documentation section of this help window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • UltimateButton GetUltimateButton()", paragraphStyle );

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
	class UltimateButtonInitialLoad
	{
		static UltimateButtonInitialLoad ()
		{
			// If the user has a older version of Ultimate Button that used the bool for startup...
			if( EditorPrefs.HasKey( "UltimateButtonStartup" ) && !EditorPrefs.HasKey( "UltimateButtonVersion" ) )
			{
				// Set the new pref to 0 so that the pref will exist and the version changes will be shown.
				EditorPrefs.SetInt( "UltimateButtonVersion", 0 );
			}

			// If this is the first time that the user has downloaded the Ultimate Button...
			if( !EditorPrefs.HasKey( "UltimateButtonVersion" ) )
			{
				// Set the current menu to the thank you page.
				NavigateForward( thankYou );

				// Set the version to current so they won't see these version changes.
				EditorPrefs.SetInt( "UltimateButtonVersion", importantChanges );

				EditorApplication.update += WaitForCompile;
			}
			else if( EditorPrefs.GetInt( "UltimateButtonVersion" ) < importantChanges )
			{
				// Set the current menu to the version changes page.
				NavigateForward( versionChanges );

				// Set the version to current so they won't see this page again.
				EditorPrefs.SetInt( "UltimateButtonVersion", importantChanges );

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