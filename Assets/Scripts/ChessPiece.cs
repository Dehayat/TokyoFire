using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public float followDuration;
    public GameObject[] chessSprites;

    private Vector3 target;
    private bool isFollowing = false;
    private float t = 0;
    private float speedMul = 1f;

    public void SetPiece(int score)
    {
        score--;
        chessSprites[score].SetActive(true);
    }

    public void Follow(Vector3 target, float mul = 1f)
    {
        this.target = target;
        t = 0;
        isFollowing = true;
        speedMul = mul;
    }

    private void FixedUpdate()
    {
        if (isFollowing)
        {
            t += Time.deltaTime * speedMul;
            var position = Vector3.Lerp(transform.position, target, t / followDuration);
            transform.position = position;
            if (t >= 1)
            {
                isFollowing = false;
            }
        }
    }
}
