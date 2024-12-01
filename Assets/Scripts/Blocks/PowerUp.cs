using UnityEngine;

public class PowerUp : MonoBehaviour {

    public enum Type {
        BronzeCoin,
        SilverCoin,
        GoldCoin,
        ExtraLife,
        ExtraHealth,
        Star,
    }

    public Type type;
    float starPowerDuration = 10f;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            Collect(collider.gameObject);
        }
    }

    private void Collect(GameObject player) {
        switch(type) {
            case Type.BronzeCoin:
                GameManager.Instance.AddCoins(1);
                break;
            case Type.SilverCoin:
                GameManager.Instance.AddCoins(3);
                break;
            case Type.GoldCoin:
                GameManager.Instance.AddCoins(5);
                break;
            case Type.ExtraHealth:
                AudioManager.Instance.PlaySFX("PlayerPickUp");
                player.GetComponent<Player>().Heal(50);
                break;
            case Type.ExtraLife:
                AudioManager.Instance.PlaySFX("PlayerPickUp");
                GameManager.Instance.AddLife(1);
                break;
            case Type.Star:
                AudioManager.Instance.PlaySFX("PlayerPickUp");
                player.GetComponent<Player>().StarPower(starPowerDuration);
                break;
        }

        // Remove object once Player collects it
        gameObject.SetActive(false);
    }

}
