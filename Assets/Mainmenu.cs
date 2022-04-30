using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Mainmenu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;

    public void Playgame()
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShowOptions()
    {
        if (MainMenu != null)
        {
            MainMenu.SetActive(false);
        }

        if (OptionsMenu != null)
        {
            OptionsMenu.SetActive(true);
        }
    }

    public void ShowMenu()
    {
        if (MainMenu != null)
        {
            MainMenu.SetActive(true);
        }

        if (OptionsMenu != null)
        {
            OptionsMenu.SetActive(false);
        }
    }
}
