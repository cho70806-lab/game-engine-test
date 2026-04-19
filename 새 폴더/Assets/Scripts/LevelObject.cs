using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObject : MonoBehaviour
{
    public string nextLevel;

    public void MoveToNextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
}
