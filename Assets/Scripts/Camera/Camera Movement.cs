using UnityEngine;

public class CameraMovement : MonoBehaviour {

    private Transform player;
    private Transform mainCamera;

    public float xMin, xMax, yMin, yMax;

    private void Awake() {
        player = GameObject.FindWithTag("Player").transform;
        mainCamera = Camera.main.transform;
    }

    // Good idea to use LateUpdate when working with camera, as it's called
    // after all the updates and all physics
    private void LateUpdate() {
        // Follows player around, if player gets close to predefined boundaries,
        // it clamps to them and stops moving camera until player moves away
        float x = Mathf.Clamp(player.position.x, xMin, xMax);
        float y = Mathf.Clamp(player.position.y, yMin, yMax);
        mainCamera.position = new Vector3(x, y, mainCamera.position.z);
    }
}
