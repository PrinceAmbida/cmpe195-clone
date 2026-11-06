// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Used for selecting level from main menu
//Scene names match the scenes in UnityGame/IntegratedGame (the combined project)
//Physical layer scene is SniffingScene (name used by SniffMovement)
public class SwitchScenes : MonoBehaviour
{
	public void MainMenu()
	{
		//Title screen scene is named TitleScene
		SceneManager.LoadSceneAsync("TitleScene");
	}
	
	public void LevelMenu()
	{
		SceneManager.LoadSceneAsync("LevelMenu");
	}
	
	public void ApplicationLevel()
	{
		SceneManager.LoadSceneAsync("Application_layer");
	}
	
    public void PresentationLevel()
	{
		SceneManager.LoadSceneAsync("PhishingScene");
	}
	
	public void SessionLevel()
	{
		SceneManager.LoadSceneAsync("SessionLayer");
	}
	
	public void NetworkLevel()
	{
		SceneManager.LoadSceneAsync("NetworkLayer");
	}
	
	public void TransportLevel()
	{
		SceneManager.LoadSceneAsync("TransportLayer");
	}
	
	public void LinkLevel()
	{
		SceneManager.LoadSceneAsync("SpoofingScene");
	}
	
	public void PhysicalLevel()
	{
		SceneManager.LoadSceneAsync("SniffingScene");
	}
}
