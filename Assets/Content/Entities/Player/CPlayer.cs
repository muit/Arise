using UnityEngine;
using TNet;
using System.Collections;

public class CPlayer : Entity
{
    public static CPlayer instance;

    public int money = 0;

    public int respawnHeight = -50;

    private HPBar canvas;

    protected override void Start()
    {
        base.Start();
        if (TNManager.isThisMyObject) {
            instance = this;
        }

        canvas = GetComponentInChildren<HPBar>();
        rigidbody = GetComponent<Rigidbody>(); 
        camera = Game.Get().playerCamera.GetComponent<CameraMovement>();
	}
	
	void Update () {
        if (!tno.isMine)
            return;

        if (transform.position.y < respawnHeight)
        {
            Respawn();
        }

        MovementUpdate();

        //UI
        //canvas.SetMoney(money);
        //canvas.SetHP((float)(live) / maxLive);
	}

    public void CollectMoney(Coin coin)
    {
        CollectMoneyAmount(coin.amount);
    }


    /****************
     * Remote Calls *
     ****************/

    [RFC(0)]
    public void CollectMoneyAmount(int amount) {
        if(tno && tno.isMine)
            tno.Send(0, TNet.Target.OthersSaved, amount);
        
        money += amount;
    }

    [RFC(1)]
    public void Respawn()
    {
        if(tno && tno.isMine)
            tno.Send(1, TNet.Target.OthersSaved);

        transform.position = Game.Get().playerSpawn.position;
        transform.rotation = Game.Get().playerSpawn.rotation;
        rigidbody.velocity = Vector3.zero;
        live = maxLive;
        money = 0;
    }



    /*****************
     * Movement Code *
     *****************/

    public float turnSmoothing = 15f;	// A smoothing value for turning the player.
    public float speedDamp = 0.1f;	// The damping for the speed parameter
    public float jumpSpeed = 50f;

    [System.NonSerialized]
    public bool canJumpAgain = false;
    [System.NonSerialized]
    public bool isMoving = false;

    private Rigidbody rigidbody;
    private CameraMovement camera;

    private void MovementUpdate()
    {
        if (camera && camera.motion != this)
            camera.SetTarget(this);

        // Cache the inputs.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        if (h != 0f || v != 0f)
        {
            isMoving = true;
            Rotating(h, v);

            rigidbody.velocity = new Vector3(-v * speedDamp, rigidbody.velocity.y, h * speedDamp);

        }
        else
        {
            isMoving = false;
        }

        //Jump
        if (Input.GetKeyDown("space") && canJumpAgain)
        {
            isMoving = true;
            rigidbody.AddForce(0, jumpSpeed, 0);
        }

        //Enable or disable motionBlur
        camera.motionBlur.enabled = (Vector3.Distance(rigidbody.velocity, Vector3.zero) > 25);
    }

    void Rotating(float horizontal, float vertical)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

        rigidbody.MoveRotation(newRotation);
    }
}
