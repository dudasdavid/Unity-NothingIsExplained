using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBreaker : MonoBehaviour
{
    // Start is called before the first frame update

    public int jumpCounter;
    private LevelManager t_LevelManager;

    public GameObject destroyPipe;

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
        if (jumpCounter >= 10)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(DestroyWithDelay(2f));
            
            //t_LevelManager.MarioNormalGravity();
        }
    }

    IEnumerator DestroyWithDelay(float time)
    {
        Debug.Log("Couroutine started");
        yield return new WaitForSeconds(time);
        Destroy(destroyPipe);
        Debug.Log("Couroutine done");
    }
}
