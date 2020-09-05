using System;
using UnityEngine;

namespace Koffie.SimpleTasks
{
    public class STasksCollection
    {        
        public STask[] tasks;

        private int _tasksAdded;

        private readonly int initialSize;
        private readonly int maxSize;

        public STasksCollection(int initialSize, int maxSize)
        {
            this.initialSize = initialSize;
            this.maxSize = maxSize;
            
            tasks = new STask[initialSize];
        }

        public void Update (float deltaTime)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                STask task = tasks[i];
                if (task != null)
                {
                    if (!task.isDone)
                    {
                        task.Update(deltaTime);
                    }
                    else
                    {
                        tasks[i] = null;
                    }
                }
            }
        }

        public void AddTask(STask task)
        {
            if (_tasksAdded == tasks.Length)
            {
                IncreaseArraySize();
            }

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i] == null)
                {
                    tasks[i] = task;
                    _tasksAdded++;
                    return;
                }
            }
        }

        private void IncreaseArraySize()
        {
            if (tasks.Length == maxSize)
            {
                Debug.LogError($"For your own safety, we can't increase the STasks capacity because you already created {maxSize} tasks. If you know what you're doing, you can increase MAX_CAPACITY in STasks.cs, but keep an eye on your performance.");
                return;
            }

            int newCapacity = Mathf.Min(tasks.Length + initialSize, maxSize);
            Array.Resize(ref tasks, newCapacity);
        }
    }

}