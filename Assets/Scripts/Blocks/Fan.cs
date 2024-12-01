using UnityEngine;

public class Fan : MonoBehaviour {

    public float fanPower = 10f;
    public float fanDistance = 10f;
    public float timerDuration = 5f;
    public bool startActive = true;

    private PlayerMovement playerMovement;
    private GameObject playerObject;
    private SpriteRenderer airBlowingSprite;
    public GameObject airBlowingObject;
    public GameObject fanObject;

    private float timer;
    private bool active;
    private float fanX, fanY;
    private float animationRotation = 15f;

    public string direction;
    private float offset, distance;
    private bool applyXForce;
    private int forceMultiplier;

    private void Awake() {
        active = startActive;
        timer = startActive ? timerDuration : -1f;
        fanX = transform.position.x;
        fanY = transform.position.y;

        playerObject = GameObject.FindWithTag("Player");
        playerMovement = playerObject.GetComponent<PlayerMovement>();

        airBlowingSprite = airBlowingObject.GetComponent<SpriteRenderer>();
        SetAirBlowingDirection();
        airBlowingSprite.enabled = active;

        SetForces();
    }

    private void Update() {
        timer -= Time.deltaTime;
        if (timer < 0f) {
            timer = timerDuration;
            active = !active;
            airBlowingSprite.enabled = active;
        }
        if (active) {
            CalculateDistances();
            PushPlayer();
            AnimateFan();
        }
    }

    // Rotates blades of fan on Z axis to make it look like its spinning
    private void AnimateFan() {
        if (Time.frameCount % 4 == 0) {
            fanObject.transform.eulerAngles = new Vector3(0f, 0f, animationRotation);
            animationRotation += 15f;
        }
        if (animationRotation == 360f) {
            animationRotation = 0f;
        }
    }

    private void SetAirBlowingDirection() {
        // Depenindg on direction of Fan, Air sprite needs to be adjusted accordingly
        switch (direction) {
            case "up":
                airBlowingObject.transform.position = new Vector3(fanX, fanY + 1f, 0f);
                airBlowingObject.transform.eulerAngles = new Vector3(0f, 0f, 90f);
                airBlowingSprite.flipX = false;
                break;
            case "down":
                airBlowingObject.transform.position = new Vector3(fanX, fanY - 1f, 0f);
                airBlowingObject.transform.eulerAngles = new Vector3(0f, 0f, 90f);
                break;
            case "right":
                airBlowingObject.transform.position = new Vector3(fanX + 1f, fanY, 0f);
                airBlowingSprite.flipX = false;
                break;
        }
    } 

    private void CalculateDistances() {
        switch (direction) {
            case "up":
                offset = Mathf.Abs(playerObject.transform.position.x - fanX) + 0.1f;
                distance = playerObject.transform.position.y - fanY;
                break;
            case "down":
                offset = Mathf.Abs(playerObject.transform.position.x - fanX) + 0.1f;
                distance = fanY - playerObject.transform.position.y;
                break;
            case "right":
                offset = Mathf.Abs(playerObject.transform.position.y - fanY) + 0.1f;
                distance = playerObject.transform.position.x - fanX;
                break;
            default:
                offset = Mathf.Abs(playerObject.transform.position.y - fanY) + 0.1f;
                distance = fanX - playerObject.transform.position.x;
                break;
        }
    }

    // This is in separate function because it should only be set once
    private void SetForces() {
        switch (direction) {
            case "up":
                applyXForce = false;
                forceMultiplier = 1;
                break;
            case "down":
                applyXForce = false;
                forceMultiplier = -1;
                break;
            case "right":
                applyXForce = true;
                forceMultiplier = 1;
                break;
            default:
                applyXForce = true;
                forceMultiplier = -1;
                break;
        }
    }

    // If Player is close enough to fan, apply correct force
    private void PushPlayer() {
        if (offset > 0f && offset < 0.9f && distance > 0f && distance < fanDistance) {
            float appliedForce = Mathf.Max(fanPower / (1f + distance * distance) * 2f, 0f);
            if (applyXForce) {
                playerMovement.ApplyForceX(appliedForce * forceMultiplier);
            } else {
                playerMovement.ApplyForceY(appliedForce * forceMultiplier);
            }
        }
    }

}
