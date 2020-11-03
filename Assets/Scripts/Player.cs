using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Forward Motion")]
    public float forwardBaseVelocity = 30.0f;
    public float forwardVelocityMax = 120.0f;
    public float forwardVelocityMin = 15.0f;
    public float forwardAccel = 50.0f;
    public float forwardAccelDuration = 1.0f;
    public float forwardDeaccel = 40.0f;
    public float forwardDeaccelDuration = 0.5f;
    //public float forwardAccelDuration = 1.0f;
    //public float forwardAccelCooldownDuration = 0.5f;
    //public float speedUpBurst = 5.0f;
    //public float slowDownBurst = 5.0f;

    [Header("Side Motion")]
    public float sideStep = 2.5f;
    public float sideVelocity = 0.5f;
    public float sideMax = 5.0f;


    // TODO remove
    [Header("Legacy - Side Motion")]
    public float xVelocity = 15.0f;
    public float xMin = -5.0f;
    public float xMax = 5.0f;
    public float xStep = 2.5f;

    // Sideways motion fields
    private float sideTarget = 0.0f;
    private float sideTimer = 0.0f;

    // Acceleration fields
    private float _accelTime = 0.0f;
    private float _deaccelTime = 0.0f;
    private float _currentForwardVelocity;
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentForwardVelocity = forwardBaseVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO change controls so the arrows are treating as swipes on a phone
        // One swipe triggers motion in that direction
        // For forward motion, each swipe adds to the acceleration,
        // so multiple swipes in a row create a much faster acceleration.
        // Each accel/deaccel motion should result in a set new velocity
        // the accel and deaccel values should be equal. A swipe up and a
        // corresponding swipe down should take you back to the original
        // velocity.

        // TODO modify the camera controls so that the motion is more fluid when
        // the vehicle moves


        Vector3 velocity = new Vector3(0, _rigidbody.velocity.y, 0);

        // Check for lateral motion
        if (Input.GetKeyDown("left"))
        {
            sideTarget = Mathf.Clamp(sideTarget - sideStep, -sideMax, sideMax);
            if (transform.position.x >= xMin)
            {
                velocity.x -= xVelocity;
            }
        }

        if (Input.GetKeyDown("right"))
        {
            if (transform.position.x <= xMax)
            {
                velocity.x += xVelocity;
            }
        }

        // Check for acceleration motion
        if (Input.GetKeyDown("up"))
        {
            _accelTime = forwardAccelDuration;
        }

        if (Input.GetKeyDown("down"))
        {
            _deaccelTime = forwardDeaccelDuration;
        }

        // Add any acceleration
        if (_accelTime > 0.0f)
        {
            _currentForwardVelocity = Mathf.Clamp(
                _currentForwardVelocity + forwardAccel * Time.deltaTime,
                forwardVelocityMin,
                forwardVelocityMax
                );
            _accelTime -= Time.deltaTime;
        }

        // Perform any deacceleration
        if (_deaccelTime > 0.0f)
        {
            _currentForwardVelocity = Mathf.Clamp(
                _currentForwardVelocity - forwardDeaccel * Time.deltaTime,
                forwardVelocityMin,
                forwardVelocityMax
                );
            _deaccelTime -= Time.deltaTime;
        }

        velocity.z = _currentForwardVelocity;
        _rigidbody.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        NavSegment navSeg;
        if (null != (navSeg = other.gameObject.GetComponent<NavSegment>()))
        {
            SceneController controller = GameObject.FindObjectOfType<SceneController>();
            controller?.NavSegmentExited(other.gameObject.GetComponent<NavSegment>());
        }
    }
}
