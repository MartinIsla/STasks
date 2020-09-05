namespace Koffie.SimpleTasks
{
    public class DoAfterFramesTask : STask
    {
        public int ElapsedFrames => _elapedFrames;

        private int _targetFrames;
        private int _elapedFrames;

        public DoAfterFramesTask(STaskSettings settings) : base(settings)
        {
            _targetFrames = settings.targetFrames;
        }

        protected override float GetProgress()
        {
            return (float)_elapedFrames / _targetFrames;
        }

        protected override void OnUpdate(float deltaTime)
        {
            _elapedFrames++;

            if (_elapedFrames == _targetFrames)
            {
                Complete();
            }
        }
    }
}