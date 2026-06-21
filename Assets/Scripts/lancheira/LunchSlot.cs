using UnityEngine;
using UnityEngine.UI;

namespace LunchboxHero
{
    /// <summary>
    /// Representa um compartimento da lancheira que aceita alimentos de uma categoria.
    /// Coloca este script numa Image (UI) dentro da área da lancheira.
    /// </summary>
    public class LunchSlot : MonoBehaviour
    {
        [Header("Configuração")]
        [Tooltip("Categoria de alimentos aceite por este compartimento.")]
        public FoodCategory category;

        [Header("Referências UI")]
        [Tooltip("Image onde o ícone do alimento escolhido é mostrado.")]
        public Image iconImage;

        [Tooltip("Sprite mostrado quando o compartimento está vazio (opcional, ex: silhueta).")]
        public Sprite emptySprite;

        [HideInInspector] public FoodItemUI currentItem;

        public bool IsEmpty => currentItem == null;

        private void Awake()
        {
            ClearSlot();
        }

        /// <summary>Coloca um alimento neste compartimento, copiando o ícone do botão clicado.</summary>
        public void SetItem(FoodItemUI item)
        {
            currentItem = item;

            if (iconImage == null) return;

            Sprite spriteToShow = item.iconImage != null ? item.iconImage.sprite : null;
            iconImage.sprite = spriteToShow;
            iconImage.enabled = spriteToShow != null;
        }

        /// <summary>Esvazia este compartimento.</summary>
        public void ClearSlot()
        {
            currentItem = null;

            if (iconImage == null) return;

            iconImage.sprite = emptySprite;
            iconImage.enabled = emptySprite != null;
        }
    }
}