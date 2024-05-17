using UnityEngine;
public class BoatShoot : MonoBehaviour
{
    public float shootCooldown = 3.0f; // Время перезарядки между выстрелами
    public float lastShootTime; // Время последнего выстрела
    [SerializeField]private Transform shootingPoint;
    [SerializeField]private GameObject[] canonBalls;
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
        lastShootTime = Time.time;
        GameObject cannonBall = canonBalls[findCanonBall()];
        cannonBall.transform.position = shootingPoint.position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 shootingDirection = (new Vector3(mousePosition.x, mousePosition.y, 0) - transform.position).normalized;
        cannonBall.GetComponent<project>().setDirection(shootingDirection);
    }


    private int findCanonBall(){
        for (int i = 0; i < canonBalls.Length; i++) {
            if(!canonBalls[i].activeInHierarchy){
                return i;
            }
        }
        return 0;
    }
}
