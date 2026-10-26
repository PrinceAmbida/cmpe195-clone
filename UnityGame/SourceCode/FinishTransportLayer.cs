// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTransportLevel : MonoBehaviour
{
    //Resets values if player replays level
	public void Restart()
	{
		SceneManager.LoadSceneAsync("TransportLayer");
	}
	
	//Goes to next level
	public void NextLevel()
	{
		SceneManager.LoadSceneAsync("NetworkLayer");
	}
	
	//Goes back to menu for level menu button
	public void LevelMenu()
	{
		SceneManager.LoadSceneAsync("LevelMenu");
	}
}
