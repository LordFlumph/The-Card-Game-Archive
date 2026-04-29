namespace CardGameArchive
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
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

		bool processing = false;

		void Awake()
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

		void OnEnable()
		{
			OnTasksFinished += SaveManager.Save;
			ProcessTasks();
		}
		void OnDisable()
		{
			OnTasksFinished -= SaveManager.Save;
			processing = false;
		}

		async void ProcessTasks()
		{
			processing = true;
			while (processing)
			{
				if (activeTasks.Count > 0)
				{
					for (int i = 0; i < activeTasks.Count; i++)
					{
						if (activeTasks[i] != null)
						{
							if (activeTasks[i].IsCompletedSuccessfully)
							{
								activeTasks.RemoveAt(i);
								i--;
							}
							else if (activeTasks[i].IsFaulted)
							{
								Debug.LogError($"Task {activeTasks[i].Id} threw an exception: {activeTasks[i].Exception}");
								activeTasks.RemoveAt(i);
								i--;
							}
						}
						else
						{
							activeTasks.RemoveAt(i);
							i--;
						}						
					}

					if (taskQueue.Count > 0)
					{
						AddTask(taskQueue.Dequeue().Invoke());
					}
					else if (activeTasks.Count == 0)
					{
						OnTasksFinished?.Invoke();
					}
				}
				else if (taskQueue.Count > 0)
				{
					AddTask(taskQueue.Dequeue().Invoke());
				}

				await Awaitable.NextFrameAsync();
			}
		}

		public void AddTask(Task task)
		{
			activeTasks.Add(task);
			OnTaskAdded?.Invoke();
		}

		public async Task WhenAll()
		{
			await Task.WhenAll(activeTasks);
		}

		public void QueueTask(Func<Task> task)
		{
			taskQueue.Enqueue(task);
		}
	}

}