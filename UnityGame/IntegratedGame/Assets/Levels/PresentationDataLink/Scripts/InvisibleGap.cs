// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to prevent player from not choosing any door
//Gap between doors will restart level
public class InvisibleGap : MonoBehaviour
{
    [SerializeField] GameObject displayMessage;
    [SerializeField] GameObject displayTime;
	[SerializeField] GameObject menuButton;
	[SerializeField] GameObject message;
	[SerializeField] GameObject fadeBlack;
    [SerializeField] AudioSource boomSound;
	[SerializeField] GameObject situations;

	//Display error message if player collides with gap
    void OnTriggerEnter(Collider other)
    {
		Debug.Log("Triggered by: " + other.name + " | Tag: " + other.tag);
		
		if (other.CompareTag("Player"))
		{
			situations.SetActive(false);
			Debug.Log("Play tune");
			boomSound.Play();
			displayTime.SetActive(false);
			displayMessage.GetComponent<TMPro.TMP_Text>().text = "Make a choice\ntaking too long can be disastrous!\nDecide faster next time!";
			fadeBlack.SetActive(true);
			menuButton.SetActive(true);
			message.SetActive(true);
			Invoke(nameof(Restart), 5f);
		}
    }
	
	//Restarts level
	void Restart()
	{
		SpoofMovement.RestartLevel();
	}
}
