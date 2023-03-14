using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    public bool go = false;
    public bool launch = false;
    public Transform target;
    public float walkDuration = 0.5f;
    public Vector2 force;
    public GameObject fried;
    public float friedLaunchForce;
    public Vector2 fakeForce;
    public float fakeForceWaitDuration = 1.5f;
    public AudioClip walkSound;

    private Coroutine walkRoutine = null;
    private Animator anim;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (go)
        {
            go = false;
            WalkToPlace(target.position);
        }
        if (launch)
        {
            launch = false;
            Launch(force);
        }
    }

    public void WalkToPlace(Vector3 destination)
    {
        if (walkRoutine == null)
        {
            walkRoutine = StartCoroutine(WalkingSequence(destination));
        }
    }

    IEnumerator WalkingSequence(Vector3 destination)
    {
        float t = 0;
        Vector3 startPos = transform.position;
        destination.y = startPos.y;
        anim.SetBool("Walking", true);
        audioSource.clip = walkSound;
        audioSource.Play();
        while (t < walkDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, destination, t / walkDuration);
            yield return new WaitForEndOfFrame();
        }
        transform.position = destination;
        walkRoutine = null;
        audioSource.Stop();
        anim.SetBool("Walking", false);
    }

    private GameManager gm;
    internal void SetManager(GameManager gameManager)
    {
        gm = gameManager;
    }

    public void Launch(Vector3 force)
    {
        anim.SetBool("Flying", true);
        anim.SetTrigger("Kicked");
        rb.AddForce(force, ForceMode2D.Impulse);
        gm.FollowCam(transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Finish")
        {
            return;
        }
        Destroy(gameObject);
        gm.ChickenDied();
        var go = Instantiate(fried, transform.position, Quaternion.identity);
        var friedRB = go.GetComponent<Rigidbody2D>();
        friedRB.AddForce(Vector2.up * friedLaunchForce, ForceMode2D.Impulse);
    }

    internal void FakeLaunch()
    {
        StartCoroutine(FakeLaunchSequence());
    }

    IEnumerator FakeLaunchSequence()
    {
        anim.SetTrigger("Kicked");
        rb.AddForce(fakeForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(fakeForceWaitDuration);
        gm.FinishFakeKick();
    }
}
