using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultadoManagerFV : MonoBehaviour
{
    public TMP_Text certasText;
    public TMP_Text erradasText;

    void Start()
    {
        certasText.text = "Certas: " + ScoreDataFV.pontuacao;
        erradasText.text = "Erradas: " + ScoreDataFV.erradas;
    }

    // Botão opcional "Reiniciar"
    public void Reiniciar()
    {
        SceneManager.LoadScene("explicaçõesfv");
    }
}