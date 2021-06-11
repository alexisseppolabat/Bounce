using UnityEngine;
using UnityEngine.SceneManagement;


public class WorldManager : MonoBehaviour {

    public Animator openGate;

    public void openEndGate() {
        openGate.enabled = true;
        openGate.Play("Open Gate");
    }

    public void reloadLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void loadLevelSelector() {
        SceneManager.LoadScene("LevelSelector");
    }

    public void loadNextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}