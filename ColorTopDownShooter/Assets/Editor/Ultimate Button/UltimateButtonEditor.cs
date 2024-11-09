/* Written by Kaz Crowe */
/* UltimateButtonEditor.cs */
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.AnimatedValues;

[CanEditMultipleObjects]
[CustomEditor( typeof( UltimateButton ) )]
public class UltimateButtonEditor : Editor
{
	/* ---< ASSIGNED VARIABLES >--- */
	SerializedProperty sizeFolder, buttonHighlight;
	SerializedProperty tensionAccent, buttonAnimator;
	SerializedProperty buttonBase;

	/* ---< SIZE AND PLACEMENT >--- */
	SerializedProperty scalingAxis, anchor, touchSize;
	SerializedProperty buttonSize, customSpacing_X, customSpacing_Y;

	/* ---< STYLES AND OPTIONS >--- */
	SerializedProperty imageStyle, inputRange;
	SerializedProperty trackInput, transmitInput, receiver;
	SerializedProperty tapCountOption, tapCountDuration, targetTapCount;

	SerializedProperty baseColor;
	SerializedProperty showHighlight, highlightColor;
	SerializedProperty showTension, tensionColorNone, tensionColorFull;
	SerializedProperty tensionFadeInDuration, tensionFadeOutDuration;
	SerializedProperty useAnimation, useFade;
	SerializedProperty fadeUntouched, fadeTouched;
	SerializedProperty fadeInDuration, fadeOutDuration;

	/* ---< SCRIPT REFERENCE >--- */
	public enum ScriptCast{ GetButtonDown, GetButtonUp, GetButton, GetTapCount, GetUltimateButton }
	ScriptCast scriptCast;
	SerializedProperty buttonName;

	/* ---< BUTTON EVENTS >--- */
	SerializedProperty onButtonDown;
	SerializedProperty onButtonUp;
	SerializedProperty tapCountEvent;

	/* ---< ANIMATED BOOLEANS >--- */
	AnimBool AssignedVariables, SizeAndPlacement, StyleAndOptions;
	AnimBool ScriptReference, ButtonEvents;
	AnimBool highlightOption, tensionOption;
	AnimBool animationOption, fadeOption, tapOption;
	AnimBool buttonNameAssigned, buttonNameUnassigned;
	AnimBool StyleAndOptionsVariables;
	AnimBool TransmitInputEnabled;
	AnimBool TouchSizeDefaultOptions;
	AnimBool TensionAccentError;

