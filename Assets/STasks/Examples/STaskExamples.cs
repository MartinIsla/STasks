using UnityEngine;

namespace Koffie.SimpleTasks.Examples
{
    public class STaskExamples : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        private STask _randomizeCameraColorTask;
        private Color _startColor;

        private void Start()
        {
            PrintHelloWorldAfterThreeSeconds();
            RandomizeCameraColorThreeTimes();
        }

        private void RandomizeCameraColorThreeTimes()
        {
            // Randomizes the color of the camera three times, with a 1.5 seconds wait between changes. 
            _startColor = _camera.backgroundColor;
            int timesRandomized = 0;

            // If we already have a task doing this, kill it
            _randomizeCameraColorTask?.Kill();
            _randomizeCameraColorTask = STasks.DoUntil(
                action: () =>
                {
                    timesRandomized++;
                    _camera.backgroundColor = new Color(Random.value, Random.value, Random.value);
                },
                condition: () => timesRandomized > 3,
                every: 1.5f);

            // Reset the color on complete
            _randomizeCameraColorTask.OnComplete(() => _camera.backgroundColor = _startColor);
        }

        private void PrintHelloWorldAfterThreeSeconds()
        {
            // Print "hello world" after three seconds
            STasks.Do(() => Debug.Log("Hello world!"), after: 3.0f);
        }
    }
}