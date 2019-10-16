using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


public class LevelManager : MonoBehaviour
{
    private const float loadSceneDelay = 1f;

    public int marioSize; // 0..2
    public int coins;

    private bool isRespawning;
    private bool isPoweringDown;

    public bool isInvinciblePowerdown;
    public bool isInvincibleStarman;
    private float transformDuration = 1;

    private PlayerControl mario;
    private Animator mario_Animator;
    private Rigidbody2D mario_Rigidbody2D;
    private CameraController camCtrl;

    public Text scoreText;
    public Text coinText;
    public Text timeText;
    public GameObject FloatingTextEffect;
    private const float floatingTextOffsetY = 2f;

    public AudioSource musicSource;
    public AudioSource soundSource;

    public AudioClip levelMusic;

    public AudioClip oneUpSound;
    public AudioClip bowserFallSound;
    public AudioClip bowserFireSound;
    public AudioClip breakBlockSound;
    public AudioClip bumpSound;
    public AudioClip coinSound;
    public AudioClip deadSound;
    public AudioClip fireballSound;
    public AudioClip flagpoleSound;
    public AudioClip jumpSmallSound;
    public AudioClip jumpSuperSound;
    public AudioClip kickSound;
    public AudioClip pipePowerdownSound;
    public AudioClip powerupSound;
    public AudioClip powerupAppearSound;
    public AudioClip stompSound;
    public AudioClip warningSound;


    void Awake()
    {
        Time.timeScale = 1;
    }

    // Use this for initialization
    void Start()
    {

        mario = FindObjectOfType<PlayerControl>();
        mario_Animator = mario.gameObject.GetComponentInChildren<Animator>();

        mario_Rigidbody2D = mario.gameObject.GetComponent<Rigidbody2D>();
        mario.UpdateSize();

        camCtrl = FindObjectOfType<CameraController>();

        // Sound volume
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
        soundSource.volume = PlayerPrefs.GetFloat("soundVolume");

        // HUD
        SetHudCoin();
        SetHudScore();
        SetHudTime();


        Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
    }


    /****************** Game pause */
    List<Animator> unscaledAnimators = new List<Animator>();
    float pauseGamePrevTimeScale;
    bool pausePrevMusicPaused;




    /****************** Powerup / Powerdown / Die */
    public void MarioPowerUp()
    {
        soundSource.PlayOneShot(powerupSound); // should play sound regardless of size
        if (marioSize < 2)
        {
            StartCoroutine(MarioPowerUpCo());
        }
        //AddScore (powerupBonus, mario.transform.position);
    }

    IEnumerator MarioPowerUpCo()
    {
        Debug.Log(mario_Animator);
        mario_Animator.SetBool("isPoweringUp", true);
        Time.timeScale = 0f;
        mario_Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

        yield return new WaitForSecondsRealtime(transformDuration);

        Time.timeScale = 1;
        mario_Animator.updateMode = AnimatorUpdateMode.Normal;

        marioSize++;
        mario.UpdateSize();
        mario_Animator.SetBool("isPoweringUp", false);

        mario.isDoubleJumpEnabled = true;

    }

    public void MarioDies()
    {
        soundSource.PlayOneShot(powerupSound); // should play sound regardless of size
        //AddScore (powerupBonus, mario.transform.position);
    }

    public void MarioInvertGravity()
    {
        soundSource.PlayOneShot(powerupSound); // should play sound regardless of size
        //AddScore (powerupBonus, mario.transform.position);
        mario.invertGravity = true;
        mario.jumpVelocity = 3;
        mario.doubleJumpVelocity = 2;
        camCtrl.followCameraY = true;
    }

    public void MarioNormalGravity()
    {
        soundSource.PlayOneShot(powerupSound); // should play sound regardless of size
        //AddScore (powerupBonus, mario.transform.position);
        mario.invertGravity = false;
        mario.jumpVelocity = 8.5f;
        mario.doubleJumpVelocity = 6;
        //camCtrl.followCameraY = false;
    }

    public void MarioSmallButFast()
    {
        MarioPowerDown();
        mario.isDoubleJumpEnabled = false;
        mario.velocity = 10;
    }



    public void MarioPowerDown()
    {
        if (!isPoweringDown)
        {
            Debug.Log(this.name + " MarioPowerDown: called and executed");
            isPoweringDown = true;

            if (marioSize > 0)
            {
                StartCoroutine(MarioPowerDownCo());
                soundSource.PlayOneShot(pipePowerdownSound);
            }
            else
            {
                MarioRespawn();
            }
            Debug.Log(this.name + " MarioPowerDown: done executing");
        }
        else
        {
            Debug.Log(this.name + " MarioPowerDown: called but not executed");
        }
    }

    IEnumerator MarioPowerDownCo()
    {
        mario_Animator.SetBool("isPoweringDown", true);
        Time.timeScale = 0f;
        mario_Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

        yield return new WaitForSecondsRealtime(transformDuration);

        Time.timeScale = 1;
        mario_Animator.updateMode = AnimatorUpdateMode.Normal;

        marioSize = 0;
        mario.UpdateSize();
        mario_Animator.SetBool("isPoweringDown", false);
        isPoweringDown = false;
    }

    public void MarioRespawn(bool timeup = false)
    {
        if (!isRespawning)
        {
            isRespawning = true;

            marioSize = 0;

            soundSource.Stop();
            musicSource.Stop();
            soundSource.PlayOneShot(deadSound);

            Time.timeScale = 0f;
            //mario.FreezeAndDie ();

            if (timeup)
            {
                Debug.Log(this.name + " MarioRespawn: called due to timeup");
            }

        }
    }




    /****************** HUD and sound effects */
    public void SetHudCoin()
    {
        coinText.text = "x" + coins.ToString("D2");
    }

    public void SetHudScore()
    {
        ///scoreText.text = scores.ToString ("D6");
    }

    public void SetHudTime()
    {
        ///timeLeftInt = Mathf.RoundToInt (timeLeft);
        ///timeText.text = timeLeftInt.ToString ("D3");
    }

    public void CreateFloatingText(string text, Vector3 spawnPos)
    {
        GameObject textEffect = Instantiate(FloatingTextEffect, spawnPos, Quaternion.identity);
        textEffect.GetComponentInChildren<TextMesh>().text = text.ToUpper();
    }



    public void AddCoin()
    {
        coins++;
        soundSource.PlayOneShot(coinSound);
        if (coins == 100)
        {
            //AddLife ();
            coins = 0;
        }
        SetHudCoin();
        //AddScore (coinBonus);
    }

    public void AddCoin(Vector3 spawnPos)
    {
        coins++;
        soundSource.PlayOneShot(coinSound);
        if (coins == 100)
        {
            //AddLife ();
            coins = 0;
        }
        SetHudCoin();
        //AddScore (coinBonus, spawnPos);
    }
}
