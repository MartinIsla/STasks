namespace Koffie.SimpleTasks
{
    public abstract class STask
    {
        internal static float deltaTime;
        internal static float fixedDeltaTime;

        /// <summary>
        /// The time since the task started (after the delay). This is different from <see cref="ElapsedTime"/>, which is the time since the task was created.
        /// </summary>
        public float TimeSinceStart { get; private set; }

        /// <summary>
        /// The progress (0 to 1) of the task. The progress means different things for the different types of tasks.
        /// Usually, the action is invoked when progress is 1.
        /// </summary>
        public float Progress => GetProgress();

        /// <summary>
        /// The time since the task was created.
        /// </summary>
        public float ElapsedTime => _elapsedTime;

        /// <summary>
        /// The delay before starting the task. 
        /// </summary>
        public readonly float Delay;

        /// <summary>
        /// Whether this task has been completed or killed.
        /// </summary>
        public bool isDone;

        /// <summary>
        /// The delegate invoked when the task is completed. It's recommended that you use <see cref="OnComplete(SAction)"/> to subscribe.
        /// </summary>
        public SAction onComplete;

        /// <summary>
        /// The delegate invoked when the task is updated. It's recommended that you use <see cref="OnUpdate(SAction)"/> to subscribe.
        /// </summary>
        public SAction onUpdate;

        /// <summary>
        /// The delegate invoked when the task is started after the initial delay. It's recommended that you use <see cref="OnStart(SAction)"/> to subscribe.
        /// </summary>
        public SAction onStart;

        /// <summary>
        /// The delegate invoked when the task is killed. It's recommended that you use <see cref="OnKill(SAction)"/> to subscribe.
        /// </summary>
        public SAction onKill;

        /// <summary>
        /// The delegate invoked when time exceeds the <see cref="MaxDuration"/>. It's recommended that you use <see cref="OnTimeout(SAction)"/> to subscribe.
        /// </summary>
        public SAction onTimeout;

        /// <summary>
        /// The maximum lifetime of the task after the delay (<seealso cref="TimeSinceStart"/>).
        /// </summary>
        public readonly float MaxDuration;

        /// <summary>
        /// Whether a <see cref="MaxDuration"/> has been set for this task.
        /// </summary>
        public readonly bool HasMaxDuration;

        protected readonly SAction action;

        private bool _isWaitingForDelay;
        private bool _taskStarted;
        private bool _isPaused;
        private float _elapsedTime;


        protected abstract void OnUpdate();
        protected abstract float GetProgress();

        public STask(in STaskSettings settings)
        {
            Delay = settings.delay;

            this.action = settings.action;
            this.MaxDuration = settings.maxDuration;

            _elapsedTime = 0;

            HasMaxDuration = MaxDuration > 0;
            _isWaitingForDelay = Delay > 0;
            _taskStarted = false;
        }

        public void Update()
        {
            UpdateInternal();
        }

        public void LateUpdate()
        {
            UpdateInternal();
        }

        public void FixedUpdate()
        {
            UpdateInternal();
        }

        private void UpdateInternal()
        {
            if (_isPaused) { return; }

            if (_isWaitingForDelay)
            {
                _isWaitingForDelay = _elapsedTime < Delay;

                if (!_isWaitingForDelay)
                {
                    OnStart();
                }
            }
            else
            {
                if (!_taskStarted)
                {
                    OnStart();
                }

                TimeSinceStart += deltaTime;
                OnUpdate();

                if (HasMaxDuration && TimeSinceStart >= MaxDuration)
                {
                    OnTimeout();
                    return;
                }
            }

            _elapsedTime += deltaTime;
            onUpdate?.Invoke();
        }

        protected virtual void OnTimeout()
        {
            onTimeout?.Invoke();
            isDone = true;
        }

        protected virtual void OnStart()
        {
            TimeSinceStart = 0;
            _taskStarted = true;
            onStart?.Invoke();
        }

        /// <summary>
        /// Forces the task to complete, invoking the scheduled action and calling OnComplete
        /// </summary>
        public virtual void Complete()
        {
            isDone = true;
            action.Invoke();
            onComplete?.Invoke();
        }

        /// <summary>
        /// Forces the task to stop without invoking the action and without calling OnComplete. OnKill will be invoked.
        /// </summary>
        public void Kill()
        {
            if (isDone) return;

            isDone = true;
            onKill?.Invoke();
        }

        /// <summary>
        /// Resumes the task
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Pauses the task
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Subscribe a method that will be invoked when the task is complete
        /// </summary>
        /// <param name="action">The action to be invoked</param>
        /// <returns>The task</returns>
        public STask OnComplete(SAction action)
        {
            onComplete += action;
            return this;
        }

        /// <summary>
        /// Subscribe a method that will be invoked when the task is Updated (every frame)
        /// </summary>
        /// <param name="action">The action to be invoked</param>
        /// <returns>The task</returns>
        public STask OnUpdate(SAction action)
        {
            onUpdate += action;
            return this;
        }

        /// <summary>
        /// Subscribe a method that will be invoked when the task is started (after the delay)
        /// </summary>
        /// <param name="action">The action to be invoked</param>
        /// <returns>The task</returns>
        public STask OnStart(SAction action)
        {
            onStart += action;
            return this;
        }

        /// <summary>
        /// Subscribe a method that will be invoked when the task is killed
        /// </summary>
        /// <param name="action">The action to be invoked</param>
        /// <returns>The task</returns>
        public STask OnKill(SAction action)
        {
            onKill += action;
            return this;
        }

        /// <summary>
        /// Subscribe a method that will be invoked when the task's time exceeds its <see cref="MaxDuration"/>
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public STask OnTimeout(SAction action)
        {
            onTimeout += action;
            return this;
        }

    }
}