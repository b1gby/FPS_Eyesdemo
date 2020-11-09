using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    

    [SerializeField] private float m_moveSpeed = 2.0f;
    [SerializeField] private float m_jumpForce = 4;

    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;


    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_runScale = 2.0f;


    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    
    private List<Collider> m_collisions = new List<Collider>();

    private float m_shootDamage = 6f;
    private float m_shootRange = 100f;
    public float m_playerHealth = 100f;
    public bool is_playerDead = false;
    private bool m_amIShoot = false;
    private bool m_amIShootHead = false;
    public bool m_isShooted = false;
    public bool m_isShootedHead = false;
    private float m_timerAmIShootHead = 0.2f;
    private float m_timerIsShooted = 1f;
    private GameObject m_headShootedPic;
    public float m_shootRate = 2f;

    private Transform targetTransform;
    private PlayerController targetPC;

    private GameObject m_gun;
    private ParticleSystem m_gunFlash;
    private GameObject m_beShootedPic;
    private float nextTimeToFire = 0f;

    public GameObject m_hitImpact;

    public string[] nameList;

    public int B_flag = 1;

    private float m_gunCapacity = 30f;
    private float m_gunNumber = 30f;
    public bool is_OnLoading = false;
    private float m_timerOnLoading = 3f;
    private bool hasBeenPressR = false;

    private float m_timerRevive = 5f;

    public Text m_nameText;
    public GameObject m_playerHealthBarHandle;

    private string whoKillMe;
    private string iKillWho;
    private bool is_iKillSomeone = false;
    private bool is_someoneKillMe = false;
    private float m_timerKillSomeone = 3f;

    private GameObject m_gameManager;


    public GameObject chatContentPrefab;
    private Transform gridLayout;
    public GameObject chatAreaPrefab;
    public GameObject chatInputField;
    private bool is_pressT = false;
    private bool is_hasChatArea = false;
    private string msg;
    
    private string AppID;
    private string AppVersion;

    void Awake()
    {
        if(!m_animator) { m_animator = gameObject.GetComponent<Animator>(); }
        if(!m_rigidBody) { m_rigidBody = gameObject.GetComponent<Rigidbody>(); }
    }

    

    void Start()
    {
        CameraController _cameraController = this.gameObject.GetComponent<CameraController>();
        m_playerHealth = 100f;
        m_gun = this.transform.Find("gun").gameObject;
        //m_gunFlash = m_gun.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        m_beShootedPic = GameObject.Find("BeShooted").gameObject;
        m_headShootedPic = GameObject.Find("HeadQuasiCenter").gameObject;
        m_gameManager = GameObject.Find("GameManager").gameObject;

        if (_cameraController != null)
        {
            _cameraController.SetFolling();
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }
    

    void Update()
    {
        //爆头准星
        if (m_amIShootHead)
        {
            m_timerAmIShootHead -= Time.fixedDeltaTime;
            //受击红色
            m_headShootedPic.GetComponent<Image>().color = new Color(222f / 255f, 222f / 255f, 222f / 255f, 255f / 255f);
            if (m_timerAmIShootHead <= 0)
            {
                m_amIShootHead = false;
                m_headShootedPic.GetComponent<Image>().color = new Color(222f / 255f, 222f / 255f, 222f / 255f, 0f / 255f);
            }
        }

        //换弹
        if (is_OnLoading)
        {
            m_timerOnLoading -= Time.fixedDeltaTime;
            Text txtOnLoading = GameObject.Find("OnLoadingTip").GetComponent<Text>();
            txtOnLoading.text = "On Loading! ";

            //Debug.Log(m_timerOnLoading);
            if (m_timerOnLoading <= 0)
            {
                txtOnLoading.text = "";
                is_OnLoading = false;
                hasBeenPressR = false;
                this.m_gunNumber = 30f;
            }
        }

        //死亡复活
        if (is_playerDead)
        {
            m_timerRevive -= Time.fixedDeltaTime;
            Text txxt = GameObject.Find("DeadTip").GetComponent<Text>();
            txxt.text = "You are Dead! Reviving ... ";

            Quaternion tmpQ = this.transform.rotation;
            this.transform.rotation = Quaternion.Euler(-90, tmpQ.eulerAngles.y, tmpQ.eulerAngles.z);
            //Debug.Log(whoKillMe);

            if (m_timerRevive <= 0)
            {
                txxt.text = "";
                this.m_playerHealth = 100f;
                this.transform.position = new Vector3(0, 3, -10);
                this.transform.rotation = Quaternion.identity;
                is_playerDead = false;
            }
        }
    }
    


	void FixedUpdate ()
    {
        Text txt = GameObject.Find("HP").GetComponent<Text>();

        txt.text = "HP :" + m_playerHealth.ToString();

        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject target in targets)
        {
            PlayerController targetPC = target.transform.GetComponent<PlayerController>();
            GameObject targethandle = target.transform.GetChild(5).GetChild(1).GetChild(0).GetChild(0).gameObject;
            targethandle.GetComponent<RectTransform>().offsetMin = new Vector2((100 - targetPC.m_playerHealth) / 100 * 160, 0);
        }

        Text txtCapacity = GameObject.Find("Capacity").GetComponent<Text>();

        txtCapacity.text = "Capacity : " + Mathf.Floor(m_gunNumber).ToString() +
            " / " + Mathf.Floor(m_gunCapacity).ToString();

        m_animator.SetBool("Grounded", m_isGrounded);


        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_runScale;
            h *= m_runScale;
        }

        if(Input.GetButton("Fire1"))
        {
            v /= m_runScale;
            h /= m_runScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;
            //Debug.Log(direction.magnitude / 2.0f);
            m_animator.SetFloat("MoveSpeed", direction.magnitude / 2.0f);
        }

        JumpingAndLanding();

        if (Input.GetKey(KeyCode.F))
        {
            m_animator.SetTrigger("Wave");
        }

        if (Input.GetKey(KeyCode.E))
        {
            m_animator.SetTrigger("Pickup");
        }

        if(Input.GetButton("Fire1") && !is_OnLoading && Time.time>=nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / m_shootRate;
            m_gunNumber--;
            Shoot();
            //m_gunFlash.Play();
        }
        
        

        if (m_gunNumber <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            is_OnLoading = true;
            if(!hasBeenPressR)
            {
                m_timerOnLoading = 3f;
                hasBeenPressR = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Text txxxt = GameObject.Find("FastShoot").GetComponent<Text>();
            if (this.B_flag == 0)
            {
                this.B_flag = 1;
                txxxt.text = "Now Fast Shooting Mode";
                this.m_shootRate = 10f;
            }
            else
            {
                this.B_flag = 0;
                txxxt.text = "Now Tap Shooting Mode";
                this.m_shootRate = 2f;
            }
        }

        if (m_playerHealth<=0 || this.transform.position.y<-10f)
        {
            is_playerDead = true;
            m_timerRevive = 5.0f;
        }

        m_wasGrounded = m_isGrounded;
         
    }

    private void Shoot()
    {

        RaycastHit hit;
        Transform camera = Camera.main.transform;
        if(Physics.Raycast(camera.position, camera.forward, out hit, m_shootRange))
        {
            targetTransform = hit.transform;
            targetPC = targetTransform.GetComponent<PlayerController>();
            
            if (hit.collider.name=="Head")
            {
                targetPC.takeDamage(m_shootDamage,true);
                m_amIShoot = true;
                m_amIShootHead = true;
                m_timerAmIShootHead = 0.2f;
            }
            else if (targetPC != null)
            {
                targetPC.takeDamage(m_shootDamage,false);
                m_amIShoot = true;
            }
            
            if(targetPC!=null && targetPC.m_playerHealth <= 0)
            {
                iKillWho = targetPC.transform.name;
                is_iKillSomeone = true;
                m_timerKillSomeone = 3f;
            }

            //Instantiate(m_hitImpact, hit.point, Quaternion.LookRotation(hit.normal));
            //GameObject[] to_destory = GameObject.FindGameObjectsWithTag("Smoke");
            //foreach(GameObject to_des in to_destory)
            //{
            //    Destroy(to_des, 5);
            //}
            
        }
    }


    public void takeDamage(float amount,bool isHead)
    {
        if(m_playerHealth > 0f)
        {
            if(isHead)
            {
                m_playerHealth -= 2*amount;
            }
            else
            {
                m_playerHealth -= amount;
            }

            if(m_playerHealth <0)
            {
                m_playerHealth = 0;
            }
            
        }
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }
    

   
}
