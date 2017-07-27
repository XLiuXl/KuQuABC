using UnityEngine;
using System.Reflection;
using Mono.CSharp;

namespace TNet
{
/// <summary>
/// Run-time code execution. Requires Mono.CSharp.dll to be included as a plugin.
/// </summary>

public static class RuntimeCode
{
	/// <summary>
	/// Set to 'true' before executing code, then 'false' after code has been executed.
	/// </summary>

	static internal bool isExecuting = false;

#if !UNITY_ANDROID
	static Assembly[] mCachedAssemblies = null;
#endif
	/// <summary>
	/// Execute the code within the specified file.
	/// </summary>

	static public object ExecuteFile (string path)
	{
		string code = Tools.ReadTextFile(path);
		if (!string.IsNullOrEmpty(code)) return Execute(code);
		Debug.LogError("Can't open " + path);
		return null;
	}

	/// <summary>
	/// Execute the specified code.
	/// </summary>

	static public object Execute (string code)
	{
#if !UNITY_ANDROID
		if (mCachedAssemblies == null)
		{
			mCachedAssemblies = TypeExtensions.GetAssemblies();

			Mono.CSharp.Evaluator.Init(new string[] { });

			for (int i = 0, imax = mCachedAssemblies.Length; i < imax; ++i)
			{
				Assembly assembly = mCachedAssemblies[i];
				if (assembly.FullName.Contains("mscorlib")) continue;
				if (assembly.FullName.Contains("UnityEditor")) continue;
				if (assembly.FullName.Contains("Cecil")) continue;

				try { Mono.CSharp.Evaluator.ReferenceAssembly(assembly); }
				catch (System.Exception) {}
			}

			isExecuting = true;
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder("");
				sb.AppendLine("using System;");
				sb.AppendLine("using UnityEngine;");
				sb.AppendLine("using TNet;");
				Mono.CSharp.Evaluator.Compile(sb.ToString());
			}
			isExecuting = false;
		}

		if (string.IsNullOrEmpty(code)) return null;
		if (code[code.Length - 1] != ';') code += ";";
#if UNITY_EDITOR
		Debug.Log("Executing:\n" + code);
#endif
		object result = null;
		bool result_set = false;
		isExecuting = true;

		try
		{
			string s = Mono.CSharp.Evaluator.Evaluate("{\n" + code + "\n}", out result, out result_set);
			if (!result_set && !string.IsNullOrEmpty(s)) Debug.LogError("Syntax error: " + s);
		}
		catch (System.Exception ex)
		{
#if UNITY_EDITOR
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);
#else
			Debug.LogError(ex.Message);
#endif
		}
		isExecuting = false;
		return result_set ? result : null;
#else
		return null;
#endif
	}
}
}
