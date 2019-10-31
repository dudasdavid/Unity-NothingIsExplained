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

    public bool gameOver = false;

    private bool isRespawning;
    public bool isPoweringDown;

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
    public Text gameOverText;
    public Text winnerText;
    public Text winnerText2;
    public GameObject FloatingTextEffect;
    private const float floatingTextOffsetY = 2f;

    private float timeSpent = 0f;
    private int timeSpentInt = 0;

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

    public GameObject movingGround;
    private Vector3 newPos;


    private textCollider t_textCollider;

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
        gameOverText.enabled = false;
        winnerText.enabled = false;
        winnerText2.enabled = false;

        t_textCollider = FindObjectOfType<textCollider>();
        t_textCollider.GetComponent<BoxCollider2D>().enabled = false;


        SetHudCoin();
        SetHudScore();
        SetHudTime();


        Debug.Log(this.name + " Start: current scene is " + SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (!gameOver)
        {
            timeSpent += Time.deltaTime / 1.0f;
            SetHudTime();
        }

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
        gameOver = true;
        gameOverText.enabled = true;
        soundSource.PlayOneShot(powerupSound); // should play sound regardless of size
        //AddScore (powerupBonus, mario.transform.position);
        MarioRespawn();
    }

    public void MarioInvertGravity()
    {
        soundSource.PlayOneShot(powerupSound); // should play sound regardless of size
        //AddScore (powerupBonus, mario.transform.position);
        mario.invertGravity = true;
        mario.jumpVelocity = mario.defaultJumpVelocityInverted;
        mario.doubleJumpVelocity = mario.defaultDoubleJumpVelocityInverted;
        camCtrl.followCameraY = true;
    }

    public void MarioNormalGravity()
    {
        soundSource.PlayOneShot(powerupSound); // should play sound regardless of size
        //AddScore (powerupBonus, mario.transform.position);
        mario.invertGravity = false;
        mario.jumpVelocity = mario.defaultJumpVelocity;
        mario.doubleJumpVelocity = mario.defaultDoubleJumpVelocity;
        //camCtrl.followCameraY = false;
    }

    public void MarioSmallButFast()
    {
        mario.jumpCount = 1;
        MarioPowerDown();
        mario.velocity = mario.defaultFastSpeed;
        
    }

    public void MarioSlowDown()
    {
        mario.velocity = mario.defaultNormalSpeed;
    }

    public void MarioPowerDown()
    {
        if (!isPoweringDown)
        {
            Debug.Log(this.name + " MarioPowerDown: called and executed");
            isPoweringDown = true;
            mario.isDoubleJumpEnabled = false;
            if (marioSize > 0)
            {
                StartCoroutine(MarioPowerDownCo());
                soundSource.PlayOneShot(pipePowerdownSound);
            }
            else
            {
                isPoweringDown = false;
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

		StartCoroutine(RestartWithDelay(2f));
        if (!isRespawning)
        {
            isRespawning = true;

            marioSize = 0;

            soundSource.Stop();
            musicSource.Stop();
            soundSource.PlayOneShot(deadSound);

            Time.timeScale = 0f;
            mario.FreezeAndDie ();


        }
    }

	IEnumerator RestartWithDelay(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
		Application.LoadLevel(Application.loadedLevel);
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
        timeSpentInt = Mathf.RoundToInt (timeSpent);
        timeText.text = timeSpentInt.ToString ("D3");
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

    public void GameOver()
    {
        Debug.Log("Game over");
    }

    public void OpenGround()
    {
        newPos = movingGround.transform.position;
        newPos.x += 50;
        movingGround.transform.position = newPos;
    }

    public void MarioReachFlagPole() {
        gameOver = true;
       

        StartCoroutine(enableWithDelay(2f));
    }

    IEnumerator enableWithDelay(float time)
    {
        Debug.Log("Couroutine started");
        yield return new WaitForSeconds(time);
        winnerText.enabled = true;
        winnerText2.enabled = true;
        t_textCollider.GetComponent<BoxCollider2D>().enabled = true;
        t_textCollider.GetComponent<Rigidbody2D>().simulated = true;
        Debug.Log("Couroutine done");
    }

}
