using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {
	[SerializeField] GameObject winFX;
	void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<Character>()!=null){
        	other.GetComponent<Character>().Kill(false,10f);
        	StartCoroutine(WinCoroutine());
        }
    }

    IEnumerator WinCoroutine(){
    		Debug.Log("POBEDABLYAT!");
        	AudioController.Play("Level_end");
        	winFX.SetActive(true);
        	yield return new WaitForSeconds(4f);
            LevelLoader.Instance.ResetCheckpoint();
        	winFX.SetActive(false);
    }
}
