// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Used for player movement in spoofing level
public class SpoofMovement : MonoBehaviour
{
	public static SpoofMovement Instance;
	
    public static bool ended = false;

	//Player speed setup
    [SerializeField] float speed = 5;
    [SerializeField] float leftLimit = -4f;
    [SerializeField] float rightLimit = 3f;
	
    [SerializeField] GameObject displayMessage;
	[SerializeField] GameObject menuButton;
	[SerializeField] GameObject message;
	[SerializeField] GameObject fadeBlack;
    [SerializeField] AudioSource background;

    void Awake()
    {
		Instance = this;
    }

	//Show player movement based on keyboard input
    void Update()
    {
		if (!ended) {
			float horizontal = 0;

			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				horizontal = -4;
			}
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				horizontal = 4;
			}
			
			//Set speed for going towards choices, no player choice
			float vertical = 1f;

			Vector3 movement = new Vector3(horizontal, 0f, vertical) * Time.deltaTime * speed;

			Vector3 newPos = transform.position + movement;

			// Prevents from exceeding limits
			newPos.x = Mathf.Clamp(newPos.x, leftLimit, rightLimit);

			transform.position = newPos;
		}
    }
	
	//Used to call restart method from Unity function
	void Restart()
	{
		RestartLevel();
	}

	//Actual restart method
	//Resets values and restarts level
	public static void RestartLevel()
	{
		Instance.fadeBlack.SetActive(false);
		Instance.displayMessage.SetActive(false);
		SituationChange.situationNum = 0;
		ended = false;
		SceneManager.LoadSceneAsync("SpoofingScene");
	}
}