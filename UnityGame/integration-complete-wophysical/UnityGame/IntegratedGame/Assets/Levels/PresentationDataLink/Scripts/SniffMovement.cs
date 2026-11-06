using UnityEngine;
using UnityEngine.SceneManagement;

public class SniffMovement : MonoBehaviour
{
	public static SniffMovement Instance;

	public static bool ended;

	[SerializeField]
	private float speed = 2f;

	[SerializeField]
	private float upLimit = 1.7f;

	[SerializeField]
	private float downLimit = -1.7f;

	[SerializeField]
	private GameObject displayMessage;

	[SerializeField]
	private GameObject menuButton;

	[SerializeField]
	private GameObject message;

	[SerializeField]
	private GameObject fadeBlack;

	[SerializeField]
	private AudioSource background;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (!ended)
		{
			float z = 0f;
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				z = 2f;
			}
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				z = -2f;
			}
			Vector3 vector = new Vector3(0f, 0f, z) * Time.deltaTime * speed;
			Vector3 position = base.transform.position + vector;
			base.transform.position = position;
		}
	}

	private void Restart()
	{
		RestartLevel();
	}

	public static void RestartLevel()
	{
		Instance.fadeBlack.SetActive(value: false);
		Instance.displayMessage.SetActive(value: false);
		SituationChange.situationNum = 0;
		ended = false;
		SceneManager.LoadSceneAsync("SniffingScene");
	}
}
