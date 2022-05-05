using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class PlayerController : MonoBehaviour
{
    // Myo game object to connect with.
    // This object must have a ThalmicMyo script attached.
    public GameObject myo = null;

    // The pose from the last update. This is used to determine if the pose has changed
    // so that actions are only performed upon making them rather than every frame during
    // which they are active.
    private Pose lastPose = Pose.Unknown;


    public float horizontalSpeed = 2.0f;
    public Laser laserPrefab;
    public float firingRate = 0.5f;
    public System.Action killed;// if player is dead

    private GameObject laserParent;
    private Coroutine firingCoroutine;
    private bool runningCoroutine = false;

    private SpriteRenderer render;
    private Rigidbody2D rb;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        laserParent = GameObject.Find("LaserParent");
        if (!laserParent)
        {
            laserParent = new GameObject("LaserParent");
        }
    }

    void Update()
    {

        // Access the ThalmicMyo component attached to the Myo game object.
        ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo>();

        // Check if the pose has changed since last update.
        // The ThalmicMyo component of a Myo game object has a pose property that is set to the
        // currently detected pose (e.g. Pose.Fist for the user making a fist). If no pose is currently
        // detected, pose will be set to Pose.Rest. If pose detection is unavailable, e.g. because Myo
        // is not on a user's arm, pose will be set to Pose.Unknown.
        if (thalmicMyo.pose != lastPose)
        {
            lastPose = thalmicMyo.pose;

            // Fire laser when fist is made
            // Vibrates the armband
            if (thalmicMyo.pose == Pose.Fist)
            {
                thalmicMyo.Vibrate(VibrationType.Medium);

                //Firing Code here
                firingCoroutine = StartCoroutine(FireCoroutine());

                ExtendUnlockAndNotifyUserAction(thalmicMyo);

            }
            else if (runningCoroutine)
            {
                StopCoroutine(firingCoroutine);
            }


            // hand position idle 
            if (thalmicMyo.pose == Pose.Rest)
            {
                render.color = Color.cyan;
                Vector2 playerVelocity = new Vector2(0, 0);
                rb.velocity = playerVelocity;

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }

            // right movement
            // red cause R
            else if (thalmicMyo.pose == Pose.WaveIn)
            {
                render.color = Color.red;
                Vector2 playerVelocity = new Vector2(horizontalSpeed, rb.velocity.y);
                rb.velocity = playerVelocity;

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }

            // left movement
            // yellow cause L
            else if (thalmicMyo.pose == Pose.WaveOut)
            {
                render.color = Color.yellow;

                Vector2 playerVelocity = new Vector2(-horizontalSpeed, rb.velocity.y);
                rb.velocity = playerVelocity;

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }
        }
    }

    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            runningCoroutine = true;
            FireLaser();
            yield return new WaitForSeconds(firingRate);
            runningCoroutine = false;
        }
    }

    private void FireLaser()
    {
        Laser l = Instantiate(laserPrefab, laserParent.transform);
        l.transform.position = gameObject.transform.position;
    }

    // seems to just be the standard 
    void ExtendUnlockAndNotifyUserAction(ThalmicMyo myo)
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard)
        {
            myo.Unlock(UnlockType.Timed);
        }
        //un comment when done
        //myo.NotifyUserAction();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Laser>())
        {
            this.killed.Invoke();
            //destroy bomb
            Destroy(collision.gameObject);
            //destroy the player
            Destroy(gameObject);

        }
    }

}
