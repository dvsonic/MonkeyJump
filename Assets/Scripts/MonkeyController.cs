using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MonkeyController : MonoBehaviour {

	// Use this for initialization
    public enum NinjiaState { RUN,FIRST_JUMP,SECOND_JUMP,DEAD};
    public enum BuffType {NONE,SPEED,HIGH,GOD};
    public NinjiaState state;
    public float speed;
    public float jumpSpeed;
    public float jumpSpeed2;
    public BlockFactory factory;
    public GameObject guide;
    public AudioSource run;
    public AudioSource jump;
    public AudioSource dead;
    public GameObject fireworks;
    public GameObject godGlow;
    public Material matNormal;
    public Material matSpeed;
    private bool isFlip;

	void Start () {
        Physics2D.IgnoreLayerCollision(8, 9, false);
        SetState(NinjiaState.RUN);
        _isStart = false;
        curBuff = BuffType.NONE;
        buffEndTime = 0;
        if (run)
            run.Play();
    }

    public void Reset()
    {
        Start();
        GetComponent<SpriteRenderer>().material = matNormal;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.position = new Vector3(-1.67f, -1.54f);
        GetComponent<TargetState>().IsDead = false;
        GetComponent<Animator>().SetBool("isDead", false);
        GetComponent<Animator>().SetBool("isJump", false);
        GetComponent<Animator>().SetTrigger("Reset");
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            switch (state)
            {
                case NinjiaState.RUN:
                case NinjiaState.FIRST_JUMP:
                    Jump();
                    if (!_isStart)
                        GameStart();
                    break;
            }
        }

#else
        if (Input.touchCount>0 && Input.touches[0].phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
        {
            switch (state)
            {
                case NinjiaState.RUN:
                case NinjiaState.FIRST_JUMP:
                    Jump();
                    if (!_isStart)
                        GameStart();
                    break;
            }
        }
#endif
        if (state == NinjiaState.FIRST_JUMP || state == NinjiaState.SECOND_JUMP)
        {
            if (GetComponent<Rigidbody2D>().velocity.y > 0)
            {
                Physics2D.IgnoreLayerCollision(8, 9, true);
            }
            else
            {
                Physics2D.IgnoreLayerCollision(8, 9, false);
            }
        }

        if (buffEndTime != 0 && Time.time > buffEndTime)
        {
            RemoveBuff();
        }


    }

    public void GameStart()
    {
        _isStart = true;
        if (guide)
            Destroy(guide.gameObject);
        factory.CreatBlock();
    }

    public void SetState(NinjiaState state)
    {
        this.state = state;
        switch (state)
        {
            case NinjiaState.FIRST_JUMP:
            case NinjiaState.SECOND_JUMP:
                Camera.main.GetComponent<CameraFollow>().enabled = true;
                GetComponent<Animator>().SetBool("isJump", true);
                if (run)
                    run.Stop();
                if (jump)
                    jump.Play(); 
                break;
            case NinjiaState.RUN:
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                GetComponent<Animator>().SetBool("isJump", false);
                if (run && !run.isPlaying)
                    run.Play();
                break;
            case NinjiaState.DEAD:
                Camera.main.GetComponent<CameraFollow>().enabled = false;
                if (run)
                    run.Stop();
                if (dead)
                    dead.Play();
                GetComponent<Animator>().SetBool("isDead", true);
                break;
        }
    }

    void FixedUpdate()
    {
        if (GetComponent<TargetState>().IsDead == false)
        {
            if (isFlip)
            {
                transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
            }
            else
            {
                transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
            }
        }

    }

    private bool _isStart;
    private void Jump()
    {
        if (!GameData.isStart || GetComponent<TargetState>().IsDead == true)
            return;
        if (state == NinjiaState.RUN)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpSpeed);
            SetState(NinjiaState.FIRST_JUMP);
        }
        else if (state == NinjiaState.FIRST_JUMP)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpSpeed2);
            SetState(NinjiaState.SECOND_JUMP);
        }
    }
    private GameObject _lastBridge;
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (state == NinjiaState.DEAD)
            return;
        if(coll.gameObject.tag == "Bridge")
        {
            if (_lastBridge != coll.gameObject)
            {
                if (_lastBridge == null || (_lastBridge && coll.gameObject.transform.position.y > _lastBridge.transform.position.y))
                {
                    _lastBridge = coll.gameObject;
                    if(_isStart)
                        GameData.score++;
                    if (fireworks != null && GameData.score>0 && GameData.score % 10 == 0)
                    {
                        fireworks.SendMessage("Create");
                    }
                    factory.CreatBlock();
                }

            }
            SetState(NinjiaState.RUN);
        }
        else if (coll.gameObject.tag == "Friend" || coll.gameObject.tag == "Enemy")
        {
            SetState(NinjiaState.DEAD);
            RemoveBuff();
            GetComponent<TargetState>().IsDead = true;
            Camera.main.GetComponent<CameraShake>().Shake();
        }
    }


    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Bridge")
        {
            if (transform.position.y < coll.transform.position.y)//特殊处理一下没跳上落下的情况
            {
                Camera.main.GetComponent<CameraFollow>().enabled = true;
                GetComponent<Animator>().SetBool("isJump", true);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.gameObject.tag == "Wall")
        {
            isFlip = !isFlip;
            if (!isFlip)
                transform.localScale = Vector3.one;
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (coll.gameObject.tag == "BonusGod")
        {
            AddBuff(BuffType.GOD);
            //coll.GetComponent<Animator>().SetBool("isDestroy", true);
            //coll.GetComponent<BoxCollider2D>().enabled = false;
            PickUp(coll.gameObject);
        }
        else if (coll.gameObject.tag == "BonusSpeed")
        {
            AddBuff(BuffType.SPEED);
           // coll.GetComponent<Animator>().SetBool("isDestroy", true);
            //coll.GetComponent<BoxCollider2D>().enabled = false;
            PickUp(coll.gameObject);
        }
        else if (coll.gameObject.tag == "BonusHigh")
        {
            AddBuff(BuffType.HIGH);
            coll.GetComponent<Animator>().SetBool("isDestroy", true);
            coll.GetComponent<BoxCollider2D>().enabled = false;
        }

    }
    public GameObject pickup;
    private void PickUp(GameObject obj)
    {
        Instantiate(pickup, obj.transform.position, obj.transform.rotation);
        DestroyObject(obj);
    }

    public GameObject headState;
    public void SetHeadState(int type)
    {
        if (headState)
        {
            headState.GetComponent<Animator>().SetInteger("type", type);
            StartCoroutine(ClearHeadState());
        }
    }

    IEnumerator ClearHeadState()
    {
        yield return new WaitForSeconds(0.3f);
        headState.GetComponent<Animator>().SetInteger("type", 0);
    }

    private BuffType curBuff;
    private float buffEndTime;
    private const int buffDuration = 10;
    public void AddBuff(BuffType buff)
    {
        if (curBuff == buff)
        {
            buffEndTime += buffDuration;
            Debug.Log("Buff duration add:" + buffDuration);
            return;
        }
        else
            RemoveBuff();
        switch (buff)
        {
            case BuffType.SPEED:
                speed = speed * 1.5f;
                GetComponent<SpriteRenderer>().material = matSpeed;
                break;
            case BuffType.HIGH:
                jumpSpeed = jumpSpeed * 1.3f;
                jumpSpeed2 = jumpSpeed2 * 1.3f;
                break;
            case BuffType.GOD:
                Physics2D.IgnoreLayerCollision(10, 9, true);
                if (godGlow)
                    godGlow.SetActive(true);
                break;
        }
        Debug.Log("AddBuff:" + buff);
        curBuff = buff;
        buffEndTime = Time.time + buffDuration;
    }

    public void RemoveBuff()
    {
        switch (curBuff)
        {
            case BuffType.SPEED:
                speed = speed / 1.5f;
                GetComponent<SpriteRenderer>().material = matNormal;
                break;
            case BuffType.HIGH:
                jumpSpeed = jumpSpeed / 1.3f;
                jumpSpeed2 = jumpSpeed2 / 1.3f;
                break;
            case BuffType.GOD:
                Physics2D.IgnoreLayerCollision(10, 9, false);
                if (godGlow)
                    godGlow.SetActive(false);
                break;
        }
        Debug.Log("Clear Buff:"+curBuff);
        buffEndTime = 0;
        curBuff = BuffType.NONE;
    }

    public void OnDead()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(GameEnd());
        Camera.main.GetComponent<CameraFollow>().enabled = false;
    }

    private IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(1.0f);
        GameScene.GotoScene(3);
    }
}
