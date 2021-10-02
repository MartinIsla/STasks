namespace Koffie.SimpleTasks
{
    public abstract class STask
    {
        /// <summary>
        /// The time since the task started (after the delay). This is different from <see cref="ElapsedTime"/>, which is the time since the task was created.
        /// </summary>
        public float TimeSinceStart => _elapsedTime - _timeOfStart;

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

        private float _elapsedTime;
        private float _timeOfStart;

        protected readonly float maxDuration;
        protected readonly bool hasMaxDuration;
        protected readonly SAction action;

        private bool _isWaitingForDelay;
        private bool _isPaused;

        protected abstract void OnUpdate(float deltaTime);
        protected abstract float GetProgress();

        public STask(STaskSettings settings)
        {
            Delay = settings.delay;

            this.action = settings.action;
            this.maxDuration = settings.maxDuration;

            _elapsedTime = 0;
            _timeOfStart = 0;

            hasMaxDuration = maxDuration > 0;
            _isWaitingForDelay = Delay > 0;
        }

        public void Update(float deltaTime)
        {
            UpdateInternal(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            UpdateInternal(deltaTime);
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            UpdateInternal(fixedDeltaTime);
        }

        private void UpdateInternal(float deltaTime)
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
                if (hasMaxDuration && TimeSinceStart > maxDuration)
                {
                    Kill();
                }
                else
                {
                    OnUpdate(deltaTime);
                }
            }

            _elapsedTime += deltaTime;
            onUpdate?.Invoke();
        }

        protected virtual void OnStart()
        {
            _timeOfStart = _elapsedTime;
        }

        /// <summary>
        /// Forces the task to complete, invoking the scheduled action and calling OnComplete
        /// </summary>
        public void Complete()
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
    }
}