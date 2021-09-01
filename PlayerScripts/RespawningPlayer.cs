using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is called by PlayerController to respawn the player and rebuild all of the deleted components on death

public class RespawningPlayer : MonoBehaviour
{
    public IEnumerator Respawn(Vector3 deathPos, Transform player, PlayerController playerControl) {
        Vector3 startValue = player.position;
        Vector3 destination = startValue + Random.onUnitSphere * 100;
        destination.y = 0;
        string playerType = playerControl.currentType;

        AssetHolder assets = gameObject.GetComponent<AssetHolder>();

        yield return new WaitForSeconds(1);

        Transform planeTransform = playerControl.planeTransform;
        Renderer planeRenderer = playerControl.planeRenderer;

        var t = 0f;
        while (t < 1.5f) {
            t += Time.deltaTime / 1.5f;
            transform.position = Vector3.Lerp(startValue, destination, t);
            yield return null;
        }

        Rigidbody playerRigid = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        playerRigid.useGravity = false;
        playerRigid.drag = 0.1f;
        playerRigid.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        ConfigurableJoint configJoint = gameObject.AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
        configJoint.xMotion = ConfigurableJointMotion.Locked;
        configJoint.yMotion = ConfigurableJointMotion.Locked;
        configJoint.zMotion = ConfigurableJointMotion.Locked;

        SphereCollider sphereCol = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        sphereCol.radius = 1.02f;
        // RE ADD COLLIDERS

        playerControl.enabled = true;
        if (playerType == "Asteroid") {
            GameObject asteroidModel = assets.asteroidModel;
            GameObject model = Instantiate(asteroidModel, transform.position, transform.rotation, transform);
            configJoint.connectedBody = model.GetComponent<Rigidbody>();
            playerControl.model = model;
            playerControl.playerRigid = playerRigid;
            AsteroidScript asteroidScript = gameObject.GetComponent<AsteroidScript>();
            asteroidScript.rigid = playerRigid;
            asteroidScript.asteroidModel = model;
            asteroidScript.MassChanged(2);

        } else if (playerType == "Planet") {
            GameObject planetModel = assets.planetModel;
            Material planetTexture = assets.getPlanetMaterial();

            GameObject model = Instantiate(planetModel, transform.position, transform.rotation, transform);
            model.GetComponent<Renderer>().material = planetTexture;
            playerControl.model = model;
            playerControl.playerRigid = playerRigid;
            PlanetScript planetScript = gameObject.GetComponent<PlanetScript>();
            planetScript.planetModel = model;
            planetScript.rigid = playerRigid;
            Rigidbody subRigid = model.AddComponent(typeof(Rigidbody)) as Rigidbody;
            subRigid.useGravity = false;
            subRigid.drag = 0;
            subRigid.angularDrag = 0;
            configJoint.connectedBody = subRigid;
            planetScript.MassChanged(Random.Range(30, 40));
        } else if (playerType == "Star") {

        }
        playerControl.Alive = true;


        // Masses

        // Asteroid: 2

        // Planet: 20 - 40
        Destroy(this);
    }
}
