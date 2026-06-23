using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizManagerFV : MonoBehaviour
{
    public QuestionPanelFV[] perguntas; // tamanho 8, por ordem

    private int perguntaAtual = 0;

    void Start()
    {
        ScoreDataFV.pontuacao = 0;
        ScoreDataFV.erradas = 0;

        foreach (var q in perguntas)
            q.panel.SetActive(false);

        perguntaAtual = 0;
        perguntas[perguntaAtual].panel.SetActive(true);
    }

    // Liga aos botões A/B/C de cada pergunta (índice: 0, 1 ou 2)
    public void ResponderPergunta(int indexResposta)
    {
        QuestionPanelFV atual = perguntas[perguntaAtual];

        if (indexResposta == atual.respostaCorreta)
            ScoreDataFV.pontuacao++;
        else
            ScoreDataFV.erradas++;

        atual.panel.SetActive(false);
        perguntaAtual++;

        if (perguntaAtual < perguntas.Length)
        {
            perguntas[perguntaAtual].panel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("resultadofv");
        }
    }
}

[System.Serializable]
public class QuestionPanelFV
{
    public GameObject panel;
    [Tooltip("Índice da resposta correta: 0=A, 1=B, 2=C")]
    public int respostaCorreta;
}