
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fgt_EffectOnLegacyAnim : MonoBehaviour {


	public AnimationClip onThisAnim;
	public GameObject here;
	public float delay=0.2f;  // this is the maximum value of delay
	private float actualDelay;  //this is where we keep the actual delay, reduced by time
	private string animName;
	private float time;
	private float changeCooldown=0.2f;
	private bool  played=false;

	public Text writeHere;

	public GameObject[] database;
	public int currentEffectNo;

	void  Start (){

		actualDelay=delay;
		currentEffectNo=database.Length-1;
		if (writeHere) writeHere.text=currentEffectNo.ToString()+" "+database[currentEffectNo].name;


	}

	void  Update (){
		if (changeCooldown>0) changeCooldown-=Time.deltaTime;

		animName=onThisAnim.name;


		if (GetComponent<Animation>().IsPlaying(animName) && played == false)  // if the animation is running, and we didn't played the anim yet
		{
			actualDelay-=Time.deltaTime;
			if (actualDelay <= 0)  // delay was done, time to play the effect
			{
				actualDelay=delay; // resetting the delay to its default value
				time = 0;			//technical value, to prevent re-playing the effect until the end of the anim
				played = true;		//prevent to play it multiple times
				GameObject effect = Instantiate(database[currentEffectNo], here.transform.position, here.transform.rotation); //creating the effect
				effect.transform.parent = here.transform; // transforming to its target
			}

		}


		if (time<onThisAnim.length)  // we reset the time when needed
		{
			time+=Time.deltaTime;
		}
		else 
		{
			played=false;
			time=0;
		}


		if (Input.GetKeyDown(KeyCode.UpArrow) && changeCooldown<=0)
		{
			changeCooldown+=0.25f;
			currentEffectNo+=1;
			if (currentEffectNo>=database.Length) currentEffectNo=0;
			if (writeHere) writeHere.text=currentEffectNo.ToString()+" "+database[currentEffectNo].name;
		}


		if (Input.GetKeyDown(KeyCode.DownArrow) && changeCooldown<=0)
		{
			changeCooldown+=0.25f;
			currentEffectNo-=1;
			if (currentEffectNo<0) currentEffectNo=database.Length-1;
			if (writeHere) writeHere.text=currentEffectNo.ToString()+" "+database[currentEffectNo].name;
		}



	}
}