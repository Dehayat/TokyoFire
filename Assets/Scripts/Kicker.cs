using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class State
{

    protected Kicker self;

    public void Init(Kicker kicker)
    {
        self = kicker;
    }

    public virtual void Enter()
    {

    }

    public virtual State Update()
    {
        return null;
    }
}

public class PreKickState : State
{

    private float currentAngle;
    private int dir;
    private float t = 0;

    public override void Enter()
    {
        currentAngle = self.legJoint.rotation.eulerAngles.z;
        if (currentAngle > 200)
        {
            currentAngle -= 360;
        }
        dir = -1;
        self.GetComponent<Animator>().SetBool("Sad", true);
        self.playSadSound();
    }

    public override State Update()
    {
        if (t < 0.8f)
        {
            t += Time.deltaTime;
            return null;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            return new KickState();
        }

        currentAngle += self.readyAnglesPerSecond * dir * Time.deltaTime;
        var rotation = self.legJoint.rotation.eulerAngles;
        rotation.z = currentAngle;
        self.legJoint.eulerAngles = rotation;
        if (currentAngle > self.maxKickAngle)
        {
            dir = -1;
        }
        if (currentAngle < self.minKickAngle)
        {
            dir = +1;
        }
        return null;
    }
}
public class KickState : State
{

    private float currentAngle;
    private float anglePower;

    public override void Enter()
    {
        anglePower = self.legJoint.rotation.eulerAngles.z;
        if (anglePower > 200)
        {
            anglePower -= 360;
        }
        anglePower += 70;
        anglePower /= 120;
        anglePower = 1 - anglePower;
        currentAngle = self.legJoint.rotation.eulerAngles.z;
        if (currentAngle > 200)
        {
            currentAngle -= 360;
        }
    }

    public override State Update()
    {
        if (anglePower < self.minRealKickPower)
        {
            return new FakeKickState();
        }
        currentAngle += self.kickAnglesPerSecond * Time.deltaTime;
        var rotation = self.legJoint.rotation.eulerAngles;
        rotation.z = currentAngle;
        self.legJoint.eulerAngles = rotation;
        if (currentAngle >= self.targetKickAngle)
        {
            self.chicken.Launch(self.chicken.force * anglePower);
            self.playKickSound();
            return new PostKickState();
        }
        return null;
    }
}
public class FakeKickState : State
{

    private float currentAngle;

    public override void Enter()
    {
        currentAngle = self.legJoint.rotation.eulerAngles.z;
        if (currentAngle > 200)
        {
            currentAngle -= 360;
        }
    }

    public override State Update()
    {
        currentAngle += self.kickAnglesPerSecond * Time.deltaTime;
        var rotation = self.legJoint.rotation.eulerAngles;
        rotation.z = currentAngle;
        self.legJoint.eulerAngles = rotation;
        if (currentAngle >= self.targetKickAngle)
        {
            self.playKickSound();
            self.chicken.FakeLaunch();
            return new PostKickState();
        }
        return null;
    }
}
public class PostKickState : State
{

    private float currentAngle;

    public override void Enter()
    {
        currentAngle = self.legJoint.rotation.eulerAngles.z;
        if (currentAngle > 200)
        {
            currentAngle -= 360;
        }
    }

    public override State Update()
    {
        currentAngle -= self.kickAnglesPerSecond * Time.deltaTime;
        var rotation = self.legJoint.rotation.eulerAngles;
        rotation.z = currentAngle;
        self.legJoint.eulerAngles = rotation;
        if (currentAngle <= self.targetIdleAngle)
        {
            return new IdleState();
        }
        return null;
    }
}
public class IdleState : State
{

    private float t;

    public override void Enter()
    {
        t = 0;
        self.GetComponent<Animator>().SetBool("Sad", false);
    }

    public override State Update()
    {
        t += Time.deltaTime;
        if (t > self.idleWaitTime)
        {
            //return new PreKickState();
        }
        return null;
    }
}



public class Kicker : MonoBehaviour
{
    public Transform legJoint;
    public Chicken chicken;
    public float minKickAngle = -65;
    public float maxKickAngle = 45;
    public float targetKickAngle = 40;
    public float readyAnglesPerSecond = 3;
    public float kickAnglesPerSecond = 3;
    public float returnAnglesPerSecond = 3;
    public float targetIdleAngle = 0;
    public float idleWaitTime = 2;
    public float minRealKickPower = 0.25f;
    public AudioClip sadClip;
    public AudioClip happyClip;
    public AudioClip kickClip;

    private State kickState;
    private AudioSource audioSource;

    public void playSadSound()
    {
        audioSource.PlayOneShot(sadClip);
    }
    public void playHappySound()
    {
        audioSource.PlayOneShot(happyClip);
    }
    public void playKickSound()
    {
        audioSource.PlayOneShot(kickClip);
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GoToState(new IdleState());
    }

    private void Update()
    {
        var state = kickState.Update();
        if (state != null)
        {
            GoToState(state);
        }
    }

    internal void GoToState(State state)
    {
        kickState = state;
        kickState.Init(this);
        kickState.Enter();
    }

    internal void SetChicken(Chicken chicken)
    {
        this.chicken = chicken;
    }

    internal void AllowKick()
    {
        GoToState(new PreKickState());
    }

    internal void SetSmile(bool smile)
    {
        GetComponent<Animator>().SetBool("Happy", smile);
        if (smile)
        {
            playHappySound();
        }
    }
}
