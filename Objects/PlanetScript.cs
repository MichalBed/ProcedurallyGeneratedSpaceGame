using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the manager script for each Planet in the project, it handles collisions as well as changing the mass if it collides with objects



public class PlanetScript : MonoBehaviour
{
    public float Mass = 30;
    public Rigidbody rigid;
    private bool isPlayer = false;
    private PlayerController playerControl;

    public GameObject planetModel; //Given by asteroid template 
    private Material planetTexture;
    private GameObject model; // This one
    private SphereCollider modelSphereCollider;

    private AudioClip collidePlanetSFX;
    private AudioSource audioSource;
    private GameObject dustParticle;

    private AssetHolder assets;

    private List<Transform> asteroidChildren = new List<Transform>();
    private List<ConfigurableJoint> orbitJoints = new List<ConfigurableJoint>();

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        assets = GetComponent<AssetHolder>();
        planetModel = assets.planetModel;
        Material planetTexture = assets.getPlanetMaterial();

        model = Instantiate(planetModel, transform.position, transform.rotation, transform);
        model.GetComponent<Renderer>().material = planetTexture;
        modelSphereCollider = model.GetComponent<SphereCollider>();
        dustParticle = assets.dustParticle; 

        audioSource = GetComponent<AudioSource>();
        collidePlanetSFX = assets.planetCollideSFX;


    }

    private void Start() {
        float scaleVal = 2 + (((Mass - 30) / 50) * 1);
        transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);

        if (GetComponent<PlayerController>()) {
            isPlayer = true;
            playerControl = GetComponent<PlayerController>();
        }
        if (!isPlayer) {
            SphereCollider sphereCol = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
            sphereCol.center = new Vector3(0, -0.03f, 0.04f);
            sphereCol.radius = 0.9f;

            SphereCollider sphereCol2 = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
            sphereCol2.center = new Vector3(0, -0.03f, 0.04f);
            sphereCol2.radius = 0.9f;
            sphereCol2.isTrigger = true;

            float randX = Random.Range(-700, 700);
            float randZ = Random.Range(-700, 700);
            rigid.AddForce(randX, 0, randZ);
        }
        if (isPlayer) {
            playerControl.model = model;
            Rigidbody rigidsubModel = model.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rigidsubModel.mass = 1;
            rigidsubModel.angularDrag = 0;
            rigidsubModel.useGravity = false;
            
            RandomRotator rotator = model.AddComponent(typeof(RandomRotator)) as RandomRotator;
            rotator.tumble = 1;
            gameObject.GetComponent<ConfigurableJoint>().connectedBody = rigidsubModel;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<PlanetScript>()) { // Has planet script

            PlanetScript planetScript = collision.gameObject.GetComponent<PlanetScript>();
            float newMass = planetScript.Mass - Mass;
            newMass = Mathf.Abs(newMass);
            planetScript.MassChanged(newMass);

            GameObject collided = collision.gameObject;
            Rigidbody collidedRigid = collided.GetComponent<Rigidbody>();

            // MASS TAKEN AWAY BASED ON SPEED
            // PUSH AWAY BASED ON SPEED

            audioSource.clip = collidePlanetSFX;
            audioSource.Play();

            collidedRigid.AddForce(Mass * 20 * (collided.transform.position - transform.position).normalized); // Push object away
            rigid.AddForce(planetScript.Mass * 20 * (transform.position - collided.transform.position).normalized); // Push self away

            GameObject impactParticle = Instantiate(dustParticle, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal), collision.gameObject.transform);
            impactParticle.transform.up = collision.GetContact(0).normal;
            var main = impactParticle.GetComponent<ParticleSystem>().main;
            main.startSize = transform.localScale.x;
            impactParticle.GetComponent<ParticleSystem>().Play();
            Destroy(impactParticle, 1);

        } else if (collision.gameObject.GetComponent<StarScript>()) {
            GameObject collided = collision.gameObject;
            Rigidbody collidedRigid = collided.GetComponent<Rigidbody>();

            // change the mass of the collided star
            StarScript collidedStarScript = collision.gameObject.GetComponent<StarScript>();
            collidedStarScript.MassChanged(collidedStarScript.Mass - Mass);

            GameObject impactParticle = Instantiate(dustParticle, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal), collision.gameObject.transform);
            impactParticle.transform.up = collision.GetContact(0).normal;
            //impactParticle.transform.localScale = new Vector3(Mass, Mass, Mass);
            impactParticle.GetComponent<ParticleSystem>().Play();
            Destroy(impactParticle, 1);

            if (isPlayer) {
                playerControl.Death();
            } else {
                Destroy(gameObject);
            }

        }
    }

    /*
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.name == "test_asteroid_LODGroup" && !asteroidChildren.Contains(collider.transform.parent) && asteroidChildren.Count < 11) {
            Transform asteroidChild = collider.transform.parent;
            asteroidChild.parent = transform;

            asteroidChildren.Add(asteroidChild);
            StartCoroutine(this.orbitAround(asteroidChild));
            //ConfigurableJoint joint = model.AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
            //joint.connectedBody = asteroidChild.GetComponent<Rigidbody>();
            //orbitJoints.Add(joint);
        }
    }

    public IEnumerator orbitAround(Transform astChild) {


        astChild.GetComponent<Rigidbody>().isKinematic = true;
        int orbitPosition = asteroidChildren.IndexOf(astChild)+1;

        float sin = 0; // Horizontal
        float cos = 0; // Vertical
        float multiplier = orbitPosition * 4f;

        while (astChild.parent != null) {
            
            astChild.position = transform.position + new Vector3(Mathf.Sin(sin * Mathf.Deg2Rad) * multiplier,0,Mathf.Cos(cos * Mathf.Deg2Rad) * multiplier);

            sin += 0.1f;
            cos += 0.1f;
            yield return new WaitForSeconds(1/60);
        }
        // Stopped being attached to the parent
        asteroidChildren.Remove(astChild);
        Debug.Log("ASTEROID HAS DIED");
    }

    */

    public void MassChanged(float newMass) {
        Mass = newMass;
        if (Mass >= 20) {
            float scaleVal = 2 + ((Mass - 30) / 50);
            transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
            rigid.mass = Mass;
        }
        if (isPlayer) {
            playerControl.MassChanged(newMass);
        }
        if (Mass >= 80) {
            this.enabled = false;
        } else if (Mass < 20) {

            GameObject impactParticle = Instantiate(dustParticle, transform.position, transform.rotation, null);
            var main = impactParticle.GetComponent<ParticleSystem>().main;
            main.startSize = 7;
            var shape = impactParticle.GetComponent<ParticleSystem>().shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            AudioSource impactSfx = impactParticle.AddComponent(typeof(AudioSource)) as AudioSource;
            impactSfx.clip = collidePlanetSFX;
            impactSfx.Play();

            impactParticle.GetComponent<ParticleSystem>().Play();
            Destroy(impactParticle, 1);

            if (isPlayer) {
                playerControl.Death();
            } else {
                Debug.Log("PLANET DIED");
                Destroy(gameObject);
            }

        }
    }

    public void playCollideSFX() {
        audioSource.clip = collidePlanetSFX;
        audioSource.Play();
    }

}
