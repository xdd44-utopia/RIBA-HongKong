using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private const float moveTime = 2.5f;
	private const float rotateTime = 2.5f;
	private float curTime = 0;
	private enum Status {
		still,
		move,
		rotate
	}
	private Status status = Status.still;
	private Queue<Vector3> targets = new Queue<Vector3>();
	private Vector3 prePos = Vector3.zero;
	private Vector3 tarPos = Vector3.zero;
	private float preAngle = 0;
	private float tarAngle = 0;

	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		switch (status) {
			case Status.still: {
				if (targets.Count > 0) {
					setTarget(targets.Dequeue());
				}
				break;
			}
			case Status.move: {
				if (curTime < moveTime) {
					transform.position = Vector3.Lerp(prePos, tarPos, curTime / moveTime);
				}
				else {
					transform.position = tarPos;
					if (targets.Count > 0) {
						setTarget(targets.Dequeue());
					}
					else {
						status = Status.still;
					}
				}
				break;
			}
			case Status.rotate: {
				if (curTime < rotateTime) {
					transform.rotation = Quaternion.Euler(0, Mathf.Lerp(preAngle, tarAngle, curTime / rotateTime), 0);
				}
				else {
					transform.rotation = Quaternion.Euler(0, tarAngle, 0);
					status = Status.move;
					curTime = 0;
				}
				break;
			}
		}
		curTime += Time.deltaTime;
	}

	private void setTarget(Vector3 t) {
		prePos = transform.position;
		tarPos = t;
		if (Vector3.Angle(t - transform.position, transform.forward) == 0) {
			status = Status.move;
		}
		else {
			status = Status.rotate;
			preAngle = transform.rotation.eulerAngles.y;
			tarAngle = transform.rotation.eulerAngles.y + Vector3.SignedAngle(transform.forward, t - transform.position, new Vector3(0, 1, 0));
		}
		curTime = 0;
	}

	public void addTarget(Vector3 t) {
		targets.Enqueue(t);
	}
}
