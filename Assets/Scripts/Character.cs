using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour {
    [SerializeField] Transform cameraTransform;
    [SerializeField] ParticleSystem ps;
    [SerializeField] Animator animator;
    [SerializeField] Transform viewTransform;
    [SerializeField] float speed;
    [SerializeField] float sprintSpeed;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float jumpTime = 0.33f;
    [SerializeField] float fallAcceleration = 10f;
    [SerializeField] float energyConsumptionSpeed = 0.01f;
    [SerializeField] float lightIntencity = 3f;
    [SerializeField] Light light;
    [SerializeField] Image progLeft;
    [SerializeField] Image progRight;
    [SerializeField] Image vignette;
    [SerializeField] GameObject battery;
    [SerializeField] GameObject deathFx;

    float energyLevel = 1f;
    float jumpProgressTime = 0f;
    float jumpStartY = 0f;
    bool isJumping;
    bool isSprinting;
    bool isMovingThisFrame;
    bool isGrounded;
    Vector3 moveDir;

    public void RestoreEnergy(){
        energyLevel = 1f;
    }

    float energyDelta;
    void ConsumeEnergy() {
        energyDelta = energyConsumptionSpeed*Time.deltaTime;
        if(energyLevel<energyDelta){
            Kill(true,3f);
        }else{
            energyLevel-=energyDelta;
            progLeft.fillAmount = energyLevel;
            progRight.fillAmount = energyLevel;
            vignette.color = new Color(0f,0f,0f,Mathf.Lerp(0.8f, 0f, energyLevel));
        }
    }

    float vert;
    float horiz;
    float jumpAxis;
    float sprint;

    public void Update() {
        if (!isDead) {
            isMovingThisFrame = false;
            moveDir = Vector3.zero;
            horiz = Input.GetAxis("Horizontal");
            vert = Input.GetAxis("Vertical");
            jumpAxis = Input.GetAxis("Jump");
            sprint = Input.GetAxis("Sprint");

            if(Mathf.Abs(horiz)>0.001f || Mathf.Abs(vert)>0.001f){
                isMovingThisFrame = true;
                moveDir += new Vector3(horiz,0f,vert);
            }

            if (jumpAxis>0.001f) {
                Jump();
            }

            if (isJumping) {
                JumpUpdate();
            } else {
                CheckGround();
            }

            isSprinting = sprint>0f;

            if (isMovingThisFrame) {
                Move(moveDir);
            } else {
                animator.SetBool("isMoving", false);
            }
            RotateUpdate();
            CliffJumpUpdate();
            ConsumeEnergy();
            ps.emissionRate = (isMovingThisFrame && isGrounded)? 20f: 0f;
        }
    }

    bool isDead;

    public void Kill(bool lowEnergy = false, float waitTime = 2f) {
        if (!isDead) {
            isDead = true;
            AudioController.Play("Death");
            viewTransform.gameObject.SetActive(false);
            if(lowEnergy){
                battery.SetActive(true);
            }
            deathFx.SetActive(true);
            StartCoroutine(DeathCoroutine(waitTime));
        }

    }

    IEnumerator DeathCoroutine(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        battery.SetActive(false);
        deathFx.SetActive(false);
        LevelLoader.Instance.ReSpawnHero();
        viewTransform.gameObject.SetActive(true);
        isDead = false;
    }

    Vector3 targetLookVector;
    Vector3 currentLookVector;
    [SerializeField] float rotatelookSpeed;
    public void Move(Vector3 move) {
        animator.SetBool("isMoving", true);
        move = cameraTransform.localRotation * move;
        targetLookVector = move;
        transform.position += move * (isSprinting? sprintSpeed:speed) * Time.deltaTime;
    }

    public void RotateUpdate() {
        currentLookVector = Vector3.RotateTowards(currentLookVector, targetLookVector, rotatelookSpeed * Time.deltaTime, 1f);
        viewTransform.localRotation = Quaternion.LookRotation(currentLookVector); //slowly rotate to that vector;
    }

    public void Jump() {
        if (isGrounded || (!isJumping && cliffJumpCountdown > 0)) {
            AudioController.Play("Jump");
            isGrounded = false;
            isJumping = true;
            jumpProgressTime = 0f;
            jumpStartY = transform.localPosition.y;
            animator.SetTrigger("JumpStart");
        }
    }

    void CliffJumpUpdate() {
        if (cliffJumpCountdown > 0) {
            cliffJumpCountdown -= Time.deltaTime;
        }
    }

    void JumpUpdate() {
        if (isJumping) {
            jumpProgressTime += Time.deltaTime;
            transform.localPosition = new Vector3(transform.localPosition.x, jumpStartY + jumpHeight * jumpCurve.Evaluate(Mathf.Clamp01(jumpProgressTime / jumpTime)), transform.localPosition.z);
            if (jumpProgressTime >= jumpTime) {
                isJumping = false;
                fallVelocity = 0f;
            }
        }
    }

    RaycastHit hitData;
    float fallVelocity = 0f;
    [SerializeField] float cliffJumpTime = 0.1f;
    float cliffJumpCountdown;

    public void CheckGround() {
        if (Physics.Raycast(transform.position, Vector3.down, out hitData)) {
            if (hitData.distance > (1f + fallVelocity * Time.deltaTime)) {
                if (isGrounded) {
                    //begin fall
                    cliffJumpCountdown = cliffJumpTime;
                    isGrounded = false;
                    fallVelocity = 0f;
                }
                fallVelocity += fallAcceleration * Time.deltaTime;
                transform.localPosition += Vector3.down * fallVelocity * Time.deltaTime;
            } else {
                if(!isGrounded){
                    transform.localPosition = new Vector3(transform.localPosition.x, hitData.point.y + 1f, transform.localPosition.z);
                    AudioController.Play("Landing");
                    animator.SetTrigger("JumpEnd");
                    isGrounded = true;
                }
            }
        } else {
            Debug.Log("No hit");
        }
    }


}
