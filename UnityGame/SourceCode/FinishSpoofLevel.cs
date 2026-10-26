// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Used for displaying end message after successful finish of spoofing level
public class FinishSpoofLevel : MonoBehaviour
{
    [SerializeField] GameObject displayMessage;
    [SerializeField] GameObject displayFact;
    [SerializeField] GameObject displayTime;
	[SerializeField] GameObject fadeBlack;
    [SerializeField] GameObject Situation6;
    [SerializeField] GameObject player;
    [SerializeField] AudioSource happySound;
    [SerializeField] AudioSource background;

	//When player collides with last situation door, show successful completion message and menu options
    void OnTriggerEnter(Collider other)
    {
		Debug.Log("Triggered by: " + other.name + " | Tag: " + other.tag);
		
		if (other.CompareTag("Player"))
		{
			Situation6.SetActive(false);
			background.Stop();
			Debug.Log("Play tune");
			happySound.Play();
			displayTime.SetActive(false);
			fadeBlack.SetActive(true);
			displayMessage.GetComponent<TMPro.TMP_Text>().text = "You have successfully completed the level!\n\nYou collected the data link header!";
			displayFact.SetActive(true);
			player.GetComponent<SpoofMovement>().enabled = false;
		}
    }
	
	//Resets values if player replays level
	public void Restart()
	{
		fadeBlack.SetActive(false);
		displayMessage.SetActive(false);
		displayTime.SetActive(true);
		SpoofMovement.RestartLevel();
	}
	
	//Goes back to menu for level menu button
	public void LevelMenu()
	{
		SceneManager.LoadSceneAsync("LevelMenu");
	}
}