	/* ---< INTERNAL >--- */
	Canvas parentCanvas;

	
	void OnEnable ()
	{
		// Store the Ultimate Button references as soon as this script is being viewed.
		StoreReferences();

		// Register the Undo function to be called for undo's.
		Undo.undoRedoPerformed += UndoRedoCallback;

		// Store the parent canvas.
		parentCanvas = GetParentCanvas();

		UltimateButton targ = ( UltimateButton )target;

		TransmitInputEnabled = new AnimBool( targ.transmitInput );

		TensionAccentError = new AnimBool( targ.tensionAccent == null );
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	Canvas GetParentCanvas ()
	{
		// If the current selection is null, then return.
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
			UltimateButtonCreator.RequestCanvas( Selection.activeGameObject );

		return null;
	}

	// Function for Undo/Redo operations.
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
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.Space();
		
		#region CANVAS ERRORS
		/* ///// ---------------------------------------- < CANVAS ERRORS > ---------------------------------------- \\\\\ */
		if( CanvasErrors() == true )
		{
			if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "The parent Canvas needs to be set to 'Screen Space - Overlay' in order for the Ultimate Button to function correctly.", MessageType.Error );
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "Update Canvas", EditorStyles.miniButtonLeft ) )
				{
					parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
					parentCanvas = GetParentCanvas();
				}
				if( GUILayout.Button( "Update Button", EditorStyles.miniButtonRight ) )
				{
					UltimateButtonCreator.RequestCanvas( Selection.activeGameObject );
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
					EditorGUILayout.HelpBox( "The Canvas Scaler component located on the parent Canvas needs to be set to 'Constant Pixel Size' in order for the Ultimate Button to function correctly.", MessageType.Error );
					EditorGUILayout.BeginHorizontal();
					if( GUILayout.Button( "Update Canvas", EditorStyles.miniButtonLeft ) )
					{
						parentCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
						parentCanvas = GetParentCanvas();
						UltimateButton button = ( UltimateButton )target;
						button.UpdatePositioning();
					}
					if( GUILayout.Button( "Update Button", EditorStyles.miniButtonRight ) )
					{
						UltimateButtonCreator.RequestCanvas( Selection.activeGameObject );
						parentCanvas = GetParentCanvas();
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
			}
			return;
		}
		/* \\\\\ -------------------------------------- < END CANVAS ERRORS > -------------------------------------- ///// */
		#endregion
		
		UltimateButton targ = ( UltimateButton ) target;

		#region ASSIGNED VARIABLES
		/* ///// ---------------------------------------- < ASSIGNED VARIABLES > ---------------------------------------- \\\\\ */
		DisplayHeader( "Assigned Variables", "UUI_Variables", AssignedVariables );
		if( EditorGUILayout.BeginFadeGroup( AssignedVariables.faded ) )
		{
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			{
				EditorGUILayout.PropertyField( sizeFolder );

				EditorGUILayout.Space();

				EditorGUILayout.LabelField( "Style and Options Variables", EditorStyles.boldLabel );

				// BASE COLOR //
				EditorGUILayout.PropertyField( buttonBase );

				// HIGHLIGHT OPTION //
				if( EditorGUILayout.BeginFadeGroup( highlightOption.faded ) )
					EditorGUILayout.PropertyField( buttonHighlight );
				if( AssignedVariables.faded == 1 )
					EditorGUILayout.EndFadeGroup();

				// TENSION OPTION //
				if( EditorGUILayout.BeginFadeGroup( tensionOption.faded ) )
					EditorGUILayout.PropertyField( tensionAccent );
				if( AssignedVariables.faded == 1 )
					EditorGUILayout.EndFadeGroup();

				// ANIMATION OPTION //
				if( EditorGUILayout.BeginFadeGroup( animationOption.faded ) )
					EditorGUILayout.PropertyField( buttonAnimator );
				if( AssignedVariables.faded == 1 )
					EditorGUILayout.EndFadeGroup();
			}
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				TensionAccentError.target = targ.tensionAccent == null ? true : false;

				targ.UpdateHighlightColor( targ.highlightColor );
				targ.UpdateTensionColors( targ.tensionColorNone, targ.tensionColorFull );

				if( targ.buttonHighlight != null )
					EditorUtility.SetDirty( targ.buttonHighlight );
				if( targ.tensionAccent != null )
					EditorUtility.SetDirty( targ.tensionAccent );
			}
		}
		EditorGUILayout.EndFadeGroup();
		/* \\\\\ -------------------------------------- < END ASSIGNED VARIABLES > -------------------------------------- ///// */
		#endregion

		EditorGUILayout.Space();
		
		#region SIZE AND PLACEMENT
		/* ///// ---------------------------------------- < SIZE AND PLACEMENT > ---------------------------------------- \\\\\ */
		DisplayHeader( "Size and Placement", "UUI_SizeAndPlacement", SizeAndPlacement );
		
		if( EditorGUILayout.BeginFadeGroup( SizeAndPlacement.faded ) )
		{
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			{
				EditorGUILayout.PropertyField( scalingAxis );
				EditorGUILayout.PropertyField( anchor );
				EditorGUILayout.PropertyField( touchSize, new GUIContent( "Touch Size", "The size of the area in which the touch can start" ) );
				EditorGUILayout.Slider( buttonSize, 0.0f, 5.0f, new GUIContent( "Button Size" ) );
			}
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				TouchSizeDefaultOptions.target = targ.touchSize == UltimateButton.TouchSize.Default ? true : false;
			}

			EditorGUILayout.BeginVertical( "Box" );
			{
				EditorGUILayout.LabelField( "Button Position" );
				EditorGUI.indentLevel = 1;
				{
					EditorGUI.BeginChangeCheck();
					{
						EditorGUILayout.Slider( customSpacing_X, 0.0f, 50.0f, new GUIContent( "X Position:" ) );
						EditorGUILayout.Slider( customSpacing_Y, 0.0f, 100.0f, new GUIContent( "Y Position:" ) );
					}
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUI.indentLevel = 0;
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFadeGroup();
		/* \\\\\ -------------------------------------- < END SIZE AND PLACEMENT > -------------------------------------- ///// */
		#endregion

		EditorGUILayout.Space();
		
		#region STYLE AND OPTIONS
		/* ///// ---------------------------------------- < STYLES AND OPTIONS > ---------------------------------------- \\\\\ */
		DisplayHeader( "Style and Options", "UUI_StyleAndOptions", StyleAndOptions );
		if( EditorGUILayout.BeginFadeGroup( StyleAndOptions.faded ) )
		{
			EditorGUILayout.Space();

			if( EditorGUILayout.BeginFadeGroup( TouchSizeDefaultOptions.faded ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( imageStyle, new GUIContent( "Image Style", "Determines whether the input range should be circular or square. This option affects how the Input Range and Track Input options function." ) );
				EditorGUILayout.Slider( inputRange, 0.0f, 1.0f, new GUIContent( "Input Range", "The range that the Ultimate Button will react to when initiating and dragging the input." ) );
				EditorGUILayout.PropertyField( trackInput, new GUIContent( "Track Input", "Enabling this option will allow the Ultimate Button to track the users input to ensure that button events and states are only called when the input is within the Input Range." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}
			if( StyleAndOptions.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			// TRANSMIT INPUT //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( transmitInput, new GUIContent( "Transmit Input", "Should the Ultimate Button transmit input events to another UI game object?" ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				TransmitInputEnabled.target = targ.transmitInput;
			}

			if( EditorGUILayout.BeginFadeGroup( TransmitInputEnabled.faded ) )
			{
				EditorGUI.indentLevel = 1;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( receiver, new GUIContent( "Input Receiver" ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			// TAP COUNT OPTIONS //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( tapCountOption );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				tapOption.target = targ.tapCountOption != UltimateButton.TapCountOption.NoCount ? true : false;
			}
			if( EditorGUILayout.BeginFadeGroup( tapOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				{
					EditorGUILayout.Slider( tapCountDuration, 0.0f, 1.0f, new GUIContent( "Tap Time Window" ) );
					EditorGUI.BeginDisabledGroup( targ.tapCountOption != UltimateButton.TapCountOption.Accumulate );
					EditorGUILayout.IntSlider( targetTapCount, 1, 5, new GUIContent( "Target Tap Count" ) );
					EditorGUI.EndDisabledGroup();
				}
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.indentLevel = 0;

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( "Example Code" ) )
				{
					ScriptReference.target = true;
					EditorPrefs.SetBool( "UUI_ScriptReference", true );
					scriptCast = ScriptCast.GetTapCount;
				}
				if( GUILayout.Button( "Button Events" ) )
				{
					ButtonEvents.target = true;
					EditorPrefs.SetBool( "UUI_ExtraOption_01", true );
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space( 8 );
			EditorGUILayout.LabelField( "───────" );
			EditorGUILayout.EndHorizontal();

			// BASE COLOR //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( baseColor );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				targ.UpdateBaseColor( targ.baseColor );
				if( targ.buttonBase != null )
					EditorUtility.SetDirty( targ.buttonBase );
			}
			if( targ.buttonBase == null )
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.HelpBox( "The Button Base image has not been assigned. Please make sure to assign this variable within the Assigned Variables section.", MessageType.Warning );
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}

			// --------------------------< HIGHLIGHT >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( showHighlight, new GUIContent( "Show Highlight", "Displays the highlight images with the Highlight Color variable." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetHighlight( targ );
				highlightOption.target = targ.showHighlight;
				if( targ.buttonHighlight != null )
					EditorUtility.SetDirty( targ.buttonHighlight );
			}
			EditorGUI.indentLevel = 1;
			if( EditorGUILayout.BeginFadeGroup( highlightOption.faded ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( highlightColor );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateHighlightColor( targ.highlightColor );
					if( targ.buttonHighlight != null )
						EditorUtility.SetDirty( targ.buttonHighlight );
				}
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.indentLevel = 0;
			// ------------------------< END HIGHLIGHT >------------------------ //

			// ---------------------------< TENSION >--------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( showTension, new GUIContent( "Show Tension", "Displays the visual state of the button using the tension color options." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetTensionAccent( targ );
				tensionOption.target = targ.showTension;
				if( targ.tensionAccent != null )
					EditorUtility.SetDirty( targ.tensionAccent );
			}

			EditorGUI.indentLevel = 1;

			if( EditorGUILayout.BeginFadeGroup( tensionOption.faded ) )
			{
				EditorGUI.BeginChangeCheck();
				{
					EditorGUILayout.PropertyField( tensionColorNone, new GUIContent( "Tension None", "The Color of the Tension with no input." ) );
					EditorGUILayout.PropertyField( tensionColorFull, new GUIContent( "Tension Full", "The Color of the Tension when there is input." ) );
					EditorGUILayout.PropertyField( tensionFadeInDuration, new GUIContent( "Fade In Duration", "Time is seconds for the tension to fade in, with 0 being instant." ) );
					EditorGUILayout.PropertyField( tensionFadeOutDuration, new GUIContent( "Fade Out Duration", "Time is seconds for the tension to fade out, with 0 being instant." ) );
				}
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					if( targ.tensionAccent != null )
					{
						targ.tensionAccent.color = targ.tensionColorNone;
						EditorUtility.SetDirty( targ.tensionAccent );
					}
				}
				if( EditorGUILayout.BeginFadeGroup( TensionAccentError.faded ) )
				{
					EditorGUILayout.HelpBox( "The Tension Accent image has not been assigned. Please make sure to assign this immediately.", MessageType.Error );
				}
				if( StyleAndOptions.faded == 1.0f && tensionOption.faded == 1.0f )
					EditorGUILayout.EndFadeGroup();

				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.indentLevel = 0;
			// -------------------------< END TENSION >------------------------- //

			// USE ANIMATION OPTIONS //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( useAnimation );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetAnimation( targ );
				animationOption.target = targ.useAnimation;
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			// USE FADE OPTIONS //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( useFade );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				fadeOption.target = targ.useFade;

				if( !targ.GetComponent<CanvasGroup>() )
					targ.gameObject.AddComponent<CanvasGroup>();

				if( targ.useFade == true )
					targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
				else
					targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
			}
			if( EditorGUILayout.BeginFadeGroup( fadeOption.faded ) )
			{
				EditorGUI.indentLevel = 1;
				{
					EditorGUI.BeginChangeCheck();
					{
						EditorGUILayout.Slider( fadeUntouched, 0.0f, 1.0f, new GUIContent( "Fade Untouched", "This controls the alpha of the button when it is NOT receiving any input." ) );
						EditorGUILayout.Slider( fadeTouched, 0.0f, 1.0f, new GUIContent( "Fade Touched", "This controls the alpha of the button when it is receiving input." ) );
					}
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						if( !targ.GetComponent<CanvasGroup>() )
							targ.gameObject.AddComponent<CanvasGroup>();

						if( targ.useFade == true )
							targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
						else
							targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
					}
					EditorGUI.BeginChangeCheck();
					{
						EditorGUILayout.PropertyField( fadeInDuration, new GUIContent( "Fade In Duration", "Time is seconds for the button to fade in, with 0 being instant." ) );
						EditorGUILayout.PropertyField( fadeOutDuration, new GUIContent( "Fade Out Duration", "Time is seconds for the button to fade out, with 0 being instant." ) );
					}
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StyleAndOptions.faded == 1 )
				EditorGUILayout.EndFadeGroup();
		}
		EditorGUILayout.EndFadeGroup();
		/* \\\\\ -------------------------------------- < END STYLES AND OPTIONS > -------------------------------------- ///// */
		#endregion

		EditorGUILayout.Space();
		
		#region SCRIPT REFERENCE
		/* \\\\\ ----------------------------------------- < SCRIPT REFERENCE > ----------------------------------------- ///// */
		DisplayHeader( "Script Reference", "UUI_ScriptReference", ScriptReference );
		if( EditorGUILayout.BeginFadeGroup( ScriptReference.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( buttonName );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				buttonNameAssigned.target = targ.buttonName == string.Empty ? false : true;
				buttonNameUnassigned.target = targ.buttonName != string.Empty ? false : true;
			}

			if( EditorGUILayout.BeginFadeGroup( buttonNameUnassigned.faded ) )
			{
				EditorGUILayout.HelpBox( "Please assign a Button Name in order to be able to get this button's states dynamically.", MessageType.Warning );
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			if( EditorGUILayout.BeginFadeGroup( buttonNameAssigned.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Example Code:", EditorStyles.boldLabel );
				EditorGUILayout.LabelField( "Please select the function you would like to copy and paste into your script.", EditorStyles.wordWrappedLabel );
				scriptCast = ( ScriptCast )EditorGUILayout.EnumPopup( scriptCast );
				GUILayout.Space( 5 );
				switch( scriptCast )
				{
					case ScriptCast.GetButtonDown:
					{
						EditorGUILayout.TextField( "if( UltimateButton.GetButtonDown( \"" + targ.buttonName + "\" ) )" );
					}break;
					case ScriptCast.GetButtonUp:
					{
						EditorGUILayout.TextField( "if( UltimateButton.GetButtonUp( \"" + targ.buttonName + "\" ) )" );
					}break;
					case ScriptCast.GetButton:
					{
						EditorGUILayout.TextField( "if( UltimateButton.GetButton( \"" + targ.buttonName + "\" ) )" );
					}break;
					case ScriptCast.GetTapCount:
					{
						EditorGUILayout.TextField( "if( UltimateButton.GetTapCount( \"" + targ.buttonName + "\" ) )" );

						if( targ.tapCountOption == UltimateButton.TapCountOption.NoCount )
							EditorGUILayout.HelpBox( "Tap Count is not being used. Please set the Tap Count Option in order to use this option.", MessageType.Warning );
					}break;
					case ScriptCast.GetUltimateButton:
					{
						EditorGUILayout.TextField( "UltimateButton.GetUltimateButton( \"" + targ.buttonName + "\" )" );
					}break;
					default:
					{
						EditorGUILayout.TextField( "if( UltimateButton.GetButton( \"" + targ.buttonName + "\" ) )" );
					}break;
				}
				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
		}
		EditorGUILayout.EndFadeGroup();
		/* ///// --------------------------------------- < END SCRIPT REFERENCE > --------------------------------------- \\\\\ */
		#endregion

		EditorGUILayout.Space();

		#region BUTTON EVENTS
		/* \\\\\ ----------------------------------------- < SCRIPT REFERENCE > ----------------------------------------- ///// */
		DisplayHeader( "Button Events", "UUI_ExtraOption_01", ButtonEvents );
		if( EditorGUILayout.BeginFadeGroup( ButtonEvents.faded ) )
		{
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( onButtonDown );
			EditorGUILayout.PropertyField( onButtonUp );

			if( EditorGUILayout.BeginFadeGroup( tapOption.faded ) )
				EditorGUILayout.PropertyField( tapCountEvent );
			if( ButtonEvents.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();
		/* ///// --------------------------------------- < END SCRIPT REFERENCE > --------------------------------------- \\\\\ */
		#endregion

		EditorGUILayout.Space();

		/* ----------------------------------------------< ** HELP TIPS ** >---------------------------------------------- */
		if( targ.sizeFolder == null )
			EditorGUILayout.HelpBox( "Size Folder needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		/* --------------------------------------------< ** END HELP TIPS ** >-------------------------------------------- */

		Repaint();
	}
	
	void StoreReferences ()
	{
		UltimateButton targ = ( UltimateButton ) target;

		/* -----< ASSIGNED VARIABLES >----- */
		sizeFolder = serializedObject.FindProperty( "sizeFolder" );
		buttonBase = serializedObject.FindProperty( "buttonBase" );
		buttonHighlight = serializedObject.FindProperty( "buttonHighlight" );
		tensionAccent = serializedObject.FindProperty( "tensionAccent" );
		buttonAnimator = serializedObject.FindProperty( "buttonAnimator" );
		/* ---< END ASSIGNED VARIABLES >--- */

		/* -----< SIZE AND PLACEMENT >----- */
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		anchor = serializedObject.FindProperty( "anchor" );
		touchSize = serializedObject.FindProperty( "touchSize" );
		buttonSize = serializedObject.FindProperty( "buttonSize" );
		customSpacing_X = serializedObject.FindProperty( "customSpacing_X" );
		customSpacing_Y = serializedObject.FindProperty( "customSpacing_Y" );
		/* ---< END SIZE AND PLACEMENT >--- */

		/* -----< STYLES AND OPTIONS >----- */
		imageStyle = serializedObject.FindProperty( "imageStyle" );
		inputRange = serializedObject.FindProperty( "inputRange" );
		trackInput = serializedObject.FindProperty( "trackInput" );
		transmitInput = serializedObject.FindProperty( "transmitInput" );
		receiver = serializedObject.FindProperty( "receiver" );
		tapCountOption = serializedObject.FindProperty( "tapCountOption" );
		tapCountDuration = serializedObject.FindProperty( "tapCountDuration" );
		targetTapCount = serializedObject.FindProperty( "targetTapCount" );

		baseColor = serializedObject.FindProperty( "baseColor" );
		showHighlight = serializedObject.FindProperty( "showHighlight" );
		highlightColor = serializedObject.FindProperty( "highlightColor" );
		showTension = serializedObject.FindProperty( "showTension" );
		tensionColorNone = serializedObject.FindProperty( "tensionColorNone" );
		tensionColorFull = serializedObject.FindProperty( "tensionColorFull" );
		tensionFadeInDuration = serializedObject.FindProperty( "tensionFadeInDuration" );
		tensionFadeOutDuration = serializedObject.FindProperty( "tensionFadeOutDuration" );
		useAnimation = serializedObject.FindProperty( "useAnimation" );
		useFade = serializedObject.FindProperty( "useFade" );
		fadeUntouched = serializedObject.FindProperty( "fadeUntouched" );
		fadeTouched = serializedObject.FindProperty( "fadeTouched" );
		fadeInDuration = serializedObject.FindProperty( "fadeInDuration" );
		fadeOutDuration = serializedObject.FindProperty( "fadeOutDuration" );
		/* ---< END STYLES AND OPTIONS >--- */
		
		/* ------< SCRIPT REFERENCE >------ */
		buttonName = serializedObject.FindProperty( "buttonName" );
		/* ----< END SCRIPT REFERENCE >---- */

		/* ------< BUTTON EVENTS >------ */
		onButtonDown = serializedObject.FindProperty( "onButtonDown" );
		onButtonUp = serializedObject.FindProperty( "onButtonUp" );
		tapCountEvent = serializedObject.FindProperty( "tapCountEvent" );
		/* ----< END BUTTON EVENTS >---- */

		/* // ---< ANIMATED BOOLEANS >--- \\ */
		AssignedVariables = new AnimBool( EditorPrefs.GetBool( "UUI_Variables" ) );
		SizeAndPlacement = new AnimBool( EditorPrefs.GetBool( "UUI_SizeAndPlacement" ) );
		StyleAndOptions = new AnimBool( EditorPrefs.GetBool( "UUI_StyleAndOptions" ) );
		ScriptReference = new AnimBool( EditorPrefs.GetBool( "UUI_ScriptReference" ) );
		highlightOption = new AnimBool( targ.showHighlight );
		tensionOption = new AnimBool( targ.showTension );
		animationOption = new AnimBool( targ.useAnimation );
		fadeOption = new AnimBool( targ.useFade );
		tapOption = new AnimBool( targ.tapCountOption != UltimateButton.TapCountOption.NoCount ? true : false );
		buttonNameAssigned = new AnimBool( targ.buttonName == string.Empty ? false : true );
		buttonNameUnassigned = new AnimBool( targ.buttonName != string.Empty ? false : true );
		ButtonEvents = new AnimBool( EditorPrefs.GetBool( "UUI_ExtraOption_01" ) );
		TouchSizeDefaultOptions = new AnimBool( targ.touchSize == UltimateButton.TouchSize.Default ? true : false );
		/* // ---< END ANIMATED BOOLEANS >--- \\ */

		SetHighlight( targ );
		SetAnimation( targ );
		SetTensionAccent( targ );

		if( !targ.GetComponent<CanvasGroup>() )
			targ.gameObject.AddComponent<CanvasGroup>();
		if( targ.useFade == true )
			targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
		else
			targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
	}

	void SetHighlight ( UltimateButton ub )
	{
		if( ub.showHighlight == true )
		{
			if( ub.buttonHighlight != null && ub.buttonHighlight.gameObject.activeInHierarchy == false )
				ub.buttonHighlight.gameObject.SetActive( true );
			
			ub.UpdateHighlightColor( ub.highlightColor );
		}
		else
		{
			if( ub.buttonHighlight != null && ub.buttonHighlight.gameObject.activeInHierarchy == true )
				ub.buttonHighlight.gameObject.SetActive( false );
		}
	}
	
	void SetTensionAccent ( UltimateButton ub )
	{
		if( ub.showTension == true )
		{
			if( ub.tensionAccent == null )
				return;
			
			if( ub.tensionAccent != null && ub.tensionAccent.gameObject.activeInHierarchy == false )
				ub.tensionAccent.gameObject.SetActive( true );

			ub.tensionAccent.color = ub.tensionColorNone;
		}
		else
		{
			if( ub.tensionAccent != null && ub.tensionAccent.gameObject.activeInHierarchy == true )
				ub.tensionAccent.gameObject.SetActive( false );
		}
	}

	void SetAnimation ( UltimateButton ub )
	{
		if( ub.useAnimation == true )
		{
			if( ub.buttonAnimator != null )
				if( ub.buttonAnimator.enabled == false )
					ub.buttonAnimator.enabled = true;
		}
		else
		{
			if( ub.buttonAnimator != null )
				if( ub.buttonAnimator.enabled == true )
					ub.buttonAnimator.enabled = false;
		}
	}
}

/* Written by Kaz Crowe */
/* UltimateButtonCreationEditor.cs */
public class UltimateButtonCreator
{
	/* ---------< ULTIMATE BUTTON MENU >--------- */
	[MenuItem( "GameObject/Ultimate UI/Ultimate Button", false, 10 )]
	private static void CreateUltimateButton ()
	{
		GameObject prefab = EditorGUIUtility.Load( "Ultimate Button/UltimateButton.prefab" ) as GameObject;
		
		if( prefab != null )
			CreateNewUI( prefab );
		else
			Debug.LogError( "Could not find 'UltimateButton.prefab' in the Editor Default Resources folder." );
	}
	
	private static void CreateNewUI ( Object buttonPrefab )
	{
		GameObject prefab = ( GameObject )Object.Instantiate( buttonPrefab, Vector3.zero, Quaternion.identity );
		prefab.name = buttonPrefab.name;
		Selection.activeGameObject = prefab;
		RequestCanvas( prefab );
	}
	
	static public void CreateNewCanvas ( GameObject button )
	{
		GameObject root = new GameObject( "Ultimate UI Canvas" );
		root.layer = LayerMask.NameToLayer( "UI" );
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		root.AddComponent<GraphicRaycaster>();
		Undo.RegisterCreatedObjectUndo( root, "Create " + root.name );
		button.transform.SetParent( root.transform, false );
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