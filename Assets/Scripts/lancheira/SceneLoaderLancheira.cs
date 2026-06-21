using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunchboxHero
{
    /// <summary>
    /// Carrega uma cena pelo nome. Usa isto nos botoes que mudam de "pagina"
    /// (ex: no menu, o botao "facil" carrega a cena "Facil"; no ecra de
    /// resultado, um botao "Menu" volta ao menu principal).
    ///
    /// Coloca este script em qualquer botao que precise de mudar de cena.
    /// </summary>
    public class SceneLoaderLancheira : MonoBehaviour
    {
        [Tooltip("Nome EXATO da cena a carregar (tem de estar adicionada em File > Build Settings > Scenes In Build).")]
        public string sceneName;

        /// <summary>Liga este metodo ao evento OnClick do botao no Inspector.</summary>
        public void LoadScene()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[LunchboxHero] SceneLoaderLancheira: campo 'sceneName' esta vazio.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}