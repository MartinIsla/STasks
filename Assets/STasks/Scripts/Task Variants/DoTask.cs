namespace Koffie.SimpleTasks
{
    public class DoTask : STask
    {
        public DoTask(in STaskSettings settings) : base(settings) { }

        protected override float GetProgress()
        {
            return ElapsedTime / Delay;
        }

        protected override void OnUpdate()
        {
            if (ElapsedTime > Delay)
            {
                Complete();
            }
        }
    }
}