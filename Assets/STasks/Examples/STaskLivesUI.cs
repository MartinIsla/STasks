using UnityEngine;
using UnityEngine.UI;

namespace Koffie.SimpleTasks.Examples
{
    public class STaskLivesUI : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        public void SetLives(int lives)
        {
            _text.text = $"Lives: {lives}";
        }
    }
}