using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplicacaoManagerFV : MonoBehaviour
{
    public GameObject[] explicacaoPanels;   // explicacao1 a explicacao6, por ordem
    public AudioClip[] audioExplicacoes;    // voz1 a voz6, na MESMA ordem
    public AudioSource audioSource;

    private int indiceAtual = 0;

    void Start()
    {
        indiceAtual = 0;

        // Desativa todas as explicações
        foreach (var p in explicacaoPanels)
            p.SetActive(false);

        // Ativa logo a primeira, já que entramos diretamente nesta Scene
        explicacaoPanels[indiceAtual].SetActive(true);
        TocarAudio(indiceAtual);
    }

    // Botão "Seguinte" / "Próximo" em cada explicação
    public void ProximaExplicacao()
    {
        explicacaoPanels[indiceAtual].SetActive(false);
        indiceAtual++;

        if (indiceAtual < explicacaoPanels.Length)
        {
            explicacaoPanels[indiceAtual].SetActive(true);
            TocarAudio(indiceAtual);
        }
        else
        {
            if (audioSource != null)
                audioSource.Stop();

            SceneManager.LoadScene("quizesfv");
        }
    }

    void TocarAudio(int index)
    {
        if (audioSource != null && audioExplicacoes != null &&
            index < audioExplicacoes.Length && audioExplicacoes[index] != null)
        {
            audioSource.Stop();
            audioSource.clip = audioExplicacoes[index];
            audioSource.Play();
        }
    }
}