using UnityEngine;
public class BoatShoot : MonoBehaviour
{
    public float shootCooldown = 3.0f; // Время перезарядки между выстрелами
    public float lastShootTime; // Время последнего выстрела
    
    private Animator animator;
    private BoatMovement boatMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boatMovement = GetComponent<BoatMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && CanShoot()){Shoot();}
    }

    public void Shooting()
    {
        return;
    }
    public bool CanShoot(){return Time.time - lastShootTime >= shootCooldown;}
    public void Shoot()
    {
        return ;
    }
}
