/* Written by Kaz Crowe */
/* UltimateButton.cs */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/*
 * First off, the script is using [ExecuteInEditMode] to be able to show changes in real time.
 * This will not affect anything within a build or play mode. This simply makes the script
 * able to be run while in the Editor in Edit Mode.
*/
[ExecuteInEditMode]
public class UltimateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	/* ----- > ASSIGNED VARIABLES < ----- */
	RectTransform baseTrans;
	public RectTransform sizeFolder;
	public Image buttonBase;
	public Image buttonHighlight, tensionAccent;
	public Animator buttonAnimator;

	/* ----- > SIZE AND PLACEMENT < ----- */
	public enum ScalingAxis{ Width, Height }
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public enum Anchor{ Left, Right }
	public Anchor anchor = Anchor.Right;
	public enum TouchSize{ Default, Medium, Large }
	public TouchSize touchSize = TouchSize.Default;
	public float buttonSize = 1.75f, customSpacing_X = 5.0f, customSpacing_Y = 20.0f;
	CanvasGroup canvasGroup;

	/* ----- > STYLE AND OPTIONS < ----- */
	public enum ImageStyle{ Circular,	Square }
	public ImageStyle imageStyle = ImageStyle.Circular;
	public float inputRange = 1.0f;
	float _inputRange = 1.0f;
	Vector2 buttonCenter = Vector2.zero;
	bool isHovering = false;
	public bool trackInput = false;
	public bool transmitInput = false;
	public GameObject receiver;
	IPointerDownHandler downHandler;
	IDragHandler dragHandler;
	IPointerUpHandler upHandler;
	public enum TapCountOption{ NoCount, Accumulate, TouchRelease }
	public TapCountOption tapCountOption = TapCountOption.NoCount;
	public float tapCountDuration = 0.5f;
	public int targetTapCount = 2;
	float currentTapTime = 0.0f;
	int tapCount = 0;
	public Color baseColor = Color.white;
	public bool showHighlight = false, showTension = false;
	public Color highlightColor = Color.white, tensionColorNone = Color.white, tensionColorFull = Color.white;
	public float tensionFadeInDuration = 1.0f, tensionFadeOutDuration = 1.0f;
	float tensionFadeInSpeed = 1.0f, tensionFadeOutSpeed = 1.0f;
	int animatorState = 0;
	public bool useAnimation = false, useFade = false;
	public float fadeUntouched = 1.0f, fadeTouched = 0.5f;
	public float fadeInDuration = 1.0f, fadeOutDuration = 1.0f;
	float fadeInSpeed = 1.0f, fadeOutSpeed = 1.0f;

	/* ----- > SCRIPT REFERENCE < ----- */
	public string buttonName;
	static Dictionary<string, UltimateButton> UltimateButtons = new Dictionary<string, UltimateButton>();
	bool getButtonDown = false;
	public bool getButton = false;
	bool getButtonUp = false;
	bool getTapCount = false;

	/* ----- > BUTTON EVENTS < ----- */
	public UnityEvent onButtonDown, onButtonUp;
	public UnityEvent tapCountEvent;

	int _pointerId = -10;// -10 is the default value set. -1 and -2 register as mouse ID's.
	

	void Awake ()
	{
		// If the application is being run, then send this button name and states to our static dictionary for reference.
		if( Application.isPlaying == true && buttonName != string.Empty )
			RegisterButton( buttonName );
	}
	
	void Start ()
	{
		// If the application is not running then return.
		if( Application.isPlaying == false )
			return;

		// Update the size and positioning of the button.
		UpdatePositioning();

		// If the user is wanting to show the highlight color of the button, update the highlight images.
		if( showHighlight == true && buttonHighlight != null )
			buttonHighlight.color = highlightColor;

		// If the user is using tension fade options...
		if( showTension == true )
		{
			// Configure the speed variables for the fade.
			tensionFadeInSpeed = 1.0f / tensionFadeInDuration;
			tensionFadeOutSpeed = 1.0f / tensionFadeOutDuration;
		}

		// If the user is wanting to show animation, then reference the HashID of the animator parameter.
		if( useAnimation == true && buttonAnimator != null )
			animatorState = Animator.StringToHash( "Touch" );

		// If the user has useFade enabled...
		if( useFade == true )
		{
			// Get the CanvasGroup component for Enable/Disable options.
			canvasGroup = GetComponent<CanvasGroup>();

			// If the canvasGroup is null, then add a CanvasGroup and get the reference.
			if( canvasGroup == null )
			{
				gameObject.AddComponent( typeof( CanvasGroup ) );
				canvasGroup = GetComponent<CanvasGroup>();
			}

			// Configure the fade speeds.
			fadeInSpeed = 1.0f / fadeInDuration;
			fadeOutSpeed = 1.0f / fadeOutDuration;

			// And apply the default fade for the button.
			canvasGroup.alpha = fadeUntouched;
		}

		// If the parent canvas doesn't have a UltimateButtonUpdater component, then add one.
		if( !GetParentCanvas().GetComponent<UltimateButtonUpdater>() )
			GetParentCanvas().gameObject.AddComponent( typeof( UltimateButtonUpdater ) );

		if( transmitInput == true && receiver != null )
		{
			downHandler = receiver.GetComponent<IPointerDownHandler>();
			dragHandler = receiver.GetComponent<IDragHandler>();
			upHandler = receiver.GetComponent<IPointerUpHandler>();
		}
	}

	// This function registers this button into the dictionary.
	public void RegisterButton ( string newName )
	{
		// Unregister old name.
		if( UltimateButtons.ContainsKey( buttonName ) )
			UltimateButtons.Remove( buttonName );

		// Assign the new name.
		buttonName = newName;

		// If the dictionary already contains a Ultimate Button with this name, then remove the button.
		if( UltimateButtons.ContainsKey( newName ) )
			UltimateButtons.Remove( newName );

		// Add the button name and this Ultimate Button into the dictionary.
		UltimateButtons.Add( newName, GetComponent<UltimateButton>() );
	}
	
	// This function is called when the user has touched down.
	public void OnPointerDown ( PointerEventData touchInfo )
	{
		// If the button is already in use, then return.
		if( _pointerId != -10 )
			return;

		if( !IsInRange( touchInfo.position ) )
			return;

		_pointerId = touchInfo.pointerId;

		// Set the buttons state to true since it is being interacted with.
		getButton = true;

		// If the button name has been assigned, then broadcast the button's state.
		if( buttonName != string.Empty )
			StartCoroutine( "GetButtonDownDelay" );

		// If the down event is assigned, then call the event.
		if( onButtonDown != null )
			onButtonDown.Invoke();

		// If the user wants to show animations on Touch, set the 'Touch' parameter to true.
		if( useAnimation == true && buttonAnimator != null )
			buttonAnimator.SetBool( animatorState, true );

		// If the user is wanting to count taps on this button...
		if( tapCountOption != TapCountOption.NoCount )
		{
			// If the user is wanting to accumulate taps...
			if( tapCountOption == TapCountOption.Accumulate )
			{
				// If the timer is not currently counting down...
				if( currentTapTime <= 0 )
				{
					// Then start the count down timer, and set the current tapCount to 1.
					StartCoroutine( "TapCountdown" );
					tapCount = 1;
				}
				// Else the timer is already running, so increase tapCount by 1.
				else
					++tapCount;

				// If the timer is still going, and the target tap count has been reached...
				if( currentTapTime > 0 && tapCount >= targetTapCount )
				{
					// Stop the timer by setting the tap time to zero, start the one frame delay for the static reference of tap count, and call the tapCountEvent.
					currentTapTime = 0;
					if( buttonName != string.Empty )
						StartCoroutine( "GetTapCountDelay" );
					
					if( tapCountEvent != null )
						tapCountEvent.Invoke();
				}
			}
			// Else the user is wanting to send tap counts by way of a quick touch and release...
			else
			{
				// If the timer is not currently counting down, then start the coroutine.
				if( currentTapTime <= 0 )
					StartCoroutine( "TapCountdown" );
				else
					currentTapTime = tapCountDuration;
			}
		}

		// If the user wants the button to fade, do that here.
		if( useFade == true && canvasGroup != null )
			StartCoroutine( "ButtonFade" );
		
		// If the user wants to display tension, and the image is assigned, then start the coroutine.
		if( showTension == true && tensionAccent != null )
			StartCoroutine( "TensionAccentFade" );

		// Set is hovering to true since the user has just initiate the touch.
		isHovering = true;

		// If the user wants to transmit the input and the event is assigned, then call the function.
		if( transmitInput == true && downHandler != null )
			downHandler.OnPointerDown( touchInfo );
	}

	// This function is called when the user is dragging the input.
	public void OnDrag ( PointerEventData touchInfo )
	{
		// If the pointer event that is calling this function is not the same as the one that initiated the button, then return.
		if( touchInfo.pointerId != _pointerId )
			return;

		// If the user is transmitting input, and the Drag event is assigned, then call the function.
		if( transmitInput == true && dragHandler != null )
			dragHandler.OnDrag( touchInfo );

		// If the user does not want to track the input when it moves, then return.
		if( trackInput == false )
			return;
		
		if( !IsInRange( touchInfo.position ) && isHovering == true )
		{
			isHovering = false;
			getButton = false;

			if( useAnimation == true && buttonAnimator != null )
				buttonAnimator.SetBool( animatorState, false );
		}
		else if( IsInRange( touchInfo.position ) && isHovering == false )
		{
			isHovering = true;
			getButton = true;

			// If the user is wanting to show tension, start the corresponding coroutine.
			if( showTension == true && tensionAccent != null )
				StartCoroutine( "TensionAccentFade" );

			// If the user is wanting to show fade on the button
			if( useFade == true && canvasGroup != null )
				StartCoroutine( "ButtonFade" );

			if( useAnimation == true && buttonAnimator != null )
				buttonAnimator.SetBool( animatorState, true );
		}
	}
	
	// This function is called when the user has let go of the input.
	public void OnPointerUp ( PointerEventData touchInfo )
	{
		// If the pointer event that is calling this function is not the same as the one that initiated the button, then return.
		if( touchInfo.pointerId != _pointerId )
			return;

		// Set the buttons state to false.
		getButton = false;

		// Set the stored pointer ID to a null value.
		_pointerId = -10;

		// If the input is not currently hovering over the button, then return.
		if( isHovering == false )
			return;

		// If the button name has been assigned, then broadcast the button's state.
		if( buttonName != string.Empty )
			StartCoroutine( "GetButtonUpDelay" );
		
		// If the up event is assigned, then call the event.
		if( onButtonUp != null )
			onButtonUp.Invoke();

		// If the user is wanting to count the amount of taps by Touch and Release...
		if( tapCountOption == TapCountOption.TouchRelease )
		{
			// Then check the current tap time to see if the release is happening within time.
			if( currentTapTime > 0 )
			{
				// Call the button events.
				if( buttonName != string.Empty )
					StartCoroutine( "GetTapCountDelay" );
					
				if( tapCountEvent != null )
					tapCountEvent.Invoke();
			}

			// Set the tap time to 0 to reset the timer.
			currentTapTime = 0;
		}
		
		// If the user is wanting to show animations, then set the animator.
		if( useAnimation == true && buttonAnimator != null )
			buttonAnimator.SetBool( animatorState, false );

		// Set isHovering to false since the touch input has been released.
		isHovering = false;

		// If the user wants to transmit the input and the OnPointerUp variable is assigned, then call the function.
		if( transmitInput == true && upHandler != null )
			upHandler.OnPointerUp( touchInfo );
	}

	bool IsInRange ( Vector2 inputPos )
	{
		bool inRange = false;
		if( touchSize != TouchSize.Default )
			inRange = true;
		else if( imageStyle == ImageStyle.Circular )
		{
			// Configure the current distance from the center of the button to the touch position.
			float currentDistance = Vector2.Distance( inputPos, buttonCenter );
			if( currentDistance > _inputRange )
				inRange = false;
			else if( currentDistance < _inputRange )
				inRange = true;
		}
		else
		{
			inputPos = inputPos - buttonCenter;
			if( inputPos.x > _inputRange || inputPos.x < -_inputRange || inputPos.y > _inputRange || inputPos.y < -_inputRange )
				inRange = false;
			else
				inRange = true;
		}
		return inRange;
	}

	// This function is used for counting down for the TapCount options.
	IEnumerator TapCountdown ()
	{
		currentTapTime = tapCountDuration;
		while( currentTapTime > 0 )
		{
			currentTapTime -= Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator GetButtonDownDelay ()
	{
		getButtonDown = true;
		yield return new WaitForEndOfFrame();
		getButtonDown = false;
	}

	IEnumerator GetButtonUpDelay ()
	{
		getButtonUp = true;
		yield return new WaitForEndOfFrame();
		getButtonUp = false;
	}

	IEnumerator GetTapCountDelay ()
	{
		getTapCount = true;
		yield return new WaitForEndOfFrame();
		getTapCount = false;
	}

	IEnumerator TensionAccentFade ()
	{
		// Store the current color.
		Color currentColor = tensionAccent.color;

		// If the fade speed is NaN, then just apply the full color.
		if( float.IsInfinity( tensionFadeInSpeed ) )
			tensionAccent.color = tensionColorFull;
		// Else run the loop to fade the tensionAccent.color.
		else
		{
			// For as long as the fade duration, or the button is released...
			for( float fadeIn = 0.0f; fadeIn < 1.0f && getButton == true; fadeIn += Time.deltaTime * tensionFadeInSpeed )
			{
				// Lerp the color between the current color to the full color by the fadeIn value above.
				tensionAccent.color = Color.Lerp( currentColor, tensionColorFull, fadeIn );
				yield return null;
			}
			// If the button is still being interacted with, then apply the final color.
			if( getButton == true )
				tensionAccent.color = tensionColorFull;
		}

		// While the button state is true, yield a frame.
		while( getButton == true )
			yield return null;

		// Re-store the current color.
		currentColor = tensionAccent.color;

		// If the fade speed is NaN, then just apply the None color.
		if( float.IsInfinity( tensionFadeOutSpeed ) )
			tensionAccent.color = tensionColorNone;
		// Else run the loop to fade the tensionAccent.color.
		else
		{
			// For as long as the fade out duration, or in the button gets pressed again...
			for( float fadeOut = 0.0f; fadeOut < 1.0f && getButton == false; fadeOut += Time.deltaTime * tensionFadeOutSpeed )
			{
				// Lerp the tension accent color between the current color to the default color by the fadeOut value above.
				tensionAccent.color = Color.Lerp( currentColor, tensionColorNone, fadeOut );
				yield return null;
			}
			// If the button is still not being interacted with, then apply the final color.
			if( getButton == false )
				tensionAccent.color = tensionColorNone;
		}
	}

	IEnumerator ButtonFade ()
	{
		// Store the current amount of fade.
		float currentFade = canvasGroup.alpha;
		
		// If the fade speed is NaN, then just apply the full color.
		if( float.IsInfinity( fadeInSpeed ) )
			canvasGroup.alpha = fadeTouched;
		else
		{
			for( float fadeIn = 0.0f; fadeIn < 1.0f && getButton == true; fadeIn += Time.unscaledDeltaTime * fadeInSpeed )
			{
				canvasGroup.alpha = Mathf.Lerp( currentFade, fadeTouched, fadeIn );
				yield return null;
			}
			if( getButton == true )
				canvasGroup.alpha = fadeTouched;
		}

		// While the button state is true, yield a frame.
		while( getButton == true )
			yield return null;

		// Store the current fade amount.
		currentFade = canvasGroup.alpha;

		// If the fade speed is NaN, then just apply the untouched color.
		if( float.IsInfinity( fadeOutSpeed ) )
			canvasGroup.alpha = fadeUntouched;
		else
		{
			for( float fadeOut = 0.0f; fadeOut < 1.0f && getButton == false; fadeOut += Time.unscaledDeltaTime * fadeOutSpeed )
			{
				canvasGroup.alpha = Mathf.Lerp( currentFade, fadeUntouched, fadeOut );
			
				yield return null;
			}
			if( getButton == false )
				canvasGroup.alpha = fadeUntouched;
		}
	}

	// This function is used only to find the canvas parent if its not the root object.
	Canvas GetParentCanvas ()
	{
		Transform parent = transform.parent;
		while( parent != null )
		{
			if( parent.transform.GetComponent<Canvas>() )
				return parent.transform.GetComponent<Canvas>();

			parent = parent.transform.parent;
		}
		return null;
	}

	CanvasGroup GetCanvasGroup ()
	{
		if( GetComponent<CanvasGroup>() )
			return GetComponent<CanvasGroup>();
		else
		{
			gameObject.AddComponent<CanvasGroup>();
			return GetComponent<CanvasGroup>();
		}
	}

	// This function will configure the position of an image based on the size and custom spacing selected.
	Vector2 ConfigureImagePosition ( Vector2 textureSize, Vector2 customSpacing )
	{
		// First, fix the customSpacing to be a value between 0.0f and 1.0f.
		Vector2 fixedCustomSpacing = customSpacing / 100;

		// Then configure position spacers according to canvasSize, the fixed spacing and texture size.
		float positionSpacerX = Screen.width * fixedCustomSpacing.x - ( textureSize.x * fixedCustomSpacing.x );
		float positionSpacerY = Screen.height * fixedCustomSpacing.y - ( textureSize.y * fixedCustomSpacing.y );

		// Create a temporary Vector2 to modify and return.
		Vector2 tempVector;

		// If it's left, simply apply the positionxSpacerX, else calculate out from the right side and apply the positionSpaceX.
		tempVector.x = anchor == Anchor.Left ? positionSpacerX : ( Screen.width - textureSize.x ) - positionSpacerX;

		// Apply the positionSpacerY variable.
		tempVector.y = positionSpacerY;

		// Return the updated temporary Vector.
		return tempVector;
	}

#if UNITY_EDITOR
	void Update ()
	{
		// The button will be updated constantly when the game is not being run.
		if( Application.isPlaying == false )
			UpdatePositioning();
	}
#endif
	
	/* --------------------------------------------- *** PUBLIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Updates the size and placement of the Ultimate Button. Useful for when applying any options changed at runtime.
	/// </summary>
	public void UpdatePositioning ()
	{
		if( sizeFolder == null )
			return;

		// Find the reference size for the axis to size the button by.
		float referenceSize = scalingAxis == ScalingAxis.Height ? Screen.height : Screen.width;

		// Configure a size for the image based on the Canvas's size and scale.
		float textureSize = referenceSize * ( buttonSize / 10 );

		// If baseTrans is null, store this object's RectTrans so that it can be positioned.
		if( baseTrans == null )
			baseTrans = GetComponent<RectTransform>();

		// Get a position for the button based on the position variables.
		Vector2 imagePosition = ConfigureImagePosition( new Vector2( textureSize, textureSize ), new Vector2( customSpacing_X, customSpacing_Y ) );

		// Temporary float to store a modifier for the touch area size.
		float fixedTouchSize = touchSize == TouchSize.Large ? 2.0f : touchSize == TouchSize.Medium ? 1.51f : 1.01f;

		// Temporary Vector2 to store the default size of the button.
		Vector2 tempVector = new Vector2( textureSize, textureSize );

		// Apply the button size multiplied by the fixedTouchSize.
		baseTrans.sizeDelta = tempVector * fixedTouchSize;

		// Apply the imagePosition modified with the difference of the sizeDelta divided by 2, multiplied by the scale of the parent canvas.
		baseTrans.position = imagePosition - ( ( baseTrans.sizeDelta - tempVector ) / 2 );

		// Apply the size and position to the sizeFolder.
		sizeFolder.sizeDelta = new Vector2( textureSize, textureSize );
		sizeFolder.position = imagePosition;

		buttonCenter = sizeFolder.position;
		buttonCenter += new Vector2( baseTrans.sizeDelta.x, baseTrans.sizeDelta.y ) / 2;
		_inputRange = ( ( baseTrans.sizeDelta.x ) / 2 ) * inputRange;

		// If the user wants to fade, and the canvasGroup is unassigned, find the CanvasGroup.
		if( useFade == true && canvasGroup == null )
			canvasGroup = GetCanvasGroup();
	}

	/// <summary>
	/// Updates the color of the base images of the Ultimate Button.
	/// </summary>
	/// <param name="targetColor">Target Color of the base images.</param>
	public void UpdateBaseColor ( Color targetColor )
	{
		if( buttonBase == null )
			return;

		baseColor = targetColor;
		buttonBase.color = baseColor;
	}

	/// <summary>
	/// Updates the color of the highlight attached to the Ultimate Button with the targeted color.
	/// </summary>
	/// <param name="targetColor">New highlight color.</param>
	public void UpdateHighlightColor ( Color targetColor )
	{
		if( showHighlight == false )
			return;

		highlightColor = targetColor;

		if( buttonHighlight != null )
			buttonHighlight.color = highlightColor;
	}

	/// <summary>
	/// Updates the colors of the tension accent attached to the Ultimate Button with the targeted colors.
	/// </summary>
	/// <param name="targetTensionNone">New idle tension color.</param>
	/// <param name="targetTensionFull">New full tension color.</param>
	public void UpdateTensionColors ( Color targetTensionNone, Color targetTensionFull )
	{
		if( showTension == false )
			return;

		tensionColorNone = targetTensionNone;
		tensionColorFull = targetTensionFull;
	}
	/* ------------------------------------------- *** END PUBLIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
	
	/* --------------------------------------------- *** STATIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Returns true on the frame that the targeted Ultimate Button is pressed down.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	public static bool GetButtonDown ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return false;
		
		return UltimateButtons[ buttonName ].getButtonDown;
	}

	/// <summary>
	/// Returns true on the frames that the targeted Ultimate Button is being interacted with.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	public static bool GetButton ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return false;

		return UltimateButtons[ buttonName ].getButton;
	}

	/// <summary>
	/// Returns true on the frame that the targeted Ultimate Button is released.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	/// <returns></returns>
	public static bool GetButtonUp ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return false;

		return UltimateButtons[ buttonName ].getButtonUp;
	}

	/// <summary>
	/// Returns true when the Tap Count option has been achieved.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	public static bool GetTapCount ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return false;

		return UltimateButtons[ buttonName ].getTapCount;
	}

	/// <summary>
	/// Returns the targeted Ultimate Button if it exists within the scene.
	/// </summary>
	/// <param name="buttonName">The name of the targeted Ultimate Button.</param>
	public static UltimateButton GetUltimateButton ( string buttonName )
	{
		if( !ButtonConfirmed( buttonName ) )
			return null;

		return UltimateButtons[ buttonName ];
	}

	static bool ButtonConfirmed ( string buttonName )
	{
		if( !UltimateButtons.ContainsKey( buttonName ) )
		{
			Debug.LogWarning( "No Ultimate Button has been registered with the name: " + buttonName + "." );
			return false;
		}
		return true;
	}
	/* ------------------------------------------- *** END STATIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
}

/* Written by Kaz Crowe */
/* UltimateButtonUpdater.cs */
public class UltimateButtonUpdater : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange ()
	{
		StartCoroutine( "YieldPositioning" );
	}

	IEnumerator YieldPositioning ()
	{
		yield return new WaitForEndOfFrame();

		UltimateButton[] allButtons = FindObjectsOfType( typeof( UltimateButton ) ) as UltimateButton[];
		for( int i = 0; i < allButtons.Length; i++ )
			allButtons[ i ].UpdatePositioning();
	}
}