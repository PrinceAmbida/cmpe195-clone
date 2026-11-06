// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to detect wall collisions
public class WallCollide : MonoBehaviour
{
    [SerializeField] GameObject displayMessage;
    [SerializeField] GameObject displayTime;
	[SerializeField] GameObject menuButton;
	[SerializeField] GameObject message;
	[SerializeField] GameObject fadeBlack;
    [SerializeField] AudioSource boomSound;

	//If player hits wall, restart game
    void OnTriggerEnter(Collider other)
    {
		Debug.Log("Triggered by: " + other.name + " | Tag: " + other.tag);
		
		if (other.CompareTag("Player"))
		{
			Debug.Log("Play tune");
			boomSound.Play();
			displayTime.SetActive(false);
			displayMessage.GetComponent<TMPro.TMP_Text>().text = "Avoid the walls!\nDon't click suspicious links or websites!\nBe more careful next time";
			fadeBlack.SetActive(true);
			menuButton.SetActive(true);
			message.SetActive(true);
			Invoke(nameof(Restart), 5f);
		}
    }
	
	//Reloads scene to restart level
	void Restart()
	{
		DataMovement.RestartLevel();
	}
}
