using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RollingCubeWaypoint {
    public Vector3 position;
    public float beforeMoveWait;
    public float moveSpeed;
}

public class RollingCube : MonoBehaviour {
    [SerializeField] bool isLooped;
    [SerializeField] float startTimeOffset;
    [SerializeField] RollingCubeWaypoint[] waypoints;
    [SerializeField] OnTriggerListener[] spikes;
    Vector3 initialPos;
    Vector3 currentWaypointPos;
    RollingCubeWaypoint currentWaypoint;
    RollingCubeWaypoint nextWaypoint;

    int currentWaypointIndex;
    float deltaMove;
    float waitTime;

    bool isPreMoveWaiting;
    bool moveBackwards;

    void Awake() {
        initialPos = transform.position;
        if(waypoints.Length>0){
            currentWaypoint = waypoints[0];
            currentWaypointPos = initialPos + currentWaypoint.position;
            transform.position = currentWaypointPos;
        }else{
            currentWaypointPos = initialPos;

        }
        foreach (var o in spikes) {
            o.OnTriggerEnterEvent += OnSpikesTriggered;
        }
    }

    void TriggerNextWaypoint() {
        if (isLooped) {
            currentWaypointIndex = (currentWaypointIndex+1) % waypoints.Length;
        } else {
            if (moveBackwards) {
                if(currentWaypointIndex == 0){
                    moveBackwards = false;
                }
                currentWaypointIndex++;
            } else {
                if (currentWaypointIndex == (waypoints.Length - 1)) {
                    moveBackwards = true;
                }
                currentWaypointIndex--;
            }
        }
        currentWaypoint = waypoints[currentWaypointIndex];
        currentWaypointPos = initialPos + currentWaypoint.position;
        waitTime = currentWaypoint.beforeMoveWait;
        isPreMoveWaiting = true;
    }

    void OnDrawGizmos() {
        var basePos = Application.isPlaying ? initialPos : transform.position;
        if (waypoints != null && waypoints.Length > 0) {
            for (int i = 0; i < waypoints.Length; i++) {
                Gizmos.DrawCube(basePos + waypoints[i].position, Vector3.one * 0.2f);
                if (i > 0) {
                    Gizmos.DrawLine(basePos + waypoints[i - 1].position, basePos + waypoints[i].position);
                }
            }
            if (isLooped) {
                Gizmos.DrawLine(basePos + waypoints[0].position, basePos + waypoints[waypoints.Length - 1].position);
            }
        }
    }

    void OnSpikesTriggered(Collider obj) {
        if (obj.GetComponent<Character>() != null) {
            Debug.Log("KILLED");
            obj.GetComponent<Character>().Kill();
        }
    }

    void Update() {
        if (startTimeOffset > 0) {
            startTimeOffset -= Time.deltaTime;
        } else {
            if (waitTime < Time.deltaTime) {
                if (isPreMoveWaiting) {
                    isPreMoveWaiting = false;
                } else {
                    Move();
                }
            } else {
                waitTime -= Time.deltaTime;
            }
        }
    }

    void Move() {
        deltaMove = Time.deltaTime * currentWaypoint.moveSpeed;
        if (Vector3.Distance(transform.position, currentWaypointPos) < deltaMove) {
            transform.position = currentWaypointPos;
            TriggerNextWaypoint();
        } else {
            transform.position = Vector3.MoveTowards(transform.position, currentWaypointPos, deltaMove);
        }
    }
}
