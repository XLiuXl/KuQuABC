using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityThreading;

public class ThreadingUnityCallsExample : MonoBehaviour
{
    private bool isCalculating = false;
    private System.Random random = new System.Random();
	private float startTime;
	private int createdElements;
	
    void OnGUI()
    {
        GUI.enabled = !isCalculating;

        GUILayout.Label("This sample will simulate a long going calculation of 50 values (each calculation will take 200ms),\nafter each simulated calculation a cube will be created.");
        GUILayout.Label("Frametime: " + (int)(Time.deltaTime * 1000) + "ms");
		if (isCalculating)
		{
			GUILayout.Label("Created elements arrays per second: " + createdElements / (Time.time - startTime));
			GUILayout.Label("Estimated needed seconds: " + 50 / (createdElements / (Time.time - startTime)));
			GUILayout.Label("Elapsed seconds: " + (Time.time - startTime));
		}
		
        if (GUILayout.Button("Start NonThreaded Calculation"))
        {
			createdElements = 0;
			startTime = Time.time;
            isCalculating = true;
            StartCoroutine("NonThreadedCalculation");
        }
        if (GUILayout.Button("Start Threaded Calculation"))
        {
			createdElements = 0;
			startTime = Time.time;
            isCalculating = true;
            UnityThreadHelper.CreateThread(() =>
            {
                ThreadedCalculation();
            });
        }
        if (GUILayout.Button("Start Threaded Task Calculation"))
        {
			createdElements = 0;
			startTime = Time.time;
            isCalculating = true;
            UnityThreadHelper.CreateThread(() =>
            {
                return ThreadedYieldCalculation();
            });
        }
        if (GUILayout.Button("Back to Example Selection"))
        {
            this.gameObject.AddComponent<UnityThreadingExamplesMenu>();
            DestroyImmediate(this);
        }

        GUI.enabled = true;
    }

    IEnumerator NonThreadedCalculation()
    {
        for (int i = 0; i < 50; ++i)
        {
            System.Threading.Thread.Sleep(200); // simulate calculation

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position += Vector3.forward * i;
            cube.name = "Cube" + (i + 1).ToString();
            Destroy(cube, 1.0f);
			System.Threading.Interlocked.Increment(ref createdElements);
            
            yield return 1;
        }
        isCalculating = false;
    }

    void ThreadedCalculation()
    {
        for (int i = 0; i < 50; ++i)
        {
            System.Threading.Thread.Sleep(200); // simulate calculation

            UnityThreadHelper.Dispatcher.Dispatch(() =>
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position += Vector3.forward * i;
                    cube.name = "Cube" + (i + 1).ToString();
                    Destroy(cube, 1.0f);
					System.Threading.Interlocked.Increment(ref createdElements);
                }).Wait();
        }
        isCalculating = false;
    }

    IEnumerator ThreadedYieldCalculation()
    {
        for (int i = 0; i < 50; ++i)
        {
            var index = i;
            System.Threading.Thread.Sleep(200); // simulate calculation
			System.Threading.Interlocked.Increment(ref createdElements);
            yield return Task.Create(() =>
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position += Vector3.forward * index;
                    cube.name = "Cube" + (index + 1).ToString();
                    Destroy(cube, 1.0f);
                });
        }
        isCalculating = false;
    }
}
