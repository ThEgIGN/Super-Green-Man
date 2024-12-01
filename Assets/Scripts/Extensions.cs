using UnityEngine;

public static class Extensions {

    // Function only checks collision with objects tagged with Default layer
    private static LayerMask layerMask = LayerMask.GetMask("Default");

    public static bool Raycast(this Rigidbody2D rigidBody, SpriteRenderer sprite, bool xCoord, Vector2 direction, float xOffset = 0f, float yOffset = 0f) {
        // Fail-safe if physics engine isn't controlling this body
        if (rigidBody.isKinematic) {
            return false;
        }

        float radius = 0.25f;

        // Depending on size of the sprite, small circle needs to be casted further
        // Since sprite may be bigger or smaller in width or height, xCoord tells which axis should be checked
        float distance = xCoord ? sprite.bounds.size.x / 3 : sprite.bounds.size.y / 2;

        // Cast small circle in given direction to check for collision with other objects
        // Normalized vector maintains its direction, but has a length of 1
        Vector2 position = new Vector2(rigidBody.position.x + xOffset, rigidBody.position.y + yOffset);
        RaycastHit2D hit = Physics2D.CircleCast(position, radius, direction.normalized, distance, layerMask);
        // Return true if we hit something. Small fail-safe if layerMask doesn't work
        return hit.collider != null && hit.rigidbody != rigidBody;
    }

    public static bool DotTest(this Transform transform, Transform other, Vector2 testDirection) {
        // Test where other (Block) is located compared to transform (Player), up/down/left/right
        Vector2 direction = other.position - transform.position;
        // Returns 1 if vectors are pointing in same direction, -1 completely opposite....
        // By putting > 0.25f, we are not being super strict on same direction of pointing
        // Example, Player is jumping sideways into block above them
        return Vector2.Dot(direction.normalized, testDirection) > 0.25f;
    }

}
