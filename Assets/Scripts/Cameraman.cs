using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameraman : MonoBehaviour {
	[SerializeField] float startSpeed;
	[SerializeField] float acc;
	[SerializeField] float maxSpeed;
	Quaternion targetRot;
	Quaternion curRot;
	Vector3 delta;

	public float updateDelay;
	float nextUpdate;
	float xCoord;
	public float angularValue;
	public Transform target;
	Vector3 mouseDelta;
	Vector3 prevMousePos;
	Vector3 currentMousePos;

	float deltaMove;
	float velocity;


	float horizCam;
	float vertCam;
	float joystickRotationSpeed = 10f;


	void Awake() {
		velocity = startSpeed;
	}

	void PosUpdate() {
		deltaMove = Time.deltaTime * velocity;
		if (Vector3.Distance(transform.position, target.transform.position) < deltaMove) {
			transform.position = target.transform.position;
			velocity = startSpeed;
		} else {
			transform.position = Vector3.MoveTowards(transform.position, target.transform.position, deltaMove);
			velocity += acc * Time.deltaTime;
			velocity = Mathf.Min(velocity, maxSpeed);
		}
	}

	void Update() {
		horizCam = Input.GetAxis("HorizontalCam");
		vertCam = Input.GetAxis("VerticalCam");
		if (Mathf.Abs(horizCam) > 0.01f || Mathf.Abs(vertCam) > 0.01f) {
			mouseDelta = new Vector3(horizCam, vertCam, 0f) * joystickRotationSpeed;
		} else {
			currentMousePos = Input.mousePosition;
			mouseDelta = prevMousePos - currentMousePos;
			mouseDelta = new Vector3(-mouseDelta.x, mouseDelta.y, mouseDelta.z);
		}
		if (mouseDelta.magnitude > 0.01f) {
			RotUpdate();
		}
		PosUpdate();
		prevMousePos = currentMousePos;
	}

	void RotUpdate() {
		if ((transform.rotation.eulerAngles.x + mouseDelta.y) > 320f && (transform.rotation.eulerAngles.x + mouseDelta.y) < 360f) {
			xCoord = mouseDelta.y;
		} else {
			xCoord = 0f;
		}
		transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(transform.rotation.eulerAngles + (new Vector3(xCoord, mouseDelta.x, 0) * angularValue)), Time.deltaTime * 5f);
	}

}
