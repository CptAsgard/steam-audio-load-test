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
	private int m_Counter = 0;

	protected void Start()
	{
		m_Stopwatch = new Stopwatch();
		m_Stopwatch.Start();

		SceneManager.sceneLoaded += StopAndLog;

		StartCoroutine(Load());
	}

	private IEnumerator Load()
	{
		for (int i = 0; i < m_ScenesToLoad.Length; i++)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var op = SceneManager.LoadSceneAsync(m_ScenesToLoad[i], LoadSceneMode.Additive);
			op.allowSceneActivation = false;

			while (op.progress < 0.9f)
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
	}

	private void StopAndLog(Scene a_Scene, LoadSceneMode a_LoadSceneMode)
	{
		m_Counter++;
		if (m_Counter >= m_ScenesToLoad.Length)
		{
			m_Stopwatch.Stop();

			var elapsedTime = m_Stopwatch.ElapsedMilliseconds;
			var timeString = "Time elapsed (ms): " + elapsedTime;

			Debug.Log(timeString);
			m_Text.text = timeString;
		}
	}
}
