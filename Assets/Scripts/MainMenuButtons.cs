using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class MainMenuButtons : MonoBehaviour
{
    private string clearCheckpointUrl = "http://127.0.0.1:5000/player/clear_checkpoints?secret=1234";
    private string lastCheckpointUrl = "http://127.0.0.1:5000/player/last_position";

    // ✅ Se llama desde el botón "Nueva partida"
    public void OnNewGame()
    {
        StartCoroutine(ClearCheckpointsAndStartGame());
    }

    // ✅ Se llama desde el botón "Continuar juego"
    public void OnContinueGame()
    {
        StartCoroutine(LoadLastCheckpoint());
    }

    private IEnumerator ClearCheckpointsAndStartGame()
    {
        UnityWebRequest request = UnityWebRequest.Delete(clearCheckpointUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No hay más escenas en el build settings.");
            }
            Debug.Log("Checkpoints eliminados");
        }
        else
        {
            Debug.LogError("Error al borrar checkpoints: " + request.error);
        }

        SceneManager.LoadScene("NombreDeTuEscena");
    }

    private IEnumerator LoadLastCheckpoint()
    {
        UnityWebRequest request = UnityWebRequest.Get(lastCheckpointUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            PlayerPrefs.SetString("lastCheckpoint", json);
            SceneManager.LoadScene("NombreDeTuEscena");
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 2;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No hay más escenas en el build settings.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró checkpoint. Iniciando nueva partida.");
            SceneManager.LoadScene("NombreDeTuEscena");
        }
    }
}
