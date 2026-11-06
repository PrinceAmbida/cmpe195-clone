// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Used for situation switch
//Swaps current situation for next situation, and shows/hides appropriate doors
public class SituationChange : MonoBehaviour
{
	public static SituationChange Instance;
	
    [SerializeField] GameObject Situation1;
    [SerializeField] GameObject Situation2;
    [SerializeField] GameObject Situation3;
    [SerializeField] GameObject Situation4;
    [SerializeField] GameObject Situation5;
    [SerializeField] GameObject Situation6;
	[SerializeField] AudioSource happySound;
	
	[SerializeField] GameObject Red1;
    [SerializeField] GameObject Red2;
    [SerializeField] GameObject Red3;
    [SerializeField] GameObject Red4;
    [SerializeField] GameObject Red5;
    [SerializeField] GameObject Red6;
	
	[SerializeField] GameObject Mid1;
    [SerializeField] GameObject Mid2;
    [SerializeField] GameObject Mid3;
    [SerializeField] GameObject Mid4;
    [SerializeField] GameObject Mid5;
    [SerializeField] GameObject Mid6;
	
	[SerializeField] GameObject Green1;
    [SerializeField] GameObject Green2;
    [SerializeField] GameObject Green3;
    [SerializeField] GameObject Green4;
    [SerializeField] GameObject Green5;
    [SerializeField] GameObject Green6;

	public static int situationNum = 0;
	
	void Awake()
	{
		Instance = this;
	}

	//Updates collision with door
    void OnTriggerEnter(Collider other)
    {
		Debug.Log("Triggered by: " + other.name + " | Tag: " + other.tag);
		
		if (other.CompareTag("Player"))
		{
			Debug.Log("Play tune");
			happySound.Play();
			nextSituation();
		}
    }
	
	//Updates to next situation
	public void nextSituation()
	{
		situationNum++;
		
		switch(situationNum)
		{
			case 1:
				Situation1.SetActive(false);
				Red1.SetActive(false);
				Mid1.SetActive(false);
				Green1.SetActive(false);
				Situation2.SetActive(true);
				Red2.SetActive(true);
				Mid2.SetActive(true);
				Green2.SetActive(true);
				break;
			
			case 2:
				Situation2.SetActive(false);
				Red2.SetActive(false);
				Mid2.SetActive(false);
				Green2.SetActive(false);
				Situation3.SetActive(true);
				Red3.SetActive(true);
				Mid3.SetActive(true);
				Green3.SetActive(true);
				break;
				
			case 3:
				Situation3.SetActive(false);
				Red3.SetActive(false);
				Mid3.SetActive(false);
				Green3.SetActive(false);
				Situation4.SetActive(true);
				Red4.SetActive(true);
				Mid4.SetActive(true);
				Green4.SetActive(true);
				break;
				
			case 4:
				Situation4.SetActive(false);
				Red4.SetActive(false);
				Mid4.SetActive(false);
				Green4.SetActive(false);
				Situation5.SetActive(true);
				Red5.SetActive(true);
				Mid5.SetActive(true);
				Green5.SetActive(true);
				break;
				
			case 5:
				Situation5.SetActive(false);
				Red5.SetActive(false);
				Mid5.SetActive(false);
				Green5.SetActive(false);
				Situation6.SetActive(true);
				Red6.SetActive(true);
				Mid6.SetActive(true);
				Green6.SetActive(true);
				break;
		}
	}
}
