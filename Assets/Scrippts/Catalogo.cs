using UnityEngine;
using UnityEngine.SceneManagement;

public class Catalogo : MonoBehaviour
{
    public string nombreEscenaAR = "SampleScene";

    public void ElegirMueble(int indice)
    {
        Seleccion.indice = indice;
        SceneManager.LoadScene(nombreEscenaAR);
    }
}