/* This is a simple script to test different settings in the editor
 * Press SPACE to start executing a task with the current settings
 * Press ESCAPE to kill the current task while it's running
 * Press UP/DOWN ARROWS to increase/decrease the value of the iteration variable (useful for testing conditional tasks) */
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.Assertions;

namespace Koffie.SimpleTasks.Examples
{
    public class STaskSimpleTest : MonoBehaviour
    {
        public enum STaskType
        {
            Simple,
            Repeating,
            When,
            Times,
            Until,
            AfterFrames
        }
        

        [Tooltip("Whether to execute a task with the current settings when entering play mode")]
        public bool autoExecuteOnStart = true;

        [Tooltip("The value of currentIteration before starting a task")]
        public int iterationsOnTaskStart = 0;

        [Tooltip("If true, a new task with the current settings will be started when the current task is completed")]
        public bool restartTaskOnComplete = false;

        [Tooltip("For visualization only. The value of the current iteration. Represents how many times a task's action has been executed")]
        public int currentIteration = 0;

        [Header("Task Settings")]
        [Tooltip("The type of task to execute")]
        public STaskType TaskType = STaskType.Simple;

        [Header("Shared Settings")]
        [Tooltip("The time between executions of a repeating task (DoRepeating, DoUntil or DoTimes)")]
        public float frequency = 0.1f;

        [Tooltip("A delay before starting the task")]
        public float startAfter = 0.0f;

        [Tooltip("The maximum duration of the task. If this value is negative, the task will run until manually killed.")]
        public float maxDuration = -1.0f;

        [Header("Conditional Task Settings")]
        [Tooltip("The number of iterations used for condition comparissons.\n\nFor DoUntil, the task will be executed repeatedly until" +
            " the current iteration is greater than or equal to this value.\n\nFor DoWhen, the task will be executed when the current iteration is greater than or equal to this value.")]
        public float maxIterations = 10;

        [Header("DoTimes")]
        [Tooltip("How many times we should execute the DoTimes task")]
        public int repeatTimes = 10;

        [Header("DoAfterFrames")]
        [Tooltip("How many frames we should wait before executing the DoAfterFrames task")]
        public int frames = 10;

        private STask _currentTask;

        private void Start()
        {
            if (autoExecuteOnStart)
            {
                ExecuteTaskWithCurrentSettings();
            }
        }

        private void ExecuteTaskWithCurrentSettings()
        {
            _currentTask?.Kill();
            currentIteration = iterationsOnTaskStart;

            if (TaskType == STaskType.Repeating)
            {
                _currentTask = STasks.DoRepeating(() => IncreaseIterationAndPrint(ref currentIteration), frequency, startAfter, maxDuration);
            }
            else if (TaskType == STaskType.Until)
            {
                _currentTask = STasks.DoUntil(() => IncreaseIterationAndPrint(ref currentIteration), () => currentIteration >= maxIterations, frequency, startAfter, maxDuration);
            }
            else if (TaskType == STaskType.Simple)
            {
                _currentTask = STasks.Do(() => IncreaseIterationAndPrint(ref currentIteration), startAfter);
            }
            else if (TaskType == STaskType.When)
            {
                _currentTask = STasks.DoWhen(() => Debug.Log($"DoWhen iteration: {currentIteration}"), () => currentIteration >= maxIterations, startAfter);
            }
            else if (TaskType == STaskType.Times)
            {
                _currentTask = STasks.DoTimes(() => IncreaseIterationAndPrint(ref currentIteration), repeatTimes, frequency, startAfter, maxDuration);
            }
            else if (TaskType == STaskType.AfterFrames)
            {
                _currentTask = STasks.DoAfterFrames(() => IncreaseIterationAndPrint(ref currentIteration), frames, startAfter);
            }

            string taskName = _currentTask.GetType().Name;
            _currentTask.OnStart(() => Debug.Log($"{taskName} started after {_currentTask.ElapsedTime.ToString("0.00")} seconds with iterations = {currentIteration}"));
            _currentTask.OnComplete(() =>
            {
                Debug.Log($"{taskName} completed after {currentIteration} iterations. Time since started: {_currentTask.TimeSinceStart.ToString("0.00")} seconds");
                
                if (restartTaskOnComplete)
                    ExecuteTaskWithCurrentSettings();
            });
            _currentTask.OnKill(() => Debug.Log($"{taskName} killed after {currentIteration} iterations. Time since created: {_currentTask.ElapsedTime.ToString("0.00")} seconds"));
            _currentTask.OnTimeout(() => Debug.Log($"{taskName} timed out after {currentIteration} iterations"));            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ExecuteTaskWithCurrentSettings();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _currentTask?.Kill();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
                currentIteration++;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                currentIteration--;
        }

        private void IncreaseIterationAndPrint(ref int iteration)
        {
            iteration++;
            Debug.Log($"Current iteration: {iteration} at {_currentTask.TimeSinceStart.ToString("0.00")} seconds.");
        }
    }
}