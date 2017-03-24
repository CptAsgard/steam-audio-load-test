using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections;

using Debug = UnityEngine.Debug;

public class LoadTimeLogger : MonoBehaviour
{
	[SerializeField]
	private string[] m_ScenesToLoad;
	[SerializeField]
	private Text m_Text;

	private Stopwatch m_Stopwatch;

	protected void Start()
	{
		StartCoroutine(Load());
	}

	private IEnumerator Load()
	{
		m_Stopwatch = new Stopwatch();
		m_Stopwatch.Start();

		for (int i = 0; i < m_ScenesToLoad.Length; i++)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var op = SceneManager.LoadSceneAsync(m_ScenesToLoad[i], LoadSceneMode.Additive);
			op.allowSceneActivation = false;

			while (op.progress < 0.9f) // magic value before it hits the scene initialization point
			{
				yield return null;
			}

			stopwatch.Stop();
			Debug.Log("Load time (ms): " + stopwatch.ElapsedMilliseconds);

			stopwatch.Reset();
			stopwatch.Start();

			op.allowSceneActivation = true;
			yield return op;

			stopwatch.Stop();
			Debug.Log("Scene activation time (ms): " + stopwatch.ElapsedMilliseconds);
		}

		m_Stopwatch.Stop();
		LogTotalTime();
	}

	private void LogTotalTime()
	{
		var timeString = "Time elapsed (ms): " + m_Stopwatch.ElapsedMilliseconds;
		Debug.Log(timeString);
		m_Text.text = timeString;
	}
}
