using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Public field to assign the target game object in the Unity Editor
    public GameObject targetGameObject;

    // Update is called once per frame
    void Update()
    {
        // Check if the target game object is assigned and active
        if (targetGameObject != null && targetGameObject.activeInHierarchy)
        {
            // Check if the Enter key is pressed
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Load the next scene
                // Make sure the scenes are added in the build settings
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                // Check if the next scene index is within the valid range
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.Log("This is the last scene in the build settings.");
                }
            }
        }
    }
}
