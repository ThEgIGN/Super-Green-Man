using UnityEngine;

public class DeathBarrier : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collider) {
        // By using interface Hittable, we can check for entities that have method Hit()
        // and call it with an insane damage. If they don't, we just manually disable them
        if (collider.gameObject.TryGetComponent<Hittable>(out Hittable hittable)) {
            hittable.Hit(100000);
            if (!collider.CompareTag("Player")) {
                AudioManager.Instance.PlaySFX("EnemyDeath", 0.6f);
            }
        } else {
            collider.gameObject.SetActive(false);
        }
    }

}
