using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFollow : MonoBehaviour
{
    public RectTransform uiElementToFollow;

    // Update is called once per frame
    void Update()
    {
        transform.position = uiElementToFollow.position;
    }
}
