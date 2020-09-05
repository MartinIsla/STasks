namespace Koffie.SimpleTasks
{
    public class DoTask : STask
    {
        public DoTask(STaskSettings settings) : base(settings) { }

        protected override float GetProgress()
        {
            return ElapsedTime / Delay;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (ElapsedTime > Delay)
            {
                Complete();
            }
        }
    }
}