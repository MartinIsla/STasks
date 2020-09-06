using System.Collections.Generic;

namespace Koffie.SimpleTasks
{
    public class STasksCollection
    {
        private List<STask> tasks;

        public STasksCollection(int initialSize)
        {
            tasks = new List<STask>(initialSize);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                STask task = tasks[i];
                while (task.isDone && i < tasks.Count)
                {
                    task = tasks[i] = tasks[tasks.Count - 1];
                    tasks.RemoveAt(tasks.Count - 1);
                }

                if (!task.isDone)
                {
                    task.Update(deltaTime);
                }
            }
        }

        public void AddTask(STask task)
        {
            tasks.Add(task);
        }

        public void RemoveTask(STask task)
        {
            int index = tasks.IndexOf(task);

            if (index >= 0)
            {
                tasks[index] = tasks[tasks.Count - 1];
                tasks.RemoveAt(tasks.Count - 1);
            }
        }
    }

}