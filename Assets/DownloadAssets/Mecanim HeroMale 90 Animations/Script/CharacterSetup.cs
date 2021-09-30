using UnityEngine;
using System.Collections;

public class CharacterSetup : MonoBehaviour {
	public enum Weapon{
		Null,
		Dagger,
		Axe,
		Sword,
		Spear,
	};
	
	public Transform Dagger;
	public Transform[] Axes;
	public Transform Sword;
	public Transform Spear;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void useWeapon(Weapon weapon) {
		Dagger.gameObject.SetActive(false);
		for (int i = 0; i < Axes.Length; i++) {
			Axes[i].gameObject.SetActive(false);
		}
		Sword.gameObject.SetActive(false);
		Spear.gameObject.SetActive(false);
		
		
		switch (weapon) {
		case Weapon.Null:
			
			break;
		case Weapon.Dagger:
			Dagger.gameObject.SetActive(true);
			break;
		case Weapon.Axe:
			for (int i = 0; i < Axes.Length; i++) {
			Axes[i].gameObject.SetActive(true);
		}
			break;
		case Weapon.Sword:
			Sword.gameObject.SetActive(true);
			break;
		case Weapon.Spear:
			Spear.gameObject.SetActive(true);
			break;
		}
	}
}
