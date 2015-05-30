using UnityEngine;
using TNet;
using System.Collections;
using InControl;

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

        //Set up controlled player reference
        if (tno.isMine)
            Game.Get().controlledPlayer = this;
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
        rigidbody.angularVelocity = Vector3.zero;
        live = maxLive;
        money = 0;

        Game.Get().playerSpawn.GetComponent<AudioSource>().Play();
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
    
    //Audio
    [SerializeField]
    private AudioSource jumpSound;

    private void MovementUpdate()
    {
        if (camera && camera.motion != this)
            camera.SetTarget(this);

        // Cache the inputs.
        float h = 0f;
        float v = 0f;
        bool jump = false;
        //Detect Correct Input
        if(Game.IsMobile()){
            InputDevice activeDevice = InputManager.ActiveDevice;
            h = activeDevice.LeftStickX;
            v = activeDevice.LeftStickY;
            jump = activeDevice.Action1.WasPressed;
        }else{
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            jump = Input.GetKeyDown("space");
        }

        if (CanMove() && (h != 0f || v != 0f))
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
        if (jump && canJumpAgain)
        {
            jumpSound.Play();
            isMoving = true;
            rigidbody.AddForce(0, jumpSpeed, 0);
        }

        //Enable or disable motionBlur
        camera.motionBlur.enabled = rigidbody.velocity.sqrMagnitude > 25*25;
    }

    void Rotating(float horizontal, float vertical)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

        rigidbody.MoveRotation(newRotation);
    }


    //Fix Wall Stuck
    private int collidersAmount = 0;

    public bool IsColliding(){
        return collidersAmount > 0;
    }

    public bool CanMove() {
        return !(IsColliding() && !canJumpAgain && rigidbody.velocity.y < 0.01);
    }

    void OnCollisionEnter(Collision col) {
        collidersAmount += 1;
    }

    void OnCollisionExit(Collision col)
    {
        if(collidersAmount > 0)
            collidersAmount -= 1;
    }
}
