using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using System.Collections;

public class CheckpointSaver : MonoBehaviour
{
    [SerializeField] private TMP_Text ayuda;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private AudioSource checkSound;
    [SerializeField] private string saveCheckpointUrl = "http://127.0.0.1:5000/player/save_position";

    private bool isPlayerInRange = false;
    private Transform playerTransform;
    private PlayerInput playerInput;
    private InputAction interactAction;
    private PlayerHealth playerHealth;
    private Weapon playerWeapon;

    private void Start()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            interactAction = playerInput.actions["Interact"];
            playerTransform = playerInput.transform;
            playerHealth = playerTransform.GetComponent<PlayerHealth>();
            playerWeapon = playerTransform.GetComponentInChildren<Weapon>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;
            ayuda.text = "Presiona 'E' para guardar checkpoint";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            playerTransform = null;
            ayuda.text = "";
        }
    }

    private void Update()
    {
        if (isPlayerInRange && interactAction != null && interactAction.WasPressedThisFrame())
        {
            StartCoroutine(SaveCheckpoint());
            ayuda.text = "";
        }
    }

    private IEnumerator SaveCheckpoint()
    {
        if (playerTransform == null || playerHealth == null || playerWeapon == null) yield break;

        Vector3 pos = playerTransform.position;

        string jsonBody = JsonUtility.ToJson(new PlayerData()
        {
            x = pos.x,
            y = pos.y,
            z = pos.z,
            vida = playerHealth.currentHealth,
            municion = playerWeapon.currentAmmo
        });

        UnityWebRequest request = new UnityWebRequest(saveCheckpointUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Checkpoint guardado");
            checkSound?.Play();
        }
        else
        {
            Debug.LogError("Error al guardar checkpoint: " + request.error);
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public float x;
        public float y;
        public float z;
        public int vida;
        public int municion;
    }

}
