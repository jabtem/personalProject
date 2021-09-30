using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayGUI : MonoBehaviour {
	
	private List<List<string>> animationList = new List<List<string>>();
	
	private string currentState = "";
	
	public Transform[] transforms;
	private Animator[] animator;
	
	
	// Use this for initialization.
	void Start () {
		initAnimationList();
		initAnimators();
		initCharaction();
	}
	
	void Update() {
		updateState();
	}
	
	// Init animation list.
	void initAnimationList() {
		List<string> animators;
		int i = 0;
		
		//idle
		animators = new List<string>();
		for (i = 0; i < 6; i++) {
			animators.Add("idle" + i);
		}
		animationList.Add(animators);
		
		//run
		animators = new List<string>();
		animators.Add("walk");
		for (i = 0; i < 4; i++) {
			animators.Add("run" + i);
		}
		animators.Add("runBackward");
		animators.Add("runLeft");
		animators.Add("runRight");
		animators.Add("jump");
		animationList.Add(animators);
		
		//attack
		animators = new List<string>();
		for (i = 0; i < 3; i++) {
			animators.Add("attack" + i);
		}
		animationList.Add(animators);
		
		//attackDagger
		animators = new List<string>();
		for (i = 0; i < 6; i++) {
			animators.Add("attackDagger" + i);
		}
		for (i = 0; i < 5; i++) {
			animators.Add("skillDagger" + i);
		}
		animationList.Add(animators);
		
		//attackAxe
		animators = new List<string>();
		for (i = 0; i < 4; i++) {
			animators.Add("attackAxe" + i);
		}
		for (i = 0; i < 7; i++) {
			animators.Add("skillAxe" + i);
		}
		animationList.Add(animators);
		
		//attackSword
		animators = new List<string>();
		for (i = 0; i < 3; i++) {
			animators.Add("attackSword" + i);
		}
		for (i = 0; i < 8; i++) {
			animators.Add("skillSword" + i);
		}
		animationList.Add(animators);
		
		//attackSpear
		animators = new List<string>();
		for (i = 0; i < 2; i++) {
			animators.Add("attackSpear" + i);
		}
		for (i = 0; i < 5; i++) {
			animators.Add("skillSpear" + i);
		}
		animationList.Add(animators);
		
		//skill
		animators = new List<string>();
		for (i = 0; i < 10; i++) {
			animators.Add("skill" + i);
		}
		animationList.Add(animators);
		animators = new List<string>();
		for (i = 10; i < 21; i++) {
			animators.Add("skill" + i);
		}
		animationList.Add(animators);
		
		//other
		animators = new List<string>();
		animators.Add("turn");
		animators.Add("rest0");
		animators.Add("rest1");
		animators.Add("wound");
		animators.Add("death");
		for (i = 0; i < 4; i++) {
			animators.Add("other" + i);
		}
		animationList.Add(animators);
	}
	
	void initAnimators() {
		animator = new Animator[transforms.Length];
		for (int i = 0; i < transforms.Length; i++) {
			animator[i] = transforms[i].GetComponent<Animator>();
		}
	}
	
	void initCharaction() {
		for (int i = 0; i < transforms.Length; i++) {
			transforms[i].GetComponent<CharacterSetup>().useWeapon(CharacterSetup.Weapon.Sword);
		}
	}

	void OnGUI() {
		GUILayout.BeginHorizontal();
		for (int i = 0; i < animationList.Count; i++) {
			GUILayout.BeginVertical("box");
			for (int j = 0; j < animationList[i].Count; j++) {
				if (GUILayout.Button(animationList[i][j])) {
					transforms[0].position = Vector3.zero;
					transforms[1].position = new Vector3(2,0,-3);
					transforms[2].position = new Vector3(-2,0,-3);
					
					currentState = animationList[i][j];
				}
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
	}
	
	void updateState() {
		
		AnimatorStateInfo stateInfo = animator[0].GetCurrentAnimatorStateInfo(0);
			
		if (!stateInfo.IsName("Base Layer.idle0")) {
			for (int j = 0; j < animator.Length; j++) {
				for (int a = 0; a < animationList.Count; a++) {
					for (int b = 0; b < animationList[a].Count; b++) {
						animator[j].SetBool(animationList[a][b], false);
					}
				}
	        }
		}
		
		if(currentState != "") {
			for (int j = 0; j < animator.Length; j++) {
				
				if(currentState.Contains("Dagger")) {
					transforms[j].GetComponent<CharacterSetup>().useWeapon(CharacterSetup.Weapon.Dagger);
				} else if(currentState.Contains("Axe")) {
					transforms[j].GetComponent<CharacterSetup>().useWeapon(CharacterSetup.Weapon.Axe);
				} else if(currentState.Contains("Spear")) {
					transforms[j].GetComponent<CharacterSetup>().useWeapon(CharacterSetup.Weapon.Spear);
				} else if(getOtherAnimation(currentState)) {
					transforms[j].GetComponent<CharacterSetup>().useWeapon(CharacterSetup.Weapon.Null);
				}else {
					transforms[j].GetComponent<CharacterSetup>().useWeapon(CharacterSetup.Weapon.Sword);
				}
				
				animator[j].SetBool(currentState, true);
			}
		}
		currentState = "";
	}
	
	
	bool getOtherAnimation(string state) {
		
		for (int i = 0; i < 21; i++) {
			if(state == "skill" + i) {
				return true;
			}
		}
		
		if(state == "turn") {
			return true;
		}else if(state == "rest0") {
			return true;
		}else if(state == "rest1") {
			return true;
		}else if(state == "other0") {
			return true;
		}else if(state == "other1") {
			return true;
		}else if(state == "other2") {
			return true;
		}else if(state == "other3") {
			return true;
		}
		
		return false;
		
	}
}
