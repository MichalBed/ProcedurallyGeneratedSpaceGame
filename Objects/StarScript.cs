using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the manager script for each Star in the project, it handles collisions as well as changing the mass if it collides hard enough with another object


public class StarScript : MonoBehaviour
{
    public float Mass = 30;
    private Rigidbody rigid;
    private bool isPlayer = false;
    private PlayerController playerControl;

    public GameObject starModel; //Given by asteroid template 
    private GameObject model; // This one
    private Renderer modelRenderer;

    private Material smallStarMat;
    private Material mediumStarMat;
    private Material largeStarMat;
    private int currentForm = 0; // 0:Small 1:Medium 2:Large

    private AudioSource audioSource;

    private AssetHolder assets;


    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        assets = GetComponent<AssetHolder>();
        starModel = assets.starModel;
        Material[] starMaterials = assets.getStarMaterials();
        smallStarMat = starMaterials[0];
        mediumStarMat = starMaterials[1];
        largeStarMat = starMaterials[2];

        model = Instantiate(starModel, transform.position, transform.rotation, transform);
        modelRenderer = model.GetComponent<Renderer>();
        modelRenderer.material = smallStarMat;

        audioSource = GetComponent<AudioSource>();
    }


    void OnCollisionEnter(Collision collision) {
        
    }






    public void MassChanged(float newMass) {
        Mass = newMass;

        // Scale from 4 to 8
        // 1000 to 2100

        float scaleVal = 4 + ((Mass - 1000) / 275);
        transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
        rigid.mass = Mass;


        // Changes the colour based on how much mass it has
        if (isPlayer) {
            playerControl.MassChanged(newMass);
        }
        if (Mass >= 2100) { // Greater than 2100
            //UpgradeToBlackHole();
        } else if (Mass < 1000) { // Less than 1000
            if (isPlayer) {
                playerControl.Death();
            } else {
                Debug.Log("STAR DIED");
                Destroy(gameObject);
            }

        } else if (Mass > 1000 && Mass < 1500) { // Small Star
            if (currentForm != 0) {
                modelRenderer.material = smallStarMat;
                currentForm = 0;
            }
        } else if (Mass >= 1500 && Mass < 1800) { // Medium Star
            if (currentForm != 1) {
                modelRenderer.material = mediumStarMat;
                currentForm = 1;
            }
        } else { // Large Star
            if (currentForm != 2) {
                modelRenderer.material = largeStarMat;
                currentForm = 2;
            }
        }
    }
}
