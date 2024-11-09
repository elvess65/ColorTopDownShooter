/* Written by Kaz Crowe */
/* UltimateJoystickEditor.cs */
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.AnimatedValues;

[CanEditMultipleObjects]
[CustomEditor( typeof( UltimateJoystick ) )]
public class UltimateJoystickEditor : Editor
{
	UltimateJoystick targ;

	/* -----< ASSIGNED VARIABLES >----- */
	SerializedProperty joystick, joystickSizeFolder;
	SerializedProperty highlightBase, highlightJoystick;
	SerializedProperty tensionAccentUp, tensionAccentDown;
	SerializedProperty tensionAccentLeft, tensionAccentRight;
	SerializedProperty joystickAnimator, joystickBase;
	
	/* -----< SIZE AND PLACEMENT >----- */
	SerializedProperty scalingAxis, anchor, joystickTouchSize;
	SerializedProperty customTouchSize_X, customTouchSize_Y;
	SerializedProperty customTouchSizePos_X, customTouchSizePos_Y;
	SerializedProperty dynamicPositioning;
	SerializedProperty joystickSize, radiusModifier;
	SerializedProperty customSpacing_X, customSpacing_Y;
	
	/* -----< STYLES AND OPTIONS >----- */
	SerializedProperty disableVisuals, throwable, draggable;
	SerializedProperty throwDuration;
	SerializedProperty showHighlight, showTension;
	SerializedProperty highlightColor, tensionColorNone, tensionColorFull;
	SerializedProperty axis, boundary;
	SerializedProperty xDeadZone, yDeadZone, deadZoneOption;
	SerializedProperty useAnimation, useFade;
	SerializedProperty tapCountOption, tapCountDuration;
	SerializedProperty targetTapCount;
	SerializedProperty fadeUntouched, fadeTouched;
	
	/* ------< SCRIPT REFERENCE >------ */
	SerializedProperty joystickName, exposeValues;
	SerializedProperty horizontalValue, verticalValue;
	
	/* // ----< ANIMATED SECTIONS >---- \\ */
	AnimBool AssignedVariables, SizeAndPlacement, StyleAndOptions;
	AnimBool TouchActions, ScriptReference;

	/* // ----< ANIMATED VARIABLE >---- \\ */
	AnimBool customTouchSizeOption, throwableOption;
	AnimBool highlightOption, tensionOption;
	AnimBool dzOneValueOption, dzTwoValueOption;
	AnimBool tcOption, tcTargetTapOption;
	AnimBool animationOption, fadeOption;
	AnimBool joystickNameUnassigned, joystickNameAssigned;
	AnimBool exposeValuesBool;

	public enum ScriptCast{ vector2, distance, horizontalAxis, verticalAxis, getJoystickState, getTapCount }
	ScriptCast scriptCast;

	SerializedProperty fadeInDuration, fadeOutDuration;

	Canvas parentCanvas;
	
	
	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();
		
		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;
		
		parentCanvas = GetParentCanvas();
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback function from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	Canvas GetParentCanvas ()
	{
		if( Selection.activeGameObject == null )
			return null;

		// Store the current parent.
		Transform parent = Selection.activeGameObject.transform.parent;

		// Loop through parents as long as there is one.
		while( parent != null )
		{
			// If there is a Canvas component, return the component.
			if( parent.transform.GetComponent<Canvas>() && parent.transform.GetComponent<Canvas>().enabled == true )
				return parent.transform.GetComponent<Canvas>();
			
			// Else, shift to the next parent.
			parent = parent.transform.parent;
		}
		if( parent == null && PrefabUtility.GetPrefabType( Selection.activeGameObject ) != PrefabType.Prefab )
			UltimateJoystickCreator.RequestCanvas( Selection.activeGameObject );

		return null;
	}

