using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using StarterAssets;

public class CheckpointLoadrer : MonoBehaviour
{
    [SerializeField] AudioSource musicaFeliz;
    [SerializeField] AudioSource musicaTriste;
    private bool activado = false;
    StarterAssetsInputs starterAssetsInputs;

    public string getUrl = "http://127.0.0.1:5000/player/last_position";

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
        if (activado == false)
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
                transform.position = new Vector3(checkpoint.x, checkpoint.y, checkpoint.z);
            Debug.Log($"Checkpoint loaded: ({checkpoint.x}, {checkpoint.y}, {checkpoint.z}) - {checkpoint.mood}");
        }
        else
        {
            Debug.Log("No existen checkpoints cargados ");
            
        }
    }

    [System.Serializable]
    public class PlayerDataWrapper
    {
        public int id;
        public float x;
        public float y;
        public float z;
        public string mood;

        public PlayerData ToPlayerData()
        {
            return new PlayerData { x = x, y = y, z = z, mood = mood };
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public float x;
        public float y;
        public float z;
        public string mood;
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
    public void reproducirMusicaFeliz()
    {
        musicaFeliz.Play();
        musicaTriste.Stop();
    }
    public void reproducirMusicaTriste()
    {
        musicaTriste.Play();
        musicaFeliz.Stop();
    }
}
