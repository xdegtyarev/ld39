using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour {
	[SerializeField] GameObject view;
	void OnTriggerEnter(Collider other) {
		if(view.activeSelf){
	        if(other.gameObject.GetComponent<Character>()!=null){
	        	other.GetComponent<Character>().RestoreEnergy();
	        	AudioController.Play("Powerup");
	        	view.SetActive(false);
	        	LevelLoader.Instance.SetCheckpoint(transform);
	        }
    	}
    }

    public void Restore(){
    	view.SetActive(true);
    }


}
