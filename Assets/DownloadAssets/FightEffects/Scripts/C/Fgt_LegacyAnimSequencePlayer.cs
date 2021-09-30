using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fgt_LegacyAnimSequencePlayer : MonoBehaviour {



	public AnimationClip[] anims;
	private int next = 0;
	private string animName;
	private float time;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		animName=anims[next].name;
		time+=Time.deltaTime;

		if (!GetComponent<Animation>().IsPlaying(animName))
		{
			time=0;
			GetComponent<Animation>().Play(animName);
		}

		if (time>anims[next].length)
		{
			next+=1;
			if (next==anims.Length) next=0;
		}


		
	}
}
