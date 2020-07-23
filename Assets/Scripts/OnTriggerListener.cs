using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerListener : MonoBehaviour {
	public event System.Action<Collider> OnTriggerEnterEvent;

	void OnTriggerEnter(Collider other) {
		if(OnTriggerEnterEvent!=null){
			OnTriggerEnterEvent(other);
		}
    }
}
