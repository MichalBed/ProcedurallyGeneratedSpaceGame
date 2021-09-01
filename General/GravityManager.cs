using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script manages the gravity force on every object in the scene. The more mass an object has and the closer it is to another the more force it will exert on it

public class GravityManager : MonoBehaviour
{
    private void Start() {

        StartCoroutine("calculateGravity");
    }

    IEnumerator calculateGravity() {

        // F = G(m1m2 / r^2)

        float G = 3f;

        while (true) {
            yield return new WaitForSeconds(0.1f);
            var objects = GameObject.FindGameObjectsWithTag("GravityObject");
            var objectCount = objects.Length;
            for (int i = 0; i < objectCount; i++) {



                for (int j = 0; j < objectCount; j++) {

                    try {
                        Rigidbody rigid = objects[i].GetComponent<Rigidbody>();
                        float rigidMass = rigid.mass;
                        Rigidbody rigidAttract = objects[j].GetComponent<Rigidbody>();

                        float distanceSquared = (rigid.position - rigidAttract.position).sqrMagnitude;

                        float forceMagnitude = (rigidMass * rigidAttract.mass) / distanceSquared;
                        Vector3 force = G * (rigid.position - rigidAttract.position).normalized * forceMagnitude;
                        if (force.sqrMagnitude > 0) {
                            rigidAttract.AddForce(force);
                        }
                    } catch (System.Exception e) {

                    }
                }
            }

        }
    }
}
