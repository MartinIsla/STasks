using UnityEngine;

namespace STasks
{
    public delegate void SAction();
    public delegate bool SCondition();

    public enum TaskType
    {
        Simple,
        Looped,
        OnCondition,
        Until
    }

    public static class STasks
    {
        public const int MAX_CAPACITY = 256;
        private static STask[] _currentTasks;
        private static bool _isPaused = false;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _isPaused = false;
            _currentTasks = new STask[MAX_CAPACITY];
            UpdateHelper.AddSubscriber(OnUpdate, UpdateHelper.UpdateType.Update);
            UpdateHelper.AddSubscriber(OnLateUpdate, UpdateHelper.UpdateType.LateUpdate);
        }

        private static void OnLateUpdate(float deltaTime)
        {
            if (_isPaused) { return; }

            for (int i = 0; i < _currentTasks.Length; i++)
            {
                if (_currentTasks[i] == null)
                {
                    continue;
                }

                if (!_currentTasks[i].IsDone)
                {
                    _currentTasks[i].LateUpdate(deltaTime);
                }
                else
                {
                    _currentTasks[i] = null;
                }
            }
        }

        private static void OnUpdate(float deltaTime)
        {
            if (_isPaused) { return; }

            for (int i = 0; i < _currentTasks.Length; i++)
            {
                if (_currentTasks[i] == null)
                {
                    continue;
                }

                if (!_currentTasks[i].IsDone)
                {
                    _currentTasks[i].Update(deltaTime);
                }
                else
                {
                    _currentTasks[i] = null;
                }
            }
        }

        /// <summary>
        /// Pauses all the tasks
        /// </summary>
        public static void PauseAll()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Unpauses all the tasks
        /// </summary>
        public static void ResumeAll()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Schedules a task so it's executed after some time
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="after">The time (in seconds) to wait before executing the task</param>
        /// <param name="completeAfterLastFrame">Optional: If true, OnComplete will be called one frame after the action was executed</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static STask Do(SAction action, float after, bool completeAfterLastFrame = false)
        {
            STask task = new STask(TaskType.Simple, action, after, completeAfterLastFrame: completeAfterLastFrame);
            AddTask(task);
            return task;
        }

        /// <summary>
        /// Performs a task every few seconds until manually stopped
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="every">The time (in seconds) to wait between executions</param>
        /// <param name="startAfter">Optional: The time (in seconds) to wait before starting the task</param>
        /// <param name="useLateUpdate">Optional: If true, the task will be executed on LateUpdate instead of Update</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static STask DoRepeating(SAction action, float every, float startAfter = 0, bool useLateUpdate = false)
        {
            STask task = new STask(TaskType.Looped, action, startAfter, null, every, useLateUpdate: useLateUpdate);
            AddTask(task);
            return task;
        }

        /// <summary>
        /// Performs a task until a condition is met
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="condition">The condition for this task to stop</param>
        /// <param name="every">Optional: The time (in seconds) to wait between executions</param>
        /// <param name="startAfter">Optional: The time (in seconds) to wait before starting the task</param>
        /// <param name="useLateUpdate">Optional: If true, the task will be executed on LateUpdate instead of Update</param>
        /// <param name="timeout">Optional: A maximum duration (in seconds) for this task. If set, the task will stop even if the condition isn't met. Note that the action won't be executed.</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static STask DoUntil(SAction action, SCondition condition, float every = 0, float startAfter = 0, bool useLateUpdate = false, float timeout = -1)
        {
            STask task = new STask(TaskType.Until, action, after: startAfter, condition, every, useLateUpdate: useLateUpdate, timeout: timeout);
            AddTask(task);
            return task;
        }

        /// <summary>
        /// Performs a task when a condition is met
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="condition">The condition for this task to stop</param>
        /// <param name="startAfter">Optional: The time (in seconds) to wait before starting the task</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static STask DoWhen(SAction action, SCondition condition, float startAfter = 0)
        {
            STask task = new STask(TaskType.OnCondition, action, startAfter, condition);
            AddTask(task);
            return task;
        }

        /// <summary>
        /// Performs a task after a given number of frames has passed
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="frames">The number of frames to wait</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static STask DoAfterFrames(SAction action, int frames)
        {
            STask task = null;
            task = new STask(TaskType.OnCondition, action, condition: () => task.ElapsedFrames == frames);
            AddTask(task);
            return task;
        }

        private static void AddTask(STask task)
        {
            for (int i = 0; i < _currentTasks.Length; i++)
            {
                if (_currentTasks[i] == null)
                {
                    _currentTasks[i] = task;
                    return;
                }
            }

            Debug.LogError("Max task capacity reached. What are you doing.");
        }
    }
}