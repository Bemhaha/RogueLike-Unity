using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float moveSpeed;
    private Vector2 moveInput;

    public Rigidbody2D theRB;

    public Transform gunArm;

    private Camera TheCam;

    public Animator anim;

    public GameObject bulletToFire;
    public Transform firePoint;

    public float timeBetweenShots;
    private float shotCounter;

    public SpriteRenderer bodySR;

    private float activeMoveSpeed;
    public float dashSpeed = 10f, dashLength = 0.5f, dashCoolDown = 1f, dashInvincibility = 0.5f;
    private float dashCoolCounter;
    public float dashCounter;

    public bool canMove = true;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        TheCam = Camera.main;

        activeMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !LevelManager.instance.isPaused)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            moveInput.Normalize();

            //transform.position += new Vector3(moveInput.x * Time.deltaTime * moveSpeed, moveInput.y * Time.deltaTime * moveSpeed, 0f);
            theRB.velocity = moveInput * activeMoveSpeed;

            Vector3 mousePos = Input.mousePosition;
            //Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
            Vector3 screenPoint = TheCam.WorldToScreenPoint(transform.localPosition);

            if (mousePos.x < screenPoint.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                gunArm.localScale = new Vector3(-0.4f, -0.4f, 0.4f);
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                gunArm.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }

            //rotate hair dryer xD
            Vector2 offset = new Vector2(mousePos.x - screenPoint.x, mousePos.y - screenPoint.y);
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            gunArm.rotation = Quaternion.Euler(0, 0, angle);

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(bulletToFire, firePoint.position, firePoint.rotation);
                shotCounter = timeBetweenShots;
            }

            if (Input.GetMouseButton(0))
            {
                shotCounter -= Time.deltaTime;

                if (shotCounter <= 0)
                {
                    Instantiate(bulletToFire, firePoint.position, firePoint.rotation);

                    AudioManager.instance.PlaySFX(12);

                    shotCounter = timeBetweenShots;
                }
            }

            //Dash System
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (dashCoolCounter <= 0 && dashCounter <= 0)
                {
                    activeMoveSpeed = dashSpeed;
                    dashCounter = dashLength;

                    AudioManager.instance.PlaySFX(8);

                    anim.SetTrigger("dash");

                    PlayerHealthController.instance.MakeInvincible(dashInvincibility);
                }
            }

            if (dashCounter > 0)
            {
                dashCounter -= Time.deltaTime;
                if (dashCounter <= 0)
                {
                    activeMoveSpeed = moveSpeed;
                    dashCoolCounter = dashCoolDown;
                }
            }


            if (dashCoolCounter > 0)
            {
                dashCoolCounter -= Time.deltaTime;
            }
            //end Dash System

            if (moveInput != Vector2.zero)
            {
                anim.SetBool("isMoving", true);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }

        }
        else
        {
            theRB.velocity = Vector2.zero;
            anim.SetBool("isMoving", false);
        }

    }
}
