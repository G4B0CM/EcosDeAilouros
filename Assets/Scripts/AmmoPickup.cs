using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; // Importante para el nuevo sistema

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 5;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private TMP_Text ayuda;

    private bool isPlayerInRange = false;
    private Weapon playerWeapon;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>(); 
        if (playerInput != null)
        {
            interactAction = playerInput.actions["Interact"]; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            ayuda.text = "Presiona 'E' para recoger munición";
            isPlayerInRange = true;
            playerWeapon = other.GetComponentInChildren<Weapon>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            ayuda.text = "";
            isPlayerInRange = false;
            playerWeapon = null;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && interactAction != null && interactAction.WasPressedThisFrame())
        {
            if (playerWeapon != null)
            {
                playerWeapon.AddAmmo(ammoAmount);
                ayuda.text = "";
                Destroy(gameObject);
            }
        }
    }
}
