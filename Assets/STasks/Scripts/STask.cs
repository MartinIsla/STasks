namespace Koffie.SimpleTasks
{
    public class STask
    {
        public readonly SAction action;
        public readonly SCondition condition;
        public float Progress => ElapsedTime / _after;
        public float ElapsedTime { get; private set; }
        public int ElapsedFrames { get; private set; }

        public bool IsDone { get; private set; }
        public SAction onComplete;
        public SAction onUpdate;
        private readonly float _timeout;
        private readonly bool _isLooped;
        private bool _loopStarted;
        private readonly TaskType _taskType;
        private bool _completeAfterLastFrame;
        private bool _isCompletionScheduled;
        private readonly bool _useLateUpdate;
        private float _loopElapsedTime;
        private readonly float _after;
        private readonly float _every;

        public STask(TaskType type, SAction action, float after = 0, SCondition condition = null, float every = -1, bool completeAfterLastFrame = false, bool useLateUpdate = false, float timeout = -1)
        {
            this.action = action;
            this._after = after;
            this.condition = condition;
            this._every = every;
            this._taskType = type;
            this._completeAfterLastFrame = completeAfterLastFrame;
            this._useLateUpdate = useLateUpdate;
            this._timeout = timeout;

            _isCompletionScheduled = false;
            _isLooped = type == TaskType.Until || type == TaskType.Looped;
            _loopStarted = false;
            IsDone = false;
            ElapsedTime = 0;
            ElapsedFrames = 0;
        }

        public void Update(float deltaTime)
        {
            if (_useLateUpdate)
            {
                return;
            }

            UpdateInternal(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            if (!_useLateUpdate)
            {
                return;
            }

            UpdateInternal(deltaTime);
        }

        private void UpdateInternal(float deltaTime)
        {
            if (IsDone)
            {
                return;
            }

            if (_isCompletionScheduled || _loopStarted && HasTimedOut())
            {
                Complete();
                return;
            }

            if (_taskType != TaskType.OnCondition)
            {
                if ((!_loopStarted || !_isLooped) && ElapsedTime > _after)
                {
                    action.Invoke();
                    if (!_isLooped)
                    {
                        if (!_completeAfterLastFrame)
                        {
                            Complete();
                        }
                        else
                        {
                            _isCompletionScheduled = true;
                        }
                    }
                    else if (!_loopStarted)
                    {
                        _loopStarted = true;
                        _loopElapsedTime = 0;
                    }
                }
                else if (_isLooped && _loopStarted)
                {
                    if (_loopElapsedTime > _every)
                    {
                        _loopElapsedTime = 0;

                        if (condition != null && condition())
                        {
                            Complete();
                        }
                        else
                        {
                            action?.Invoke();
                        }
                    }
                    _loopElapsedTime += deltaTime;
                }
            }
            else
            {
                if (condition != null && condition())
                {
                    action?.Invoke();
                    Complete();
                }
            }

            ElapsedTime += deltaTime;
            ElapsedFrames++;
            onUpdate?.Invoke();
        }

        public void Complete()
        {
            IsDone = true;
            onComplete?.Invoke();
        }

        public STask OnComplete(SAction action)
        {
            onComplete += action;
            return this;
        }

        public STask OnUpdate(SAction action)
        {
            onUpdate += action;
            return this;
        }

        public void Kill()
        {
            IsDone = true;
        }

        private bool HasTimedOut()
        {
            return _timeout > 0 && ElapsedTime > _timeout;
        }
    }
}