using UnityEngine;

namespace TNet
{
/// <summary>
/// MonoBehaviour with delegate functions that can be assigned from code compiled at run-time.
/// </summary>

public class RuntimeBehaviour : MonoBehaviour
{
	static System.Collections.Generic.Dictionary<string, RuntimeBehaviour> mDict =
		new System.Collections.Generic.Dictionary<string, RuntimeBehaviour>();

	public delegate void Callback (RuntimeBehaviour cb);

	/// <summary>
	/// Callback that will be executed in Start().
	/// </summary>

	public Callback onStart;

	/// <summary>
	/// Callback that will be executed on Update().
	/// </summary>

	public Callback onUpdate;

	/// <summary>
	/// Callback that will be executed on FixedUpdate().
	/// </summary>

	public Callback onFixedUpdate;

	/// <summary>
	/// Callback that will be executed in OnDestroy().
	/// </summary>

	public Callback onDestroy;

	/// <summary>
	/// Callback that will be executed when the Runtime Behaviour's Custom() function is called.
	/// </summary>

	public Callback onCustom;

	[System.NonSerialized] string mName;

	void Start ()
	{
		if (onStart != null) onStart(this);
		if (onStart == null && onUpdate == null && onFixedUpdate == null && onCustom == null) Destroy(gameObject);
	}

	void Update ()
	{
		if (onUpdate != null) onUpdate(this);
		if (onStart == null && onUpdate == null && onFixedUpdate == null && onCustom == null) Destroy(gameObject);
	}

	void FixedUpdate ()
	{
		if (onFixedUpdate != null) onFixedUpdate(this);
		if (onStart == null && onUpdate == null && onFixedUpdate == null && onCustom == null) Destroy(gameObject);
	}

	void OnDestroy ()
	{
		if (onDestroy != null) onDestroy(this);
		if (!string.IsNullOrEmpty(mName)) mDict.Remove(mName);
	}

	/// <summary>
	/// Call this function yourself or subscribe it to a delegate. It won't be called otherwise.
	/// </summary>

	public void Custom ()
	{
		if (onCustom != null) onCustom(this);
		if (onStart == null && onUpdate == null && onFixedUpdate == null && onCustom == null) Destroy(gameObject);
	}

	/// <summary>
	/// Create a new Runtime Behaviour or return an existing one if the name already exists.
	/// You must assign at least one of the delegates using the returned value or the
	/// Runtime Behaviour will destroy itself in its Start() function.
	/// </summary>

	static public RuntimeBehaviour Create (string name = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			GameObject go = new GameObject();
			go.name = "CB: " + go.GetInstanceID();
			return go.AddComponent<RuntimeBehaviour>();
		}
		else
		{
			RuntimeBehaviour val;
			if (mDict.TryGetValue(name, out val)) return val;

			GameObject go = new GameObject("CB: " + name);
			val = go.AddComponent<RuntimeBehaviour>();
			val.mName = name;
			mDict[name] = val;
			return val;
		}
	}

	/// <summary>
	/// Get an existing Runtime Behaviour.
	/// </summary>

	static public RuntimeBehaviour Get (string name)
	{
		if (name != null)
		{
			RuntimeBehaviour val;
			if (mDict.TryGetValue(name, out val)) return val;
		}
		return null;
	}
}
}
