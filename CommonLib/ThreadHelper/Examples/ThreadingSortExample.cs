using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThreadingSortExample : MonoBehaviour
{
    private bool isCalculating = false;
    private System.Random random = new System.Random();
    private int sortedArrays = 0;
	private float startTime;

    void OnGUI()
    {
        GUI.enabled = !isCalculating;

        GUILayout.Label("This sample sorts 1000 arrays with each 10.000 elements.");
        if (sortedArrays > 0)
		{
            GUILayout.Label("Sorted arrays: " + sortedArrays);
			GUILayout.Label("Sorted arrays per second: " + sortedArrays / (Time.time - startTime));
			GUILayout.Label("Estimated needed seconds: " + 1000 / (sortedArrays / (Time.time - startTime)));
			GUILayout.Label("Elapsed seconds: " + (Time.time - startTime));
		}

        if (GUILayout.Button("Start NonThreaded Sort"))
        {
			startTime = Time.time;
            isCalculating = true;
            sortedArrays = 0;
            StartCoroutine("NonThreadedSort");
        }
        if (GUILayout.Button("Start Threaded Sort"))
        {
			startTime = Time.time;
            isCalculating = true;
            sortedArrays = 0;
            var arrays = CreateRandomizedArray();

            UnityThreadHelper.CreateThread(() =>
                {
                    for (int i = 0; i < arrays.Length; ++i)
                    {
                        System.Array.Sort(arrays[i]);
                        System.Threading.Interlocked.Increment(ref sortedArrays);
                    }

                    isCalculating = false;
                    sortedArrays = 0;
                });
        }
        if (GUILayout.Button("Start TaskDistributor Sort"))
        {
			startTime = Time.time;
            isCalculating = true;
            sortedArrays = 0;

            var arrays = CreateRandomizedArray();
            for (int i = 0; i < arrays.Length; ++i)
            {
                var index = i;
                
                UnityThreadHelper.TaskDistributor.Dispatch(() =>
                {
                    System.Array.Sort(arrays[index]);
                    System.Threading.Interlocked.Increment(ref sortedArrays);

                    if (sortedArrays == arrays.Length)
                    {
                        isCalculating = false;
                        sortedArrays = 0;
                    }
                });
            }
        }
        if (GUILayout.Button("Back to Example Selection"))
        {
            this.gameObject.AddComponent<UnityThreadingExamplesMenu>();
            DestroyImmediate(this);
        }

        GUI.enabled = true;
    }

    IEnumerator NonThreadedSort()
    {
        var arrays = CreateRandomizedArray();
        for (int i = 0; i < arrays.Length; ++i)
        {
            System.Array.Sort(arrays[i]);
            sortedArrays++;
            yield return 1;
        }

        isCalculating = false;
        sortedArrays = 0;
    }

    int[][] CreateRandomizedArray()
    {
        int[][] unsortedArray = new int[1000][];
        for (int i = 0; i < unsortedArray.Length; ++i)
        {
            unsortedArray[i] = new int[10000];
            for (int k = 0; k < unsortedArray[i].Length; ++k)
            {
                unsortedArray[i][k] = random.Next(10000);
            }
        }
        return unsortedArray;
    }
}
