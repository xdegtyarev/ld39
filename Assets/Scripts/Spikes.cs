using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {
    [SerializeField] Animator animator;
    [SerializeField] float startTimeOffset;
    [SerializeField] float downWait;
    [SerializeField] float previewWait;
    [SerializeField] float upWait;
    [SerializeField] OnTriggerListener spikes;

    bool isDownWait;
    bool isPreviewWait;
    bool isUpWait;

    float waitTime;

    void OnSpikesTriggered(Collider obj) {
        if(obj.GetComponent<Character>()!=null){
        	Debug.Log("KILLED");
        	obj.GetComponent<Character>().Kill();
        }
    }

    void Awake() {
        DownSpikes();
        spikes.OnTriggerEnterEvent+=OnSpikesTriggered;
    }


    void Update() {
        if (startTimeOffset > 0) {
            startTimeOffset -= Time.deltaTime;
        } else {
            if (waitTime < Time.deltaTime) {
                if (isDownWait) {
                    PreviewSpikes();
                } else if (isPreviewWait) {
                    UpSpikes();
                } else if (isUpWait) {
                    DownSpikes();
                }
            } else {
                waitTime -= Time.deltaTime;
            }
        }
    }

    void DownSpikes() {
        isDownWait = true;
        isPreviewWait = false;
        isUpWait = false;
        waitTime = downWait;
        animator.SetTrigger("spikes_down");
    }

    void UpSpikes() {
        isDownWait = false;
        isPreviewWait = false;
        isUpWait = true;
        waitTime = upWait;
        animator.SetTrigger("spikes_up");
        // AudioController.Play("spikes_up", transform.position, transform);
    }

    void PreviewSpikes() {
        isDownWait = false;
        isPreviewWait = true;
        isUpWait = false;
        waitTime = previewWait;
        animator.SetTrigger("spikes_preview");
    }
}


