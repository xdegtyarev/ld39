using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetComponent<Character>()!=null){
        	other.GetComponent<Character>().Kill();
        }
    }
}
