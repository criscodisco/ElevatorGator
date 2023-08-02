using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GatorController : MonoBehaviour
{
    public enum TypeOfTriggerEntered
    {
        GROUNDTRIGGER, UPTRIGGER, DOWNTRIGGER, JUMPING
    }

    // horizontal input
    public float h;

    public Vector3 playerOrigin;
    public float MoveSpeed = 10;
    private float JumpForce = 175;
    public LayerMask whatIsGround;
    public Animator animator;
    public Rigidbody2D rigidBody;
    public int jumpCount = 0;
    public int maxJumps = 1;

    public Camera mainCamera;
    public DeathGroundTrigger deathTrigger;

    public Canvas deathCanvas;
    public Canvas gameOverCanvas;
    public Canvas pauseCanvas;
    public Canvas levelCompleteCanvas;

    public SFX sfx;

    private float timeWhenAllowedLoopAudioClip = 0f;
    private float timeBetweenLifeAudioClip = .05f;
    private float timeBetweenDuckAudioClip = .1f;

    TypeOfTriggerEntered triggerType;
    private Donut donut;
    public static int donutPointTotal;
    public int donutPoint = 1;

    private AlligatorLife life;
    public static int alligatorPointTotal = 3;
    public int alligatorPoint = 1;
    public int gameStartLives = 3;

    public TMP_Text donutCounterText;
    public TMP_Text alligatorLifeCounterText;

    private float maxVelocity = 20f;

    public ExitSign exitSign;
 
    void SignLoadsNextLevel()
    {
        exitSign.isLevelComplete = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void RevivePlayer()
    {
        deathTrigger.isPlayerDead = false;
        deathCanvas.gameObject.SetActive(false);
    }

    void Start()
    {
        triggerType = TypeOfTriggerEntered.JUMPING;
        jumpCount = 0;
        playerOrigin = transform.position;
        animator = GetComponent<Animator>();
        alligatorLifeCounterText.text = alligatorPointTotal.ToString();
        gameOverCanvas.gameObject.SetActive(false);
        pauseCanvas.gameObject.SetActive(false);
        levelCompleteCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, maxVelocity);

        if (exitSign.isLevelComplete)
        {
            levelCompleteCanvas.gameObject.SetActive(true);
            Invoke("SignLoadsNextLevel", 2f);
        }

        if (deathTrigger.isGameOver)
        {
            gameOverCanvas.gameObject.SetActive(true);
            if (Input.anyKey)
            {
                Application.Quit();
            }
        }

        if (deathTrigger.isPlayerDead)
        {
            Invoke("RevivePlayer", 2f);
            rigidBody.velocity = Vector2.zero;
            transform.position = playerOrigin;
        }

        if (!PauseControl.gameIsPaused)
        {
            h = Input.GetAxis("Horizontal");
        }
        else
        {

            h = 0f;
        }

        animator.SetFloat("MoveSpeed", Mathf.Abs(h));

        if (h < 0.0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.Translate(Vector2.left * MoveSpeed * Time.deltaTime);

        }
        else if (h > 0.0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.Translate(Vector2.right * MoveSpeed * Time.deltaTime);

        }

        if (!PauseControl.gameIsPaused)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (jumpCount > 0)
                {                 
                    rigidBody.AddForce(Vector3.up * JumpForce, ForceMode2D.Impulse);
                    animator.SetBool("Jump", true);
                    jumpCount -= 1;
                }
            }          
        }

        switch (triggerType)
        {
            case TypeOfTriggerEntered.GROUNDTRIGGER:
            {
                animator.SetBool("Grounded", true);
                animator.SetBool("Jump", false);
                jumpCount = maxJumps;
                break;
            }
            case TypeOfTriggerEntered.UPTRIGGER:
            {
                animator.SetBool("Climbing", true);
                animator.SetBool("Jump", false);
                jumpCount = maxJumps;

                break;
            }
            case TypeOfTriggerEntered.DOWNTRIGGER:
        {
                Vector3 downVatorVelocity = rigidBody.velocity;
                downVatorVelocity.y = Mathf.Clamp(downVatorVelocity.y, -10, 0);
                rigidBody.velocity = downVatorVelocity;
                animator.SetBool("Climbing", true);
                animator.SetBool("Jump", false);
                jumpCount = maxJumps;
                
                break;
            }
            case TypeOfTriggerEntered.JUMPING:
            {
                    jumpCount = 0;
                    animator.SetBool("Grounded", false);
                    animator.SetBool("Jump", true);
                    animator.SetBool("Climbing", false);

                    break;
            }
            default: break;
        }
    }      

    void OnTriggerEnter2D(Collider2D coll)
    {
        if ((coll.gameObject.tag == "duck") || (coll.gameObject.tag == "duck" && (triggerType == TypeOfTriggerEntered.DOWNTRIGGER || triggerType == TypeOfTriggerEntered.UPTRIGGER)))
        {
            if (timeWhenAllowedLoopAudioClip <= Time.time)
            {
                sfx.audioSourceDuckPickup.PlayOneShot(sfx.duckClip);
                timeWhenAllowedLoopAudioClip = Time.time + timeBetweenDuckAudioClip;
            }

            donutPointTotal = donutPointTotal + donutPoint;
            donutCounterText.text = donutPointTotal.ToString();

            if (donutPointTotal%30 == 0 && donutPointTotal > 0)
            {
                alligatorPointTotal = alligatorPointTotal + alligatorPoint;
                alligatorLifeCounterText.text = alligatorPointTotal.ToString();
            }

            Destroy(coll.gameObject);
        }

        else if (coll.gameObject.tag == "alligatorLife")
        {
            if (timeWhenAllowedLoopAudioClip <= Time.time)
            {
                sfx.audioSourceLifePickup.PlayOneShot(sfx.lifeClip);
                timeWhenAllowedLoopAudioClip = Time.time + timeBetweenLifeAudioClip;
            }
    
            alligatorPointTotal = alligatorPointTotal + alligatorPoint;
            alligatorLifeCounterText.text = alligatorPointTotal.ToString();
            Destroy(coll.gameObject); 
        }

        else if (coll.gameObject.tag == "ground")
        {
            triggerType = TypeOfTriggerEntered.GROUNDTRIGGER;
        }

        else if (coll.gameObject.tag == "elevator")
        {
            triggerType = TypeOfTriggerEntered.UPTRIGGER;
        }

        else if (coll.gameObject.tag == "downvator")
        {
            triggerType = TypeOfTriggerEntered.DOWNTRIGGER;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "ground")
        {
            triggerType = TypeOfTriggerEntered.JUMPING;
        }

        if (coll.gameObject.tag == "elevator")
        {  
                triggerType = TypeOfTriggerEntered.JUMPING;
        }

        if (coll.gameObject.tag == "downvator")
        {
            if (jumpCount > 0)
            {
                triggerType = TypeOfTriggerEntered.JUMPING;
            }
        }
    }
}
