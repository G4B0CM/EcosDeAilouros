using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject hitVFX;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] int damageAmount = 1;
    [SerializeField] private AudioSource shootSound;

    StarterAssetsInputs starterAssetsInputs;
    

    const string SHOOT_STRING = "shoot";


    // Update is called once per frame
    private void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        

    }

    void Update()
    {
        HandleShoot();

    }

    private void HandleShoot()
    {
        if (!starterAssetsInputs.shoot) return;
        muzzleFlash.Play();
        shootSound.Play();
        animator.Play(SHOOT_STRING, 0, 0f);
        starterAssetsInputs.ShootInput(false);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            
            Instantiate(hitVFX, hit.point, Quaternion.identity);
            MonsterHealth enemyHealth = hit.collider.GetComponent<MonsterHealth>();
            enemyHealth?.TakeDamage(damageAmount);
        }
    }
}
