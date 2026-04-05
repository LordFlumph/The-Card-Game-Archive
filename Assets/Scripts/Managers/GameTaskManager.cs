namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using UnityEngine;

    public class GameTaskManager : MonoBehaviour
    {
        public static GameTaskManager Instance { get; private set; }

        List<Task> activeTasks = new();
        Queue<Func<Task>> taskQueue = new();

        public int TaskCount => activeTasks.Count + taskQueue.Count;

        public event Action OnTaskAdded;
        public event Action OnTasksFinished;

        public void Awake()
        {
            if (Instance == null)
            {
				Instance = this;
			}                
            else
            {
				Destroy(gameObject);
			}                
		}

        async void Update()
        {
            if (activeTasks.Count > 0)
            {
                await Task.WhenAll(activeTasks);
                activeTasks.Clear();
                if (taskQueue.Count > 0)
                {
                    activeTasks.Add(taskQueue.Dequeue().Invoke());
                }
                else
                {
					OnTasksFinished?.Invoke();
				}                
			}
        }

        public async Task AddTask(Task task)
        {
            activeTasks.Add(task);
            OnTaskAdded?.Invoke();
            await WhenAll();
        }

        public async Task WhenAll()
        {
            await Task.WhenAll(activeTasks);
		}

        public async Task QueueTask(Func<Task> task)
        {
            taskQueue.Enqueue(task);
            await WhenAll();
        }
    }

}