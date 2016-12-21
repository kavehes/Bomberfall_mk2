using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {


    //On va foutre toutes les stats ici, si ils veulent des trucs on iras les chercher ici

    [Header("Character Stats")]
    public int PlayerNumber = 0;
    public int BombInBag = 3;
    public bool BombOut;
    bool outBagPressed;
    bool throwPressed;
    public float Speed = 5f;
    public float jumpForce = 5;
    bool jumpPressed;

    //Tout ce qui a trait à grounded
    public GameObject groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask layermask;

    //Tout ce qui a trait à Walljump
    public GameObject Wallcheck;
    public float wallcheckRadius = 0.2f;
    public bool wallChecked;
    public float WJstrength = 200;

    public bool grounded;
    Rigidbody2D rigid;

    Animator anim;

    bool CanMove = true;
    bool FacingRight = true;

    public GameObject Bombe;


	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}

    void Update()
    {
        //Fuckin' get the input
        if (!jumpPressed)
            jumpPressed = Input.GetButtonDown("A_P1");
        if (!throwPressed)
            throwPressed = Input.GetButtonDown("X_P1");
        if (!outBagPressed)
            outBagPressed = Input.GetButtonDown("Y_P1");
    }

    void FixedUpdate()
    {
        Mover();
        Bomb();
    }

    //#################################################################

    /* Ici on va foutre tout ce qui a rapport aux bombage */

    void Bomb()
    {
        if(!BombOut && outBagPressed)
        {
            Debug.Log("Yo");
            outBagPressed = false;
            BombOut = true;
            GameObject bomb = (GameObject) Instantiate(Bombe,transform.position + Vector3.up*1.62f,Quaternion.identity,gameObject.transform);
            bomb.GetComponent<BombScript>().Hold = true;
        }
        if (BombOut && (outBagPressed || throwPressed))
        {
            outBagPressed = false;
            throwPressed = false;
            ThrowBomb();
        }
        if(!BombOut && throwPressed)
        {
            GameObject bomb = (GameObject)Instantiate(Bombe, transform.position + Vector3.up * 1.62f, Quaternion.identity, gameObject.transform);
            ThrowBomb();
        }
    }

    void ThrowBomb()
    {
        throwPressed = false;
        GameObject bomb = GetComponentInChildren<BombScript>().gameObject;
        if (bomb != null)
        {
            bomb.transform.parent = null;
            bomb.GetComponent<BombScript>().Hold = false;
            bomb.GetComponent<Rigidbody2D>().isKinematic = false;
            bomb.GetComponent<Rigidbody2D>().AddForce(new Vector2(rigid.velocity.x * 100, 400 + rigid.velocity.y*100));
            BombOut = false;
        }
    }

    //#################################################################

    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        FacingRight = !FacingRight;
    }

    void Mover()
    {
        //GroundChecker
        if (grounded && !Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, layermask))
            anim.SetTrigger("Jump");
        grounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, layermask);
        anim.SetBool("Grounded", grounded);

        if (!grounded)
        {
            wallChecked = Physics2D.OverlapCircle(Wallcheck.transform.position, wallcheckRadius, layermask); 
        }
        else
        {
            wallChecked = false;
        }
        if (rigid.velocity.y > 0)
            wallChecked = false;

        //Jumper
        if (jumpPressed && (grounded || wallChecked))
        {
            if (wallChecked)
            {
                rigid.gravityScale = 1;
                if (FacingRight)
                    rigid.AddForce(Vector2.left * WJstrength);
                else
                    rigid.AddForce(Vector2.right* WJstrength);
                wallChecked = false;
                StartCoroutine(Cooldown("CanMove", false, 15));
            }
            rigid.AddForce(Vector2.up * jumpForce);
            wallChecked = false;
            anim.SetTrigger("Jump");
            jumpPressed = false;
        }

        //Mover, en fonction de wallcheck
        if (!wallChecked)
        {
            if (CanMove)
                rigid.velocity = new Vector2(Input.GetAxis("Horizontal") * Speed, rigid.velocity.y);
            anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
            anim.SetFloat("Speed_h", rigid.velocity.y);
        }
        else
        {
            if (Input.GetAxis("Vertical") > 0.1f)
            {
                rigid.gravityScale = 0;
                rigid.velocity = new Vector2(0, 0);
                Debug.Log("Hey");
            }
            else
            {
                rigid.gravityScale = 1;
                rigid.velocity = new Vector2(Input.GetAxis("Horizontal") * Speed, rigid.velocity.y * 0.25f);
            }
        }

        //Fliper
        if ((rigid.velocity.x > 0 && !FacingRight) || (rigid.velocity.x < 0) && FacingRight)
        {
            Flip();
        }
    }


    //Say which variable, then change it's value by a and time frames later set it back to !a
    IEnumerator Cooldown(string var, bool a, float time)
    {
        Debug.Log("Starting Cooldown for " + time / 60f + " seconds");
        switch (var)
        {
            case "CanMove":
                CanMove = a;
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(time / 60f);
        switch (var)
        {
            case "CanMove":
                CanMove = !a;
            break;
            default:
            break;
        }
        yield return new WaitForFixedUpdate();
    }
}
