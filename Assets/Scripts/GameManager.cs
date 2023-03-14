using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Chicken chickenPrefab;
    public Transform[] chickenPositions;
    public Transform chickenTarget;
    public Kicker kicker;
    public SmoothFollowCam cam;
    public float watchFriedWaitTime = 2f;
    public GameObject chessPiecePrefab;
    public Transform chessTarget;
    public Transform chessTarget2;
    public float nextChickenWaitTime = 0.8f;

    public static GameManager instance;

    private Queue<Chicken> chickenQueue = new Queue<Chicken>();

    private Queue<int> scoreList = new Queue<int>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartSequence());
    }

    private bool waiting = false;
    [HideInInspector]
    public int currentScore;

    IEnumerator StartSequence()
    {
        GenerateChickens();
        yield return new WaitForSeconds(2f);
        waiting = true;
        BlackScreen.instance.done += FadeDone;
        BlackScreen.instance.FadeFromBlack();
        yield return new WaitWhile(() => waiting);
        yield return new WaitForSeconds(1f);
        ReadyChicken();
    }

    private void FadeDone()
    {
        waiting = false;
    }

    private void ReadyChicken()
    {
        StartCoroutine(ReadyChickenSequence());
    }

    IEnumerator ReadyChickenSequence()
    {
        chickenQueue.Peek().WalkToPlace(chickenTarget.position);
        kicker.SetChicken(chickenQueue.Peek());
        chickenQueue.Dequeue();
        yield return new WaitForSecondsRealtime(1.3f);
        kicker.AllowKick();
    }

    public void FinishFakeKick()
    {
        kicker.AllowKick();
    }

    private void GenerateChickens()
    {
        foreach (var pos in chickenPositions)
        {
            var chickenGO = Instantiate(chickenPrefab, pos.position, Quaternion.identity);
            var chicken = chickenGO.GetComponent<Chicken>();
            chickenQueue.Enqueue(chicken);
            chicken.SetManager(this);
        }
    }

    internal void FollowCam(Transform target)
    {
        cam.Follow(target);
    }

    internal void ChickenDied()
    {
        StartCoroutine(NextChickenSequence());
    }

    IEnumerator NextChickenSequence()
    {
        scoreList.Enqueue(currentScore);
        cam.StopFollowing();
        yield return new WaitForSeconds(watchFriedWaitTime);
        var chessPos = cam.transform.position;
        chessPos.z = 0;
        var chessPiece = Instantiate(chessPiecePrefab, chessPos, Quaternion.identity).GetComponent<ChessPiece>();
        chessPiece.SetPiece(currentScore);
        yield return new WaitForSeconds(0.7f);
        chessPiece.Follow(chessTarget.position);
        cam.ReturnToOrigin();
        yield return new WaitForSeconds(chessPiece.followDuration + 0.1f);
        kicker.SetSmile(true);
        cam.StopFollowing();
        chessPiece.Follow(chessTarget2.position, 3);
        chessPiece.GetComponent<Animator>().SetBool("Away", true);
        yield return new WaitForSeconds(1f);
        Destroy(chessPiece.gameObject);
        kicker.SetSmile(false);
        if (chickenQueue.Count > 0)
        {
            yield return new WaitForSeconds(nextChickenWaitTime);
            yield return StartCoroutine(ReadyChickenSequence());
        }
    }
}
