using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBreaker : MonoBehaviour
{
    // Start is called before the first frame update

    public int jumpCounter;
    private LevelManager t_LevelManager;

    void Start()
    {
        jumpCounter = 0;
        t_LevelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        jumpCounter++;
        if (jumpCounter == 10)
        {
            Destroy(gameObject);
            t_LevelManager.MarioNormalGravity();
        }
    } 
}
