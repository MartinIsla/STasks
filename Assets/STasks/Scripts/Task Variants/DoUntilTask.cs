using UnityEngine;

namespace Koffie.SimpleTasks
{
    public class DoUntilTask : DoRepeatingTask
    {
        private readonly SCondition _condition;

        public DoUntilTask(in STaskSettings settings) : base(settings)
        {
            _condition = settings.condition;
        }

        protected override void OnTimeout()
        {
            onTimeout?.Invoke();
            isDone = true;
        }

        protected override float GetProgress()
        {
            Debug.LogWarning($"{nameof(DoUntilTask)} can't keep track of the progress, you'll need to write your own implementation for this. Returning -1.");
            return -1;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_condition())
            {
                Complete();
            }
        }
    }
}
