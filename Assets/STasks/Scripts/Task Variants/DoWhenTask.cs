using UnityEngine;

namespace Koffie.SimpleTasks
{
    public class DoWhenTask : STask
    {
        public SCondition condition;

        public DoWhenTask(STaskSettings settings) : base(settings)
        {
            this.condition = settings.condition;
        }

        protected override float GetProgress()
        {
            Debug.LogWarning("DoWhen can't keep track of the progress, you'll need to write your own implementation for this. Returning -1.");
            return -1;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (condition())
            {
                Complete();
            }
        }
    }
}