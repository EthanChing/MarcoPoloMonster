using UnityEngine;
using Spine.Unity;
using System.Collections;

public class PlayerController : MonoBehaviour {

    // Scuba Steeve
    // Main Player Character and Controller
    // Audio Source Controll for all things Steeve

    [Header("Player Information")]
    public Rigidbody2D player_Rigidbody2D;
    public Transform harpoon_SpawnPoint;
    public Transform glowstick_SpawnPoint;

    [Header("Movement Variables")]
    public float braking;
    public float accel;
    public float vertSlowPercent;
    public float speed;
    public bool lookingLeft = true;
   
    [Header("Harpoon Information")]
    public GameObject harpoon;
    public float fireRate = 0.5f;
    private float nextFireTime = 0.0f;

    [Header("Glowstick Information")]
    public GameObject glowstick;
   
    
    [Header("Animation Information")]
    public SkeletonAnimation swimAnim;
    public float idleAnimSpeed = 2;
    public float swimAnimSpeed = 4;

    [Header("Particle Systems")]
    public ParticleSystem bubbler;

    [Header("Audio Source Information")]
    public AudioSource audio_Breathing;
    public AudioClip clip_BreathingNormal;
    public AudioClip clip_BreathingHeavy;
    [Space(10)]
    public AudioSource audio_Harpoon;
    public AudioClip clip_HarpoonFiring;
    public AudioClip clip_HarpoonReloading;
    [Space(10)]
    public AudioSource audio_Meter;

    [Header("Audio Variables")]
    [Range(0,1)] public float audio_HarpoonStartTime = 0.8f;
    public float audioTimer_Breathing;
    public float audioTimer_ReloadDelay;
    private float audioTimer_HarpoonReload = 999;


    void Start()
    {

    }

    void Update ()
    {
        //Shoot Harpoon
        //If the fire button is pressed and timer done. Plays audio for shooting.
        if (Input.GetButtonDown("fire") && Time.time > nextFireTime)
        {
            //Fires the harpoon
            FireHarpoon();

            //Trims front of audio clip for harpoon, then plays the clip
            audio_Harpoon.clip = clip_HarpoonFiring;
            audio_Harpoon.time = audio_HarpoonStartTime;
            audio_Harpoon.Play();

            //Starts the next timer
            nextFireTime = Time.time + fireRate;

            //Creates timer for when to start the reload sound
            audioTimer_HarpoonReload = fireRate - audioTimer_ReloadDelay;
        }

        //Countdown timer for harpoon reload sound
        if (audioTimer_HarpoonReload > 0)
        {
            audioTimer_HarpoonReload -= Time.deltaTime;
        }

        else
        {
            //Prevent clip from playing twice in a row
            audioTimer_HarpoonReload = 999;

            //Play audio clip
            audio_Harpoon.Stop();
            audio_Harpoon.clip = clip_HarpoonReloading;
            try
            {
                audio_Harpoon.Play();
            }
            catch 
            {
                
            }
        }


        var x = Input.GetAxisRaw("Horizontal") * speed;
        var y = Input.GetAxisRaw("Vertical") * speed;

        float xSpeed = x == 0 ? braking : accel;
        float ySpeed = y == 0 ? braking : accel;

        y -= y*vertSlowPercent;
        
            player_Rigidbody2D.velocity = new Vector3(
            Mathf.Lerp(player_Rigidbody2D.velocity.x, x, Time.deltaTime * xSpeed),
            Mathf.Lerp(player_Rigidbody2D.velocity.y, y, Time.deltaTime * ySpeed)
            );
        SpriteFlip();

        //if swimming
        if( x != 0 || y != 0)
        {
            swimAnim.timeScale = swimAnimSpeed;
        }
        else
        {
            swimAnim.timeScale = idleAnimSpeed;
        }
    }


	void SpriteFlip()
    {
        if (player_Rigidbody2D.velocity.x > 0 && lookingLeft == true)
        {
            lookingLeft = false;
            transform.localScale = Vector3.Reflect(transform.localScale, Vector3.right);
        }

        if (player_Rigidbody2D.velocity.x < 0 && lookingLeft == false)
        {
            lookingLeft = true;
            transform.localScale = Vector3.Reflect(transform.localScale, Vector3.right);
        }

        
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "wall")
        {
            GameManager.instance.ResetGame();
        }
    }


    void FireHarpoon()
    {
        if (lookingLeft)
        {
            Instantiate(harpoon, harpoon_SpawnPoint.position, Quaternion.Euler(0,0,0));
        }
        else
        {
            Instantiate(harpoon, harpoon_SpawnPoint.position, Quaternion.Euler(0, 180, 0));
        }
        
    }
    
    void GlowstickDrop()
    {
        if (lookingLeft)
        {
            Instantiate(glowstick, glowstick_SpawnPoint.position, Quaternion.Euler(0, 0, 0));
        }

        else
        {
            Instantiate(glowstick, glowstick_SpawnPoint.position, Quaternion.Euler(0, 180, 0));
        }
                
                }
}
