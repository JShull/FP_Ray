using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;



namespace FuzzPhyte.Ray.Examples
{

    public class Patrol : MonoBehaviour
    {



    public float Normal_speed=5f;
    public float Slow_speed=3f;
    public float Fast_speed=7f;
    public float speed;

    private Vector3 Velocity;
    private UnityEngine.CharacterController controller;
    public Transform CharacterRoot;
    public UnityEvent OnRayEnterEvent;
    [Space]
    [Header("Wall Collider Raycast")]
    public FP_RayMono RayCastReference;
    [Space]
    [Header("Coin Collider Raycast")]
    public FP_RayMono CoinRaycast;
    [Space]
    [Header("Rupoor Collider Raycast")]
    public FP_RayMono RupoorRaycast;

//UI Stuff
    public TextMeshProUGUI scoreTextC;
    private int scoreCoin;
    public TextMeshProUGUI scoreTextR;
    private int scoreRupoor;

    public Vector3 move;

        // Start is called before the first frame update
        private void Awake(){
            controller = CharacterRoot.gameObject.AddComponent<CharacterController>();
            speed = Normal_speed;
        }
        public void Start()
        {
            
            RayCastReference.Raycaster.OnFPRayEnterHit += OnRayEnter;
            CoinRaycast.Raycaster.OnFPRayEnterHit += OnRayEnterCoin;
            RupoorRaycast.Raycaster.OnFPRayEnterHit += OnRayEnterRupoor;
            
            scoreCoin = 0;
            scoreTextC.text = "Coins: " + scoreCoin;

            scoreRupoor = 0;
            scoreTextR.text = "Rupoors: " + scoreRupoor;

        }
        public void OnDisable(){
            RayCastReference.Raycaster.OnFPRayEnterHit -= OnRayEnter;
            CoinRaycast.Raycaster.OnFPRayEnterHit-=OnRayEnterCoin;
            RupoorRaycast.Raycaster.OnFPRayEnterHit -= OnRayEnterRupoor;


        }

        // Update is called once per frame
        public void Update()
        {
         
            controller.Move(move * Time.deltaTime * speed);

            if (Input.GetKeyDown(KeyCode.N)){ //just to reset to normal speed if wanted
           
                speed=Normal_speed*(Mathf.Sign(speed));

        }


            scoreTextC.text = "Coins: " + scoreCoin;
            scoreTextR.text = "Rupoors: " + scoreRupoor;
            
            
        }


        public void OnRayEnter(object sender, FP_RayArgumentHit arg) //needs to happen with event
        {
            //Debug.Log($"Hello {arg.RayType.ToString()}");

            speed*=-1;
            CharacterRoot.rotation *= Quaternion.Euler(0,180,0);
            OnRayEnterEvent.Invoke();

        }
        public void OnRayEnterCoin(object sender, FP_RayArgumentHit arg){
            //Debug.Log($"Coin Ray Hit a thing...");
            //arg.HitObject.gameObject.SetActive(false);
            scoreCoin+=1;
            Destroy(arg.HitObject.gameObject);
            speed=Fast_speed*(Mathf.Sign(speed));
        }

        public void OnRayEnterRupoor(object sender, FP_RayArgumentHit arg){

            //arg.HitObject.gameObject.SetActive(false);
            scoreRupoor+=1;
            Destroy(arg.HitObject.gameObject);
            speed=Slow_speed*(Mathf.Sign(speed));
        }

        

    } //class
}


