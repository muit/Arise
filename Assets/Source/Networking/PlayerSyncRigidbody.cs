//---------------------------------------------
//            Tasharen Network
// Copyright © 2012-2014 Tasharen Entertainment
//---------------------------------------------

using UnityEngine;
using TNet;

/// <summary>
/// This script makes it easy to sync rigidbodies across the network.
/// Use this script on all the objects in your scene that have a rigidbody
/// and can move as a result of physics-based interaction with other objects.
/// Note that any user-based interaction (such as applying a force of any kind)
/// should still be sync'd via an explicit separate RFC call for optimal results.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("TNet/Sync Rigidbody")]
public class PlayerSyncRigidbody : TNBehaviour
{
	/// <summary>
	/// How many times per second to send updates.
	/// The actual number of updates sent may be higher (if new players connect) or lower (if the rigidbody is still).
	/// </summary>

	public float updatesPerSecond = 10f;

	/// <summary>
	/// Whether to send through UDP or TCP. If it's important, TCP will be used. If not, UDP.
	/// If you have a lot of frequent updates, mark it as not important.
	/// </summary>

	public bool isImportant = false;

    public float movementSmooth = 20.0f;

	/// <summary>
	/// Set this to 'false' to stop sending updates.
	/// </summary>

	[System.NonSerialized] public bool isActive = true;

	Rigidbody mRb; 
	float mNext = 0f;
	bool mWasSleeping = false;

	Vector3 lastPosition;
	Quaternion lastRotation;

    Vector3 nextPosition;
    Quaternion nextRotation;

    protected virtual void Awake()
	{
		mRb = GetComponent<Rigidbody>();
		lastPosition = mRb.position;
		lastRotation = mRb.rotation;
		UpdateInterval();
	}

	/// <summary>
	/// Update the timer, offsetting the time by the update frequency.
	/// </summary>

	void UpdateInterval () { mNext = Random.Range(0.85f, 1.15f) * (updatesPerSecond > 0f ? (1f / updatesPerSecond) : 0f); }

	/// <summary>
	/// Only the host should be sending out updates. Everyone else should be simply observing the changes.
	/// </summary>

	protected virtual void FixedUpdate ()
    {
		if (updatesPerSecond < 0.001f) return;

		if (isActive && TNManager.isInChannel)
		{
            if (tno.isMine)
            {
                if (mRb.isKinematic) mRb.isKinematic = false;

                bool isSleeping = mRb.IsSleeping();
                if (isSleeping && mWasSleeping) return;

                mNext -= Time.deltaTime;
                if (mNext > 0f) return;
                UpdateInterval();

                Vector3 pos = mRb.position;
                Quaternion rot = mRb.rotation;

                if (mWasSleeping || Vector3.Distance(pos, lastPosition) > 0.1f || Quaternion.Angle(rot, lastRotation) > 5f)
                {
                    lastPosition = pos;
                    lastRotation = rot;

                    // Send the update. Note that we're using an RFC ID here instead of the function name.
                    // Using an ID speeds up the function lookup time and reduces the size of the packet.
                    // Since the target is "OthersSaved", even players that join later will receive this update.
                    // Each consecutive Send() updates the previous, so only the latest one is kept on the server.

                    if (isImportant)
                    {
                        tno.Send(255, Target.OthersSaved, pos, rot, mRb.velocity, mRb.angularVelocity);
                    }
                    else tno.SendQuickly(255, Target.OthersSaved, pos, rot, mRb.velocity, mRb.angularVelocity);
                }
                mWasSleeping = isSleeping;
            }
            else {
                if (!mRb.isKinematic) mRb.isKinematic = true;

                if (nextPosition != Vector3.zero) {
                    mRb.position = Vector3.Lerp(mRb.position, nextPosition, Time.deltaTime * movementSmooth);
                }
                if (nextRotation != Quaternion.identity) {
                    mRb.rotation = Quaternion.Lerp(mRb.rotation, nextRotation, Time.deltaTime * movementSmooth);
                }
            }
		}
	}

    bool IsMoving() {
        return mRb.velocity.sqrMagnitude > 0.1f*0.1f && mRb.angularVelocity.sqrMagnitude > 0.1f*0.1f;
    }

	/// <summary>
	/// Actual synchronization function -- arrives only on clients that aren't hosting the game.
	/// Note that an RFC ID is specified here. This shrinks the size of the packet and speeds up
	/// the function lookup time. It's a good idea to do this with all frequently called RFCs.
	/// </summary>

	[RFC(255)]
	void OnSync (Vector3 pos, Quaternion rot, Vector3 vel, Vector3 ang)
	{
		nextPosition = pos;
		nextRotation = rot;
		//mRb.MovePosition(pos);
		//mRb.MoveRotation(Quaternion.Euler(rot));
        /*
		if (!mRb.isKinematic)
		{
			mRb.velocity = vel;
			mRb.angularVelocity = ang;
		}*/

		UpdateInterval();
	}

	/// <summary>
	/// It's a good idea to send an update when a collision occurs.
	/// </summary>

	//void OnCollisionEnter () { if (tno.isMine) Sync(); }

	/// <summary>
	/// Send out an update to everyone on the network.
	/// </summary>

	public void Sync ()
	{
		if (isActive && TNManager.isInChannel)
		{
			UpdateInterval();
			mWasSleeping = false;
			lastPosition = transform.position;
			lastRotation = transform.rotation;
			tno.Send(255, Target.OthersSaved, lastPosition, lastRotation, mRb.velocity, mRb.angularVelocity);
		}
	}
}
