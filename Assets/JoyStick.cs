using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour {

    public float h;
    public float v;
    public bool t1;
    public bool t2;
	// Use this for initialization
	void Start () {
        //UltimateJoystick.DisableJoystick("WSJ");
        //UltimateJoystick.EnableJoystick("WSJ");

    }
	
	// Update is called once per frame
	void Update () {
        h = UltimateJoystick.GetHorizontalAxis("WSJ");
        v = UltimateJoystick.GetVerticalAxis("WSJ");
        t1 = UltimateJoystick.GetJoystickState("WSJ");
        t2 = UltimateJoystick.GetTapCount("WSJ");
    }
}
