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

        public void Update()
        {
            STask task;
            for (int i = 0; i < tasks.Count; i++)
            {
                task = tasks[i];
                while (task.isDone && i < tasks.Count)
                {
                    task = tasks[i] = tasks[tasks.Count - 1];
                    tasks.RemoveAt(tasks.Count - 1);
                }

                if (!task.isDone)
                {
                    task.Update();
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