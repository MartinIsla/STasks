namespace Koffie.SimpleTasks
{
    public class DoRepeatingTask : STask
    {
        public float frequency;

        private float _timeSinceLastLoop;

        public DoRepeatingTask(in STaskSettings settings) : base(settings)
        {
            this.frequency = settings.frequency;
            _timeSinceLastLoop = 0;
        }

        public override void Restart()
        {
            base.Restart();
            _timeSinceLastLoop = 0;
        }

        protected override void OnTimeout()
        {
            onTimeout?.Invoke();
            Complete();
        }

        public override void Complete()
        {
            onComplete?.Invoke();
            isDone = true;
        }

        protected override float GetProgress()
        {
            return _timeSinceLastLoop / frequency;
        }

        protected override void OnUpdate()
        {
            _timeSinceLastLoop += deltaTime;

            if (_timeSinceLastLoop > frequency)
            {
                action.Invoke();
                _timeSinceLastLoop -= frequency;
            }
        }
    }
}
