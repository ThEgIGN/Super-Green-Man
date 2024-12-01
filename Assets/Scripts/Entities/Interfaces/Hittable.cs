// Interface for all Entities that can be hit. It has damage built-in for Player and
// entities that will take damage. Rest of the entites just ignore the damage parameter
public interface Hittable {

    void Hit(int damage = 500);

}