	// Function called for Undo/Redo operations.
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	// Function called to display an interactive header.
	void DisplayHeader ( string headerName, string editorPref, AnimBool targetAnim )
	{
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( headerName, EditorStyles.boldLabel );
		if( GUILayout.Button( EditorPrefs.GetBool( editorPref ) == true ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
		{
			EditorPrefs.SetBool( editorPref, EditorPrefs.GetBool( editorPref ) == true ? false : true );
			targetAnim.target = EditorPrefs.GetBool( editorPref );
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	bool CanvasErrors ()
	{
		// If the selection is currently empty, then return false.
		if( Selection.activeGameObject == null )
			return false;

		// If the selection is actually the prefab within the Project window, then return no errors.
		if( PrefabUtility.GetPrefabType( Selection.activeGameObject ) == PrefabType.Prefab )
			return false;

		// If parentCanvas is unassigned, then get a new canvas and return no errors.
		if( parentCanvas == null )
		{
			parentCanvas = GetParentCanvas();
			return false;
		}

		// If the parentCanvas is not enabled, then return true for errors.
		if( parentCanvas.enabled == false )
			return true;

		// If the canvas' renderMode is not the needed one, then return true for errors.
		if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			return true;

		// If the canvas has a CanvasScaler component and it is not the correct option.
		if( parentCanvas.GetComponent<CanvasScaler>() && parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
			return true;

		return false;
	}

	void ImageNotDisplayedWarning ( string message )
	{
		GUIStyle style = new GUIStyle( GUI.skin.label );
		style.fontSize = 10;
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( message, style, GUILayout.Width( 100 ) );
		EditorGUILayout.EndHorizontal();
	}
	
	/*
	For more information on the OnInspectorGUI and adding your own variables
	in the UltimateJoystick.cs script and displaying them in this script,
	see the EditorGUILayout section in the Unity Documentation to help out.
	*/
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.Space();

		#region ERROR CHECK
		if( CanvasErrors() == true )
		{
			if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "The parent Canvas needs to be set to 'Screen Space - Overlay' in order for the Ultimate Joystick to function correctly.", MessageType.Error );
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "Update Canvas", EditorStyles.miniButtonLeft ) )
				{
					parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
					parentCanvas = GetParentCanvas();
				}
				if( GUILayout.Button( "Update Joystick", EditorStyles.miniButtonRight ) )
				{
					UltimateJoystickCreator.RequestCanvas( Selection.activeGameObject );
					parentCanvas = GetParentCanvas();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			if( parentCanvas.GetComponent<CanvasScaler>() )
			{
				if( parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
				{
					EditorGUILayout.BeginVertical( "Box" );
					EditorGUILayout.HelpBox( "The Canvas Scaler component located on the parent Canvas needs to be set to 'Constant Pixel Size' in order for the Ultimate Joystick to function correctly.", MessageType.Error );
					EditorGUILayout.BeginHorizontal();
					if( GUILayout.Button( "Update Canvas", EditorStyles.miniButtonLeft ) )
					{
						parentCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
						parentCanvas = GetParentCanvas();
						UltimateJoystick joystick = ( UltimateJoystick )target;
						joystick.UpdatePositioning();
					}
					if( GUILayout.Button( "Update Joystick", EditorStyles.miniButtonRight ) )
					{
						UltimateJoystickCreator.RequestCanvas( Selection.activeGameObject );
						parentCanvas = GetParentCanvas();
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
			}
			return;
		}
		#endregion
		
		#region ASSIGNED VARIABLES
		/* ----------------------------------------< ** ASSIGNED VARIABLES ** >---------------------------------------- */
		DisplayHeader( "Assigned Variables", "UUI_Variables", AssignedVariables );
		if( EditorGUILayout.BeginFadeGroup( AssignedVariables.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.indentLevel = 1;
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystick );
			if( targ.joystick == null )
				ImageNotDisplayedWarning( "▲ Required" );

			EditorGUILayout.PropertyField( joystickSizeFolder, new GUIContent( "Size Folder" ) );
			if( targ.joystickSizeFolder == null )
				ImageNotDisplayedWarning( "▲ Required" );

			EditorGUILayout.PropertyField( joystickBase );
			if( targ.joystickBase == null )
				ImageNotDisplayedWarning( "▲ Required" );

			EditorGUI.indentLevel = 0;
			
			if( EditorGUILayout.BeginFadeGroup( highlightOption.faded ) )
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField( "Highlight Variables", EditorStyles.boldLabel );
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( highlightBase );
				if( targ.highlightBase == null )
					ImageNotDisplayedWarning( "Not Displayed" );

				EditorGUILayout.PropertyField( highlightJoystick );
				if( targ.highlightJoystick == null )
					ImageNotDisplayedWarning( "Not Displayed" );

				EditorGUI.indentLevel = 0;
			}
			if( AssignedVariables.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			
			if( EditorGUILayout.BeginFadeGroup( tensionOption.faded ) )
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField( "Tension Variables", EditorStyles.boldLabel );
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( tensionAccentUp, new GUIContent( "Tension Up" ) );
				if( targ.tensionAccentUp == null )
					ImageNotDisplayedWarning( "Not Displayed" );

				EditorGUILayout.PropertyField( tensionAccentDown, new GUIContent( "Tension Down" ) );
				if( targ.tensionAccentDown == null )
					ImageNotDisplayedWarning( "Not Displayed" );

				EditorGUILayout.PropertyField( tensionAccentLeft, new GUIContent( "Tension Left" ) );
				if( targ.tensionAccentLeft == null )
					ImageNotDisplayedWarning( "Not Displayed" );

				EditorGUILayout.PropertyField( tensionAccentRight, new GUIContent( "Tension Right" ) );
				if( targ.tensionAccentRight == null )
					ImageNotDisplayedWarning( "Not Displayed" );

				EditorGUI.indentLevel = 0;
			}
			if( AssignedVariables.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			
			if( EditorGUILayout.BeginFadeGroup( animationOption.faded ) )
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField( "Animator", EditorStyles.boldLabel );
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField( joystickAnimator );
				if( targ.joystickAnimator == null )
					ImageNotDisplayedWarning( "▲ Required" );
				EditorGUI.indentLevel = 0;
			}
			if( AssignedVariables.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();
		/* --------------------------------------< ** END ASSIGNED VARIABLES ** >-------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();
		
		#region SIZE AND PLACEMENT
		/* ----------------------------------------< ** SIZE AND PLACEMENT ** >---------------------------------------- */
		DisplayHeader( "Size and Placement", "UUI_SizeAndPlacement", SizeAndPlacement );
		if( EditorGUILayout.BeginFadeGroup( SizeAndPlacement.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( scalingAxis, new GUIContent( "Scaling Axis", "The axis to scale the Ultimate Joystick from." ) );
			EditorGUILayout.PropertyField( anchor, new GUIContent( "Anchor", "The side of the screen that the\njoystick will be anchored to." ) );
			EditorGUILayout.PropertyField( joystickTouchSize, new GUIContent( "Touch Size", "The size of the area in which\nthe touch can be initiated." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				if( targ.joystickTouchSize == UltimateJoystick.JoystickTouchSize.Custom )
					customTouchSizeOption.target = true;
				else
					customTouchSizeOption.target = false;
			}
			if( EditorGUILayout.BeginFadeGroup( customTouchSizeOption.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Touch Size Customization" );
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				{
					EditorGUILayout.Slider( customTouchSize_X, 0.0f, 100.0f, new GUIContent( "Width", "The width of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSize_Y, 0.0f, 100.0f, new GUIContent( "Height", "The height of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSizePos_X, 0.0f, 100.0f, new GUIContent( "X Position", "The x position of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSizePos_Y, 0.0f, 100.0f, new GUIContent( "Y Position", "The y position of the Joystick Touch Area." ) );
				}
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( SizeAndPlacement.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( dynamicPositioning, new GUIContent( "Dynamic Positioning", "Moves the joystick to the position of the initial touch." ) );
			EditorGUILayout.Slider( joystickSize, 1.0f, 4.0f, new GUIContent( "Joystick Size", "The overall size of the joystick." ) );
			EditorGUILayout.Slider( radiusModifier, 2.0f, 7.0f, new GUIContent( "Radius", "Determines how far the joystick can\nmove visually from the center." ) );
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.LabelField( "Joystick Position" );
			EditorGUI.indentLevel = 1;
			EditorGUILayout.Slider( customSpacing_X, 0.0f, 50.0f, new GUIContent( "X Position:", "The horizontal position of the joystick." ) );
			EditorGUILayout.Slider( customSpacing_Y, 0.0f, 100.0f, new GUIContent( "Y Position:", "The vertical position of the joystick." ) );
			EditorGUI.indentLevel = 0;
			GUILayout.Space( 1 );
			EditorGUILayout.EndVertical();
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();
		/* --------------------------------------< ** END SIZE AND PLACEMENT ** >-------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();

		#region STYLE AND OPTIONS
		/* ----------------------------------------< ** STYLE AND OPTIONS ** >----------------------------------------- */
		DisplayHeader( "Style and Options", "UUI_StyleAndOptions", StyleAndOptions );
		if( EditorGUILayout.BeginFadeGroup( StyleAndOptions.faded ) )
		{
			EditorGUILayout.Space();
			
			// -----------------------< DISABLE VISUALS >---------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( disableVisuals, new GUIContent( "Disable Visuals", "Disables the visuals of the joystick." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				if( targ.disableVisuals == true )
				{
					showHighlight.boolValue = false;
					showTension.boolValue = false;
					useFade.boolValue = false;
					useAnimation.boolValue = false;
					serializedObject.ApplyModifiedProperties();
					highlightOption.target = false;
					tensionOption.target = false;
					fadeOption.target = false;
					animationOption.target = false;
				}

				SetDisableVisuals( targ );
				SetHighlight( targ );
				SetTensionAccent( targ );
				SetAnimation( targ );
			}
			
			if( targ.disableVisuals == true && targ.joystickBase == null )
				EditorGUILayout.HelpBox( "Joystick Base needs to be assigned in the Assigned Variables section.", MessageType.Error );
			// ---------------------< END DISABLE VISUALS >-------------------- //

			EditorGUI.BeginDisabledGroup( targ.disableVisuals == true );// This is the start of the disabled fields if the user is using the disableVisuals option.

			// --------------------------< USE FADE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( useFade, new GUIContent( "Use Fade", "Fades the joystick visuals when interacted with." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				if( targ.useFade == true )
					targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
				else
					targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;

				fadeOption.target = targ.useFade;
			}
			if( EditorGUILayout.BeginFadeGroup( fadeOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( fadeUntouched, 0.0f, 1.0f, new GUIContent( "Fade Untouched", "The alpha of the joystick when it is NOT receiving input." ) );
				EditorGUILayout.Slider( fadeTouched, 0.0f, 1.0f, new GUIContent( "Fade Touched", "The alpha of the joystick when receiving input." ) );
				EditorGUILayout.PropertyField( fadeInDuration );
				EditorGUILayout.PropertyField( fadeOutDuration );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
				}
				
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			// ------------------------< END USE FADE >------------------------ //

			// -----------------------< USE ANIMATION >------------------------ //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( useAnimation, new GUIContent( "Use Animation", "Plays animations in reaction to input." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetAnimation( targ );
				animationOption.target = targ.useAnimation;
			}
			if( targ.useAnimation == true )
			{
				EditorGUI.indentLevel = 1;
				if( targ.joystickAnimator == null )
					EditorGUILayout.HelpBox( "Joystick Animator needs to be assigned.", MessageType.Error );
				EditorGUI.indentLevel = 0;
			}
			// ----------------------< END USE ANIMATION >---------------------- //
			
			// --------------------------< HIGHLIGHT >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( showHighlight, new GUIContent( "Show Highlight", "Displays the highlight images with the Highlight Color variable." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetHighlight( targ );
				highlightOption.target = targ.showHighlight;
			}
			
			if( EditorGUILayout.BeginFadeGroup( highlightOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( highlightColor );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateHighlightColor( targ.highlightColor );

					// For every highlight image that is assigned, set the object to dirty so the properties will be applied. This is needed for prefab instances.
					if( targ.highlightBase != null )
						EditorUtility.SetDirty( targ.highlightBase );
					if( targ.highlightJoystick != null )
						EditorUtility.SetDirty( targ.highlightJoystick );
				}
				
				if( targ.highlightBase == null && targ.highlightJoystick == null )
					EditorGUILayout.HelpBox( "No highlight images have been assigned. Please assign some highlight images before continuing.", MessageType.Error );
				
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			// ------------------------< END HIGHLIGHT >------------------------ //

			// ---------------------------< TENSION >--------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( showTension, new GUIContent( "Show Tension", "Displays the visual direction of the joystick using the tension color options." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetTensionAccent( targ );
				tensionOption.target = targ.showTension;
			}
			
			if( EditorGUILayout.BeginFadeGroup( tensionOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( tensionColorNone, new GUIContent( "Tension None", "The color displayed when the joystick\nis closest to center." ) );
				EditorGUILayout.PropertyField( tensionColorFull, new GUIContent( "Tension Full", "The color displayed when the joystick\nis at the furthest distance." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					TensionAccentReset( targ );

					// For every tension accent that is assigned, set it dirty to apply the color properties. This is needed for applying properties to prefab instances.
					if( targ.tensionAccentUp != null )
						EditorUtility.SetDirty( targ.tensionAccentUp );
					if( targ.tensionAccentDown != null )
						EditorUtility.SetDirty( targ.tensionAccentDown );
					if( targ.tensionAccentLeft != null )
						EditorUtility.SetDirty( targ.tensionAccentLeft );
					if( targ.tensionAccentRight != null )
						EditorUtility.SetDirty( targ.tensionAccentRight );
				}

				if( targ.tensionAccentUp == null && targ.tensionAccentDown == null && targ.tensionAccentLeft == null && targ.tensionAccentRight == null )
					EditorGUILayout.HelpBox( "No tension accent images have been assigned. Please assign some images before continuing.", MessageType.Error );

				EditorGUI.indentLevel = 0;
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			// -------------------------< END TENSION >------------------------- //

			EditorGUI.EndDisabledGroup();// This is the end for the Touch Pad option.

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space( 8 );
			EditorGUILayout.LabelField( "───────" );
			EditorGUILayout.EndHorizontal();

			// --------------------------< THROWABLE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( throwable, new GUIContent( "Throwable", "Smoothly transitions the joystick back to\ncenter when the input is released." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				throwableOption.target = targ.throwable;
			}
			
			if( EditorGUILayout.BeginFadeGroup( throwableOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( throwDuration, 0.05f, 1.0f, new GUIContent( "Throw Duration", "Time in seconds to return to center." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			// ------------------------< END THROWABLE >------------------------ //
			
			// --------------------------< DRAGGABLE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( draggable, new GUIContent( "Draggable", "Drags the joystick to follow the touch if it is farther than the radius." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			
			if( targ.draggable == true && targ.boundary == UltimateJoystick.Boundary.Square )
				EditorGUILayout.HelpBox( "Draggable option will force the boundary to being circular. " +
				                        "Please use a circular boundary when using the draggable option.", MessageType.Warning );
			// ------------------------< END DRAGGABLE >------------------------ //
			
			// -----------------------< AXIS AND BOUNDARY >--------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( axis, new GUIContent( "Axis", "Constrains the joystick to a certain axis." ) );
			EditorGUILayout.PropertyField( boundary, new GUIContent( "Boundary", "Determines how the joystick's position is clamped." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// ---------------------< END AXIS AND BOUNDARY >------------------- //
			
			// --------------------------< DEAD ZONE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( deadZoneOption, new GUIContent( "Dead Zone", "Forces the joystick position to being only values of -1, 0, and 1." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				dzOneValueOption.target = targ.deadZoneOption == UltimateJoystick.DeadZoneOption.OneValue ? true : false;
				dzTwoValueOption.target = targ.deadZoneOption == UltimateJoystick.DeadZoneOption.TwoValues ? true : false;
			}
			EditorGUI.indentLevel = 1;
			EditorGUI.BeginChangeCheck();
			if( EditorGUILayout.BeginFadeGroup( dzTwoValueOption.faded ) )
			{
				EditorGUILayout.Slider( xDeadZone, 0.0f, 1.0f, new GUIContent( "X Dead Zone", "X values within this range will be forced to 0." ) );
				EditorGUILayout.Slider( yDeadZone, 0.0f, 1.0f, new GUIContent( "Y Dead Zone", "Y values within this range will be forced to 0." ) );
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			
			if( EditorGUILayout.BeginFadeGroup( dzOneValueOption.faded ) )
			{
				EditorGUILayout.Slider( xDeadZone, 0.0f, 1.0f, new GUIContent( "Dead Zone", "Values within this range will be forced to 0." ) );
				targ.yDeadZone = targ.xDeadZone;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.indentLevel = 0;
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// ------------------------< END DEAD ZONE >------------------------ //
			
			// TAP COUNT //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( tapCountOption, new GUIContent( "Tap Count", "Allows the joystick to calculate double taps and a touch and release within a certain time window." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				tcOption.target = targ.tapCountOption != UltimateJoystick.TapCountOption.NoCount ? true : false;
				tcTargetTapOption.target = targ.tapCountOption == UltimateJoystick.TapCountOption.Accumulate ? true : false;
			}

			if( EditorGUILayout.BeginFadeGroup( tcOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( tapCountDuration, 0.0f, 1.0f, new GUIContent( "Tap Time Window", "Time in seconds that the joystick can receive taps." ) );
				if( EditorGUILayout.BeginFadeGroup( tcTargetTapOption.faded ) )
					EditorGUILayout.IntSlider( targetTapCount, 1, 5, new GUIContent( "Target Tap Count", "How many taps to activate the Tap Count Event?" ) );
				if( TouchActions.faded == 1 )
					EditorGUILayout.EndFadeGroup();

				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				
				EditorGUI.indentLevel = 0;
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
		}
		EditorGUILayout.EndFadeGroup();
		/* ----------------------------------------< ** END STYLE AND OPTIONS ** >--------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();
		
		#region SCRIPT REFERENCE
		/* ------------------------------------------< ** SCRIPT REFERENCE ** >------------------------------------------- */
		DisplayHeader( "Script Reference", "UUI_ScriptReference", ScriptReference );
		if( EditorGUILayout.BeginFadeGroup( ScriptReference.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystickName, new GUIContent( "Joystick Name", "The name of the targeted joystick used for static referencing." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				if( targ.joystickName == string.Empty )
				{
					joystickNameUnassigned.target = true;
					joystickNameAssigned.target = false;
				}
				else
				{
					joystickNameUnassigned.target = false;
					joystickNameAssigned.target = true;
				}
			}

			if( EditorGUILayout.BeginFadeGroup( joystickNameUnassigned.faded ) )
			{
				EditorGUILayout.HelpBox( "Please assign a Joystick Name in order to be able to get this joystick's position dynamically.", MessageType.Warning );
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			if( EditorGUILayout.BeginFadeGroup( joystickNameAssigned.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Example Code:", EditorStyles.boldLabel );
				scriptCast = ( ScriptCast )EditorGUILayout.EnumPopup( "Joystick Use: ", scriptCast );
				GUILayout.Space( 5 );
				if( scriptCast == ScriptCast.vector2 )
					EditorGUILayout.TextField( "UltimateJoystick.GetPosition( \"" + targ.joystickName + "\" )" );
				else if( scriptCast == ScriptCast.distance )
					EditorGUILayout.TextField( "UltimateJoystick.GetDistance( \"" + targ.joystickName + "\" )" );
				else if( scriptCast == ScriptCast.horizontalAxis )
					EditorGUILayout.TextField( "UltimateJoystick.GetHorizontalAxis( \"" + targ.joystickName + "\" )" );
				else if( scriptCast == ScriptCast.verticalAxis )
					EditorGUILayout.TextField( "UltimateJoystick.GetVerticalAxis( \"" + targ.joystickName + "\" )" );
				else if( scriptCast == ScriptCast.getJoystickState )
					EditorGUILayout.TextField( "UltimateJoystick.GetJoystickState( \"" + targ.joystickName + "\" )" );
				else
					EditorGUILayout.TextField( "UltimateJoystick.GetTapCount( \"" + targ.joystickName + "\" )" );
				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( exposeValues, new GUIContent( "Expose Values", "Should this script expose it's values for certain game making plugins? This option does not effect performance of other references." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				exposeValuesBool.target = targ.exposeValues;
			}

			if( EditorGUILayout.BeginFadeGroup( exposeValuesBool.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Current Position:", EditorStyles.boldLabel );
				EditorGUILayout.LabelField( "Horizontal Value: " + targ.horizontalValue.ToString( "F2" ) );
				EditorGUILayout.LabelField( "Vertical Value: " + targ.verticalValue.ToString( "F2" ) );
				EditorGUILayout.EndVertical();
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
		}
		EditorGUILayout.EndFadeGroup();
		/* -----------------------------------------< ** END SCRIPT REFERENCE ** >---------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();
		
		/* ----------------------------------------------< ** HELP TIPS ** >---------------------------------------------- */
		if( targ.joystick == null )
			EditorGUILayout.HelpBox( "Joystick needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		if( targ.joystickSizeFolder == null )
			EditorGUILayout.HelpBox( "Joystick Size Folder needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		if( targ.joystickBase == null )
			EditorGUILayout.HelpBox( "Joystick Base needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		/* --------------------------------------------< ** END HELP TIPS ** >-------------------------------------------- */

		Repaint();
	}
	
	// This function stores the references to the variables of the target.
	void StoreReferences ()
	{
		targ = ( UltimateJoystick )target;

		/* -----< ASSIGNED VARIABLES >----- */
		joystick = serializedObject.FindProperty( "joystick" );
		joystickSizeFolder = serializedObject.FindProperty( "joystickSizeFolder" );
		joystickBase = serializedObject.FindProperty( "joystickBase" );
		highlightBase = serializedObject.FindProperty( "highlightBase" );
		highlightJoystick = serializedObject.FindProperty( "highlightJoystick" );
		tensionAccentUp = serializedObject.FindProperty( "tensionAccentUp" );
		tensionAccentDown = serializedObject.FindProperty( "tensionAccentDown" );
		tensionAccentLeft = serializedObject.FindProperty( "tensionAccentLeft" );
		tensionAccentRight = serializedObject.FindProperty( "tensionAccentRight" );
		joystickAnimator = serializedObject.FindProperty( "joystickAnimator" );
		
		/* -----< SIZE AND PLACEMENT >----- */
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		anchor = serializedObject.FindProperty( "anchor" );
		joystickTouchSize = serializedObject.FindProperty( "joystickTouchSize" );
		customTouchSize_X = serializedObject.FindProperty( "customTouchSize_X" );
		customTouchSize_Y = serializedObject.FindProperty( "customTouchSize_Y" );
		customTouchSizePos_X = serializedObject.FindProperty( "customTouchSizePos_X" );
		customTouchSizePos_Y = serializedObject.FindProperty( "customTouchSizePos_Y" );
		dynamicPositioning = serializedObject.FindProperty( "dynamicPositioning" );
		joystickSize = serializedObject.FindProperty( "joystickSize" );
		radiusModifier = serializedObject.FindProperty( "radiusModifier" );
		customSpacing_X = serializedObject.FindProperty( "customSpacing_X" );
		customSpacing_Y = serializedObject.FindProperty( "customSpacing_Y" );
		
		/* -----< STYLES AND OPTIONS >----- */
		disableVisuals = serializedObject.FindProperty( "disableVisuals" );
		throwable = serializedObject.FindProperty( "throwable" );
		draggable = serializedObject.FindProperty( "draggable" );
		throwDuration = serializedObject.FindProperty( "throwDuration" );
		showHighlight = serializedObject.FindProperty( "showHighlight" );
		highlightColor = serializedObject.FindProperty( "highlightColor" );
		showTension = serializedObject.FindProperty( "showTension" );
		tensionColorNone = serializedObject.FindProperty( "tensionColorNone" );
		tensionColorFull = serializedObject.FindProperty( "tensionColorFull" );
		axis = serializedObject.FindProperty( "axis" );
		boundary = serializedObject.FindProperty( "boundary" );
		deadZoneOption = serializedObject.FindProperty( "deadZoneOption" );
		xDeadZone = serializedObject.FindProperty( "xDeadZone" );
		yDeadZone = serializedObject.FindProperty( "yDeadZone" );
		
		/* --------< TOUCH ACTION >-------- */
		useAnimation = serializedObject.FindProperty( "useAnimation" );
		useFade = serializedObject.FindProperty( "useFade" );
		tapCountOption = serializedObject.FindProperty( "tapCountOption" );
		tapCountDuration = serializedObject.FindProperty( "tapCountDuration" );
		targetTapCount = serializedObject.FindProperty( "targetTapCount" );
		fadeUntouched = serializedObject.FindProperty( "fadeUntouched" );
		fadeTouched = serializedObject.FindProperty( "fadeTouched" );
		fadeInDuration = serializedObject.FindProperty( "fadeInDuration" );
		fadeOutDuration = serializedObject.FindProperty( "fadeOutDuration" );

		/* ------< SCRIPT REFERENCE >------ */
		joystickName = serializedObject.FindProperty( "joystickName" );
		exposeValues = serializedObject.FindProperty( "exposeValues" );
		
		/* // ----< ANIMATED SECTIONS >---- \\ */
		AssignedVariables = new AnimBool( EditorPrefs.GetBool( "UUI_Variables" ) );
		SizeAndPlacement = new AnimBool( EditorPrefs.GetBool( "UUI_SizeAndPlacement" ) );
		StyleAndOptions = new AnimBool( EditorPrefs.GetBool( "UUI_StyleAndOptions" ) );
		TouchActions = new AnimBool( EditorPrefs.GetBool( "UUI_TouchActions" ) );
		ScriptReference = new AnimBool( EditorPrefs.GetBool( "UUI_ScriptReference" ) );
		
		/* // ----< ANIMATED VARIABLES >---- \\ */
		customTouchSizeOption = new AnimBool( targ.joystickTouchSize == UltimateJoystick.JoystickTouchSize.Custom );
		throwableOption = new AnimBool( targ.throwable );
		highlightOption = new AnimBool( targ.showHighlight );
		tensionOption = new AnimBool( targ.showTension );
		dzOneValueOption = new AnimBool( targ.deadZoneOption == UltimateJoystick.DeadZoneOption.OneValue );
		dzTwoValueOption = new AnimBool( targ.deadZoneOption == UltimateJoystick.DeadZoneOption.TwoValues );
		tcOption = new AnimBool( targ.tapCountOption != UltimateJoystick.TapCountOption.NoCount );
		tcTargetTapOption = new AnimBool( targ.tapCountOption == UltimateJoystick.TapCountOption.Accumulate );
		animationOption = new AnimBool( targ.useAnimation );
		fadeOption = new AnimBool( targ.useFade );

		joystickNameUnassigned = new AnimBool( targ.joystickName == string.Empty );
		joystickNameAssigned = new AnimBool( targ.joystickName != string.Empty );
		exposeValuesBool = new AnimBool( targ.exposeValues );

		SetDisableVisuals( targ );
		SetHighlight( targ );
		SetAnimation( targ );
		SetTensionAccent( targ );

		if( targ.useFade == true )
		{
			if( !targ.GetComponent<CanvasGroup>() )
				targ.gameObject.AddComponent<CanvasGroup>();

			targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
		}
		else
		{
			if( !targ.GetComponent<CanvasGroup>() )
				targ.gameObject.AddComponent<CanvasGroup>();

			targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
		}
	}
	
	void SetDisableVisuals ( UltimateJoystick targ )
	{
		if( targ.disableVisuals == true )
		{
			if( targ.showHighlight == true )
				targ.showHighlight = false;
			if( targ.showTension == true )
				targ.showTension = false;

			if( targ.joystickBase != null && targ.joystickBase.GetComponent<Image>().enabled == true )
				targ.joystickBase.GetComponent<Image>().enabled = false;

			if( targ.joystick != null && targ.joystick.GetComponent<Image>().enabled == true )
				targ.joystick.GetComponent<Image>().enabled = false;
		}
		else
		{
			if( targ.joystickBase != null && targ.joystickBase.GetComponent<Image>().enabled == false )
				targ.joystickBase.GetComponent<Image>().enabled = true;
			if( targ.joystick != null && targ.joystick.GetComponent<Image>().enabled == false )
				targ.joystick.GetComponent<Image>().enabled = true;
		}
	}

	void SetHighlight ( UltimateJoystick targ )
	{
		if( targ.showHighlight == true )
		{
			if( targ.highlightBase != null && targ.highlightBase.gameObject.activeInHierarchy == false )
				targ.highlightBase.gameObject.SetActive( true );
			if( targ.highlightJoystick != null && targ.highlightJoystick.gameObject.activeInHierarchy == false )
				targ.highlightJoystick.gameObject.SetActive( true );

			targ.UpdateHighlightColor( targ.highlightColor );
		}
		else
		{
			if( targ.highlightBase != null && targ.highlightBase != targ.joystickBase.GetComponent<Image>() && targ.highlightBase.gameObject.activeInHierarchy == true )
				targ.highlightBase.gameObject.SetActive( false );
			if( targ.highlightJoystick != null && targ.highlightJoystick != targ.joystick.GetComponent<Image>() && targ.highlightJoystick.gameObject.activeInHierarchy == true )
				targ.highlightJoystick.gameObject.SetActive( false );
		}
	}
	
	void SetTensionAccent ( UltimateJoystick targ )
	{
		if( targ.showTension == true )
		{	
			if( targ.tensionAccentUp != null && targ.tensionAccentUp.gameObject.activeInHierarchy == false )
				targ.tensionAccentUp.gameObject.SetActive( true );
			if( targ.tensionAccentDown != null && targ.tensionAccentDown.gameObject.activeInHierarchy == false )
				targ.tensionAccentDown.gameObject.SetActive( true );
			if( targ.tensionAccentLeft != null && targ.tensionAccentLeft.gameObject.activeInHierarchy == false )
				targ.tensionAccentLeft.gameObject.SetActive( true );
			if( targ.tensionAccentRight != null && targ.tensionAccentRight.gameObject.activeInHierarchy == false )
				targ.tensionAccentRight.gameObject.SetActive( true );
			
			TensionAccentReset( targ );
		}
		else
		{
			if( targ.tensionAccentUp != null && targ.tensionAccentUp.gameObject.activeInHierarchy == true )
				targ.tensionAccentUp.gameObject.SetActive( false );
			if( targ.tensionAccentDown != null && targ.tensionAccentDown.gameObject.activeInHierarchy == true )
				targ.tensionAccentDown.gameObject.SetActive( false );
			if( targ.tensionAccentLeft != null && targ.tensionAccentLeft.gameObject.activeInHierarchy == true )
				targ.tensionAccentLeft.gameObject.SetActive( false );
			if( targ.tensionAccentRight != null && targ.tensionAccentRight.gameObject.activeInHierarchy == true )
				targ.tensionAccentRight.gameObject.SetActive( false );
		}
	}

	void TensionAccentReset ( UltimateJoystick targ )
	{
		if( targ.tensionAccentUp != null )
			targ.tensionAccentUp.color = targ.tensionColorNone;

		if( targ.tensionAccentDown != null )
			targ.tensionAccentDown.color = targ.tensionColorNone;

		if( targ.tensionAccentLeft != null )
			targ.tensionAccentLeft.color = targ.tensionColorNone;

		if( targ.tensionAccentRight != null )
			targ.tensionAccentRight.color = targ.tensionColorNone;
	}
	
	void SetAnimation ( UltimateJoystick targ )
	{
		if( targ.useAnimation == true )
		{
			if( targ.joystickAnimator != null && targ.joystickAnimator.enabled == false )
				targ.joystickAnimator.enabled = true;
		}
		else
		{
			if( targ.joystickAnimator != null && targ.joystickAnimator.enabled == true )
				targ.joystickAnimator.enabled = false;
		}
	}
}

/* Written by Kaz Crowe */
/* UltimateJoystickCreator.cs */
public class UltimateJoystickCreator
{
	[MenuItem( "GameObject/Ultimate UI/Ultimate Joystick", false, 0 )]
	private static void CreateUltimateJoystick ()
	{
		GameObject joystickPrefab = EditorGUIUtility.Load( "Ultimate Joystick/UltimateJoystick.prefab" ) as GameObject;

		if( joystickPrefab == null )
		{
			Debug.LogError( "Could not find 'UltimateJoystick.prefab' in any Editor Default Resources folders." );
			return;
		}
		CreateNewUI( joystickPrefab );
	}
	
	private static void CreateNewUI ( Object objectPrefab )
	{
		GameObject prefab = ( GameObject )Object.Instantiate( objectPrefab, Vector3.zero, Quaternion.identity );
		prefab.name = objectPrefab.name;
		Selection.activeGameObject = prefab;
		RequestCanvas( prefab );
	}

	private static void CreateNewCanvas ( GameObject child )
	{
		GameObject root = new GameObject( "Ultimate UI Canvas" );
		root.layer = LayerMask.NameToLayer( "UI" );
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		root.AddComponent<GraphicRaycaster>();
		Undo.RegisterCreatedObjectUndo( root, "Create " + root.name );

		child.transform.SetParent( root.transform, false );
		
		CreateEventSystem();
	}

	private static void CreateEventSystem ()
	{
		Object esys = Object.FindObjectOfType<EventSystem>();
		if( esys == null )
		{
			GameObject eventSystem = new GameObject( "EventSystem" );
			esys = eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<StandaloneInputModule>();

			Undo.RegisterCreatedObjectUndo( eventSystem, "Create " + eventSystem.name );
		}
	}

	/* PUBLIC STATIC FUNCTIONS */
	public static void RequestCanvas ( GameObject child )
	{
		Canvas[] allCanvas = Object.FindObjectsOfType( typeof( Canvas ) ) as Canvas[];

		for( int i = 0; i < allCanvas.Length; i++ )
		{
			if( allCanvas[ i ].renderMode == RenderMode.ScreenSpaceOverlay && allCanvas[ i ].enabled == true && !allCanvas[ i ].GetComponent<CanvasScaler>() )
			{
				child.transform.SetParent( allCanvas[ i ].transform, false );
				CreateEventSystem();
				return;
			}
		}
		CreateNewCanvas( child );
	}
}