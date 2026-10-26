// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Removes shields or ends game if no shields left, based on situation
public class WrongOption : MonoBehaviour
{
    [SerializeField] GameObject shield1;
    [SerializeField] GameObject shield2;
	[SerializeField] GameObject shield3;
    [SerializeField] AudioSource endSound;
    [SerializeField] AudioSource boomSound;
    [SerializeField] GameObject displayMessage;
	[SerializeField] GameObject menuButton;
	[SerializeField] GameObject message;
	[SerializeField] GameObject fadeBlack;
	[SerializeField] GameObject situations;
	[SerializeField] GameObject player;

	//Checks if player chose wrong door
    void OnTriggerEnter(Collider other)
    {
		Debug.Log("Triggered by: " + other.name + " | Tag: " + other.tag);
		
		if (other.CompareTag("Player"))
		{
			wrongChoice();
			SituationChange.Instance.nextSituation();
		}
    }
	
	//For wrong door, removes 1 shield
	//or ends current game if no shields left and restarts game
	void wrongChoice()
	{
		Debug.Log("Play tune");
		boomSound.Play();
		
		if (shield3.activeSelf)
		{
			shield3.SetActive(false);
		} else if (shield2.activeSelf)
		{
			shield2.SetActive(false);
		} else if (shield1.activeSelf)
		{
			shield1.SetActive(false);
		} else {
			endSound.Play();
			situations.SetActive(false);
			displayMessage.GetComponent<TMPro.TMP_Text>().text = "Continually making the right choice matters!\nRead the situation more carefully next time!";
			fadeBlack.SetActive(true);
			menuButton.SetActive(true);
			message.SetActive(true);
			SpoofMovement.ended = true;
			Invoke(nameof(Restart), 5f);
		}
	}
	
	//Restarts level
	void Restart()
	{
		SpoofMovement.RestartLevel();
	}
}
