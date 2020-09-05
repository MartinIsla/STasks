using UnityEngine;

namespace Koffie.SimpleTasks.Examples
{
    public class STaskPlayer : MonoBehaviour
    {
        [SerializeField]
        private STaskCooldownUI _cooldownUI;

        [SerializeField]
        private STaskLivesUI _livesUI;

        [SerializeField]
        private GameObject _gameOverScreen;

        [SerializeField]
        private int _startLives = 30;

        [SerializeField]
        private float _healingCooldown;

        private int _currentLives;        
        private bool _canExecuteHealingAbility;

        private SpriteRenderer _renderer;

        private STask _loseLivesTask;
        private STask _healingAbilityCooldownTask;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _currentLives = _startLives;
            _canExecuteHealingAbility = true;

            // Start losing lives when the game starts
            _loseLivesTask = STasks.DoUntil(LoseOneLife, () => _currentLives == 0, 1.0f).OnComplete(OnGameOver);
        }

        private void OnGameOver()
        {
            _gameOverScreen.SetActive(true);
            Destroy(gameObject);
        }

        private void Update()
        {
            if (_canExecuteHealingAbility && Input.GetKeyDown(KeyCode.Space))
            {
                GainOneLife();
            }
        }

        private void GainOneLife()
        {
            _canExecuteHealingAbility = false;
            _currentLives++;
            _livesUI.SetLives(_currentLives);

            // Show the cooldown UI
            _cooldownUI.Show();

            _healingAbilityCooldownTask?.Kill();

            // Reenable the healing ability after the cooldown
            _healingAbilityCooldownTask = STasks.Do(() => _canExecuteHealingAbility = true, _healingCooldown);

            // Update the cooldown UI every frame
            _healingAbilityCooldownTask.OnUpdate(() => _cooldownUI.SetProgress(_healingAbilityCooldownTask.Progress));

            // Hide the cooldown UI when the task is complete
            _healingAbilityCooldownTask.OnComplete(() => _cooldownUI.Hide());
        }

        private void LoseOneLife()
        {
            _currentLives--;
            _renderer.color = Color.Lerp(Color.red, Color.white, (float)_currentLives / _startLives);
            _livesUI.SetLives(_currentLives);
        }

        private void OnDestroy()
        {
            // Kill the Lose Lives task when the player is destroyed
            _loseLivesTask?.Kill();
            _healingAbilityCooldownTask?.Kill();
        }
    }
}