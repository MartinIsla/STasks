namespace Koffie.SimpleTasks
{
    public class DoRepeatingTask : STask
    {
        public float frequency;

        private float _timeSinceLastLoop;

        public DoRepeatingTask(STaskSettings settings) : base(settings)
        {
            this.frequency = settings.frequency;

            _timeSinceLastLoop = 0;
        }

        protected override float GetProgress()
        {
            return _timeSinceLastLoop / frequency;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (_timeSinceLastLoop > frequency)
            {
                action.Invoke();
                _timeSinceLastLoop -= frequency;
            }
            else
            {
                _timeSinceLastLoop += deltaTime;
            }
        }
    }
}
