using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LunchboxHero
{
    [System.Serializable]
    public class ScoreRule
    {
        [Tooltip("Categoria avaliada por esta regra.")]
        public FoodCategory category;

        [Tooltip("Pontos (0-100) atribuidos quando este compartimento esta preenchido " +
                 "com um alimento de healthScore = 1.")]
        [Range(0f, 100f)]
        public float weight = 20f;
    }

    public class GameManagerlancheira : MonoBehaviour
    {
        public static GameManagerlancheira Instance { get; private set; }

        [Header("Paineis (ecras) desta cena")]
        public GameObject gamePanel;
        public GameObject resultPanel;

        [Header("Compartimentos da lancheira")]
        public List<LunchSlot> lunchSlots = new List<LunchSlot>();

        [Tooltip("Numero MAXIMO de slots que o jogador pode preencher ao mesmo tempo nesta cena " +
                 "(ex: 5 no nivel facil, mesmo que existam 7 LunchSlot/categorias disponiveis). " +
                 "Define 0 ou um numero >= ao total de slots para nao ter limite.")]
        public int maxFilledSlots = 5;

        [Tooltip("Numero MAXIMO de alimentos da MESMA categoria que podem estar na lancheira ao mesmo tempo " +
                 "(ex: 2 = no maximo 2 frutas, mesmo havendo mais slots de Fruit disponiveis). " +
                 "Define 0 para nao ter limite por categoria.")]
        public int maxPerCategory = 2;

        public List<FoodItemUI> foodItems = new List<FoodItemUI>();

        [Header("Regras de pontuacao")]
        public List<ScoreRule> scoreRules = new List<ScoreRule>();

        [Header("Texto de slots livres (opcional)")]
        public TMP_Text slotsRemainingText;
        public string slotsRemainingFormat = "Slots livres: {0}/{1}";

        [Header("Ecra de resultado")]
        public TMP_Text scoreText;
        public TMP_Text feedbackText;

        [Header("Mensagens de feedback")]
        public string perfectMessage = "Perfeito! Conseguiste um lanche completo e equilibrado!";
        public string greatMessage = "Muito bom! O teu lanche esta quase perfeito.";
        public string okMessage = "Nada mau! Mas o teu lanche podia ser mais equilibrado.";
        public string poorMessage = "Hmm... este lanche precisa de mais alimentos saudaveis!";

        private readonly Dictionary<FoodItemUI, LunchSlot> selections = new Dictionary<FoodItemUI, LunchSlot>();

        private void Awake()
        {
            Instance = this;

            if (gamePanel != null) gamePanel.SetActive(true);
            if (resultPanel != null) resultPanel.SetActive(false);

            ResetLunchbox();
        }

        private int FilledSlotsCount => lunchSlots.Count(s => !s.IsEmpty);

        public void OnFoodItemClicked(FoodItemUI item)
        {
            if (selections.ContainsKey(item))
            {
                RemoveSelection(item);
                UpdateSlotsRemainingText();
                return;
            }

            LunchSlot slot = FindSlotForCategory(item.category);
            if (slot == null)
            {
                Debug.LogWarning($"[LunchboxHero] Nao existe nenhum LunchSlot configurado para a categoria {item.category} nesta cena.");
                return;
            }

            bool slotJaTemItemDestaCategoria = !slot.IsEmpty;

            if (slotJaTemItemDestaCategoria)
            {
                var occupant = selections.FirstOrDefault(kv => kv.Value == slot);
                if (occupant.Key != null)
                    RemoveSelection(occupant.Key);

                slot.SetItem(item);
                item.SetSelected(true);
                selections[item] = slot;

                UpdateSlotsRemainingText();
                return;
            }

            bool haLimiteGeral = maxFilledSlots > 0;
            bool lancheiraCheia = haLimiteGeral && FilledSlotsCount >= maxFilledSlots;

            if (lancheiraCheia)
            {
                Debug.Log("[LunchboxHero] A lancheira ja esta cheia! Remove um alimento antes de adicionar outro.");
                return;
            }

            bool haLimitePorCategoria = maxPerCategory > 0;
            int preenchidosDestaCategoria = lunchSlots.Count(s => s.category == item.category && !s.IsEmpty);
            bool categoriaCheia = haLimitePorCategoria && preenchidosDestaCategoria >= maxPerCategory;

            if (categoriaCheia)
            {
                Debug.Log($"[LunchboxHero] Ja tens o maximo de {maxPerCategory} alimentos da categoria {item.category}.");
                return;
            }

            slot.SetItem(item);
            item.SetSelected(true);
            selections[item] = slot;

            UpdateSlotsRemainingText();
        }

        private void RemoveSelection(FoodItemUI item)
        {
            if (selections.TryGetValue(item, out LunchSlot slot))
                slot.ClearSlot();

            item.SetSelected(false);
            selections.Remove(item);
        }

        private LunchSlot FindSlotForCategory(FoodCategory category)
        {
            LunchSlot vazio = lunchSlots.FirstOrDefault(s => s.category == category && s.IsEmpty);
            if (vazio != null) return vazio;

            return lunchSlots.FirstOrDefault(s => s.category == category);
        }

        private void ResetLunchbox()
        {
            foreach (var slot in lunchSlots)
                slot.ClearSlot();

            foreach (var item in foodItems)
                item.SetSelected(false);

            selections.Clear();

            UpdateSlotsRemainingText();
        }

        private void UpdateSlotsRemainingText()
        {
            if (slotsRemainingText == null) return;
            if (maxFilledSlots <= 0) return;

            int livres = Mathf.Max(0, maxFilledSlots - FilledSlotsCount);
            slotsRemainingText.text = string.Format(slotsRemainingFormat, livres, maxFilledSlots);
        }

        public void ShowResult()
        {
            if (gamePanel != null) gamePanel.SetActive(false);
            if (resultPanel != null) resultPanel.SetActive(true);

            float score = CalculateScore();
            DisplayResult(score);
        }

        public void RetrySameDifficulty()
        {
            if (resultPanel != null) resultPanel.SetActive(false);
            if (gamePanel != null) gamePanel.SetActive(true);

            ResetLunchbox();
        }

        private float CalculateScore()
        {
            float total = 0f;

            foreach (var slot in lunchSlots)
            {
                if (slot.IsEmpty) continue;

                ScoreRule rule = scoreRules.FirstOrDefault(r => r.category == slot.category);
                if (rule != null)
                    total += rule.weight * slot.currentItem.HealthScore;
            }

            return Mathf.Clamp(total, 0f, 100f);
        }

        private void DisplayResult(float score)
        {
            if (scoreText != null)
                scoreText.text = $"{Mathf.RoundToInt(score)}%";

            string message;

            if (score >= 100f) message = perfectMessage;
            else if (score >= 80f) message = greatMessage;
            else if (score >= 50f) message = okMessage;
            else message = poorMessage;

            if (feedbackText != null)
                feedbackText.text = message;
        }
    }
}