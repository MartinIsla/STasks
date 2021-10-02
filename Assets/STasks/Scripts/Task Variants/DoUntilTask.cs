using UnityEngine;

namespace Koffie.SimpleTasks
{
    public class DoUntilTask : DoRepeatingTask
    {
        private readonly SCondition _condition;

        public DoUntilTask(STaskSettings settings) : base(settings)
        {
            _condition = settings.condition;
        }

        protected override float GetProgress()
        {
            Debug.LogWarning($"{nameof(DoUntilTask)} can't keep track of the progress, you'll need to write your own implementation for this. Returning -1.");
            return -1;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (_condition())
            {
                Kill();
                onComplete?.Invoke();
            }
        }
    }
}
