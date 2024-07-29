using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject instructionScreen; // Drag the instruction screen GameObject here in the Inspector

    // This method will be called when the Play button is pressed
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // This method will be called when the Instruction button is pressed
    public void ShowInstructions()
    {
        instructionScreen.SetActive(true);
    }

    // This method will be called when the Close button on the instruction screen is pressed
    public void CloseInstructions()
    {
        instructionScreen.SetActive(false);
    }
}
