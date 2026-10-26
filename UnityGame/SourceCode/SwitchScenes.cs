// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Used for selecting level from main menu
public class SwitchScenes : MonoBehaviour
{
	public void MainMenu()
	{
		SceneManager.LoadSceneAsync("MainMenu");
	}
	
	public void LevelMenu()
	{
		SceneManager.LoadSceneAsync("LevelMenu");
	}
	
	public void ApplicationLevel()
	{
		SceneManager.LoadSceneAsync("AppScene");
	}
	
    public void PresentationLevel()
	{
		SceneManager.LoadSceneAsync("PhishingScene");
	}
	
	public void SessionLevel()
	{
		SceneManager.LoadSceneAsync("SessionScene");
	}
	
	public void NetworkLevel()
	{
		SceneManager.LoadSceneAsync("NetworkScene");
	}
	
	public void TransportLevel()
	{
		SceneManager.LoadSceneAsync("TransportScene");
	}
	
	public void LinkLevel()
	{
		SceneManager.LoadSceneAsync("SpoofingScene");
	}
	
	public void PhysicalLevel()
	{
		SceneManager.LoadSceneAsync("PhysicalScene");
	}
}
