using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
	public static Vector3 xz(this Vector3 vector)
	{
		return new Vector3(vector.x, 0, vector.z);
	}
	public static Vector3 yz(this Vector3 vector)
	{
		return new Vector3(0, vector.y, vector.z);
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
}