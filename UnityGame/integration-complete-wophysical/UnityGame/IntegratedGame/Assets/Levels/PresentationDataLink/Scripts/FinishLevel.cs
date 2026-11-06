// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Used for displaying end message after successful finish of phishing level
public class FinishLevel : MonoBehaviour
{
    [SerializeField] GameObject displayMessage;
    [SerializeField] GameObject displayFact;
    [SerializeField] GameObject displayTime;
	[SerializeField] GameObject fadeBlack;
    [SerializeField] GameObject player;
    [SerializeField] AudioSource happySound;
    [SerializeField] AudioSource background;

    //When player reaches maze exit, show successful completion message and menu options
	void OnTriggerEnter(Collider other)
    {
		Debug.Log("Triggered by: " + other.name + " | Tag: " + other.tag);
		
		if (other.CompareTag("Player"))
		{
			background.Stop();
			Debug.Log("Play tune");
			happySound.Play();
			displayTime.SetActive(false);
			fadeBlack.SetActive(true);
			displayMessage.GetComponent<TMPro.TMP_Text>().text = "You have successfully completed the level!\n\nYou collected the presentation header!";
			displayFact.SetActive(true);
			player.GetComponent<DataMovement>().enabled = false;
		}
    }
	
	//Resets values if player replays level
	public void Restart()
	{
		fadeBlack.SetActive(false);
		displayMessage.SetActive(false);
		displayTime.SetActive(true);
		DataMovement.RestartLevel();
	}
	
	//Goes back to menu for level menu button
	public void LevelMenu()
	{
		SceneManager.LoadSceneAsync("LevelMenu");
	}
}
