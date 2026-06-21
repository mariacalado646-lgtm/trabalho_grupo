using UnityEngine;
using UnityEngine.UI;

namespace LunchboxHero
{
    /// <summary>
    /// Botao de mutar/desmutar a musica de fundo. Troca de sprite consoante
    /// o estado (som ligado/desligado) e chama o MusicManagerLancheira.
    ///
    /// Coloca este script no botao de mute (pode estar em qualquer cena,
    /// desde que o MusicManagerLancheira ja tenha sido criado no Menu).
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class MuteButtonLancheira : MonoBehaviour
    {
        [Header("Referencias UI")]
        [Tooltip("Image do botao onde o icone e mostrado.")]
        public Image iconImage;

        [Tooltip("Sprite mostrado quando o som esta LIGADO.")]
        public Sprite soundOnSprite;

        [Tooltip("Sprite mostrado quando o som esta DESLIGADO (mute).")]
        public Sprite soundOffSprite;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(HandleClick);
        }

        private void Start()
        {
            UpdateIcon();
        }

        private void HandleClick()
        {
            if (MusicManagerLancheira.Instance == null)
            {
                Debug.LogWarning("[LunchboxHero] MuteButtonLancheira: nao existe nenhum MusicManagerLancheira na cena.");
                return;
            }

            MusicManagerLancheira.Instance.ToggleMute();
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            if (iconImage == null) return;
            if (MusicManagerLancheira.Instance == null) return;

            bool isMuted = MusicManagerLancheira.Instance.IsMuted;
            iconImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }
}