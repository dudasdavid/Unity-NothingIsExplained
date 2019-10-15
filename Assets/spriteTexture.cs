using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class spriteTexture : MonoBehaviour
{
    public Sprite sprite;

    public void Awake()
    {
        Apply();
    }

    public void Apply()
    {
        GetComponent<Renderer>().material.mainTexture = sprite.texture;
        Debug.Log(sprite.texture.width + " " + sprite.texture.height);
        Debug.Log(sprite.textureRect);

        GetComponent<Renderer>().material.mainTextureScale = new Vector2(
            sprite.textureRect.width / sprite.texture.width,
            sprite.textureRect.height / sprite.texture.height
        );

        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(
            sprite.textureRect.x / sprite.texture.width,
            sprite.textureRect.y / sprite.texture.height
        );
    }
}