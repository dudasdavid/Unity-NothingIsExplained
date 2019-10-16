using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeWarpDown : MonoBehaviour {
	private LevelManager t_LevelManager;
	private Mario mario;
	private Transform stop;
	private bool isMoving;

	private float platformVelocityY = -0.05f;
	public string sceneName;
	public bool leadToSameLevel = true;

	// Use this for initialization
	void Start () {
		t_LevelManager = FindObjectOfType<LevelManager> ();
		mario = FindObjectOfType<Mario> ();
		stop = transform.parent.transform.Find ("Platform Stop");
	}


	bool marioEntered = false;
	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player" && mario.isCrouching && !marioEntered) {
			mario.AutomaticCrouch ();
			isMoving = true;
			marioEntered = true;
			t_LevelManager.musicSource.Stop ();
			t_LevelManager.soundSource.PlayOneShot (t_LevelManager.pipePowerdownSound);
		}
	}
}
