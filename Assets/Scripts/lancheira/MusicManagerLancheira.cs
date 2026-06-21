using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunchboxHero
{
    public class MusicManagerLancheira : MonoBehaviour
    {
        public static MusicManagerLancheira Instance { get; private set; }

        [Tooltip("Musica de fundo a tocar em loop.")]
        public AudioClip backgroundMusic;

        [Range(0f, 1f)]
        public float volume = 0.5f;

        [Tooltip("Nomes EXATOS das cenas que pertencem ao teu jogo da lancheira.")]
        public List<string> lancheiraSceneNames = new List<string>
        {
            "explicaçãolancheira",
            "escolhalancheira",
            "jogolancheira1",
            "jogolancheira2",
            "jogolancheira3"
        };

        private AudioSource audioSource;

        public bool IsMuted => audioSource != null && audioSource.mute;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.playOnAwake = false;

            if (backgroundMusic != null)
                audioSource.Play();

            SilenceMainMenuMusic();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool aindaDentroDoJogo = lancheiraSceneNames.Contains(scene.name);

            if (!aindaDentroDoJogo)
            {
                StopAndDestroy();
            }
        }

        public void StopAndDestroy()
        {
            if (audioSource != null)
                audioSource.Stop();

            RestoreMainMenuMusic();

            Instance = null;
            Destroy(gameObject);
        }

        private void SilenceMainMenuMusic()
        {
            if (MusicManager.Instance != null)
                MusicManager.Instance.MusicOff();
        }

        private void RestoreMainMenuMusic()
        {
            if (MusicManager.Instance != null)
                MusicManager.Instance.MusicOn();
        }

        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            if (audioSource != null)
                audioSource.volume = volume;
        }

        public void ToggleMute()
        {
            if (audioSource == null) return;
            audioSource.mute = !audioSource.mute;
        }
    }
}