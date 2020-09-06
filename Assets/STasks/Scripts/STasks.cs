using UnityEngine;

namespace Koffie.SimpleTasks
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
        public const int UPDATE_INITIAL_CAPACITY = 64;
        public const int LATEUPDATE_INITIAL_CAPACITY = 0;
        public const int FIXEDUPDATE_INITIAL_CAPACITY = 0;

        private static STasksCollection _updateTasks;
        private static STasksCollection _lateUpdateTasks;
        private static STasksCollection _fixedUpdateTasks;

        private static bool _isPaused = false;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _updateTasks = new STasksCollection(UPDATE_INITIAL_CAPACITY);
            _lateUpdateTasks = new STasksCollection(LATEUPDATE_INITIAL_CAPACITY);
            _fixedUpdateTasks = new STasksCollection(FIXEDUPDATE_INITIAL_CAPACITY);

            UpdateHelper.AddSubscriber(OnUpdate, UpdateType.Update);
            UpdateHelper.AddSubscriber(OnLateUpdate, UpdateType.LateUpdate);
            UpdateHelper.AddSubscriber(OnFixedUpdate, UpdateType.FixedUpdate);
        }

        private static void OnUpdate(float deltaTime)
        {
            if (_isPaused) { return; }

            _updateTasks.Update(deltaTime);
        }

        private static void OnLateUpdate(float deltaTime)
        {
            if (_isPaused) { return; }

            _lateUpdateTasks.Update(deltaTime);
        }

        private static void OnFixedUpdate(float fixedDeltaTime)
        {
            if (_isPaused) { return; }

            _fixedUpdateTasks.Update(fixedDeltaTime);
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
        /// <param name="updateType">Optional: The update method this task should use (Update/LateUpdate/FixedUpdate)</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static DoTask Do(SAction action, float after, UpdateType updateType = UpdateType.Update)
        {
            STaskSettings settings = new STaskSettings()
            {
                action = action,
                delay = after,
            };

            DoTask task = new DoTask(settings);
            AddTask(task, updateType);
            return task;
        }

        /// <summary>
        /// Performs a task every few seconds until manually stopped
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="every">The time (in seconds) to wait between executions</param>
        /// <param name="startAfter">Optional: The time (in seconds) to wait before starting the task</param>
        /// <param name="updateType">Optional: The update method this task should use (Update/LateUpdate/FixedUpdate)</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static DoRepeatingTask DoRepeating(SAction action, float every, float startAfter = 0, float maxDuration = -1, UpdateType updateType = UpdateType.Update)
        {
            STaskSettings settings = new STaskSettings()
            {
                action = action,
                frequency = every,
                delay = startAfter,
                maxDuration = maxDuration,
            };

            DoRepeatingTask task = new DoRepeatingTask(settings);
            AddTask(task, updateType);
            return task;
        }

        /// <summary>
        /// Performs a task until a condition is met
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="condition">The condition for this task to stop</param>
        /// <param name="every">Optional: The time (in seconds) to wait between executions</param>
        /// <param name="startAfter">Optional: The time (in seconds) to wait before starting the task</param>
        /// <param name="updateType">Optional: The update method this task should use (Update/LateUpdate/FixedUpdate)</param>
        /// <param name="timeout">Optional: A maximum duration (in seconds) for this task. If set, the task will stop even if the condition isn't met. Note that the action won't be executed.</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static DoUntilTask DoUntil(SAction action, SCondition condition, float every = 0, float startAfter = 0, float timeout = -1, UpdateType updateType = UpdateType.Update)
        {
            STaskSettings settings = new STaskSettings()
            {
                action = action,
                condition = condition,
                frequency = every,
                delay = startAfter,
                maxDuration = timeout,
            };

            DoUntilTask task = new DoUntilTask(settings);
            AddTask(task, updateType);
            return task;
        }

        /// <summary>
        /// Performs a task when a condition is met
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="condition">The condition for this task to stop</param>
        /// <param name="startAfter">Optional: The time (in seconds) to wait before starting the task</param>
        /// <param name="updateType">Optional: The update method this task should use (Update/LateUpdate/FixedUpdate)</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static DoWhenTask DoWhen(SAction action, SCondition condition, float startAfter = 0, UpdateType updateType = UpdateType.Update)
        {
            STaskSettings settings = new STaskSettings()
            {
                action = action,
                condition = condition,
                delay = startAfter,
            };

            DoWhenTask task = new DoWhenTask(settings);
            AddTask(task, updateType);
            return task;
        }

        /// <summary>
        /// Performs a task after a given number of frames has passed
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="frames">The number of frames to wait</param>
        /// <param name="startAfter">Optional: The time (in seconds) to wait before starting to count the frames</param>
        /// <param name="updateType">Optional: The update method this task should use (Update/LateUpdate/FixedUpdate)</param>
        /// <returns>The STask. You can save this task to stop it before it's finished and to subscribe to events such us OnComplete</returns>
        public static DoAfterFramesTask DoAfterFrames(SAction action, int frames, float startAfter = 0, UpdateType updateType = UpdateType.Update)
        {
            STaskSettings settings = new STaskSettings()
            {
                action = action,
                targetFrames = frames,
                delay = startAfter,
            };

            DoAfterFramesTask task = new DoAfterFramesTask(settings);
            AddTask(task, updateType);
            return task;
        }

        private static void AddTask(STask task, UpdateType updateType)
        {
            if (updateType == UpdateType.Update)
            {
                _updateTasks.AddTask(task);
            }
            else if (updateType == UpdateType.LateUpdate)
            {
                _lateUpdateTasks.AddTask(task);
            }
            else
            {
                _fixedUpdateTasks.AddTask(task);
            }
        }
    }
}