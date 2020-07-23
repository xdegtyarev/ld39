using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicAutoPlay : MonoBehaviour {
	void OnEnable () {
		AudioController.PlayMusicPlaylist();
	}
}
