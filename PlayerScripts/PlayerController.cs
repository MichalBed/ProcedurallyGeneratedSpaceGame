using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script handles the player interactions, such as moving around, the user interface and death

public class PlayerController : MonoBehaviour
{
    public float Mass = 1;
    public string currentType = "Asteroid";
    public GameObject model;
    public bool Alive = true;

    public GameObject asteroidModel;
    public GameObject planetModel;
    public GameObject starModel;

    public Rigidbody playerRigid;
    private Transform playerCamera;
    public Renderer planeRenderer;
    public Transform planeTransform;
    private RandomObjectPlacers randomObjectsScript;

    public Text massLabel;
    public Text nameOfObject;
    public Scrollbar scrollbar;
    public Image icon;
    public Text NextLabel;

    public Sprite asteroidIcon;
    public Sprite planetIcon;
    public Sprite starIcon;

    private AssetHolder assets;

    private Vector3 currentChunk = new Vector3(0,0,0);
    
    void Awake()
    {
        playerRigid = transform.GetComponent<Rigidbody>();
        playerCamera = transform.GetChild(1).GetComponent<Transform>();
        //StartCoroutine("rotatePlayer");
        model = transform.GetChild(0).gameObject;
        planeTransform = GameObject.Find("Plane").transform;
        planeRenderer = planeTransform.gameObject.GetComponent<Renderer>();
        randomObjectsScript = GameObject.Find("Manager").GetComponent<RandomObjectPlacers>();
        randomObjectsScript.playerTransform = transform;
        assets = GetComponent<AssetHolder>();
        assets.setPlayerControllerUIComponent(this);

    }

    private void Start() {
        Debug.Log("START");
        if (GetComponent<AsteroidScript>()) {
            Mass = 1;
            currentType = "Asteroid";
        } else if (GetComponent<PlanetScript>()) {
            Mass = 30;
            currentType = "Planet";
        }
    }


    private float ForceValue = 10;
    void FixedUpdate() {
        if (Input.GetKey(KeyCode.A)) {
            playerRigid.AddForce( new Vector3(0, 0, ForceValue) );
        }
        if (Input.GetKey(KeyCode.D)) {
            playerRigid.AddForce(new Vector3(0, 0, -ForceValue));
        }
        if (Input.GetKey(KeyCode.S)) {
            playerRigid.AddForce(new Vector3(-ForceValue, 0, 0));
        }
        if (Input.GetKey(KeyCode.W)) {
            playerRigid.AddForce(new Vector3(ForceValue, 0, 0));
        }

        if (calculateCurrentChunk(transform.position) != currentChunk) {
            Vector3 chunk = calculateCurrentChunk(transform.position);
            // Handles the spawning of objects
            randomObjectsScript.enteredNewChunk(chunk);
            currentChunk = chunk;
        }
        
    }

    /*
    void OnCollisionEnter(Collision collision) {
        Debug.Log("PLAYER COLLISION WITH " + collision.gameObject.name.Split(' ')[0]);

        if (collision.gameObject.name.Split(' ')[0] == "AsteroidEnemy") {
            GameObject collided = collision.gameObject;
            Rigidbody collidedRigid = collided.GetComponent<Rigidbody>();

            if (playerRigid.velocity.magnitude > 7 && (playerRigid.velocity.magnitude * playerRigid.mass + collidedRigid.velocity.magnitude * collidedRigid.mass) > 4) {
                collided.GetComponent<AsteroidScript>().enabled = false;
                MassChanged(Mass + collided.GetComponent<AsteroidScript>().Mass);
                Destroy(collided);
            }
        }

    }
    */

    Vector3 calculateCurrentChunk(Vector3 pos) {
        Vector3 chunk = new Vector3(Mathf.Ceil(pos.x / 100)*100, 0 , Mathf.Ceil(pos.z / 100)*100);      
        return chunk;
    }

    public void MassChanged(float newMass) {
        Mass = newMass;
        massLabel.text = Mass.ToString();
        if (currentType == "Asteroid") {
            scrollbar.size = (Mass / 20);
        } else if (currentType == "Planet") {
            // From 20 to 80
            //Mass - (LeftValue) / (FinalValue - FirstValue)
            scrollbar.size = (Mass - 20)/60;
        }
    
        ForceValue = 10 * Mass;
    }

    public void UIToPlanet() {
        nameOfObject.text = "Planet";
        scrollbar.size = (Mass - 20) / 60;
        icon.sprite = planetIcon;
        NextLabel.text = "To Star";
        currentType = "Planet";
    }

    public void Death() {
        if (Alive == true) {
            Alive = false;
            Debug.Log("Died");
            Destroy(model);
            Destroy(GetComponent<ConfigurableJoint>());
            Destroy(playerRigid);
            Destroy(GetComponent<SphereCollider>());
            //Destroy(GetComponent<SphereCollider>());

            RespawningPlayer respawnScript = gameObject.AddComponent(typeof(RespawningPlayer)) as RespawningPlayer;
            respawnScript.StartCoroutine(respawnScript.Respawn(gameObject.transform.position, transform, this));

            this.enabled = false;
        }
    }

    private float parallaxAmount = 0.04f;
    private void Update() {
        planeTransform.position = new Vector3(transform.position.x, -4, transform.position.z);
        planeRenderer.material.SetTextureOffset("_MainTex", new Vector2(-transform.position.x * parallaxAmount, -transform.position.z * parallaxAmount) );
    }

}
