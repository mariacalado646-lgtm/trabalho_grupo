using UnityEngine;
using UnityEngine.UI;

namespace LunchboxHero
{
    /// <summary>
    /// Toca um som quando o botao e clicado. Usa um AudioSource que vive
    /// num GameObject PROPRIO e PERMANENTE (criado automaticamente na
    /// primeira vez que e preciso), para que o som continue a tocar mesmo
    /// que o botao esteja dentro de um painel que se desativa logo a seguir
    /// ao clique (ex: o botao "Terminar" desativa o GamePanel onde esta).
    ///
    /// Coloca este script nos botoes de navegacao (dificuldade, "proximo"/
    /// terminar, jogar de novo, voltar ao menu, etc.) - NAO nos botoes de
    /// alimento da roda.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonSoundLancheira : MonoBehaviour
    {
        [Tooltip("Som a tocar quando este botao e clicado.")]
        public AudioClip clickSound;

        [Range(0f, 1f)]
        [Tooltip("Volume do som (0 = mudo, 1 = volume maximo).")]
        public float volume = 1f;

        private static AudioSource sharedAudioSource;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(PlaySound);

            EnsureSharedAudioSource();
        }

        private static void EnsureSharedAudioSource()
        {
            if (sharedAudioSource != null) return;

            GameObject audioObject = new GameObject("ButtonSoundPlayer_Lancheira");
            sharedAudioSource = audioObject.AddComponent<AudioSource>();
            sharedAudioSource.playOnAwake = false;
        }

        private void PlaySound()
        {
            if (clickSound == null)
            {
                Debug.LogWarning($"[LunchboxHero] ButtonSoundLancheira em '{gameObject.name}' nao tem nenhum clip definido.");
                return;
            }

            EnsureSharedAudioSource();
            sharedAudioSource.PlayOneShot(clickSound, volume);
        }
    }
}