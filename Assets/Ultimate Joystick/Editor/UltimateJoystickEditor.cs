/* UltimateJoystickEditor.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//[CanEditMultipleObjects]
[CustomEditor( typeof( UltimateJoystick ) )]
public class UltimateJoystickEditor : Editor
{
	/* TODO LIST EDIT:
	 * 
	 * Try to make the inspector easier to work with, we don't want the user to have to ever leave the Ultimate Joystick to get something working the way that they want.
	 * 
	 * PROBLEM when the user is editing Prefabs inside the project instead of the scene. The script should not create or delete anything when selecting a prefab.
	 * 
	 */

	UltimateJoystick targ;
	bool isPrefabInProjectWindow = false;
	const int afterIndentSpace = 5;

	// -----< SIZE AND PLACEMENT >----- //
	Canvas parentCanvas;
	SerializedProperty joystickBase, joystick;
	Sprite joystickBaseSprite, joystickSprite;
	SerializedProperty scalingAxis, anchor;
	SerializedProperty activationRange, customActivationRange;
	SerializedProperty activationWidth, activationHeight;
	SerializedProperty activationPositionHorizontal, activationPositionVertical;
	
	SerializedProperty dynamicPositioning;
	SerializedProperty joystickSize, radiusModifier;
	SerializedProperty positionHorizontal, positionVertical;

	// -----< JOYSTICK FUNCTIONALITY >----- //
	SerializedProperty gravity, extendRadius;
	SerializedProperty axis, boundary, deadZone;
	SerializedProperty tapCountOption, tapCountDuration;
	SerializedProperty targetTapCount, useTouchInput;

	// -----< VISUAL OPTIONS >----- //
	Color baseColor;
	SerializedProperty disableVisuals, inputTransition;
	SerializedProperty useFade, useScale;
	SerializedProperty transitionUntouchedDuration, transitionTouchedDuration;
	SerializedProperty fadeUntouched, fadeTouched, scaleTouched;
	SerializedProperty showHighlight, highlightColor;
	SerializedProperty highlightBase, highlightJoystick;
	Sprite highlightBaseSprite, highlightJoystickSprite;
	SerializedProperty showTension, tensionColorNone, tensionColorFull;
	SerializedProperty tensionType, rotationOffset, tensionDeadZone;
	Sprite tensionAccentSprite;
	bool editTensionSprites = true, editTensionImages = false;
	bool noSpriteDirection = false;
	float tensionScale = 1.0f;

	/* ------< SCRIPT REFERENCE >------ */
	SerializedProperty joystickName;

	// DEVELOPMENT MODE //
	public bool showDefaultInspector = false;

	// ----->>> EXAMPLE CODE //
	class ExampleCode
	{
		public string optionName = "";
		public string optionDescription = "";
		public string basicCode = "";
	}
	ExampleCode[] exampleCodes = new ExampleCode[]
	{
		new ExampleCode() { optionName = "GetHorizontalAxis ( string joystickName )", optionDescription = "Returns the horizontal axis value of the targeted Ultimate Joystick.", basicCode = "float h = UltimateJoystick.GetHorizontalAxis( \"{0}\" );" },
		new ExampleCode() { optionName = "GetVerticalAxis ( string joystickName )", optionDescription = "Returns the vertical axis value of the targeted Ultimate Joystick.", basicCode = "float v = UltimateJoystick.GetVerticalAxis( \"{0}\" );" },
		new ExampleCode() { optionName = "GetHorizontalAxisRaw ( string joystickName )", optionDescription = "Returns the raw horizontal axis value of the targeted Ultimate Joystick.", basicCode = "float h = UltimateJoystick.GetHorizontalAxisRaw( \"{0}\" );" },
		new ExampleCode() { optionName = "GetVerticalAxisRaw ( string joystickName )", optionDescription = "Returns the raw vertical axis value of the targeted Ultimate Joystick.", basicCode = "float v = UltimateJoystick.GetVerticalAxisRaw( \"{0}\" );" },
		new ExampleCode() { optionName = "GetDistance ( string joystickName )", optionDescription = "Returns the distance of the joystick image from the center of the targeted Ultimate Joystick.", basicCode = "float distance = UltimateJoystick.GetDistance( \"{0}\" );" },
		new ExampleCode() { optionName = "GetJoystickState ( string joystickName )", optionDescription = "Returns the bool value of the current state of interaction of the targeted Ultimate Joystick.", basicCode = "if( UltimateJoystick.GetJoystickState( \"{0}\" ) )" },
		new ExampleCode() { optionName = "GetTapCount ( string joystickName )", optionDescription = "Returns the bool value of the current state of taps of the targeted Ultimate Joystick.", basicCode = "if( UltimateJoystick.GetTapCount( \"{0}\" ) )" },
		new ExampleCode() { optionName = "DisableJoystick ( string joystickName )", optionDescription = "Disables the targeted Ultimate Joystick.", basicCode = "UltimateJoystick.DisableJoystick( \"{0}\" );" },
		new ExampleCode() { optionName = "EnableJoystick ( string joystickName )", optionDescription = "Enables the targeted Ultimate Joystick.", basicCode = "UltimateJoystick.EnableJoystick( \"{0}\" );" },
		new ExampleCode() { optionName = "GetUltimateJoystick ( string joystickName )", optionDescription = "Returns the Ultimate Joystick component that has been registered with the targeted name.", basicCode = "UltimateJoystick movementJoystick = UltimateJoystick.GetUltimateJoystick( \"{0}\" );" },
	};
	List<string> exampleCodeOptions = new List<string>();
	int exampleCodeIndex = 0;

	// SCENE GUI //
	class DisplaySceneGizmo
	{
		public int frames = maxFrames;
		public bool hover = false;

		public bool HighlightGizmo
		{
			get
			{
				return hover || frames < maxFrames;
			}
		}

		public void PropertyUpdated ()
		{
			frames = 0;
		}
	}
	DisplaySceneGizmo DisplayActivationRange = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayActivationCustomWidth = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayActivationCustomHeight = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayRadius = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayBoundary = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayAxis = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayDeadZone = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayTensionDeadZone = new DisplaySceneGizmo();
	const int maxFrames = 200;

	// Gizmo Colors //
	Color colorDefault = Color.black;
	Color colorValueChanged = Color.black;

	// EDITOR STYLES //
	GUIStyle handlesCenteredText = new GUIStyle();
	GUIStyle collapsableSectionStyle = new GUIStyle();
	

	bool CanvasErrors
	{
		get
		{
			// If the selection is currently empty, then return false.
			if( Selection.activeGameObject == null )
				return false;

			// If the selection is actually the prefab within the Project window, then return no errors.
			if( AssetDatabase.Contains( Selection.activeGameObject ) )
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
	}

	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();
		
		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;

		if( targ != null )
		{
			if( !targ.gameObject.GetComponent<Image>() )
				Undo.AddComponent<Image>( targ.gameObject );

			Undo.RecordObject( targ.gameObject.GetComponent<Image>(), "Null Joystick Alpha" );
			targ.gameObject.GetComponent<Image>().color = new Color( 1.0f, 1.0f, 1.0f, 0.0f );
		}

		if( EditorPrefs.HasKey( "UJ_ColorHexSetup" ) )
		{
			ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "UJ_ColorDefaultHex" ), out colorDefault );
			ColorUtility.TryParseHtmlString( EditorPrefs.GetString( "UJ_ColorValueChangedHex" ), out colorValueChanged );
		}

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
		if( parent == null && !AssetDatabase.Contains( Selection.activeGameObject ) )
			RequestCanvas( Selection.activeGameObject );

		return null;
	}
	
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}
	
	void DisplayHeaderDropdown ( string headerName, string editorPref )
	{
		EditorGUILayout.Space();

		GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11 };
		GUILayout.BeginHorizontal();
		GUILayout.Space( -10 );
		EditorPrefs.SetBool( editorPref, GUILayout.Toggle( EditorPrefs.GetBool( editorPref ), ( EditorPrefs.GetBool( editorPref ) ? "▼ " : "► " ) + headerName, toolbarStyle ) );
		GUILayout.EndHorizontal();

		if( EditorPrefs.GetBool( editorPref ) == true )
			EditorGUILayout.Space();
	}

	bool DisplayCollapsibleBoxSection ( string sectionTitle, string editorPref, SerializedProperty enabledProp, ref bool valueChanged )
	{
		if( EditorPrefs.GetBool( editorPref ) && enabledProp.boolValue )
			collapsableSectionStyle.fontStyle = FontStyle.Bold;

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();
		enabledProp.boolValue = EditorGUILayout.Toggle( enabledProp.boolValue, GUILayout.Width( 25 ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			if( enabledProp.boolValue )
				EditorPrefs.SetBool( editorPref, true );
			else
				EditorPrefs.SetBool( editorPref, false );

			valueChanged = true;
		}

		GUILayout.Space( -25 );

		EditorGUI.BeginDisabledGroup( !enabledProp.boolValue );
		if( GUILayout.Button( sectionTitle, collapsableSectionStyle ) )
			EditorPrefs.SetBool( editorPref, !EditorPrefs.GetBool( editorPref ) );
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.EndHorizontal();

		if( EditorPrefs.GetBool( editorPref ) )
			collapsableSectionStyle.fontStyle = FontStyle.Normal;

		return EditorPrefs.GetBool( editorPref ) && enabledProp.boolValue;
	}
	
	void CheckPropertyHover ( DisplaySceneGizmo displaySceneGizmo )
	{
		displaySceneGizmo.hover = false;
		var rect = GUILayoutUtility.GetLastRect();
		if( Event.current.type == EventType.Repaint && rect.Contains( Event.current.mousePosition ) )
			displaySceneGizmo.hover = true;
	}

	void StoreReferences ()
	{
		targ = ( UltimateJoystick )target;

		isPrefabInProjectWindow = AssetDatabase.Contains( targ.gameObject );

		// -----< SIZE AND PLACEMENT >----- //
		joystickBase = serializedObject.FindProperty( "joystickBase" );
		if( targ.joystickBase != null && targ.joystickBase.GetComponent<Image>() && targ.joystickBase.GetComponent<Image>().sprite != null )
			joystickBaseSprite = targ.joystickBase.GetComponent<Image>().sprite;

		joystick = serializedObject.FindProperty( "joystick" );
		if( targ.joystick != null && targ.joystick.GetComponent<Image>() && targ.joystick.GetComponent<Image>().sprite != null )
			joystickSprite = targ.joystick.GetComponent<Image>().sprite;
		
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		anchor = serializedObject.FindProperty( "anchor" );
		activationRange = serializedObject.FindProperty( "activationRange" );
		customActivationRange = serializedObject.FindProperty( "customActivationRange" );
		activationWidth = serializedObject.FindProperty( "activationWidth" );
		activationHeight = serializedObject.FindProperty( "activationHeight" );
		activationPositionHorizontal = serializedObject.FindProperty( "activationPositionHorizontal" );
		activationPositionVertical = serializedObject.FindProperty( "activationPositionVertical" );
		dynamicPositioning = serializedObject.FindProperty( "dynamicPositioning" );
		joystickSize = serializedObject.FindProperty( "joystickSize" );
		radiusModifier = serializedObject.FindProperty( "radiusModifier" );
		positionHorizontal = serializedObject.FindProperty( "positionHorizontal" );
		positionVertical = serializedObject.FindProperty( "positionVertical" );

		// COPY DEPRECIATED VALUES //
		if( !isPrefabInProjectWindow )
		{
			if( targ.customSpacing_X != -10 || targ.customSpacing_Y != -10 )
			{
				positionHorizontal.floatValue = targ.customSpacing_X;
				positionVertical.floatValue = targ.customSpacing_Y;

				serializedObject.FindProperty( "customSpacing_X" ).floatValue = -10;
				serializedObject.FindProperty( "customSpacing_Y" ).floatValue = -10;

				serializedObject.ApplyModifiedProperties();
			}

			if( ( int )targ.joystickTouchSize >= 0 )
			{
				if( ( int )targ.joystickTouchSize == 0 )
					activationRange.floatValue = 1.0f;
				else if( ( int )targ.joystickTouchSize == 1 )
					activationRange.floatValue = 1.5f;
				else if( ( int )targ.joystickTouchSize == 2 )
					activationRange.floatValue = 2.0f;
				else if( ( int )targ.joystickTouchSize == 3 )
					customActivationRange.boolValue = true;

				serializedObject.FindProperty( "joystickTouchSize" ).intValue = -1;
				serializedObject.ApplyModifiedProperties();
			}

			if( targ.customTouchSize_X != -10 || targ.customTouchSize_Y != -10 )
			{
				activationWidth.floatValue = targ.customTouchSize_X;
				activationHeight.floatValue = targ.customTouchSize_Y;

				serializedObject.FindProperty( "customTouchSize_X" ).floatValue = -10;
				serializedObject.FindProperty( "customTouchSize_Y" ).floatValue = -10;

				serializedObject.ApplyModifiedProperties();
			}

			if( targ.customTouchSizePos_X != -10 || targ.customTouchSizePos_Y != -10 )
			{
				activationPositionHorizontal.floatValue = targ.customTouchSizePos_X;
				activationPositionVertical.floatValue = targ.customTouchSizePos_Y;

				serializedObject.FindProperty( "customTouchSizePos_X" ).floatValue = -10;
				serializedObject.FindProperty( "customTouchSizePos_Y" ).floatValue = -10;

				serializedObject.ApplyModifiedProperties();
			}

			if( targ.joystickSizeFolder != null && targ.joystickBase != null )
			{
				Undo.SetTransformParent( targ.joystickBase.transform, targ.transform, "Fix Older Joysticks" );

				if( targ.showHighlight && targ.highlightBase != null )
					Undo.SetTransformParent( targ.highlightBase.transform, targ.joystickBase.transform, "Fix Older Joysticks" );

				if( targ.showTension )
				{
					if( targ.tensionAccentUp != null )
						Undo.SetTransformParent( targ.tensionAccentUp.transform, targ.joystickBase.transform, "Fix Older Joysticks" );
					if( targ.tensionAccentDown != null )
						Undo.SetTransformParent( targ.tensionAccentDown.transform, targ.joystickBase.transform, "Fix Older Joysticks" );
					if( targ.tensionAccentLeft != null )
						Undo.SetTransformParent( targ.tensionAccentLeft.transform, targ.joystickBase.transform, "Fix Older Joysticks" );
					if( targ.tensionAccentRight != null )
						Undo.SetTransformParent( targ.tensionAccentRight.transform, targ.joystickBase.transform, "Fix Older Joysticks" );
				}

				Undo.SetTransformParent( targ.joystick.transform, targ.joystickBase.transform, "Fix Older Joysticks" );

				if( targ.showHighlight && targ.highlightJoystick != null && targ.highlightJoystick.gameObject != targ.joystick.gameObject )
					Undo.SetTransformParent( targ.highlightJoystick.transform, targ.joystick.transform, "Fix Older Joysticks" );

				Undo.DestroyObjectImmediate( targ.joystickSizeFolder.gameObject );

				serializedObject.FindProperty( "joystickSizeFolder" ).objectReferenceValue = null;
				serializedObject.ApplyModifiedProperties();
			}

			if( targ.tensionAccentUp != null || targ.tensionAccentLeft != null || targ.tensionAccentDown != null || targ.tensionAccentRight != null )
			{
				if( targ.TensionAccents.Count > 0 )
				{
					List<GameObject> gameObjectsToDestroy = new List<GameObject>();
					for( int i = 0; i < targ.TensionAccents.Count; i++ )
					{
						if( targ.TensionAccents[ i ] == null )
							continue;

						gameObjectsToDestroy.Add( targ.TensionAccents[ i ].gameObject );
					}

					serializedObject.FindProperty( "TensionAccents" ).ClearArray();
					serializedObject.ApplyModifiedProperties();

					for( int i = 0; i < gameObjectsToDestroy.Count; i++ )
						Undo.DestroyObjectImmediate( gameObjectsToDestroy[ i ] );
				}

				for( int i = 0; i < 4; i++ )
				{
					serializedObject.FindProperty( "TensionAccents" ).InsertArrayElementAtIndex( i );
					serializedObject.ApplyModifiedProperties();
				}

				if( targ.tensionAccentUp != null )
					serializedObject.FindProperty( string.Format( "TensionAccents.Array.data[{0}]", 0 ) ).objectReferenceValue = targ.tensionAccentUp.GetComponent<Image>();
				if( targ.tensionAccentLeft != null )
					serializedObject.FindProperty( string.Format( "TensionAccents.Array.data[{0}]", 1 ) ).objectReferenceValue = targ.tensionAccentLeft.GetComponent<Image>();
				if( targ.tensionAccentDown != null )
					serializedObject.FindProperty( string.Format( "TensionAccents.Array.data[{0}]", 2 ) ).objectReferenceValue = targ.tensionAccentDown.GetComponent<Image>();
				if( targ.tensionAccentRight != null )
					serializedObject.FindProperty( string.Format( "TensionAccents.Array.data[{0}]", 3 ) ).objectReferenceValue = targ.tensionAccentRight.GetComponent<Image>();

				serializedObject.FindProperty( "tensionAccentUp" ).objectReferenceValue = null;
				serializedObject.FindProperty( "tensionAccentLeft" ).objectReferenceValue = null;
				serializedObject.FindProperty( "tensionAccentDown" ).objectReferenceValue = null;
				serializedObject.FindProperty( "tensionAccentRight" ).objectReferenceValue = null;
				serializedObject.ApplyModifiedProperties();
			}
		}
		// END COPY DEPRECIATED VALUES //

		// -----< JOYSTICK SETTINGS >----- //
		gravity = serializedObject.FindProperty( "gravity" );
		extendRadius = serializedObject.FindProperty( "extendRadius" );
		axis = serializedObject.FindProperty( "axis" );
		boundary = serializedObject.FindProperty( "boundary" );
		deadZone = serializedObject.FindProperty( "deadZone" );
		tapCountOption = serializedObject.FindProperty( "tapCountOption" );
		tapCountDuration = serializedObject.FindProperty( "tapCountDuration" );
		targetTapCount = serializedObject.FindProperty( "targetTapCount" );
		useTouchInput = serializedObject.FindProperty( "useTouchInput" );

		// -----< VISUAL OPTIONS >----- //
		disableVisuals = serializedObject.FindProperty( "disableVisuals" );
		baseColor = targ.joystickBase == null ? Color.white : targ.joystickBase.GetComponent<Image>().color;
		inputTransition = serializedObject.FindProperty( "inputTransition" );
		useFade = serializedObject.FindProperty( "useFade" );
		useScale = serializedObject.FindProperty( "useScale" );
		fadeUntouched = serializedObject.FindProperty( "fadeUntouched" );
		transitionUntouchedDuration = serializedObject.FindProperty( "transitionUntouchedDuration" );
		fadeTouched = serializedObject.FindProperty( "fadeTouched" );
		scaleTouched = serializedObject.FindProperty( "scaleTouched" );
		transitionTouchedDuration = serializedObject.FindProperty( "transitionTouchedDuration" );
		showHighlight = serializedObject.FindProperty( "showHighlight" );
		highlightBase = serializedObject.FindProperty( "highlightBase" );

		if( targ.highlightBase != null && targ.highlightBase.sprite != null )
			highlightBaseSprite = targ.highlightBase.sprite;

		highlightJoystick = serializedObject.FindProperty( "highlightJoystick" );

		if( targ.highlightJoystick != null && targ.highlightJoystick.sprite != null )
			highlightJoystickSprite = targ.highlightJoystick.sprite;

		highlightColor = serializedObject.FindProperty( "highlightColor" );
		showTension = serializedObject.FindProperty( "showTension" );
		tensionType = serializedObject.FindProperty( "tensionType" );
		tensionColorNone = serializedObject.FindProperty( "tensionColorNone" );
		tensionColorFull = serializedObject.FindProperty( "tensionColorFull" );
		rotationOffset = serializedObject.FindProperty( "rotationOffset" );
		tensionDeadZone = serializedObject.FindProperty( "tensionDeadZone" );

		noSpriteDirection = NoSpriteDirection;
		
		for( int i = 0; i < targ.TensionAccents.Count; i++ )
		{
			if( targ.TensionAccents[ i ] == null || targ.TensionAccents[ i ].sprite == null )
				continue;

			tensionAccentSprite = targ.TensionAccents[ i ].sprite;
		}

		if( targ.TensionAccents.Count > 0 && targ.TensionAccents[ 0 ] != null )
			tensionScale = targ.TensionAccents[ 0 ].transform.localScale.x;

		// ------< SCRIPT REFERENCE >------ //
		joystickName = serializedObject.FindProperty( "joystickName" );
		exampleCodeOptions = new List<string>();
		for( int i = 0; i < exampleCodes.Length; i++ )
			exampleCodeOptions.Add( exampleCodes[ i ].optionName );
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		handlesCenteredText = new GUIStyle( EditorStyles.label ) { normal = new GUIStyleState() { textColor = Color.white } };

		collapsableSectionStyle = new GUIStyle( EditorStyles.label ) { alignment = TextAnchor.MiddleCenter };
		collapsableSectionStyle.active.textColor = collapsableSectionStyle.normal.textColor;
		
		bool valueChanged = false;

		// PREFAB WARNINGS //
		if( isPrefabInProjectWindow )
		{
			bool stopEditorDisplay = targ.joystickSizeFolder != null && targ.joystickBase != null;
			
			if( targ.tensionAccentUp != null || targ.tensionAccentLeft != null || targ.tensionAccentDown != null || targ.tensionAccentRight != null )
				stopEditorDisplay = true;

			if( stopEditorDisplay )
			{
				GUIStyle wordWrappedParagraph = new GUIStyle( EditorStyles.label ) { wordWrap = true };

				collapsableSectionStyle.fontStyle = FontStyle.Bold;

				EditorGUILayout.BeginVertical( "Box" );

				EditorGUILayout.LabelField( "OUTDATED PREFAB", collapsableSectionStyle );

				EditorGUILayout.LabelField( "This is an outdated prefab. In order to fix this prefab, please drag it into your scene and then click the apply button at the top of the Inspector window.", wordWrappedParagraph );
				EditorGUILayout.EndVertical();
				return;
			}
		}

		// CHECK FOR CANVAS ERRORS //
		if( CanvasErrors )
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
					RequestCanvas( Selection.activeGameObject );
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
						RequestCanvas( Selection.activeGameObject );
						parentCanvas = GetParentCanvas();
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
			}
			return;
		}

		// SIZE AND PLACEMENT //
		DisplayHeaderDropdown( "Joystick Positioning", "UUI_SizeAndPlacement" );
		if( EditorPrefs.GetBool( "UUI_SizeAndPlacement" ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystickBase, new GUIContent( "Joystick Base" ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				if( targ.joystickBase != null && targ.joystickBase.GetComponent<Image>() )
					joystickBaseSprite = targ.joystickBase.GetComponent<Image>().sprite;
			}

			EditorGUI.BeginChangeCheck();
			joystickBaseSprite = ( Sprite )EditorGUILayout.ObjectField( "└ Base Sprite", joystickBaseSprite, typeof( Sprite ), true, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( targ.joystickBase != null && targ.joystickBase.GetComponent<Image>() )
				{
					Undo.RecordObject( targ.joystickBase.GetComponent<Image>(), "Update Joystick Base Sprite" );
					targ.joystickBase.GetComponent<Image>().sprite = joystickBaseSprite;
				}
			}

			if( targ.joystickBase == null )
			{
				EditorGUI.BeginDisabledGroup( joystickBaseSprite == null );
				if( GUILayout.Button( "Generate Joystick Base", EditorStyles.miniButton ) )
				{
					GameObject newGameObject = new GameObject();
					newGameObject.AddComponent<RectTransform>();
					newGameObject.AddComponent<CanvasRenderer>();
					newGameObject.AddComponent<Image>();

					newGameObject.GetComponent<Image>().sprite = joystickBaseSprite;
					newGameObject.GetComponent<Image>().color = baseColor;

					newGameObject.transform.SetParent( targ.transform );
					newGameObject.transform.SetAsFirstSibling();

					newGameObject.name = "Joystick Base";

					RectTransform trans = newGameObject.GetComponent<RectTransform>();

					trans.anchorMin = new Vector2( 0.5f, 0.5f );
					trans.anchorMax = new Vector2( 0.5f, 0.5f );
					trans.pivot = new Vector2( 0.5f, 0.5f );
					trans.anchoredPosition = Vector2.zero;

					serializedObject.FindProperty( "joystickBase" ).objectReferenceValue = newGameObject.GetComponent<RectTransform>();
					serializedObject.ApplyModifiedProperties();

					Undo.RegisterCreatedObjectUndo( newGameObject, "Create Joystick Base Object" );
				}
				EditorGUI.EndDisabledGroup();
			}

			GUILayout.Space( afterIndentSpace );

			EditorGUI.BeginDisabledGroup( targ.joystickBase == null );

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystick, new GUIContent( "Joystick" ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				if( targ.joystick != null && targ.joystick.GetComponent<Image>() )
					joystickSprite = targ.joystick.GetComponent<Image>().sprite;
			}

			EditorGUI.BeginChangeCheck();
			joystickSprite = ( Sprite )EditorGUILayout.ObjectField( "└ Joystick Sprite", joystickSprite, typeof( Sprite ), true, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( targ.joystick != null && targ.joystick.GetComponent<Image>() )
				{
					Undo.RecordObject( targ.joystick.GetComponent<Image>(), "Update Joystick Sprite" );
					targ.joystick.GetComponent<Image>().sprite = joystickSprite;
				}
			}

			if( targ.joystick == null )
			{
				EditorGUI.BeginDisabledGroup( joystickSprite == null );
				if( GUILayout.Button( "Generate Joystick", EditorStyles.miniButton ) )
				{
					GameObject newGameObject = new GameObject();
					newGameObject.AddComponent<RectTransform>();
					newGameObject.AddComponent<CanvasRenderer>();
					newGameObject.AddComponent<Image>();

					newGameObject.GetComponent<Image>().sprite = joystickSprite;
					newGameObject.GetComponent<Image>().color = baseColor;

					newGameObject.transform.SetParent( targ.joystickBase );
					newGameObject.transform.SetAsFirstSibling();

					newGameObject.name = "Joystick";

					RectTransform trans = newGameObject.GetComponent<RectTransform>();

					trans.anchorMin = new Vector2( 0.0f, 0.0f );
					trans.anchorMax = new Vector2( 1.0f, 1.0f );
					trans.offsetMin = Vector2.zero;
					trans.offsetMax = Vector2.zero;
					trans.pivot = new Vector2( 0.5f, 0.5f );
					trans.anchoredPosition = Vector2.zero;

					serializedObject.FindProperty( "joystick" ).objectReferenceValue = newGameObject.GetComponent<RectTransform>();
					serializedObject.ApplyModifiedProperties();

					Undo.RegisterCreatedObjectUndo( newGameObject, "Create Joystick Object" );
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUI.EndDisabledGroup();

			GUILayout.Space( afterIndentSpace );

			if( targ.joystickBase == null || targ.joystick == null )
				EditorGUILayout.HelpBox( "Please make sure the above variables are assigned before continuing.", MessageType.Warning );

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( scalingAxis, new GUIContent( "Scaling Axis", "The axis to scale the Ultimate Joystick from." ) );
			EditorGUILayout.PropertyField( anchor, new GUIContent( "Anchor", "The side of the screen that the\njoystick will be anchored to." ) );
			EditorGUILayout.Slider( joystickSize, 1.0f, 4.0f, new GUIContent( "Joystick Size", "The overall size of the joystick." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider( radiusModifier, 2.0f, 7.0f, new GUIContent( "Radius", "Determines how far the joystick can\nmove visually from the center." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				DisplayRadius.PropertyUpdated();
			}
			CheckPropertyHover( DisplayRadius );

			EditorGUI.BeginDisabledGroup( targ.customActivationRange );
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider( activationRange, 0.0f, 2.0f, new GUIContent( "Activation Range", "The size of the area in which\nthe touch can be initiated." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				DisplayActivationRange.PropertyUpdated();
			}
			CheckPropertyHover( DisplayActivationRange );
			EditorGUI.EndDisabledGroup();

			EditorGUI.indentLevel++;
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( customActivationRange, new GUIContent( "Custom Activation Range", "Enabling this option will allow you to define a specific area on the screen where the user can interact with the joystick." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			EditorGUI.indentLevel--;

			if( targ.customActivationRange )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Custom Activation Range", EditorStyles.boldLabel );
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( activationWidth, 0.0f, 100.0f, new GUIContent( "Activation Width", "The width of the activation range." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					DisplayActivationCustomWidth.PropertyUpdated();
				}
				CheckPropertyHover( DisplayActivationCustomWidth );

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( activationHeight, 0.0f, 100.0f, new GUIContent( "Activation Height", "The height of the activation range." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					DisplayActivationCustomHeight.PropertyUpdated();
				}
				CheckPropertyHover( DisplayActivationCustomHeight );

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( activationPositionHorizontal, 0.0f, 100.0f, new GUIContent( "Horizontal Position", "The horizontal position of the activation range." ) );
				EditorGUILayout.Slider( activationPositionVertical, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the activation range." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUILayout.EndVertical();
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.LabelField( "Joystick Position", EditorStyles.boldLabel );
			EditorGUILayout.Slider( positionHorizontal, 0.0f, 50.0f, new GUIContent( "Horizontal Position", "The horizontal position of the joystick on the screen." ) );
			EditorGUILayout.Slider( positionVertical, 0.0f, 100.0f, new GUIContent( "Vertical Position", "The vertical position of the joystick on the screen." ) );
			GUILayout.Space( 1 );
			EditorGUILayout.EndVertical();
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.BeginDisabledGroup( targ.joystickBase == null || targ.joystick == null );

		// JOYSTICK SETTINGS //
		DisplayHeaderDropdown( "Joystick Settings", "UUI_Functionality" );
		if( EditorPrefs.GetBool( "UUI_Functionality" ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( dynamicPositioning, new GUIContent( "Dynamic Positioning", "Moves the joystick to the position of the initial touch." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( gravity, new GUIContent( "Gravity", "The speed to apply to the joystick when returning to center." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				gravity.floatValue = Mathf.Clamp( gravity.floatValue, 0.0f, 60.0f );
				serializedObject.ApplyModifiedProperties();
			}

			// --------------------------< EXTEND RADIUS, AXIS, DEAD ZONE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( extendRadius, new GUIContent( "Extend Radius", "Drags the joystick to follow the touch if it is farther than the radius." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( axis, new GUIContent( "Axis", "Constrains the joystick to a certain axis." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				DisplayAxis.PropertyUpdated();
			}
			CheckPropertyHover( DisplayAxis );

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( boundary, new GUIContent( "Boundary", "Determines how the joystick's position is clamped." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				DisplayBoundary.PropertyUpdated();
			}
			CheckPropertyHover( DisplayBoundary );

			if( targ.extendRadius == true && targ.boundary == UltimateJoystick.Boundary.Square )
				EditorGUILayout.HelpBox( "Extend Radius option will force the boundary to being circular. Please use a circular boundary when using the Extend Radius option.", MessageType.Warning );

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider( deadZone, 0.0f, 1.0f, new GUIContent( "Dead Zone", "Size of the dead zone. All values within this range map to neutral." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				DisplayDeadZone.PropertyUpdated();
			}
			CheckPropertyHover( DisplayDeadZone );
			// ------------------------< END EXTEND RADIUS, AXIS, DEAD ZONE >------------------------ //

			// TAP COUNT //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( tapCountOption, new GUIContent( "Tap Count", "Allows the joystick to calculate double taps and a touch and release within a certain time window." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( targ.tapCountOption != UltimateJoystick.TapCountOption.NoCount )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( tapCountDuration, 0.0f, 1.0f, new GUIContent( "Tap Time Window", "Time in seconds that the joystick can receive taps." ) );
				if( targ.tapCountOption == UltimateJoystick.TapCountOption.Accumulate )
					EditorGUILayout.IntSlider( targetTapCount, 1, 5, new GUIContent( "Target Tap Count", "How many taps to activate the Tap Count Event?" ) );

				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.indentLevel = 0;
			}

			EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField( useTouchInput, new GUIContent( "Use Touch Input", "Determines if the joystick should use input from the EventSystem or directly calculate from the touch input on the screen." ) );
            if( EditorGUI.EndChangeCheck() )
                serializedObject.ApplyModifiedProperties();
        }

		// VISUAL OPTIONS //
		DisplayHeaderDropdown( "Visual Options", "UUI_VisualOptions" );
		if( EditorPrefs.GetBool( "UUI_VisualOptions" ) )
		{
			// -----------------------< DISABLE VISUALS >---------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( disableVisuals, new GUIContent( "Disable Visuals", "Disables the visuals of the joystick." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				if( targ.disableVisuals )
				{
					inputTransition.boolValue = false;
					showHighlight.boolValue = false;
					showTension.boolValue = false;
					serializedObject.ApplyModifiedProperties();

					Undo.RecordObject( targ.gameObject.GetComponent<CanvasGroup>(), "Disable Joystick Visuals" );
					targ.gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
				}
				else
				{
					Undo.RecordObject( targ.gameObject.GetComponent<CanvasGroup>(), "Enable Joystick Visuals" );
					targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
				}
				
				CheckHighlightGameObjects();
				CheckTensionAccentGameObjects();
			}
			// ---------------------< END DISABLE VISUALS >-------------------- //

			EditorGUI.BeginDisabledGroup( targ.disableVisuals == true );// This is the start of the disabled fields if the user is using the disableVisuals option.

			// --------------------------< BASE COLOR >------------------------- //
			if( targ.joystickBase != null && targ.joystick != null )
			{
				EditorGUI.BeginChangeCheck();
				baseColor = EditorGUILayout.ColorField( "Base Color", baseColor );
				if( EditorGUI.EndChangeCheck() )
				{
					if( targ.joystick != null )
					{
						Undo.RecordObject( targ.joystick.GetComponent<Image>(), "Change Base Color" );
						targ.joystick.GetComponent<Image>().enabled = false;
						targ.joystick.GetComponent<Image>().color = baseColor;
						targ.joystick.GetComponent<Image>().enabled = true;
					}

					if( targ.joystickBase != null )
					{
						Undo.RecordObject( targ.joystickBase.GetComponent<Image>(), "Change Base Color" );
						targ.joystickBase.GetComponent<Image>().enabled = false;
						targ.joystickBase.GetComponent<Image>().color = baseColor;
						targ.joystickBase.GetComponent<Image>().enabled = true;
					}
				}
			}
			// ------------------------< END BASE COLOR >----------------------- //

			// -----------------------< INPUT TRANSITION >---------------------- //
			valueChanged = false;
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Input Transition", "UJ_InputTransition", inputTransition, ref valueChanged ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( useFade );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					Undo.RecordObject( targ.gameObject.GetComponent<CanvasGroup>(), "Enable Joystick Fade" );
					targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.useFade ? targ.fadeUntouched : 1.0f;
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( useScale );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				if( targ.useFade || targ.useScale )
				{
					EditorGUILayout.Space();

					EditorGUILayout.LabelField( "Untouched State", EditorStyles.boldLabel );

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( transitionUntouchedDuration, new GUIContent( "Transition Duration", "The time is seconds for the transition to the untouched state." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					if( targ.useFade )
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.Slider( fadeUntouched, 0.0f, 1.0f, new GUIContent( "Untouched Alpha", "The alpha of the joystick when it is not receiving input." ) );
						if( EditorGUI.EndChangeCheck() )
						{
							serializedObject.ApplyModifiedProperties();
							Undo.RecordObject( targ.gameObject.GetComponent<CanvasGroup>(), "Edit Joystick Fade" );
							targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
						}
					}
					
					EditorGUILayout.Space();

					EditorGUILayout.LabelField( "Touched State", EditorStyles.boldLabel );
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( transitionTouchedDuration, new GUIContent( "Transition Duration", "The time is seconds for the transition to the touched state." ) );
					if( targ.useFade )
						EditorGUILayout.Slider( fadeTouched, 0.0f, 1.0f, new GUIContent( "Touched Alpha", "The alpha of the joystick when receiving input." ) );
					if( targ.useScale )
						EditorGUILayout.Slider( scaleTouched, 0.0f, 2.0f, new GUIContent( "Touched Scale", "The scale of the joystick when receiving input." ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				
				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
			if( valueChanged )
			{
				if( !targ.gameObject.GetComponent<CanvasGroup>() )
					targ.gameObject.AddComponent<CanvasGroup>();

				if( targ.inputTransition && targ.useFade )
				{
					Undo.RecordObject( targ.gameObject.GetComponent<CanvasGroup>(), "Enable Input Transition" );
					targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
				}
				else
				{
					Undo.RecordObject( targ.gameObject.GetComponent<CanvasGroup>(), "Disable Input Transition" );
					targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
				}
			}
			// ---------------------< END INPUT TRANSITION >-------------------- //

			// ------------------------< USE HIGHLIGHT >------------------------ //
			valueChanged = false;
			EditorGUILayout.BeginVertical( "Box" );
			if( DisplayCollapsibleBoxSection( "Highlight", "UJ_Highlight", showHighlight, ref valueChanged ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( highlightColor );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					if( targ.highlightBase != null )
					{
						Undo.RecordObject( targ.highlightBase, "Update Highlight Color" );
						targ.highlightBase.color = targ.highlightColor;
					}

					if( targ.highlightJoystick != null )
					{
						Undo.RecordObject( targ.highlightJoystick, "Update Highlight Color" );
						targ.highlightJoystick.color = targ.highlightColor;
					}
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( highlightBase, new GUIContent( "Base Highlight" ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					if( targ.highlightBase != null )
					{
						Undo.RecordObject( targ.highlightBase, "Assign Base Highlight" );
						targ.highlightBase.color = targ.highlightColor;
					}
				}

				EditorGUI.BeginChangeCheck();
				highlightBaseSprite = ( Sprite )EditorGUILayout.ObjectField( "└ Image Sprite", highlightBaseSprite, typeof( Sprite ), true, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
				if( EditorGUI.EndChangeCheck() )
				{
					if( targ.highlightBase != null )
					{
						Undo.RecordObject( targ.highlightBase, "Update Base Highlight Sprite" );
						targ.highlightBase.sprite = highlightBaseSprite;
					}
				}

				if( targ.highlightBase == null )
				{
					EditorGUI.BeginDisabledGroup( highlightBaseSprite == null );
					if( GUILayout.Button( "Generate Base Highlight", EditorStyles.miniButton ) )
					{
						GameObject newGameObject = new GameObject();
						newGameObject.AddComponent<RectTransform>();
						newGameObject.AddComponent<CanvasRenderer>();
						newGameObject.AddComponent<Image>();
						
						newGameObject.GetComponent<Image>().sprite = highlightBaseSprite;
						newGameObject.GetComponent<Image>().color = targ.highlightColor;

						newGameObject.transform.SetParent( targ.joystickBase );
						newGameObject.transform.SetAsFirstSibling();

						newGameObject.name = "Base Highlight";
						
						RectTransform trans = newGameObject.GetComponent<RectTransform>();

						trans.anchorMin = new Vector2( 0.0f, 0.0f );
						trans.anchorMax = new Vector2( 1.0f, 1.0f );
						trans.offsetMin = Vector2.zero;
						trans.offsetMax = Vector2.zero;
						trans.pivot = new Vector2( 0.5f, 0.5f );
						trans.anchoredPosition = Vector2.zero;

						serializedObject.FindProperty( "highlightBase" ).objectReferenceValue = newGameObject.GetComponent<Image>();
						serializedObject.ApplyModifiedProperties();

						Undo.RegisterCreatedObjectUndo( newGameObject, "Create Base Highlight Object" );
					}
					EditorGUI.EndDisabledGroup();
				}

				EditorGUILayout.Space();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( highlightJoystick, new GUIContent( "Joystick Highlight" ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();

					if( targ.highlightJoystick != null )
					{
						Undo.RecordObject( targ.highlightJoystick, "Assign Joystick Highlight" );
						targ.highlightJoystick.color = targ.highlightColor;
					}
				}

				EditorGUI.BeginChangeCheck();
				highlightJoystickSprite = ( Sprite )EditorGUILayout.ObjectField( "└ Image Sprite", highlightJoystickSprite, typeof( Sprite ), true, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
				if( EditorGUI.EndChangeCheck() )
				{
					if( targ.highlightJoystick != null )
					{
						Undo.RecordObject( targ.highlightJoystick, "Update Joystick Highlight Sprite" );
						targ.highlightJoystick.sprite = highlightJoystickSprite;
					}
				}

				if( targ.highlightJoystick == null )
				{
					EditorGUI.BeginDisabledGroup( highlightJoystickSprite == null );
					if( GUILayout.Button( "Generate Joystick Highlight", EditorStyles.miniButton ) )
					{
						GameObject newGameObject = new GameObject();
						newGameObject.AddComponent<RectTransform>();
						newGameObject.AddComponent<CanvasRenderer>();
						newGameObject.AddComponent<Image>();

						newGameObject.GetComponent<Image>().sprite = highlightJoystickSprite;
						newGameObject.GetComponent<Image>().color = targ.highlightColor;

						newGameObject.transform.SetParent( targ.joystick );

						newGameObject.name = "Joystick Highlight";

						RectTransform trans = newGameObject.GetComponent<RectTransform>();

						trans.anchorMin = new Vector2( 0.0f, 0.0f );
						trans.anchorMax = new Vector2( 1.0f, 1.0f );
						trans.offsetMin = Vector2.zero;
						trans.offsetMax = Vector2.zero;
						trans.pivot = new Vector2( 0.5f, 0.5f );
						trans.anchoredPosition = Vector2.zero;

						serializedObject.FindProperty( "highlightJoystick" ).objectReferenceValue = newGameObject.GetComponent<Image>();
						serializedObject.ApplyModifiedProperties();

						Undo.RegisterCreatedObjectUndo( newGameObject, "Create Base Highlight Object" );
					}
					EditorGUI.EndDisabledGroup();
				}

				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
			if( valueChanged )
				CheckHighlightGameObjects();
			// ------------------------< END HIGHLIGHT >------------------------ //

			// ---------------------------< TENSION >--------------------------- //
			valueChanged = false;
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUI.BeginChangeCheck();
			if( DisplayCollapsibleBoxSection( "Tension Accent", "UJ_TensionAccent", showTension, ref valueChanged ) )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( tensionColorNone, new GUIContent( "Tension None", "The color displayed when the joystick\nis closest to center." ) );
				EditorGUILayout.PropertyField( tensionColorFull, new GUIContent( "Tension Full", "The color displayed when the joystick\nis at the furthest distance." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					ApplyTensionColors();
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( tensionType, new GUIContent( "Tension Type", "This option determines how the tension accent will be displayed, whether by using 4 images to show each direction or by using just one image to highlight the direction that the joystick is being used." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					if( tensionType.intValue != ( int )targ.tensionType )
						GenerateTensionImages();

					serializedObject.ApplyModifiedProperties();

					if( targ.TensionAccents.Count > 0 && targ.TensionAccents[ 0 ] != null )
						tensionScale = targ.TensionAccents[ 0 ].transform.localScale.x;
				}
				
				if( targ.TensionAccents.Count == 0 || !TensionObjectAssigned )
				{
					tensionAccentSprite = ( Sprite )EditorGUILayout.ObjectField( "Tension Sprite", tensionAccentSprite, typeof( Sprite ), true, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );

					EditorGUI.BeginDisabledGroup( tensionAccentSprite == null );

					if( GUILayout.Button( "Generate Tension Images", EditorStyles.miniButton ) )
						GenerateTensionImages();

					EditorGUI.EndDisabledGroup();
				}
				else
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.Slider( tensionDeadZone, 0.0f, 1.0f, new GUIContent( "Dead Zone", "The distance that the joystick will need to move from center before the tension image will start to display tension." ) );
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						DisplayTensionDeadZone.PropertyUpdated();
					}
					CheckPropertyHover( DisplayTensionDeadZone );

					if( targ.tensionType == UltimateJoystick.TensionType.Free )
					{
						EditorGUI.BeginChangeCheck();
						tensionScale = EditorGUILayout.Slider( new GUIContent( "Tension Scale", "The overall scale of the tension accent image." ), tensionScale, 0.0f, 2.0f );
						if( EditorGUI.EndChangeCheck() )
						{
							Undo.RecordObject( targ.TensionAccents[ 0 ].transform, "Modify Tension Scale" );
							targ.TensionAccents[ 0 ].transform.localScale = Vector3.one * tensionScale;
						}
					}

					EditorGUI.BeginDisabledGroup( Application.isPlaying );

					EditorGUILayout.BeginHorizontal();

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle( !noSpriteDirection && Mathf.Round( targ.rotationOffset ) == 0, "Up", EditorStyles.miniButtonLeft );
					if( EditorGUI.EndChangeCheck() )
					{
						bool identicalSprites = IdenticalSprites;
						if( identicalSprites || DisplayOverwriteSpriteWarning )
						{
							if( !identicalSprites )
								UpdateTensionImageSprites();

							rotationOffset.floatValue = 0.0f;
							serializedObject.ApplyModifiedProperties();

							RotateTensionImages();
						}
					}

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle( !noSpriteDirection && Mathf.Round( targ.rotationOffset ) == 270, "Left", EditorStyles.miniButtonMid );
					if( EditorGUI.EndChangeCheck() )
					{
						bool identicalSprites = IdenticalSprites;
						if( identicalSprites || DisplayOverwriteSpriteWarning )
						{
							if( !identicalSprites )
								UpdateTensionImageSprites();

							rotationOffset.floatValue = 270.0f;
							serializedObject.ApplyModifiedProperties();

							RotateTensionImages();
						}
					}

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle( !noSpriteDirection && Mathf.Round( targ.rotationOffset ) == 180, "Down", EditorStyles.miniButtonMid );
					if( EditorGUI.EndChangeCheck() )
					{
						bool identicalSprites = IdenticalSprites;
						if( identicalSprites || DisplayOverwriteSpriteWarning )
						{
							if( !identicalSprites )
								UpdateTensionImageSprites();

							rotationOffset.floatValue = 180.0f;
							serializedObject.ApplyModifiedProperties();

							RotateTensionImages();
						}
					}

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle( !noSpriteDirection && Mathf.Round( targ.rotationOffset ) == 90, "Right", targ.TensionAccents.Count > 1 ? EditorStyles.miniButtonMid : EditorStyles.miniButtonRight );
					if( EditorGUI.EndChangeCheck() )
					{
						bool identicalSprites = IdenticalSprites;
						if( identicalSprites || DisplayOverwriteSpriteWarning )
						{
							if( !identicalSprites )
								UpdateTensionImageSprites();

							rotationOffset.floatValue = 90.0f;
							serializedObject.ApplyModifiedProperties();

							RotateTensionImages();
						}
					}

					if( targ.TensionAccents.Count > 1 )
					{
						EditorGUI.BeginChangeCheck();
						GUILayout.Toggle( noSpriteDirection, "None", EditorStyles.miniButtonRight );
						if( EditorGUI.EndChangeCheck() )
						{
							for( int i = 0; i < targ.TensionAccents.Count; i++ )
							{
								if( targ.TensionAccents[ i ] == null )
									continue;

								Undo.RecordObject( targ.TensionAccents[ i ].transform, "Update Rotation Offset" );
								targ.TensionAccents[ i ].transform.eulerAngles = Vector3.zero;
							}

							noSpriteDirection = NoSpriteDirection;
						}
					}

					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle( editTensionSprites, "Edit Sprite", EditorStyles.miniButtonLeft );
					if( EditorGUI.EndChangeCheck() )
					{
						editTensionSprites = true;
						editTensionImages = false;
					}

					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle( editTensionImages, "Edit Images", EditorStyles.miniButtonRight );
					if( EditorGUI.EndChangeCheck() )
					{
						editTensionImages = true;
						editTensionSprites = false;
					}

					EditorGUILayout.EndHorizontal();
					if( !editTensionImages )
					{
						if( !noSpriteDirection )
						{
							EditorGUI.BeginChangeCheck();
							tensionAccentSprite = ( Sprite )EditorGUILayout.ObjectField( "Tension Sprite", tensionAccentSprite, typeof( Sprite ), true, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
							if( EditorGUI.EndChangeCheck() )
							{
								for( int i = 0; i < targ.TensionAccents.Count; i++ )
								{
									if( targ.TensionAccents[ i ] == null )
										continue;

									Undo.RecordObject( targ.TensionAccents[ i ], "Update Tension Sprite" );
									targ.TensionAccents[ i ].sprite = tensionAccentSprite;
								}
							}
						}
						else
						{
							for( int i = 0; i < targ.TensionAccents.Count; i++ )
							{
								if( targ.TensionAccents[ i ] == null )
									continue;

								Sprite targetSprite = targ.TensionAccents[ i ].sprite;
								string tensionDirection = i == 0 ? "Up" : "Left";
								if( i >= 2 )
									tensionDirection = i == 2 ? "Down" : "Right";

								EditorGUI.BeginChangeCheck();
								targetSprite = ( Sprite )EditorGUILayout.ObjectField( "Tension Sprite " + tensionDirection, targetSprite, typeof( Sprite ), true, GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
								if( EditorGUI.EndChangeCheck() )
								{
									Undo.RecordObject( targ.TensionAccents[ i ], "Update Tension Sprite" );
									targ.TensionAccents[ i ].sprite = targetSprite;
								}
							}
						}
					}
					else
					{
						for( int i = 0; i < targ.TensionAccents.Count; i++ )
						{
							string tensionDirection = i == 0 ? "Up" : "Left";
							if( i >= 2 )
								tensionDirection = i == 2 ? "Down" : "Right";

							if( targ.TensionAccents.Count == 1 )
								tensionDirection = "";

							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( serializedObject.FindProperty( string.Format( "TensionAccents.Array.data[{0}]", i ) ), new GUIContent( "Tension Image " + tensionDirection ) );
							if( EditorGUI.EndChangeCheck() )
								serializedObject.ApplyModifiedProperties();
						}
					}

					EditorGUI.EndDisabledGroup();
				}

				GUILayout.Space( 1 );
			}
			EditorGUILayout.EndVertical();
			if( valueChanged )
				CheckTensionAccentGameObjects();
			// -------------------------< END TENSION >------------------------- //

			EditorGUI.EndDisabledGroup();
		}

		EditorGUI.EndDisabledGroup();

		// SCRIPT REFERENCE //
		DisplayHeaderDropdown( "Script Reference", "UUI_ScriptReference" );
		if( EditorPrefs.GetBool( "UUI_ScriptReference" ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystickName, new GUIContent( "Joystick Name", "The name of the targeted joystick used for static referencing." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( targ.joystickName == string.Empty )
				EditorGUILayout.HelpBox( "Please assign a Joystick Name in order to be able to get this joystick's position dynamically.", MessageType.Warning );
			else
			{
				EditorGUILayout.BeginVertical( "Box" );
				GUILayout.Space( 1 );
				EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );

				exampleCodeIndex = EditorGUILayout.Popup( "Function", exampleCodeIndex, exampleCodeOptions.ToArray() );

				EditorGUILayout.LabelField( "Function Description", EditorStyles.boldLabel );
				GUIStyle wordWrappedLabel = new GUIStyle( GUI.skin.label ) { wordWrap = true };
				EditorGUILayout.LabelField( exampleCodes[ exampleCodeIndex ].optionDescription, wordWrappedLabel );

				EditorGUILayout.LabelField( "Example Code", EditorStyles.boldLabel );
				GUIStyle wordWrappedTextArea = new GUIStyle( GUI.skin.textArea ) { wordWrap = true };
				EditorGUILayout.TextArea( string.Format( exampleCodes[ exampleCodeIndex ].basicCode, joystickName.stringValue ), wordWrappedTextArea );

				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}

			if( GUILayout.Button( "Open Documentation" ) )
				UltimateJoystickReadmeEditor.OpenReadmeDocumentation();

			if( Selection.activeGameObject != null && !AssetDatabase.Contains( Selection.activeGameObject ) && Application.isPlaying )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Current Position:", EditorStyles.boldLabel );
				EditorGUILayout.LabelField( "Horizontal Axis: " + targ.HorizontalAxis.ToString( "F2" ) );
				EditorGUILayout.LabelField( "Vertical Axis: " + targ.VerticalAxis.ToString( "F2" ) );
				EditorGUILayout.LabelField( "Joystick State: " + targ.GetJoystickState() );
				EditorGUILayout.EndVertical();
			}
		}

		// DEVELOPMENT MODE //
		if( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) )
		{
			EditorGUILayout.Space();
			GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11, richText = true };
			GUILayout.BeginHorizontal();
			GUILayout.Space( -10 );
			showDefaultInspector = GUILayout.Toggle( showDefaultInspector, ( showDefaultInspector ? "▼ " : "► " ) + "<color=#ff0000ff>Development Inspector</color>", toolbarStyle );
			GUILayout.EndHorizontal();
			if( showDefaultInspector )
			{
				EditorGUILayout.Space();

				base.OnInspectorGUI();
			}
		}

		EditorGUILayout.Space();
		
		Repaint();
	}

	void CheckHighlightGameObjects ()
	{
		if( targ.highlightBase != null )
		{
			Undo.RecordObject( targ.highlightBase.gameObject, ( targ.showHighlight ? "Enable" : "Disable" ) + " Joystick Highlight" );
			targ.highlightBase.gameObject.SetActive( targ.showHighlight );
		}

		if( targ.highlightJoystick != null )
		{
			Undo.RecordObject( targ.highlightJoystick.gameObject, ( targ.showHighlight ? "Enable" : "Disable" ) + " Joystick Highlight" );
			targ.highlightJoystick.gameObject.SetActive( targ.showHighlight );
		}
	}
	
	void CheckTensionAccentGameObjects ()
	{
		for( int i = 0; i < targ.TensionAccents.Count; i++ )
		{
			if( targ.TensionAccents[ i ] == null )
				continue;

			Undo.RecordObject( targ.TensionAccents[ i ].gameObject, ( targ.showTension ? "Enable" : "Disable" ) + " Tension Accent" );
			targ.TensionAccents[ i ].gameObject.SetActive( targ.showTension );
		}
	}

	bool NoSpriteDirection
	{
		get
		{
			bool noDirection = false;

			for( int i = 0; i < targ.TensionAccents.Count; i++ )
			{
				if( targ.TensionAccents[ i ] != null )
				{
					for( int n = i + 1; n < targ.TensionAccents.Count; n++ )
					{
						if( targ.TensionAccents[ n ] != null )
						{
							if( targ.TensionAccents[ i ].transform.eulerAngles.z == targ.TensionAccents[ n ].transform.eulerAngles.z )
							{
								noDirection = true;
								break;
							}
						}
					}
					break;
				}
			}

			return noDirection;
		}
	}

	bool IdenticalSprites
	{
		get
		{
			bool identicalSprites = true;

			for( int i = 0; i < targ.TensionAccents.Count; i++ )
			{
				if( targ.TensionAccents[ i ] != null && targ.TensionAccents[ i ].sprite != null )
				{
					for( int n = i + 1; n < targ.TensionAccents.Count; n++ )
					{
						if( targ.TensionAccents[ n ] != null && targ.TensionAccents[ n ].sprite != null )
						{
							if( targ.TensionAccents[ i ].sprite != targ.TensionAccents[ n ].sprite )
							{
								identicalSprites = false;
								break;
							}
						}
					}
					break;
				}
			}

			return identicalSprites;
		}
	}

	bool TensionObjectAssigned
	{
		get
		{
			for( int i = 0; i < targ.TensionAccents.Count; i++ )
			{
				if( targ.TensionAccents[ i ] != null )
					return true;
			}

			return false;
		}
	}

	bool DisplayOverwriteSpriteWarning
	{
		get
		{
			return EditorUtility.DisplayDialog( "Ultimate Joystick", "You are about to overwrite any settings made with the \"None\" Origin Option selected. Are you sure you want to do this?", "Continue", "Cancel" );
		}
	}

	void UpdateTensionImageSprites ()
	{
		for( int i = 0; i < targ.TensionAccents.Count; i++ )
		{
			if( targ.TensionAccents[ i ] == null )
				continue;

			Undo.RecordObject( targ.TensionAccents[ i ], "Update Tension Sprite" );
			targ.TensionAccents[ i ].sprite = tensionAccentSprite;
		}
	}

	void ApplyTensionColors ()
	{
		for( int i = 0; i < targ.TensionAccents.Count; i++ )
		{
			if( targ.TensionAccents[ i ] == null )
				continue;

			Undo.RecordObject( targ.TensionAccents[ i ], "Update Tension Color" );
			targ.TensionAccents[ i ].color = targ.tensionColorNone;
		}
	}
	
	void GenerateTensionImages ()
	{
		if( tensionAccentSprite == null || isPrefabInProjectWindow )
			return;

		if( targ.TensionAccents.Count > 0 )
		{
			List<GameObject> gameObjectsToDestroy = new List<GameObject>();
			for( int i = 0; i < targ.TensionAccents.Count; i++ )
			{
				if( targ.TensionAccents[ i ] != null )
					gameObjectsToDestroy.Add( targ.TensionAccents[ i ].gameObject );
			}

			serializedObject.FindProperty( "TensionAccents" ).ClearArray();
			serializedObject.ApplyModifiedProperties();

			for( int i = 0; i < gameObjectsToDestroy.Count; i++ )
				Undo.DestroyObjectImmediate( gameObjectsToDestroy[ i ] );
		}

		if( targ.tensionType == UltimateJoystick.TensionType.Directional )
		{
			for( int i = 0; i < 4; i++ )
			{
				serializedObject.FindProperty( "TensionAccents" ).InsertArrayElementAtIndex( i );
				serializedObject.ApplyModifiedProperties();

				GameObject newGameObject = new GameObject();
				newGameObject.AddComponent<RectTransform>();
				newGameObject.AddComponent<CanvasRenderer>();
				newGameObject.AddComponent<Image>();

				if( tensionAccentSprite != null )
				{
					newGameObject.GetComponent<Image>().sprite = tensionAccentSprite;
					newGameObject.GetComponent<Image>().color = targ.tensionColorNone;
				}
				else
					newGameObject.GetComponent<Image>().color = Color.clear;

				newGameObject.transform.SetParent( targ.joystickBase );
				newGameObject.transform.SetSiblingIndex( targ.joystick.transform.GetSiblingIndex() );

				newGameObject.name = "Tension Accent " + ( i == 0 ? "Up" : "Left" );
				if( i >= 2 )
					newGameObject.name = "Tension Accent " + ( i == 2 ? "Down" : "Right" );

				RectTransform trans = newGameObject.GetComponent<RectTransform>();

				trans.anchorMin = new Vector2( 0.0f, 0.0f );
				trans.anchorMax = new Vector2( 1.0f, 1.0f );
				trans.offsetMin = Vector2.zero;
				trans.offsetMax = Vector2.zero;
				trans.pivot = new Vector2( 0.5f, 0.5f );
				trans.anchoredPosition = Vector2.zero;

				serializedObject.FindProperty( string.Format( "TensionAccents.Array.data[{0}]", i ) ).objectReferenceValue = newGameObject.GetComponent<Image>();
				serializedObject.ApplyModifiedProperties();

				Undo.RegisterCreatedObjectUndo( newGameObject, "Create Tension Accent Object" );
			}
		}
		else
		{
			serializedObject.FindProperty( "TensionAccents" ).InsertArrayElementAtIndex( 0 );
			serializedObject.ApplyModifiedProperties();

			GameObject newGameObject = new GameObject();
			newGameObject.AddComponent<RectTransform>();
			newGameObject.AddComponent<CanvasRenderer>();
			newGameObject.AddComponent<Image>();

			if( tensionAccentSprite != null )
			{
				newGameObject.GetComponent<Image>().sprite = tensionAccentSprite;
				newGameObject.GetComponent<Image>().color = targ.tensionColorNone;
			}
			else
				newGameObject.GetComponent<Image>().color = Color.clear;

			newGameObject.transform.SetParent( targ.joystickBase );
			newGameObject.transform.SetSiblingIndex( targ.joystick.transform.GetSiblingIndex() );

			newGameObject.name = "Tension Accent Free";

			RectTransform trans = newGameObject.GetComponent<RectTransform>();

			trans.anchorMin = new Vector2( 0.0f, 0.0f );
			trans.anchorMax = new Vector2( 1.0f, 1.0f );
			trans.offsetMin = Vector2.zero;
			trans.offsetMax = Vector2.zero;
			trans.pivot = new Vector2( 0.5f, 0.5f );
			trans.anchoredPosition = Vector2.zero;

			serializedObject.FindProperty( string.Format( "TensionAccents.Array.data[{0}]", 0 ) ).objectReferenceValue = newGameObject.GetComponent<Image>();
			serializedObject.ApplyModifiedProperties();

			Undo.RegisterCreatedObjectUndo( newGameObject, "Create Tension Accent Object" );
		}
		RotateTensionImages();
	}

	void RotateTensionImages ()
	{
		for( int i = 0; i < targ.TensionAccents.Count; i++ )
		{
			if( targ.TensionAccents[ i ] == null )
				continue;

			Undo.RecordObject( targ.TensionAccents[ i ].transform, "Update Rotation Offset" );
			targ.TensionAccents[ i ].transform.eulerAngles = new Vector3( 0, 0, ( 90 * i ) + targ.rotationOffset );
		}

		noSpriteDirection = NoSpriteDirection;
	}

	void OnSceneGUI ()
	{
		if( targ == null || Selection.activeGameObject == null || Application.isPlaying || Selection.objects.Length > 1 )
			return;

		if( targ.joystickBase == null )
			return;

		RectTransform trans = targ.transform.GetComponent<RectTransform>();
		Vector3 transCenter = trans.position;
		Vector3 joystickCenter = targ.joystickBase.position;
		float halfSize = ( targ.joystickBase.sizeDelta.x / 2 ) - ( targ.joystickBase.sizeDelta.x / 20 );

		Handles.color = colorDefault;
		DisplayActivationRange.frames++;
		DisplayActivationCustomWidth.frames++;
		DisplayActivationCustomHeight.frames++;
		DisplayRadius.frames++;
		DisplayBoundary.frames++;
		DisplayAxis.frames++;
		DisplayDeadZone.frames++;
		DisplayTensionDeadZone.frames++;

		if( EditorPrefs.GetBool( "UUI_SizeAndPlacement" ) )
		{
			if( DisplayActivationRange.HighlightGizmo && !targ.customActivationRange )
			{
				Handles.color = colorValueChanged;

				if( targ.boundary == UltimateJoystick.Boundary.Circular && targ.joystickTouchSize != UltimateJoystick.JoystickTouchSize.Custom )
					Handles.DrawWireDisc( joystickCenter, new Vector3( 0, 0, 1 ), trans.sizeDelta.x / 2 );
				else
					Handles.DrawWireCube( transCenter, new Vector3( trans.sizeDelta.x, trans.sizeDelta.y, 0 ) );

				Handles.Label( transCenter + ( -trans.transform.up * ( trans.sizeDelta.x / 2 ) ), "Activation Range: " + targ.activationRange, handlesCenteredText );
			}
			
			if( DisplayActivationCustomWidth.HighlightGizmo )
			{
				Handles.color = colorValueChanged;
				Handles.DrawLine( transCenter + new Vector3( -trans.sizeDelta.x / 2, trans.sizeDelta.y / 2, 0 ), transCenter + new Vector3( -trans.sizeDelta.x / 2, -trans.sizeDelta.y / 2, 0 ) );
				Handles.DrawLine( transCenter + new Vector3( trans.sizeDelta.x / 2, trans.sizeDelta.y / 2, 0 ), transCenter + new Vector3( trans.sizeDelta.x / 2, -trans.sizeDelta.y / 2, 0 ) );
			}

			if( DisplayActivationCustomHeight.HighlightGizmo )
			{
				Handles.color = colorValueChanged;
				Vector3 topLeft = transCenter + new Vector3( -trans.sizeDelta.x / 2, trans.sizeDelta.y / 2, 0 );
				Vector3 topRight = transCenter + new Vector3( trans.sizeDelta.x / 2, trans.sizeDelta.y / 2, 0 );
				Vector3 bottomLeft = transCenter + new Vector3( -trans.sizeDelta.x / 2, -trans.sizeDelta.y / 2, 0 );
				Vector3 bottomRight = transCenter + new Vector3( trans.sizeDelta.x / 2, -trans.sizeDelta.y / 2, 0 );
				Handles.DrawLine( topLeft, topRight );
				Handles.DrawLine( bottomLeft, bottomRight );
			}

			if( DisplayRadius.HighlightGizmo )
			{
				Handles.color = colorValueChanged;

				if( targ.boundary == UltimateJoystick.Boundary.Circular )
					Handles.DrawWireDisc( targ.joystickBase.position, Vector3.forward, targ.joystickBase.sizeDelta.x * ( targ.radiusModifier / 10 ) );
				else
				{
					float joystickRadius = ( targ.joystickBase.sizeDelta.x * ( targ.radiusModifier / 10 ) ) * 2;
					Handles.DrawWireCube( joystickCenter, new Vector3( joystickRadius, joystickRadius, 0 ) );
				}
				Handles.Label( joystickCenter + ( -trans.transform.up * ( targ.joystickBase.sizeDelta.x * ( targ.radiusModifier / 10 ) ) ), "Radius: " + targ.radiusModifier, handlesCenteredText );
			}
		}

		if( EditorPrefs.GetBool( "UUI_Functionality" ) )
		{
			if( DisplayBoundary.HighlightGizmo )
			{
				Handles.color = colorValueChanged;

				if( targ.boundary == UltimateJoystick.Boundary.Circular )
					Handles.DrawWireDisc( joystickCenter, new Vector3( 0, 0, 1 ), targ.joystickBase.sizeDelta.x * ( targ.radiusModifier / 10 ) );
				else
					Handles.DrawWireCube( joystickCenter, new Vector3( targ.joystickBase.sizeDelta.x, targ.joystickBase.sizeDelta.y, 0 ) );
			}

			if( DisplayAxis.HighlightGizmo )
			{
				Handles.color = colorValueChanged;

				if( targ.axis != UltimateJoystick.Axis.X )
				{
					Handles.ArrowHandleCap( 0, joystickCenter, Quaternion.Euler( 90, 90, 0 ), halfSize, EventType.Repaint );
					Handles.ArrowHandleCap( 0, joystickCenter, Quaternion.Euler( -90, 90, 0 ), halfSize, EventType.Repaint );
				}

				if( targ.axis != UltimateJoystick.Axis.Y )
				{
					Handles.ArrowHandleCap( 0, joystickCenter, Quaternion.Euler( 0, 90, 0 ), halfSize, EventType.Repaint );
					Handles.ArrowHandleCap( 0, joystickCenter, Quaternion.Euler( 180, 90, 0 ), halfSize, EventType.Repaint );
				}
			}

			if( DisplayDeadZone.HighlightGizmo && targ.deadZone > 0.0f )
			{
				Color redColor = Color.red;
				redColor.a = 0.25f;
				Handles.color = redColor;
				Handles.DrawSolidDisc( joystickCenter, Vector3.forward, ( targ.joystickBase.sizeDelta.x / 2 ) * targ.deadZone );

				Handles.color = colorValueChanged;
				Handles.DrawWireDisc( joystickCenter, Vector3.forward, ( targ.joystickBase.sizeDelta.x / 2 ) * targ.deadZone );
			}
		}

		if( EditorPrefs.GetBool( "UUI_VisualOptions" ) )
		{
			if( EditorPrefs.GetBool( "UJ_TensionAccent" ) )
			{
				if( DisplayTensionDeadZone.HighlightGizmo && targ.tensionDeadZone > 0.0f )
				{
					Color redColor = Color.red;
					redColor.a = 0.25f;
					Handles.color = redColor;
					Handles.DrawSolidDisc( joystickCenter, Vector3.forward, ( targ.joystickBase.sizeDelta.x / 2 ) * targ.tensionDeadZone );

					Handles.color = colorValueChanged;
					Handles.DrawWireDisc( joystickCenter, Vector3.forward, ( targ.joystickBase.sizeDelta.x / 2 ) * targ.tensionDeadZone );
				}
			}
		}

		SceneView.RepaintAll();
	}

	// ---------------------------------< JOYSTICK CREATOR FUNCTIONS >--------------------------------- //
	public static void CreateNewUltimateJoystick ( GameObject joystickPrefab )
	{
		GameObject prefab = ( GameObject )Object.Instantiate( joystickPrefab, Vector3.zero, Quaternion.identity );
		prefab.name = joystickPrefab.name;
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
			if( allCanvas[ i ].renderMode == RenderMode.ScreenSpaceOverlay && allCanvas[ i ].enabled == true && ValidateCanvasScalerComponent( allCanvas[ i ] ) )
			{
				child.transform.SetParent( allCanvas[ i ].transform, false );
				CreateEventSystem();
				return;
			}
		}
		CreateNewCanvas( child );
	}

	static bool ValidateCanvasScalerComponent ( Canvas canvas )
	{
		if( !canvas.GetComponent<CanvasScaler>() )
			return true;
		else if( canvas.GetComponent<CanvasScaler>().uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize )
			return true;

		return false;
	}
	// -------------------------------< END JOYSTICK CREATOR FUNCTIONS >------------------------------- //
}