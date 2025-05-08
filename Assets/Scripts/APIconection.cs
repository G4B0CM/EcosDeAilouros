using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIconection : MonoBehaviour
{
    public string apiUrl = "http://127.0.0.1:5000/player/position"; // Cambia si corres en servidor
    public string moodApiUrl = "http://127.0.0.1:5000/player/last_mood";
    public string mood = "happy";

    CheckpointLoadrer ck;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Vector3 pos = transform.position;
            ck = other.gameObject.GetComponentInChildren<CheckpointLoadrer>();
            StartCoroutine(SendPosition(pos.x, pos.y, pos.z, mood));
            if(mood == "happy")
            {
                ck.reproducirMusicaFeliz();
            }
            else if (mood == "sad")
            {
                ck.reproducirMusicaTriste();
            }
        }
    }

    IEnumerator SendPosition(float x, float y, float z, string mood)
    {
        PlayerData data = new PlayerData { x = x, y = y, z = z, mood = mood };
        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data sent successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending data: " + request.error);
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

    
}
