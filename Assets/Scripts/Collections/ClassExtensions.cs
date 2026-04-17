using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is a class that contains helpful extensions on some of Unity's and C#'s default classes
/// </summary>
public static class ClassExtensions
{
    #region Vector3
    public static Vector3 xy(this Vector3 vector)
	{
		return new Vector3(vector.x, vector.y, 0);
	}
	public static Vector3 xy(this Vector3 vector, float z)
	{
		return new Vector3(vector.x, vector.y, z);
	}

	public static Vector3 xz(this Vector3 vector)
	{
		return new Vector3(vector.x, 0, vector.z);
	}
	public static Vector3 xz(this Vector3 vector, float y)
	{
		return new Vector3(vector.x, y, vector.z);
	}

	public static Vector3 yz(this Vector3 vector)
	{
		return new Vector3(0, vector.y, vector.z);
	}
	public static Vector3 yz(this Vector3 vector, float x)
	{
		return new Vector3(x, vector.y, vector.z);
	}


	public static float[] ToArray(this Vector3 vector)
	{
		return new float[3] { vector.x, vector.y, vector.z };
	}

	


	public static bool CloseEnough(this Vector3 vector, Vector3 otherVector, float maxDistance)
	{
		return Vector3.Distance(vector, otherVector) <= maxDistance;
	}
    #endregion

    #region Vector2
    public static Vector2 ClosestDirection(this Vector2 vector)
	{
		Vector2 direction = Vector2.up;
		if (Vector2.Distance(vector, Vector2.right) < Vector2.Distance(vector, direction))
			direction = Vector2.right;
		if (Vector2.Distance(vector, Vector2.down) < Vector2.Distance(vector, direction))
			direction = Vector2.down;
		if (Vector2.Distance(vector, Vector2.left) < Vector2.Distance(vector, direction))
			direction = Vector2.left;

		return direction;
	}

	public static Vector3 ToVector3(this Vector2 vector)
	{
		return new Vector3(vector.x, vector.y, 0);
	}
	#endregion

	#region List
	/// <summary>
	/// Randomise the order of this list
	/// </summary>
	public static void Shuffle<T>(this List<T> list)
	{
		if (list == null || list.Count <= 1)
			return;

		int n = list.Count;
		System.Random rng = new System.Random();
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	/// <summary>
	/// Remove all null references from this list
	/// </summary>
	public static void ClearNull<T>(this List<T> list) where T : class
	{		
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == null)
            {
                list.RemoveAt(i);
            }
        }
    }
	#endregion

	#region Array
	public static List<T> ToList<T>(this T[] array)
    {
		List<T> list = new List<T>();
		foreach (T item in array)
        {
			list.Add(item);
        }

		return list;
    }
    #endregion

    #region Transform
	public static void DestroyChildren(this Transform transform)
    {
		for (int i = transform.childCount-1; i >= 0; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }

	public static Transform GetBottomChild(this Transform transform, int initialIndex = 0, int subsequentIndex = 0)
	{
		if (initialIndex < 0)
		{
			Debug.LogWarning("Child index cannot be negative");
			return transform;
		}
		
		if (transform.childCount != 0)
			transform = transform.GetChild(initialIndex);

		while (transform.childCount > subsequentIndex)
		{
			transform = transform.GetChild(subsequentIndex);
		}

		return transform;
	}

	public static T GetLastComponentInChildren<T>(this Transform transform, bool includeParent = false) where T : Component
	{
		List<Transform> allChildren = transform.GetAllChildren(includeParent);
		for (int i = allChildren.Count - 1; i >= 0; --i)
		{
			if (allChildren[i].TryGetComponent(out T component))
			{
				return component;
			}
		}

		return null;
	}

	public static List<Transform> GetAllChildren(this Transform transform, bool includeParent = false)
	{
		List<Transform> children = new List<Transform>();

		if (includeParent)
			children.Add(transform);

		foreach (Transform child in transform)
		{
			children.Add(child);

			if (child.childCount > 0)
				children.AddRange(child.GetAllChildren());
		}

		return children;
	}
    #endregion

    #region String
    public static string SeparateWords(this string words)
    {
		for (int i = 1; i < words.Length; i++)
		{
			if (char.IsUpper(words[i]))
			{
				words = words.Insert(i, " ");
				i++;
			}
		}

		return words;
	}
	#endregion

	public static async Task FadeIn(this CanvasGroup group, float timeToFade, bool setInteractable = true, bool fadeParent = false)
	{
		if (fadeParent)
		{
			List<CanvasGroup> parents = group.GetComponentsInParent<CanvasGroup>().ToList();
			parents.Remove(group);

			foreach (var parent in parents)
			{
				parent.FadeIn(timeToFade, setInteractable, false);
			}
		}		

		if (setInteractable)
		{
			group.blocksRaycasts = true;
		}

		if (timeToFade > 0)
		{
			while (group.alpha < 1)
			{
				group.alpha += Time.deltaTime / timeToFade;
				await Task.Yield();
			}
		}
		
		group.alpha = 1;

		if (setInteractable)
		{
			group.interactable = true;
		}
	}
	public static async Task FadeOut(this CanvasGroup group, float timeToFade, bool setInteractable = true, bool fadeParent = false)
	{
		if (setInteractable)
		{
			group.interactable = false;			
		}

		if (fadeParent)
		{
			List<CanvasGroup> parents = group.GetComponentsInParent<CanvasGroup>().ToList();
			parents.Remove(group);

			foreach (var parent in parents)
			{
				parent.FadeOut(timeToFade, setInteractable, false);
			}
		}

		if (timeToFade > 0)
		{
			while (group.alpha > 0)
			{
				group.alpha -= Time.deltaTime / timeToFade;
				await Task.Yield();
			} 
		}
		group.alpha = 0;		

		if (setInteractable)
		{
			group.blocksRaycasts = false;
		}
	}
}