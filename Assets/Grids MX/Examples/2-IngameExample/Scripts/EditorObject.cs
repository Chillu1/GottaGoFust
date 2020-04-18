using UnityEngine;
using System.Collections;

public class EditorObject : MonoBehaviour
{
	private void Awake()
	{
		this.gameObject.SetActive(false);
	}
}
