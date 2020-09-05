namespace Koffie.SimpleTasks
{
    public class DoUntilTask : DoRepeatingTask
    {
        private SCondition condition;

        public DoUntilTask(STaskSettings settings) : base(settings)
        {
            condition = settings.condition;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (condition())
            {
                Complete();
            }
        }
    }
}