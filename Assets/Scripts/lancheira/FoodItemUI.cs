using UnityEngine;
using UnityEngine.UI;

namespace LunchboxHero
{
    /// <summary>
    /// Botao clicavel de um alimento na roda/tabuleiro.
    /// Ao clicar, alterna entre selecionado/nao selecionado e avisa o GameManager.
    ///
    /// Coloca este script diretamente em cada prefab de alimento (ex: dentro
    /// do prefab "frutas", "vegetais", "docesmaus", etc.) e preenche os
    /// campos no Inspector - nao precisas de criar nenhum asset a parte.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class FoodItemUI : MonoBehaviour
    {
        [Header("Dados deste alimento")]
        [Tooltip("Nome do alimento, so para te organizares (ex: 'Maca', 'Brocolos').")]
        public string itemName;

        [Tooltip("Categoria/compartimento da lancheira a que este alimento pertence.")]
        public FoodCategory category;

        [Tooltip("Marca esta caixa se o alimento for saudavel (conta pontos). " +
                 "Deixa desmarcada para alimentos nao saudaveis (ex: doces, snacks) - nao conta pontos se for escolhido.")]
        public bool isHealthy = true;

        [Header("Referencias UI")]
        [Tooltip("Image onde o icone do alimento e mostrado (normalmente a Image do proprio botao).")]
        public Image iconImage;

        [Tooltip("Elemento visual (ex: contorno/checkmark) mostrado quando o item esta selecionado.")]
        public GameObject selectedHighlight;

        public bool IsSelected { get; private set; }

        /// <summary>Valor usado no calculo da pontuacao: 1 se saudavel, 0 se nao saudavel.</summary>
        public float HealthScore => isHealthy ? 1f : 0f;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(HandleClick);

            SetSelected(false);
        }

        private void HandleClick()
        {
            GameManagerlancheira.Instance.OnFoodItemClicked(this);
        }

        /// <summary>Atualiza o visual de selecionado/nao selecionado.</summary>
        public void SetSelected(bool selected)
        {
            IsSelected = selected;

            if (selectedHighlight != null)
                selectedHighlight.SetActive(selected);
        }
    }
}