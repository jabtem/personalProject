/* UltimateJoystick.cs */
/* Written by Kaz Crowe */
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// First off, the script is using [ExecuteInEditMode] to be able to show changes in real time. This will not affect anything within a build or play mode. This simply makes the script able to be run while in the Editor in Edit Mode.
[ExecuteInEditMode]
public class UltimateJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	// INTERNAL CALCULATIONS //
	RectTransform baseTrans;
	Vector2 defaultPos = Vector2.zero;
	Vector3 joystickCenter = Vector3.zero;
	int _inputId = -10;
	bool joystickReset = false;
	Rect joystickRect;
	CanvasGroup joystickGroup;
	float radius = 1.0f;

	// JOYSTICK POSITIONING //
	public RectTransform joystickBase, joystick;
	public enum ScalingAxis
	{
		Width,
		Height
	}
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public enum Anchor
	{
		Left,
		Right
	}
	public Anchor anchor = Anchor.Left;
	public float activationRange = 1.0f;
	public bool customActivationRange = false;
	public float activationWidth = 50.0f, activationHeight = 75.0f;
	public float activationPositionHorizontal = 0.0f, activationPositionVertical = 0.0f;
	public float joystickSize = 2.5f, radiusModifier = 4.5f;
	public float positionHorizontal = 5.0f, positionVertical = 20.0f;

	// JOYSTICK SETTINGS //
	public bool dynamicPositioning = false;
	public float gravity = 60.0f;
	bool gravityActive = false;
	public bool extendRadius = false;
	public enum Axis
	{
		Both,
		X,
		Y
	}
	public Axis axis = Axis.Both;
	public enum Boundary
	{
		Circular,
		Square
	}
	public Boundary boundary = Boundary.Circular;
	public float deadZone = 0.0f;
	public enum TapCountOption
	{
		NoCount,
		Accumulate,
		TouchRelease
	}
	public TapCountOption tapCountOption = TapCountOption.NoCount;
	public float tapCountDuration = 0.5f;
	public int targetTapCount = 2;
	float currentTapTime = 0.0f;
	int tapCount = 0;
	public bool useTouchInput = false;

	// VISUAL OPTIONS //
	public bool disableVisuals = false;
	public bool inputTransition = false;
	public float transitionUntouchedDuration = 0.1f, transitionTouchedDuration = 0.1f;
	float transitionUntouchedSpeed, transitionTouchedSpeed;
	public bool useFade = false;
	public float fadeUntouched = 1.0f, fadeTouched = 0.5f;
	public bool useScale = false;
	public float scaleTouched = 0.9f;
	public bool showHighlight = false;
	public Color highlightColor = new Color( 1, 1, 1, 1 );
	public Image highlightBase, highlightJoystick;
	public bool showTension = false;
	public Color tensionColorNone = new Color( 1, 1, 1, 1 ), tensionColorFull = new Color( 1, 1, 1, 1 );
	public enum TensionType
	{
		Directional,
		Free
	}
	public TensionType tensionType = TensionType.Directional;
	public float rotationOffset = 0.0f;
	public float tensionDeadZone = 0.0f;
	public List<Image> TensionAccents = new List<Image>();
	
	// SCRIPT REFERENCE //
	static Dictionary<string,UltimateJoystick> UltimateJoysticks = new Dictionary<string, UltimateJoystick>();
	public string joystickName;
	bool joystickState = false;
	bool tapCountAchieved = false;

	// PUBLIC CALLBACKS //
	public event Action OnPointerDownCallback, OnPointerUpCallback, OnDragCallback;
	public event Action OnUpdatePositioning;
	
	// OBSOLETE // NOTE: We are keeping these variables in the script and public so that the values can be copied to the new variables for a smooth transition to the new version.
	public enum JoystickTouchSize
	{
		Default,
		Medium,
		Large,
		Custom
	}
	[Header( "Depreciated Variables" )]
	public JoystickTouchSize joystickTouchSize = JoystickTouchSize.Default;
	public float customSpacing_X = -10, customSpacing_Y = -10;
	public float customTouchSize_X = -10, customTouchSize_Y = -10;
	public float customTouchSizePos_X = -10, customTouchSizePos_Y = -10;
	public RectTransform joystickSizeFolder;
	public Image tensionAccentUp, tensionAccentDown;
	public Image tensionAccentLeft, tensionAccentRight;


	void Awake ()
	{
		// If the game is not being run and the joystick name has been assigned...
		if( Application.isPlaying && joystickName != string.Empty )
		{
			// If the static dictionary has this joystick registered, then remove it from the list.
			if( UltimateJoysticks.ContainsKey( joystickName ) )
				UltimateJoysticks.Remove( joystickName );

			// Then register the joystick.
			UltimateJoysticks.Add( joystickName, GetComponent<UltimateJoystick>() );
		}
	}

	void Start ()
	{
		// If the game is not running then return.
		if( !Application.isPlaying )
			return;

		// Update the size and placement of the joystick.
		UpdateJoystickPositioning();
		
		// Set the highlight is the user is wanting to show highlight.
		if( showHighlight )
			UpdateHighlightColor( highlightColor );

		// Reset the tension accents if the user is wanting to show tension.
		if( showTension )
			TensionAccentReset();

		// If the user wants to transition on different input...
		if( inputTransition )
		{
			// Try to store the canvas group.
			joystickGroup = GetComponent<CanvasGroup>();

			// If the canvas group is still null, then add a canvas group component.
			if( joystickGroup == null )
				joystickGroup = baseTrans.gameObject.AddComponent<CanvasGroup>();

			// Configure the transition speeds.
			transitionUntouchedSpeed = 1.0f / transitionUntouchedDuration;
			transitionTouchedSpeed = 1.0f / transitionTouchedDuration;
		}
		
		// Store this objects current parent.
		Transform parent = transform.parent;

		// Loop this while the parent transform is assigned.
		while( parent != null )
		{
			// If the parent transform has a Canvas component...
			if( parent.transform.GetComponent<Canvas>() )
			{
				// And the parent canvas does not have an updater script attached to it...
				if( !parent.transform.GetComponent<UltimateJoystickScreenSizeUpdater>() )
				{
					// Then add the updater script to the object.
					parent.gameObject.AddComponent( typeof( UltimateJoystickScreenSizeUpdater ) );

					// And break the loop.
					break;
				}

				// Else the canvas does have a updater script, so just break the loop.
				break;
			}

			// Since the parent did not contain a Canvas component, then store the next parent.
			parent = parent.transform.parent;
		}

		// If the user wants to calculate using touch input, then start the coroutine to catch the input.
		if( useTouchInput )
			StartCoroutine( "ProcessTouchInput" );
	}

	// THIS IS FOR THE UNITY EVENT SYSTEM IF THE USER WANTS THAT //
	public void OnPointerDown ( PointerEventData touchInfo )
	{
		if( useTouchInput )
			return;

		ProcessOnInputDown( touchInfo.position, touchInfo.pointerId );
	}

	public void OnDrag ( PointerEventData touchInfo )
	{
		if( useTouchInput )
			return;

		ProcessOnInputMoved( touchInfo.position, touchInfo.pointerId );
	}

	public void OnPointerUp ( PointerEventData touchInfo )
	{
		if( useTouchInput )
			return;

		ProcessOnInputUp( touchInfo.position, touchInfo.pointerId );
	}
	// END FOR UNITY EVENT SYSTEM //

	/// <summary>
	/// The coroutine will process the touch input if the user has the useTouchInput boolean enabled.
	/// </summary>
	IEnumerator ProcessTouchInput ()
	{
		// Loop for as long as useTouchInput is true.
		while( useTouchInput )
		{
			// If there are touches on the screen...
			if( Input.touchCount > 0 )
			{
				// Loop through each finger on the screen...
				for( int fingerId = 0; fingerId < Input.touchCount; fingerId++ )
				{
					// If the input phase has begun...
					if( Input.GetTouch( fingerId ).phase == TouchPhase.Began )
					{
						// If the touch input position is within the bounds of the joystick rect, then process the down input on the joystick.
						if( joystickRect.Contains( Input.GetTouch( fingerId ).position ) )
							ProcessOnInputDown( Input.GetTouch( fingerId ).position, fingerId );
					}
					// Else if the input has moved, then process the moved input.
					else if( Input.GetTouch( fingerId ).phase == TouchPhase.Moved )
						ProcessOnInputMoved( Input.GetTouch( fingerId ).position, fingerId );
					// Else if the input has ended or if it was canceled, then process the input being released.
					else if( Input.GetTouch( fingerId ).phase == TouchPhase.Ended || Input.GetTouch( fingerId ).phase == TouchPhase.Canceled )
						ProcessOnInputUp( Input.GetTouch( fingerId ).position, fingerId );
				}
			}
			// Else there are no touches on the screen.
			else
			{
				// If the joystick has not been reset, then reset the joystick since there are no touches.
				if( !joystickReset )
					ResetJoystick();
			}
			
			yield return null;
		}
	}
	
	/// <summary>
	/// Processes the input when it has been initiated on the joystick.
	/// </summary>
	/// <param name="inputPosition">The position of the input on the screen.</param>
	/// <param name="inputId">The unique id of the input that has been initiated on the joystick.</param>
	void ProcessOnInputDown ( Vector2 inputPosition, int inputId )
	{
		// If the joystick is already in use, then return.
		if( joystickState )
			return;

		// If the user wants a circular boundary...
		if( boundary == Boundary.Circular )
		{
			// Then calculate the distance of the input to see if it is within range of the touch size.
			float distance = Vector2.Distance( joystick.position, inputPosition );

			// If the distance is out of range, then just return.
			if( ( distance / baseTrans.sizeDelta.x ) > 0.5f && joystickTouchSize != JoystickTouchSize.Custom )
				return;
		}

		// Set the joystick state since the joystick is being interacted with.
		joystickState = true;

		// Set joystickReset to false so that the joystick will know that it needs to be reset.
		joystickReset = false;

		// Assign the inputId so that the other functions can know if the pointer calling the function is the correct one.
		_inputId = inputId;

		// If the throwable option is selected and isThrowing, then stop the current movement.
		if( gravity > 0 && gravityActive )
			StopCoroutine( "GravityHandler" );

		// If dynamicPositioning or disableVisuals are enabled...
		if( dynamicPositioning || disableVisuals )
		{
			// Then move the joystickBase to the position of the touch.
			joystickBase.position = inputPosition;

			// Set the joystickCenter so that the position can be calculated correctly.
			joystickCenter = inputPosition;
		}

		// If the user wants to show the input transitions...
		if( inputTransition )
		{
			// If either of the transition durations are set to something other than 0, then start the coroutine to transition over time.
			if( transitionUntouchedDuration > 0 || transitionTouchedDuration > 0 )
				StartCoroutine( "InputTransition" );
			// Else the user does not want to transition over time.
			else
			{
				// So just apply the touched alpha value.
				if( useFade )
					joystickGroup.alpha = fadeTouched;

				// And apply the touched scale.
				if( useScale )
					joystickBase.localScale = Vector3.one * scaleTouched;
			}
		}

		// If the user is wanting to use any tap count...
		if( tapCountOption != TapCountOption.NoCount )
		{
			// If the user is accumulating taps...
			if( tapCountOption == TapCountOption.Accumulate )
			{
				// If the TapCountdown is not counting down...
				if( currentTapTime <= 0 )
				{
					// Set tapCount to 1 since this is the initial touch and start the TapCountdown.
					tapCount = 1;
					StartCoroutine( "TapCountdown" );
				}
				// Else the TapCountdown is currently counting down, so increase the current tapCount.
				else
					++tapCount;

				if( currentTapTime > 0 && tapCount >= targetTapCount )
				{
					// Set the current time to 0 to interrupt the coroutine.
					currentTapTime = 0;

					// Start the delay of the reference for one frame.
					StartCoroutine( "TapCountDelay" );
				}
			}
			// Else the user wants to touch and release, so start the TapCountdown timer.
			else
				StartCoroutine( "TapCountdown" );
		}

		// Call UpdateJoystick with the info from the current PointerEventData.
		ProcessInput( inputPosition );

		// Notify any subscribers that the OnPointerDown function has been called.
		if( OnPointerDownCallback != null )
			OnPointerDownCallback();
	}

	/// <summary>
	/// Processes the input when it has been moved on the screen.
	/// </summary>
	/// <param name="inputPosition">The position of the input on the screen.</param>
	/// <param name="inputId">The unique id of the input being sent in to this function.</param>
	void ProcessOnInputMoved ( Vector2 inputPosition, int inputId )
	{
		// If the pointer event that is calling this function is not the same as the one that initiated the joystick, then return.
		if( inputId != _inputId )
			return;

		// Then call UpdateJoystick with the info from the current PointerEventData.
		ProcessInput( inputPosition );

		// Notify any subscribers that the OnDrag function has been called.
		if( OnDragCallback != null )
			OnDragCallback();
	}

	/// <summary>
	/// Processes the input when it has been released.
	/// </summary>
	/// <param name="inputPosition">The position of the input on the screen.</param>
	/// <param name="inputId">The unique id of the input being sent into this function.</param>
	void ProcessOnInputUp ( Vector2 inputPosition, int inputId )
	{
		// If the pointer event that is calling this function is not the same as the one that initiated the joystick, then return.
		if( inputId != _inputId )
			return;

		// Since the touch has lifted, set the state to false and reset the local pointerId.
		joystickState = false;
		_inputId = -10;

		// If dynamicPositioning, disableVisuals, or extendRadius are enabled...
		if( dynamicPositioning || disableVisuals || extendRadius )
		{
			// The joystickBase needs to be reset back to the default position.
			joystickBase.position = defaultPos;

			// Reset the joystickCenter since the touch has been released.
			joystickCenter = joystickBase.position;
		}

		// If the user has the gravity set to something more than 0 but less than 60, begin GravityHandler().
		if( gravity > 0 && gravity < 60 )
			StartCoroutine( "GravityHandler" );
		// Reset the joystick's position back to center.
		else
			joystick.anchoredPosition = Vector2.zero;

		// If the user has showTension enabled, then reset the tension if throwable is disabled.
		if( showTension && ( gravity <= 0 || gravity >= 60 ) )
			TensionAccentReset();

		// If the user wants an input transition, but the durations of both touched and untouched states are zero...
		if( inputTransition && ( transitionTouchedDuration <= 0 && transitionUntouchedDuration <= 0 ) )
		{
			// Then just apply the alpha.
			if( useFade )
				joystickGroup.alpha = fadeUntouched;

			// And reset the scale back to one.
			if( useScale )
				joystickBase.localScale = Vector3.one;
		}

		// If the user is wanting to use the TouchAndRelease tap count...
		if( tapCountOption == TapCountOption.TouchRelease )
		{
			// If the tapTime is still above zero, then start the delay function.
			if( currentTapTime > 0 )
				StartCoroutine( "TapCountDelay" );

			// Reset the current tap time to zero.
			currentTapTime = 0;
		}

		// Update the position values.
		UpdatePositionValues();

		// Notify any subscribers that the OnPointerUp function has been called.
		if( OnPointerUpCallback != null )
			OnPointerUpCallback();
	}

	/// <summary>
	/// Processes the input provided and moves the joystick accordingly.
	/// </summary>
	/// <param name="inputPosition">The current position of the input.</param>
	void ProcessInput ( Vector2 inputPosition )
	{
		// Create a new Vector2 to equal the vector from the current touch to the center of joystick.
		Vector2 tempVector = inputPosition - ( Vector2 )joystickCenter;

		// If the user wants only one axis, then zero out the opposite value.
		if( axis == Axis.X )
			tempVector.y = 0;
		else if( axis == Axis.Y )
			tempVector.x = 0;

		// If the user wants a circular boundary for the joystick, then clamp the magnitude by the radius.
		if( boundary == Boundary.Circular )
			tempVector = Vector2.ClampMagnitude( tempVector, radius );
		// Else the user wants a square boundary, so clamp X and Y individually.
		else if( boundary == Boundary.Square )
		{
			tempVector.x = Mathf.Clamp( tempVector.x, -radius, radius );
			tempVector.y = Mathf.Clamp( tempVector.y, -radius, radius );
		}

		// Apply the tempVector to the joystick's position.
		joystick.transform.position = ( Vector2 )joystickCenter + tempVector;
		
		// If the user wants to drag the joystick along with the touch...
		if( extendRadius )
		{
			// Store the position of the current touch.
			Vector3 currentTouchPosition = inputPosition;

			// If the user is using any axis option, then align the current touch position.
			if( axis != Axis.Both )
			{
				if( axis == Axis.X )
					currentTouchPosition.y = joystickCenter.y;
				else
					currentTouchPosition.x = joystickCenter.x;
			}
			// Then find the distance that the touch is from the center of the joystick.
			float touchDistance = Vector3.Distance( joystickCenter, currentTouchPosition );

			// If the touchDistance is greater than the set radius...
			if( touchDistance >= radius )
			{
				// Figure out the current position of the joystick.
				Vector2 joystickPosition = ( joystick.position - joystickCenter ) / radius;

				// Move the joystickBase in the direction that the joystick is, multiplied by the difference in distance of the max radius.
				joystickBase.position += new Vector3( joystickPosition.x, joystickPosition.y, 0 ) * ( touchDistance - radius );

				// Reconfigure the joystickCenter since the joystick has now moved it position.
				joystickCenter = joystickBase.position;
			}
		}

		// Update the position values since the joystick has been updated.
		UpdatePositionValues();

		// If the user has showTension enabled, then display the Tension.
		if( showTension )
			TensionAccentDisplay();
	}
	
	/// <summary>
	/// This function will configure the position of an image based on the size and positioning set by the user.
	/// </summary>
	/// <param name="imageSize">The size of the image for calculating the position of the rect transform.</param>
	/// <param name="rawPosition">The raw position values (0-100).</param>
	Vector2 ConfigureImagePosition ( Vector2 imageSize, Vector2 rawPosition )
	{
		// First, fix the customSpacing to be a value between 0.0f and 1.0f.
		Vector2 fixedCustomSpacing = rawPosition / 100;

		// Then configure position spacers according to the screen's dimensions, the fixed spacing and texture size.
		float positionSpacerX = Screen.width * fixedCustomSpacing.x - ( imageSize.x * fixedCustomSpacing.x );
		float positionSpacerY = Screen.height * fixedCustomSpacing.y - ( imageSize.y * fixedCustomSpacing.y );

		// Create a temporary Vector2 to modify and return.
		Vector2 tempVector;

		// If it's left, simply apply the positionxSpacerX, else calculate out from the right side and apply the positionSpaceX.
		tempVector.x = anchor == Anchor.Left ? positionSpacerX : ( Screen.width - imageSize.x ) - positionSpacerX;
		tempVector.x += ( imageSize.x / 2 );
		
		// Apply the positionSpacerY variable.
		tempVector.y = positionSpacerY + ( imageSize.y / 2 );

		// Return the updated temporary Vector.
		return tempVector;
	}

	/// <summary>
	/// This function updates the joystick's position on the screen.
	/// </summary>
	void UpdateJoystickPositioning ()
	{
		// If any of the needed components are left unassigned, then inform the user and return.
		if( joystickBase == null )
		{
			if( Application.isPlaying )
				Debug.LogError( "Ultimate Joystick\nThere are some needed components that are not currently assigned. Please check the Assigned Variables section and be sure to assign all of the components." );
			return;
		}

		// Set the current reference size for scaling.
		float referenceSize = scalingAxis == ScalingAxis.Height ? Screen.height : Screen.width;

		// Configure the target size for the joystick graphic.
		float textureSize = referenceSize * ( joystickSize / 10 );

		// If baseTrans is null, store this object's RectTrans so that it can be positioned.
		if( baseTrans == null )
			baseTrans = GetComponent<RectTransform>();

		// Force the anchors and pivot so the joystick will function correctly. This is also needed here for older versions of the Ultimate Joystick that didn't use these rect transform settings.
		baseTrans.anchorMin = Vector2.zero;
		baseTrans.anchorMax = Vector2.zero;
		baseTrans.pivot = new Vector2( 0.5f, 0.5f );

		// Get a position for the joystick based on the position variables.
		Vector2 imagePosition = ConfigureImagePosition( new Vector2( textureSize, textureSize ), new Vector2( positionHorizontal, positionVertical ) );

		// If the user wants a custom touch size...
		if( customActivationRange )
		{
			// Fix the custom size variables.
			float fixedFBPX = activationWidth / 100;
			float fixedFBPY = activationHeight / 100;

			// Depending on the joystickTouchSize options, configure the size.
			baseTrans.sizeDelta = new Vector2( Screen.width * fixedFBPX, Screen.height * fixedFBPY );

			// Send the size and custom positioning to the ConfigureImagePosition function to get the exact position.
			Vector2 imagePos = ConfigureImagePosition( baseTrans.sizeDelta, new Vector2( activationPositionHorizontal, activationPositionVertical ) );

			// Apply the new position.
			baseTrans.position = imagePos;
		}
		else
		{
			// Temporary Vector2 to store the default size of the joystick.
			Vector2 tempVector = new Vector2( textureSize, textureSize );

			// Apply the joystick size multiplied by the activation range.
			baseTrans.sizeDelta = tempVector * activationRange;

			// Apply the imagePosition modified with the difference of the sizeDelta divided by 2, multiplied by the scale of the parent canvas.
			baseTrans.position = imagePosition;
		}

		// If the options dictate that the default position needs to be stored, then store it here.
		if( dynamicPositioning || disableVisuals || extendRadius )
			defaultPos = imagePosition;

		// Set the anchors of the joystick base. It is important to have the anchors centered for calculations.
		joystickBase.anchorMin = new Vector2( 0.5f, 0.5f );
		joystickBase.anchorMax = new Vector2( 0.5f, 0.5f );
		joystickBase.pivot = new Vector2( 0.5f, 0.5f );

		// Apply the size and position to the joystickBase.
		joystickBase.sizeDelta = new Vector2( textureSize, textureSize );
		joystickBase.position = imagePosition;

		// Configure the size of the Ultimate Joystick's radius.
		radius = joystickBase.sizeDelta.x * ( radiusModifier / 10 );

		// Store the joystick's center so that JoystickPosition can be configured correctly.
		joystickCenter = joystickBase.position;

		// If the user wants to transition, and the joystickGroup is unassigned, find the CanvasGroup.
		if( inputTransition && joystickGroup == null )
		{
			joystickGroup = GetComponent<CanvasGroup>();
			if( joystickGroup == null )
				joystickGroup = gameObject.AddComponent<CanvasGroup>();
		}

		// If the user wants to use touch input, then configure the joystick rect for hit calculations.
		if( useTouchInput )
			joystickRect = new Rect( new Vector2( baseTrans.position.x - ( baseTrans.sizeDelta.x / 2 ), baseTrans.position.y - ( baseTrans.sizeDelta.y / 2 ) ), baseTrans.sizeDelta );
	}

	/// <summary>
	/// This function is called only when showTension is true, and only when the joystick is moving.
	/// </summary>
	void TensionAccentDisplay ()
	{
		// If the tension accent images are null, then inform the user and return.
		if( TensionAccents.Count == 0 )
		{
			Debug.LogError( "Ultimate Joystick\nThere are no tension accent images assigned. This could be happening for several reasons, but all of them should be fixable in the Ultimate Joystick inspector." );
			return;
		}

		// If the user wants to display directional tension...
		if( tensionType == TensionType.Directional )
		{
			// Calculate the joystick axis values.
			Vector2 joystickAxis = ( joystick.position - joystickCenter ) / radius;

			// If the joystick is to the right...
			if( joystickAxis.x > 0 )
			{
				// Then lerp the color according to tension's X position.
				if( TensionAccents[ 3 ] != null )
					TensionAccents[ 3 ].color = Color.Lerp( tensionColorNone, tensionColorFull, joystickAxis.x <= tensionDeadZone ? 0 : ( joystickAxis.x - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );

				// If the opposite tension is not tensionColorNone, the make it so.
				if( TensionAccents[ 1 ] != null && TensionAccents[ 1 ].color != tensionColorNone )
					TensionAccents[ 1 ].color = tensionColorNone;
			}
			// Else the joystick is to the left...
			else
			{
				// Repeat above steps...
				if( TensionAccents[ 1 ] != null )
					TensionAccents[ 1 ].color = Color.Lerp( tensionColorNone, tensionColorFull, Mathf.Abs( joystickAxis.x ) <= tensionDeadZone ? 0 : ( Mathf.Abs( joystickAxis.x ) - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );
				if( TensionAccents[ 3 ] != null && TensionAccents[ 3 ].color != tensionColorNone )
					TensionAccents[ 3 ].color = tensionColorNone;
			}

			// If the joystick is up...
			if( joystickAxis.y > 0 )
			{
				// Then lerp the color according to tension's Y position.
				if( TensionAccents[ 0 ] != null )
					TensionAccents[ 0 ].color = Color.Lerp( tensionColorNone, tensionColorFull, joystickAxis.y <= tensionDeadZone ? 0 : ( joystickAxis.y - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );

				// If the opposite tension is not tensionColorNone, the make it so.
				if( TensionAccents[ 2 ] != null && TensionAccents[ 2 ].color != tensionColorNone )
					TensionAccents[ 2 ].color = tensionColorNone;
			}
			// Else the joystick is down...
			else
			{
				// Repeat above steps...
				if( TensionAccents[ 2 ] != null )
					TensionAccents[ 2 ].color = Color.Lerp( tensionColorNone, tensionColorFull, Mathf.Abs( joystickAxis.y ) <= tensionDeadZone ? 0 : ( Mathf.Abs( joystickAxis.y ) - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );
				if( TensionAccents[ 0 ] != null && TensionAccents[ 0 ].color != tensionColorNone )
					TensionAccents[ 0 ].color = tensionColorNone;
			}
		}
		// Else the user wants to display free tension...
		else
		{
			// If the first index tension is null, then inform the user and return to avoid errors.
			if( TensionAccents[ 0 ] == null )
			{
				Debug.LogError( "Ultimate Joystick\nThere are no tension accent images assigned. This could be happening for several reasons, but all of them should be fixable in the Ultimate Joystick inspector." );
				return;
			}

			// Store the distance for calculations.
			float distance = GetDistance();

			// Lerp the color according to the distance of the joystick from center.
			TensionAccents[ 0 ].color = Color.Lerp( tensionColorNone, tensionColorFull, distance <= tensionDeadZone ? 0 : ( distance - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );

			// Rotate the tension transform to aim at the direction that the joystick is pointing.
			TensionAccents[ 0 ].transform.rotation = Quaternion.Euler( 0, 0, ( Mathf.Atan2( VerticalAxis, HorizontalAxis ) * Mathf.Rad2Deg ) + rotationOffset - 90 );
		}
	}
	
	/// <summary>
	/// This function resets the tension image's colors back to default.
	/// </summary>
	void TensionAccentReset ()
	{
		// Loop through each tension accent.
		for( int i = 0; i < TensionAccents.Count; i++ )
		{
			// If the tension accent is unassigned, then skip this index.
			if( TensionAccents[ i ] == null )
				continue;

			// Reset the color of this tension image back to no tension.
			TensionAccents[ i ].color = tensionColorNone;
		}

		// If the joystick is using a free tension, then reset the tension rotation back to center.
		if( tensionType == TensionType.Free && TensionAccents.Count > 0 && TensionAccents[ 0 ] != null )
			TensionAccents[ 0 ].transform.rotation = Quaternion.identity;
	}
	
	/// <summary>
	/// This function is for returning the joystick back to center for a set amount of time.
	/// </summary>
	IEnumerator GravityHandler ()
	{
		// Set gravityActive to true so other functions know it is running.
		gravityActive = true;
		float speed = 1.0f / ( GetDistance() / gravity );

		// Store the position of where the joystick is currently.
		Vector3 startJoyPos = joystick.position;

		// Loop for the time it will take for the joystick to return to center.
		for( float t = 0.0f; t < 1.0f && gravityActive; t += Time.deltaTime * speed )
		{
			// Lerp the joystick's position from where this coroutine started to the center.
			joystick.position = Vector3.Lerp( startJoyPos, joystickCenter, t );

			// If the user a direction display option enabled, then display the direction as the joystick moves.
			if( showTension )
				TensionAccentDisplay();

			// Update the position values since the joystick has moved.
			UpdatePositionValues();

			yield return null;
		}

		// If the gravityActive controller is still true, then the user has not interrupted the joystick returning to center.
		if( gravityActive )
		{
			// Finalize the joystick's position.
			joystick.position = joystickCenter;

			// Here at the end, reset the direction display.
			if( showTension )
				TensionAccentReset();

			// And update the position values since the joystick has reached the center.
			UpdatePositionValues();
		}

		// Set gravityActive to false so that other functions can know it is finished.
		gravityActive = false;
	}

	/// <summary>
	/// This coroutine will handle the input transitions over time according to the users options.
	/// </summary>
	IEnumerator InputTransition ()
	{
		// Store the current values for the alpha and scale of the joystick.
		float currentAlpha = joystickGroup.alpha;
		float currentScale = joystickBase.localScale.x;

		// If the scaleInSpeed is NaN....
		if( float.IsInfinity( transitionTouchedSpeed ) )
		{
			// Set the alpha to the touched value.
			if( useFade )
				joystickGroup.alpha = fadeTouched;

			// Set the scale to the touched value.
			if( useScale )
				joystickBase.localScale = Vector3.one * scaleTouched;
		}
		// Else run the loop to transition to the desired values over time.
		else
		{
			// This for loop will continue for the transition duration.
			for( float transition = 0.0f; transition < 1.0f && joystickState; transition += Time.deltaTime * transitionTouchedSpeed )
			{
				// Lerp the alpha of the canvas group.
				if( useFade )
					joystickGroup.alpha = Mathf.Lerp( currentAlpha, fadeTouched, transition );

				// Lerp the scale of the joystick.
				if( useScale )
					joystickBase.localScale = Vector3.one * Mathf.Lerp( currentScale, scaleTouched, transition );

				yield return null;
			}

			// If the joystick is still being interacted with, then finalize the values since the loop above has ended.
			if( joystickState )
			{
				if( useFade )
					joystickGroup.alpha = fadeTouched;

				if( useScale )
					joystickBase.localScale = Vector3.one * scaleTouched;
			}
		}

		// While loop for while joystickState is true
		while( joystickState )
			yield return null;

		// Set the current values.
		currentAlpha = joystickGroup.alpha;
		currentScale = joystickBase.localScale.x;

		// If the scaleOutSpeed value is NaN, then apply the desired alpha and scale.
		if( float.IsInfinity( transitionUntouchedSpeed ) )
		{
			if( useFade )
				joystickGroup.alpha = fadeUntouched;

			if( useScale )
				joystickBase.localScale = Vector3.one;
		}
		// Else run the loop to transition to the desired values over time.
		else
		{
			for( float transition = 0.0f; transition < 1.0f && !joystickState; transition += Time.deltaTime * transitionUntouchedSpeed )
			{
				if( useFade )
					joystickGroup.alpha = Mathf.Lerp( currentAlpha, fadeUntouched, transition );

				if( useScale )
					joystickBase.localScale = Vector3.one * Mathf.Lerp( currentScale, 1.0f, transition );
				yield return null;
			}

			// If the joystick is still not being interacted with, then finalize the alpha and scale since the loop above finished.
			if( !joystickState )
			{
				if( useFade )
					joystickGroup.alpha = fadeUntouched;

				if( useScale )
					joystickBase.localScale = Vector3.one;
			}
		}
	}

	/// <summary>
	/// This function counts down the tap count duration. The current tap time that is being modified is check within the input functions.
	/// </summary>
	IEnumerator TapCountdown ()
	{
		// Set the current tap time to the max.
		currentTapTime = tapCountDuration;
		while( currentTapTime > 0 )
		{
			// Reduce the current time.
			currentTapTime -= Time.deltaTime;
			yield return null;
		}
	}

	/// <summary>
	/// This function delays for one frame so that it can be correctly referenced as soon as it is achieved.
	/// </summary>
	IEnumerator TapCountDelay ()
	{
		tapCountAchieved = true;
		yield return new WaitForEndOfFrame();
		tapCountAchieved = false;
	}
	
	/// <summary>
	/// This function updates the position values of the joystick so that they can be referenced.
	/// </summary>
	void UpdatePositionValues ()
	{
		// Store the relative position of the joystick and divide the Vector by the radius of the joystick. This will normalize the values.
		Vector2 joystickPosition = ( joystick.position - joystickCenter ) / radius;

		// If the distance of the joystick from center is less that the dead zone set by the user...
		if( GetDistance() <= deadZone )
		{
			// Then zero out the axis values.
			joystickPosition.x = 0.0f;
			joystickPosition.y = 0.0f;
		}

		// Finally, set the horizontal and vertical axis values for reference.
		HorizontalAxis = joystickPosition.x;
		VerticalAxis = joystickPosition.y;
	}

	/// <summary>
	/// Returns with a confirmation about the existence of the targeted Ultimate Joystick.
	/// </summary>
	static bool JoystickConfirmed ( string joystickName )
	{
		if( !UltimateJoysticks.ContainsKey( joystickName ) )
		{
			Debug.LogWarning( "Ultimate Joystick\nNo Ultimate Joystick has been registered with the name: " + joystickName + "." );
			return false;
		}
		return true;
	}

	/// <summary>
	/// Resets the joystick position and input information and stops any coroutines that might have been running.
	/// </summary>
	void ResetJoystick ()
	{
		joystickReset = true;
		gravityActive = false;
		StopCoroutine( "GravityHandler" );

		// Since the touch has lifted, set the state to false and reset the local pointerId.
		joystickState = false;
		_inputId = -10;
		
		// If dynamicPositioning, disableVisuals, or draggable are enabled...
		if( dynamicPositioning || disableVisuals || extendRadius )
		{
			// The joystickBase needs to be reset back to the default position.
			joystickBase.position = defaultPos;

			// Reset the joystickCenter since the touch has been released.
			joystickCenter = joystickBase.position;
		}
		// Reset the joystick's position back to center.
		joystick.position = joystickCenter;

		// If the user has showTension enabled, then reset the tension if throwable is disabled.
		if( showTension )
			TensionAccentReset();
	}

	#if UNITY_EDITOR
	void Update ()
	{
		// Keep the joystick updated while the game is not being played.
		if( !Application.isPlaying )
			UpdateJoystickPositioning();
	}
	#endif

	/* --------------------------------------------- *** PUBLIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Resets the joystick and updates the size and placement of the Ultimate Joystick. Useful for screen rotations, changing of screen size, or changing of size and placement options.
	/// </summary>
	public void UpdatePositioning ()
	{
		// If the game is running, then reset the joystick.
		if( Application.isPlaying )
			ResetJoystick();

		// Update the positioning.
		UpdateJoystickPositioning();

		// Notify any subscribers that the UpdatePositioning function has been called.
		if( OnUpdatePositioning != null )
			OnUpdatePositioning();
	}
	
	/// <summary>
	/// Returns a float value between -1 and 1 representing the horizontal value of the Ultimate Joystick.
	/// </summary>
	public float GetHorizontalAxis ()
	{
		return HorizontalAxis;
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the vertical value of the Ultimate Joystick.
	/// </summary>
	public float GetVerticalAxis ()
	{
		return VerticalAxis;
	}

	/// <summary>
	/// Returns a value of -1, 0 or 1 representing the raw horizontal value of the Ultimate Joystick.
	/// </summary>
	public float GetHorizontalAxisRaw ()
	{
		float temp = HorizontalAxis;

		if( Mathf.Abs( temp ) <= deadZone )
			temp = 0.0f;
		else
			temp = temp < 0.0f ? -1.0f : 1.0f;

		return temp;
	}

	/// <summary>
	/// Returns a value of -1, 0 or 1 representing the raw vertical value of the Ultimate Joystick.
	/// </summary>
	public float GetVerticalAxisRaw ()
	{
		float temp = VerticalAxis;
		if( Mathf.Abs( temp ) <= deadZone )
			temp = 0.0f;
		else
			temp = temp < 0.0f ? -1.0f : 1.0f;

		return temp;
	}

	/// <summary>
	/// Returns the current value of the horizontal axis.
	/// </summary>
	public float HorizontalAxis
	{
		get;
		private set;
	}

	/// <summary>
	/// Returns the current value of the vertical axis.
	/// </summary>
	public float VerticalAxis
	{
		get;
		private set;
	}

	/// <summary>
	/// Returns a float value between 0 and 1 representing the distance of the joystick from the base.
	/// </summary>
	public float GetDistance ()
	{
		return Vector3.Distance( joystick.position, joystickCenter ) / radius;
	}

	/// <summary>
	/// Updates the color of the highlights attached to the Ultimate Joystick with the targeted color.
	/// </summary>
	/// <param name="targetColor">New highlight color.</param>
	public void UpdateHighlightColor ( Color targetColor )
	{
		if( !showHighlight )
			return;

		highlightColor = targetColor;
		
		// Check if each variable is assigned so there is not a null reference exception when applying color.
		if( highlightBase != null )
			highlightBase.color = highlightColor;
		if( highlightJoystick != null )
			highlightJoystick.color = highlightColor;
	}

	/// <summary>
	/// Updates the colors of the tension accents attached to the Ultimate Joystick with the targeted colors.
	/// </summary>
	/// <param name="targetTensionNone">New idle tension color.</param>
	/// <param name="targetTensionFull">New full tension color.</param>
	public void UpdateTensionColors ( Color targetTensionNone, Color targetTensionFull )
	{
		if( !showTension )
			return;

		tensionColorNone = targetTensionNone;
		tensionColorFull = targetTensionFull;
	}

	/// <summary>
	/// Returns the current state of the Ultimate Joystick. This function will return true when the joystick is being interacted with, and false when not.
	/// </summary>
	public bool GetJoystickState ()
	{
		return joystickState;
	}

	/// <summary>
	/// Returns the tap count to the Ultimate Joystick.
	/// </summary>
	public bool GetTapCount ()
	{
		return tapCountAchieved;
	}

	/// <summary>
	/// Disables the Ultimate Joystick.
	/// </summary>
	public void DisableJoystick ()
	{
		// Set the states to false.
		joystickState = false;
		_inputId = -10;
		
		// If the joystick center has been changed, then reset it.
		if( dynamicPositioning || disableVisuals || extendRadius )
		{
			joystickBase.position = defaultPos;
			joystickCenter = joystickBase.position;
		}
		
		// Reset the position of the joystick.
		joystick.position = joystickCenter;

		// Update the joystick position values since the joystick has been reset.
		UpdatePositionValues();
		
		// If the user is displaying tension accents, then reset them here.
		if( showTension )
			TensionAccentReset();

		// If the user wants to show a transition on the different input states...
		if( inputTransition )
		{
			// If the user is displaying a fade, then reset to the untouched state.
			if( useFade )
				joystickGroup.alpha = fadeUntouched;

			// If the user is scaling the joystick, then reset the scale.
			if( useScale )
				joystickBase.transform.localScale = Vector3.one;
		}
		
		// Disable the gameObject.
		gameObject.SetActive( false );
	}

	/// <summary>
	/// Enables the Ultimate Joystick.
	/// </summary>
	public void EnableJoystick ()
	{
		// Reset the joystick's position again.
		joystick.position = joystickCenter;

		// Enable the gameObject.
		gameObject.SetActive( true );
	}
	/* ------------------------------------------- *** END PUBLIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
	
	/* --------------------------------------------- *** STATIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Returns the Ultimate Joystick of the targeted name if it exists within the scene.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	public static UltimateJoystick GetUltimateJoystick ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return null;

		return UltimateJoysticks[ joystickName ];
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the horizontal value of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetHorizontalAxis ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetHorizontalAxis();
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the vertical value of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetVerticalAxis ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetVerticalAxis();
	}

	/// <summary>
	/// Returns a value of -1, 0 or 1 representing the raw horizontal value of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetHorizontalAxisRaw ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetHorizontalAxisRaw();
	}

	/// <summary>
	/// Returns a value of -1, 0 or 1 representing the raw vertical value of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetVerticalAxisRaw ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetVerticalAxisRaw();
	}

	/// <summary>
	/// Returns a float value between 0 and 1 representing the distance of the joystick from the base.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetDistance ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetDistance();
	}

	/// <summary>
	/// Returns the current interaction state of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static bool GetJoystickState ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return false;

		return UltimateJoysticks[ joystickName ].joystickState;
	}

	/// <summary>
	/// Returns the current state of the tap count according to the options set.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static bool GetTapCount ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return false;

		return UltimateJoysticks[ joystickName ].GetTapCount();
	}

	/// <summary>
	/// Disables the targeted Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static void DisableJoystick ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return;

		UltimateJoysticks[ joystickName ].DisableJoystick();
	}

	/// <summary>
	/// Enables the targeted Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static void EnableJoystick ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return;

		UltimateJoysticks[ joystickName ].EnableJoystick();
	}
	/* ------------------------------------------- *** END STATIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
}