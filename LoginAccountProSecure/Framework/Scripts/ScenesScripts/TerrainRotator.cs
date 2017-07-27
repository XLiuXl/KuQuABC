using UnityEngine;
using System.Collections;

public class TerrainRotator : MonoBehaviour
{
	public int rotateSpeed = 30;

	void FixedUpdate ()
	{
		// Rotate the terrain
		transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
	}
}
