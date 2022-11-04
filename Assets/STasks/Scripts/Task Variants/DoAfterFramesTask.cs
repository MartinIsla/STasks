﻿namespace Koffie.SimpleTasks
{
    public class DoAfterFramesTask : STask
    {
        public int ElapsedFrames => _elapsedFrames;

        private int _targetFrames;
        private int _elapsedFrames;

        public DoAfterFramesTask(in STaskSettings settings) : base(settings)
        {
            _targetFrames = settings.targetFrames;
        }

        protected override float GetProgress()
        {
            return (float)_elapsedFrames / _targetFrames;
        }

        protected override void OnUpdate()
        {
            _elapsedFrames++;

            if (_elapsedFrames >= _targetFrames)
            {
                Complete();
            }
        }
    }
}
