using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STasks;

namespace STasks.Examples
{
    public class STaskExamples : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        private STask _randomizeCameraColorTask;

        private void Start()
        {
            PrintHelloWorldAfterThreeSeconds();
            RandomizeCameraColorThreeTimes();
        }

        private void RandomizeCameraColorThreeTimes()
        {
            // Randomizes the color of the camera three times, with a 1.5 seconds wait between changes. 
            
            int timesRandomized = 0;

            // If we already have a task doing this, kill it
            _randomizeCameraColorTask?.Kill();
            _randomizeCameraColorTask = STasks.DoUntil(
                action: () =>
                {
                    timesRandomized++;
                    _camera.backgroundColor = new Color(Random.value, Random.value, Random.value);
                },
                condition: () => timesRandomized == 3,
                every: 1.5f);

            // Resets the color to black on complete
            _randomizeCameraColorTask.OnComplete(() => _camera.backgroundColor = Color.black);
        }

        private void PrintHelloWorldAfterThreeSeconds ()
        {
            // Print "hello world" after three seconds
            STasks.Do(() => Debug.Log("Hello world!"), after: 3.0f);
        }
    }
}