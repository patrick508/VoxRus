using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class enemyAIScript01 : MonoBehaviour
{
    Vector3 StartPos;
    public Slider mySlider;

    RaycastHit hitBody;
    RaycastHit hitHead;
    bool HeightDifference = false;
    public float distanceToSee;
    Vector3 HeadRayPos = new Vector3(0f, 1.3f, 0f);
    Vector3 BodyRayPos = new Vector3(0f, 0.2f, 0f);
    public float jumpSpeed = 10.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

    CharacterController controller;
    public Transform player;
    public float playerDistance;
    public float rotationSpeed = 8;
    public GameObject playercontroller;
    int damage = 10;
    private bool Invoke_Attack = false;

    //public GameObject cubetestpos;

    // Use this for initialization
    void Start(){
        playercontroller = GameObject.Find("Player");
        player = GameObject.Find("Player").transform;
        StartPos = this.transform.position;
        mySlider = GameObject.Find("Slider_Health").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update(){
        playerDistance = Vector3.Distance(player.position, transform.position);
        state_switch();
        RayCastManager();
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
        //Makes Enemy run faster when he is hunting you down
        if (EnemySt == EnemyStates.HUNTING) {
            Wa_speed = 5f;
        } else {
            Wa_speed = 1f;
        }
    }



            //Awake is called even if the object is disabled
            void Awake(){
        controller = GetComponent<CharacterController>();
    }
    #region Raycast jumping
    void RayCastManager() {
            RaycastForward();
            CheckHeight();
    }
    //Checks if the block above is closer or equel to the block down (to check if you can continue walking)
    //If the block above is closer or equel to the one down you can't continue walking.
    void CheckHeight() {
        HeightDifference = false;
        if (+Vector3.Distance(this.transform.position, hitBody.point) >= Vector3.Distance(this.transform.position + HeadRayPos, hitHead.point) && hitBody.collider != null) {
            HeightDifference = true;
        }
    }

    float RaycastForward() {
        float DisToForward = -1f;
        //Lowest Ray
        if (controller.isGrounded) {
            //Shoots a raycast from the middle of the enemy for the blocks on the 1st layer & Jumps if close
            //Makes the line visible from your position to forward and goes as far as distanceToSee
            Debug.DrawRay(this.transform.position - BodyRayPos, this.transform.forward * distanceToSee, Color.green);
            if (Physics.Raycast(this.transform.position - BodyRayPos, this.transform.forward, out hitBody, distanceToSee)) {
                //Debug.Log("I hitted something!" + Vector3.Distance(this.transform.position, hitBody.point));
                if (controller.isGrounded && Vector3.Distance(this.transform.position, hitBody.point) <= 0.8f && HeightDifference == false) {
                    moveDirection.y = jumpSpeed;
                    // print("Ja verdomme nu kan ik niet springen");
                }
            }
        }
        //Highest Ray
        if (controller.isGrounded) {
            //shoots a second raycast above his head for the blocks on the 2nd layer
            //Makes the line visible from your position to forward and goes as far as distanceToSee
            Debug.DrawRay(this.transform.position + HeadRayPos, this.transform.forward * distanceToSee, Color.green);
            if (Physics.Raycast(this.transform.position + HeadRayPos, this.transform.forward, out hitHead, distanceToSee)) {
                //Debug.Log("I hitted something higher!" + Vector3.Distance(this.transform.position, hitHead.point));
            }
        }
        return DisToForward;
    }
    //Empty's the Coord list creates a new one and walks fixed amount in z and x axis and can not walk slant(schuin). It walks on the X axis first.
    void backoff() {
        Vector3 NorthPos = new Vector3(this.transform.position.x - 3f, this.transform.position.y, this.transform.position.z - 3f);
        Vector3 EastPos = new Vector3(this.transform.position.x - 3f, this.transform.position.y, this.transform.position.z + 3f);
        Vector3 SouthPos = new Vector3(this.transform.position.x + 3f, this.transform.position.y, this.transform.position.z + 3f);
        Vector3 WestPos = new Vector3(this.transform.position.x + 3f, this.transform.position.y, this.transform.position.z + 3f);
        int _DirInt = GetDirection();
        if (_DirInt == 0 || _DirInt == 4) {
            //noorden
            Wa_CurrentCoord = 0;
            Wa_Coords = new List<Vector3>();
            Wa_Coords.Add(NorthPos); // add fixed position 

        } else if (_DirInt == 1) {
            //oosten
            Wa_CurrentCoord = 0;
            Wa_Coords = new List<Vector3>();
            Wa_Coords.Add(EastPos); // add fixed position 

        } else if (_DirInt == 2) {
            //zuiden
            Wa_CurrentCoord = 0;
            Wa_Coords = new List<Vector3>();
            Wa_Coords.Add(SouthPos); // add fixed position 

        } else if (_DirInt == 3) {
            //westen
            Wa_CurrentCoord = 0;
            Wa_Coords = new List<Vector3>();
            Wa_Coords.Add(WestPos); // add fixed position 

        } else {
            print("Er gaat iets fout");
        }    

    }
    #endregion raycast Jumping
    #region Behaviour on distances
    //All the states the EnemyAI can be in.
    enum EnemyStates {
        WANDERING = 0,
        STARING = 1,
        HUNTING = 2,
        ATTACKING = 3,
        BACKINGOFF = 4

    };
    //Starting State
    EnemyStates EnemySt = EnemyStates.WANDERING;
    //Reads what to do when every state is active and switches when enemy gets closer/further away
    void state_switch(){
        switch (EnemySt)
        {
            //Starts randomly wandering around
            case EnemyStates.WANDERING:{
                    ApplyWandering();
                    if (playerDistance <= 15f) {
                        EnemySt = EnemyStates.STARING;
                    }
                    if (HeightDifference == true) {
                        // print("Heightdifference = true");
                        backoff();
                        ApplyWandering();
                       // EnemySt = EnemyStates.BACKINGOFF;
                    }
                }
                break;
                //Starts staring at the player
            case EnemyStates.STARING:{
                    LookAtPlayer();
                    if (playerDistance <= 10f) {
                        EnemySt = EnemyStates.HUNTING;
                   }if (playerDistance > 15f) {
                        Wa_Coords.Clear();
                        EnemySt = EnemyStates.WANDERING;
                    }
                }
                break;
                //Starts hunting for the player
            case EnemyStates.HUNTING:{
                    if (HeightDifference == false) {
                        follow();
                    } else if (HeightDifference == true) {

                        EnemySt = EnemyStates.BACKINGOFF;
                    }
                    ApplyWandering();
                    if(playerDistance > 10f) {
                        EnemySt = EnemyStates.STARING;
                    } if(playerDistance <= 3f) {
                        EnemySt = EnemyStates.ATTACKING;
                    }
                }
                break;
                //Starts attacking the player
            case EnemyStates.ATTACKING:
                {
                    AttackManager();
                    ApplyWandering();
                    follow();
                    if(playerDistance > 3f) {
                        EnemySt = EnemyStates.HUNTING;
                    }
                }
                break;
            //BAcks off to a fixed position (because it's stuck against some wall
            case EnemyStates.BACKINGOFF:
                {
                    print("Ik zit nu in state BACKOFF");
                    backoff();

                }
                break;
        }
    }
    //Checks distance from enemy to player and do things at certain distances and checkIfClose to true if distance is further than 15 (wich makes it wander)
    void AttackManager(){
        //If player is closer than 3 run attack every 2 seconds else stop running it.
        //Also if player is closer than 3 distract 10 from the healthbar, else stop doing it.
       if (playerDistance <= 3f && Invoke_Attack == false){
            InvokeRepeating("attack", 2f, 2f);
            Invoke_Attack = true;
            InvokeRepeating("health", 2f, 2f);
        } else if (playerDistance > 3f) {
            CancelInvoke("attack");
            Invoke_Attack = false;
            CancelInvoke("health");
        }
    }

    void health() {
        mySlider.value = mySlider.value -0.1f;
    }
    //Turns to player(lookat)
    void LookAtPlayer(){
        Quaternion rotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }
    //Empty's the Coord list creates a new one and follows the player position and can not walk slant(schuin). It walks on the X axis first.
    void follow() {
        Wa_CurrentCoord = 0;
        Wa_Coords = new List<Vector3>();
        Wa_Coords.Add(player.position);  // Adds player.position
        Vector3 temp = new Vector3(this.transform.position.x + (player.position.x - this.transform.position.x), this.transform.position.y, this.transform.position.z);
        if (Vector3.Distance(this.transform.position, temp) > 1.01f) {
            Wa_Coords.Insert(0, temp);
        }
    }

    //Deals damage based on the given damage.
    void attack(){
        BasePlayer bp = player.GetComponent<BasePlayer>();
        bp.health = bp.health - damage;
    }

    #endregion
    #region Wandering Code (WA)

    List<Vector3> Wa_Coords = new List<Vector3>();
    int Wa_CurrentCoord;
    //Wandering speed
    public float Wa_speed = 1;

    //Checks all 4 corners
    int GetDirection() {
        float _Rot = this.transform.eulerAngles.y;
        float[] _Options = new float[5] { 0f, 90f, 180f, 270f, 360f };
        float _Dist = -1f;
        int _Dir = -1;
        for (int i = 0; i < _Options.Length; i++) {
            float _D = Mathf.Abs(_Options[i] - _Rot);
            if (_D < _Dist || _Dist == -1) {
                _Dir = i;
                _Dist = _D;
            }
        }
        return _Dir;
    }
    //Generates a random amount between 1 and 20 of random coordinates to walk to and puts them in Wa_Coords
    void GenerateWa_Coords()
    {
        Wa_CurrentCoord = 0;
        Wa_Coords = new List<Vector3>();
        int NoWa_Coords = Mathf.RoundToInt(Random.Range(1f, 20f));
        for (int i = 0; i < NoWa_Coords; i++)
        {
            Vector3 now = transform.position;
            float x = (Random.Range(-10f, 10f));
            float z = (Random.Range(-10f, 10f));
            now.x += x;
            now.z += z;
            Wa_Coords.Add(now);
        }
        //Makes the enemy unable to walk slant(schuin). It follows the X axis first.
        List<Vector3> _out = new List<Vector3>();
        for (int _i = 0; _i < (Wa_Coords.Count - 1); _i++) {
            _out.Add(Wa_Coords[_i]);
            _out.Add(new Vector3(Wa_Coords[_i].x + (Wa_Coords[_i + 1].x - Wa_Coords[_i].x), Wa_Coords[_i].y, Wa_Coords[_i].z));
        }
        _out.Add(Wa_Coords[(Wa_Coords.Count - 1)]);
        Wa_Coords = _out;
    }

    //Uses the coordinates genereated earlier and walks to these, when finished repeats with a new set of random coordinates
    void ApplyWandering(){
        if (Wa_CurrentCoord >= Wa_Coords.Count){
            GenerateWa_Coords();
            }
            else{
            //It's a Vector2 because otherwise it can not reach it's destination if the destination point is set inside of a blok below(or above) him.
            Vector2 _epos = new Vector2(this.transform.position.x, this.transform.position.z);
            Vector2 _tpos = new Vector2(Wa_Coords[Wa_CurrentCoord].x, Wa_Coords[Wa_CurrentCoord].z);
                if (Vector2.Distance(_epos, _tpos) <= .2f){
                    Wa_CurrentCoord++;
                }
                //Lookat destination coord.x and .y & Walk forward with the given speed
                else{
                    this.transform.LookAt(new Vector3(Wa_Coords[Wa_CurrentCoord].x, this.transform.position.y, Wa_Coords[Wa_CurrentCoord].z));
                    var forward = transform.TransformDirection(Vector3.forward);
                    controller.SimpleMove(forward * Wa_speed);
                }
            }

    }   
    }
    #endregion