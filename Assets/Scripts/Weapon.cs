using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Para manejar el texto de la UI

public class Weapon : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject hitVFX;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] int damageAmount = 1;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource jamSound;

    [Header("Ammo Settings")]
    [SerializeField] int maxAmmo = 20;
    [SerializeField] private TMP_Text balas; 

    private int currentAmmo;

    StarterAssetsInputs starterAssetsInputs;

    const string SHOOT_STRING = "shoot";

    private void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        HandleShoot();
    }

    private void HandleShoot()
    {
        if (!starterAssetsInputs.shoot) return;

        if (currentAmmo <= 0)
        {
            jamSound.Play();
        }
            
        else
        {
            muzzleFlash.Play();
            shootSound.Play();
            animator.Play(SHOOT_STRING, 0, 0f);
            starterAssetsInputs.ShootInput(false);
            currentAmmo--;
            UpdateAmmoUI();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
            {
                Instantiate(hitVFX, hit.point, Quaternion.identity);
                MonsterHealth enemyHealth = hit.collider.GetComponent<MonsterHealth>();
                enemyHealth?.TakeDamage(damageAmount);
            }
        }
            
    }

    private void UpdateAmmoUI()
    {
        if (balas != null)
        {
            balas.text = $"{currentAmmo}/{maxAmmo}";
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        UpdateAmmoUI();
    }
}
