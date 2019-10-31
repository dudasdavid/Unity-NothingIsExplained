using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;


public class textCollider : MonoBehaviour
{
    private LevelManager t_LevelManager;
    private Vector3 position;

    public Text text2hide;

    public int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        t_LevelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && t_LevelManager.winnerText.enabled)
        {
            // = transform.position;
            //position.y -= 5;
            //transform.position = position;
            //gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 10;
            counter++;
            if (counter > 62)
            {
                text2hide.enabled = false;
                t_LevelManager.MarioDies();
            }

        }
    }
}
