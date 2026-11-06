// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataMovement : MonoBehaviour
{
	public static DataMovement Instance;
	
    public static int count = 9;
	public bool ended = false;

	[SerializeField] float time = 60f;

	//Used for player movement in phishing level
    [SerializeField] float speed = 2;
    [SerializeField] float leftLimit = -5f;
    [SerializeField] float rightLimit = 2f;
    [SerializeField] float upLimit = 20f;
    [SerializeField] float downLimit = 250f;
	
    [SerializeField] GameObject displayTime;
    [SerializeField] GameObject displayMessage;
	[SerializeField] GameObject menuButton;
	[SerializeField] GameObject message;
	[SerializeField] GameObject fadeBlack;
    [SerializeField] AudioSource endSound;
    [SerializeField] AudioSource background;

    void Awake()
    {
		Instance = this;
    }

	//Updates timer and player movement
    void Update()
    {
		//Checks for time remaining and ends current game if no time left, automatically restarts
		if (time > 0)
		{
			time -= Time.deltaTime;
			displayTime.GetComponent<TMPro.TMP_Text>().text = "Time Remaining: " + time.ToString("F0") + " s";
		}
		
		if (time <= 0 && ended == false)
		{
			ended = true;
			background.Stop();
			Debug.Log("Play tune");
			Debug.Log("Play tune");
			endSound.Play();
			displayTime.SetActive(false);
			displayMessage.GetComponent<TMPro.TMP_Text>().text = "Out of time!\nSpeed matters for data transfer\nTry to be faster next time";
			fadeBlack.SetActive(true);
			menuButton.SetActive(true);
			message.SetActive(true);
			Invoke(nameof(Restart), 5f);
		}


		//Show player movement based on keyboard input
		float horizontal = 0;

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			horizontal = -1;
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			horizontal = 1;
		}

		float vertical = 0;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			vertical = 1;
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			vertical = -1;
		}

		Vector3 movement = new Vector3(horizontal, 0f, vertical) * Time.deltaTime * speed;

		Vector3 newPos = transform.position + movement;

		// Prevents from exceeding limits
		newPos.x = Mathf.Clamp(newPos.x, leftLimit, rightLimit);
		newPos.z = Mathf.Clamp(newPos.z, upLimit, downLimit);

		transform.position = newPos;
    }
	
	//Used to call restart method from Unity function
	void Restart()
	{
		RestartLevel();
	}

	//Actual restart method
	//Removes messages and restarts level
	public static void RestartLevel()
	{
		Instance.fadeBlack.SetActive(false);
		Instance.displayMessage.SetActive(false);
		Instance.displayTime.SetActive(true);
		SceneManager.LoadSceneAsync("PhishingScene");
	}
}