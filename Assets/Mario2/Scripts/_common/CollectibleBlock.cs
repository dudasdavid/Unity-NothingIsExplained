﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Spawn object if bumped by Player's head
 * Applicable to: Collectible brick and question blocks
 */

public class CollectibleBlock : MonoBehaviour {
	private Animator m_Animator;
	private LevelManager t_LevelManager;

	public bool isPowerupBlock;
    public bool isSelfDestroy;
    public bool isInvertGravity;
    public bool isRevertGravity;
    public bool isSmallButFast;
    public bool isSlowDown;
    public bool isCoinOnly;


    public GameObject objectToSpawn;
	public GameObject bigMushroom;
	public GameObject fireFlower;
	public int timesToSpawn = 1;
	public Vector3 spawnPositionOffset;

	private float WaitBetweenBounce = .25f;
	private bool isActive;
	private float time1, time2;

	public List<GameObject> enemiesOnTop = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		m_Animator = GetComponent<Animator> ();
		t_LevelManager = FindObjectOfType<LevelManager> ();
		time1 = Time.time;
		isActive = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		time2 = Time.time;
		if (other.tag == "Player" && time2 - time1 >= WaitBetweenBounce) {
			//t_LevelManager.soundSource.PlayOneShot (t_LevelManager.bumpSound);

			if (isActive) {
				m_Animator.SetTrigger ("bounce");


				if (timesToSpawn > 0) {
					if (isPowerupBlock) { // spawn mushroom or fireflower depending on Mario's size
                        t_LevelManager.MarioPowerUp();
                        //Destroy(gameObject);
                        /*
                        if (t_LevelManager.marioSize == 0) {
							objectToSpawn = bigMushroom;
						} else {
							objectToSpawn = fireFlower;
						}
                        */
                    }
                    if (isSelfDestroy)
                    {
                        t_LevelManager.MarioDies();
                    }
                    if (isInvertGravity)
                    {
                        t_LevelManager.MarioInvertGravity();
                        
                    }
                    if (isRevertGravity)
                    {
                        t_LevelManager.MarioNormalGravity();

                    }
                    if (isSlowDown)
                    {
                        t_LevelManager.MarioSlowDown();
                    }
                    if (isSmallButFast)
                    {
                        t_LevelManager.MarioSmallButFast();
                    }

                    if (isCoinOnly)
                    {
                        t_LevelManager.AddCoin();
                    }

                    //Instantiate (objectToSpawn, transform.position + spawnPositionOffset, Quaternion.identity);
                    timesToSpawn--;

					if (timesToSpawn == 0) {
						m_Animator.SetTrigger ("deactivated");
						isActive = false;
					}
                    if (isInvertGravity)
                    {
                        Destroy(gameObject);
                    }

                    
                }
			}

			time1 = Time.time;
		}
	}


	// check for enemy on top
	void OnCollisionStay2D(Collision2D other) {
		Vector2 normal = other.contacts[0].normal;
		Vector2 topSide = new Vector2 (0f, -1f);
		bool topHit = normal == topSide;
		if (other.gameObject.tag.Contains("Enemy") && topHit && !enemiesOnTop.Contains(other.gameObject)) {
			enemiesOnTop.Add(other.gameObject);
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.tag.Contains("Enemy")) {
			enemiesOnTop.Remove (other.gameObject);
		}
	}
}
