using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the manager script for each Asteroid in the project, it handles collisions as well as changing the mass if it collides hard enough with another asteroid


public class AsteroidScript : MonoBehaviour
{
    public float Mass = 1;
    public Rigidbody rigid;
    private bool isPlayer = false;
    private PlayerController playerControl;

    public GameObject asteroidModel;
    private AudioClip asteroidCollideSFX;
    private AudioClip absorbSFX;
    private AudioSource audioSource;
    private GameObject dustParticle;

    private AssetHolder assets;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        assets = GetComponent<AssetHolder>();

        asteroidCollideSFX = assets.asteroidCollideSFX;
        absorbSFX = assets.absorbSFX;
        dustParticle = assets.dustParticle;
    }

    private void Start() {
        if (GetComponent<PlayerController>()) {
            isPlayer = true;
            playerControl = GetComponent<PlayerController>();
        }
        if (!isPlayer) {
            float randX = Random.Range(-70, 70);
            float randZ = Random.Range(-70, 70);
            rigid.AddForce(randX, 0, randZ);
        }
        float scaleVal = 1 + ((Mass / 40) * 1);
        transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
    }

    void OnCollisionEnter(Collision collision) {
        if (!rigid) {
            rigid = GetComponent<Rigidbody>();
        }
        if (collision.gameObject.name.Split(' ')[0] == "AsteroidEnemy(Clone)") {
            GameObject collided = collision.gameObject;
            Rigidbody collidedRigid = collided.GetComponent<Rigidbody>();
            AsteroidScript collidedAsteroid = collided.GetComponent<AsteroidScript>();

            collidedRigid.AddForce(Mass * 20 * (collided.transform.position - transform.position).normalized ); // Push object away
            rigid.AddForce(collidedAsteroid.Mass * 20 * (transform.position - collided.transform.position).normalized); // Push self away

            // If current rigidbody momentum is bigger than collided objects
            //Debug.Log("TOTAL MOMENTUM IS "+rigid.velocity.magnitude * rigid.mass + collidedRigid.velocity.magnitude * collidedRigid.mass);
            audioSource.clip = asteroidCollideSFX;
            audioSource.Play();

            GameObject impactParticle = Instantiate(dustParticle, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal), collision.gameObject.transform);
            impactParticle.transform.up = collision.GetContact(0).normal;
            var main = impactParticle.GetComponent<ParticleSystem>().main;
            main.startSize = transform.localScale.x;
            impactParticle.GetComponent<ParticleSystem>().Play();
            Destroy(impactParticle, 1);

            // FIX VELOCITY MASS EQUATION
            
            //Debug.Log((rigid.velocity.magnitude * rigid.mass + collidedRigid.velocity.magnitude * collidedRigid.mass));

            


            if ((rigid.velocity.magnitude * rigid.mass + collidedRigid.velocity.magnitude * collidedRigid.mass) > 3) {
                if (!isPlayer || isPlayer && rigid.velocity.magnitude > 5) {
                    if (Mathf.Abs(rigid.velocity.magnitude) * rigid.mass > Mathf.Abs(collidedRigid.velocity.magnitude) * collidedRigid.mass) {
                        collided.GetComponent<AsteroidScript>().enabled = false;
                        MassChanged(Mass + collided.GetComponent<AsteroidScript>().Mass);
                        Destroy(collided);
                        audioSource.clip = absorbSFX;
                        audioSource.Play();
                    }
                }
            }
        } else if (collision.gameObject.GetComponent<PlanetScript>()) {
            GameObject collided = collision.gameObject;
            Rigidbody collidedRigid = collided.GetComponent<Rigidbody>();

            // change the mass of the collidedp lanet
            PlanetScript collidedPlanetScript = collision.gameObject.GetComponent<PlanetScript>();
            collidedPlanetScript.MassChanged(collidedPlanetScript.Mass - Mass);
            collidedRigid.AddForce(Mass * 3 * rigid.velocity);
            collidedPlanetScript.playCollideSFX();

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

    public void MassChanged(float newMass) {
        Mass = newMass;
        float scaleVal = 1 + (Mass / 40);
        transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
        rigid.mass = Mass;
        if (isPlayer) {
            playerControl.MassChanged(newMass);
        }
        if (Mass >= 20) {
            UpgradeToPlanet();
        }
    }


    public void UpgradeToPlanet() {
        PlanetScript planetScript = gameObject.AddComponent<PlanetScript>();
        planetScript.Mass = 30;
        planetScript.enabled = true;
        if (isPlayer) {
            playerControl.MassChanged(30);
            playerControl.UIToPlanet();
        } else {
            transform.name = "PlanetEnemy";
        }
        Destroy(asteroidModel);
        Destroy(this);
    }

}
