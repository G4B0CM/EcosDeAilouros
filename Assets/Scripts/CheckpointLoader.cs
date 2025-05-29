using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using StarterAssets;

public class CheckpointLoader : MonoBehaviour
{
    private bool activado = false;
    StarterAssetsInputs starterAssetsInputs;

    [SerializeField] private string getUrl = "http://127.0.0.1:5000/player/last_position";
    [SerializeField] private Weapon weapon; // Asigna el arma del jugador
    [SerializeField] private PlayerHealth healthSystem; // Asigna el componente de salud del jugador

    private void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
    }

    void Start()
    {
        StartCoroutine(GetLastCheckpoint());
    }

    private void Update()
    {
        if (!activado)
            clearCheckpoints();
    }

    IEnumerator GetLastCheckpoint()
    {
        UnityWebRequest request = UnityWebRequest.Get(getUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            PlayerData checkpoint = JsonUtility.FromJson<PlayerDataWrapper>(json).ToPlayerData();

            // Mover al jugador a la posición guardada
            transform.position = new Vector3(checkpoint.x, checkpoint.y, checkpoint.z);
            Debug.Log($"Checkpoint loaded: ({checkpoint.x}, {checkpoint.y}, {checkpoint.z})");

            // Restaurar munición
            if (weapon != null)
            {
                weapon.currentAmmo = checkpoint.municion;
                Debug.Log("Munición restaurada: " + checkpoint.municion);
            }

            // Restaurar vida
            if (healthSystem != null)
            {
                healthSystem.currentHealth = checkpoint.vida;
                Debug.Log("Vida restaurada: " + checkpoint.vida);
            }
        }
        else
        {
            Debug.LogWarning("No existen checkpoints cargados");
        }
    }

    [System.Serializable]
    public class PlayerDataWrapper
    {
        public int id;
        public float x;
        public float y;
        public float z;
        public int vida;
        public int municion;

        public PlayerData ToPlayerData()
        {
            return new PlayerData
            {
                x = x,
                y = y,
                z = z,
                vida = vida,
                municion = municion
            };
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

    IEnumerator DeleteCheckpoints()
    {
        string url = "http://127.0.0.1:5000/player/clear_checkpoints?secret=1234";
        UnityWebRequest request = UnityWebRequest.Delete(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Se borraron los checkpoints: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error al borrar checkpoints: " + request.error);
        }
    }

    private void clearCheckpoints()
    {
        if (starterAssetsInputs.ClearCheckpoints)
        {
            Debug.Log("Eliminando Checkpoints");
            StartCoroutine(DeleteCheckpoints());
            Debug.Log("Checkpoints Eliminados");
            activado = true;
        }
    }
}
