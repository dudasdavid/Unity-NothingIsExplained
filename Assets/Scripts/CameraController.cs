using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public Transform player;

    private Vector3 offset;        
    public bool followCameraY;


    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
     }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Max(position.x, player.position.x);
        transform.position = position;
    }

}