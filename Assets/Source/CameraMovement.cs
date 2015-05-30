using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    public CPlayer motion;

    public float distance = 10;
    public float inclination = 45;
    public float yRotation = 180;
    public float moveDamping = 3;
    public float teleportAtDistance = 100;

    private Transform spawn;

    [System.NonSerialized]
    public MotionBlur motionBlur;

    void Start() {
        spawn = Game.Get().activeSpawn.transform;
        motionBlur = GetComponent<MotionBlur>();
    }

	void Update () {
        if (motion != null) {
            UpdatePosition(motion.transform);
        }
        else {
            UpdatePosition(spawn);
        }
    }

    public void SetTarget(CPlayer _motion)
    {
        motion = _motion;
    }

    private void UpdatePosition(Transform trans)
    {
        if (!trans)
        {
            Debug.LogWarning("Camera target transform is null.");
            return;
        }

        Vector3 position = trans.position;
        position.y += distance * Mathf.Sin((90 - inclination) * 2 * Mathf.PI / 360);
        position.x += distance * Mathf.Cos((90 - inclination) * 2 * Mathf.PI / 360);

        transform.LookAt(trans);

        //Teleport camera if is too far
        if (Vector3.Distance(trans.position, transform.position) > teleportAtDistance)
            transform.position = position;
        else
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * moveDamping);
    }
}
