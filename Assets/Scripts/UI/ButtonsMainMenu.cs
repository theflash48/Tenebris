using UnityEngine;

//----------------FUNCIONAMIENTO BOTONES MAIN MENU----------------//

public class ButtonsMainMenu : MonoBehaviour
{
    public void EntryGame()
    {
        Debug.Log("Entrar al Juego");
    }
    
    //Salir del Juego
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del Juego");
    }
}
