using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour {

    public bool Hold;
    
    public int Phase;
    //Doesn't blink ?
    public float Phase_1 = 2;
    //Blink Slowly
    public float Phase_2 = 3;
    //Blink Fast
    public float Phase_3 = 1f;

    Rigidbody2D rigid;
    Animator anim;

	// Use this for initialization
	void Start () {
        StartCoroutine(BombTimer());
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
        anim.SetInteger("Phase", Phase);
        if (Hold)
        {
            rigid.gravityScale = 0;
            rigid.isKinematic = true;
        }
        else
        {
            rigid.gravityScale = 1;
            rigid.isKinematic = false;
        }
    }

    public void Boom()
    {
        Destroy(gameObject);
    }

    IEnumerator BombTimer()
    {
        Phase = 1;
        yield return new WaitForSeconds(Phase_1);
        Phase = 2;
        yield return new WaitForSeconds(Phase_2);
        Phase = 3;
        yield return new WaitForSeconds(Phase_3);
        anim.SetTrigger("Boom");
    }
}
