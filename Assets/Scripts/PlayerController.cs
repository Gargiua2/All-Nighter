using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float gravity = 4;
    [SerializeField] float accelerationTime = .2f;
    Vector3 forward, right;

    [SerializeField] float tweenAnimationHeight = 1.1f;
    [SerializeField] float tweenCycleLength = .2f;
    [SerializeField] Ease tweenEasing;
    CharacterController controller;
    float velocity;
    float speed = 0;

    SpriteRenderer spriteRenderer;

    Sequence s = null;
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        controller.Move(new Vector3(0, -50, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.paused)
            return;

        if(Input.anyKey)
        {
            Move();
        } else if(Input.GetAxis("HorizontalKey") == 0 && Input.GetAxis("VerticalKey") == 0)
        {
            speed = 0;
            velocity = 0;
        }
    }

    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        if (Input.GetAxis("HorizontalKey") < 0)
        {
            spriteRenderer.flipX = true;
        } else if(Input.GetAxis("HorizontalKey") > 0)
        {
            spriteRenderer.flipX = false;
        }

        controller.transform.position += rightMovement;
        controller.transform.position += upMovement;

        speed = Mathf.SmoothDamp(speed, moveSpeed, ref velocity, accelerationTime);

        controller.Move(Vector3.ClampMagnitude(rightMovement + upMovement, speed) + Vector3.down * gravity);


        if(Mathf.Abs(speed) > .001)
            WalkTween();
    }

    void WalkTween()
    {
        if(s == null || !s.active)
        {
            s.Kill();
            s = DOTween.Sequence();
            s.Append(transform.GetChild(0).DOScaleY(tweenAnimationHeight, tweenCycleLength / 2).SetEase(tweenEasing))
            .Join(transform.GetChild(0).DOLocalMoveY(.5f, tweenCycleLength/2))
            .Append(transform.GetChild(0).DOScaleY(1, tweenCycleLength / 2).SetEase(tweenEasing))
            .Join(transform.GetChild(0).DOLocalMoveY(0, tweenCycleLength / 2));
        }   
    }

    
}
