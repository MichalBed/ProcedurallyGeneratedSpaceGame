using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHolder : MonoBehaviour
{
    // This script holds all references to prefabs and models needed to the project to function and can be accessed by other scripts


    // ---------------General------------------
    public void Awake() {
        canvasObject = GameObject.Find("Canvas");
    }

    public GameObject dustParticle;

    private GameObject canvasObject;

   

    public void setPlayerControllerUIComponent(PlayerController playerController) {
        
    }




    // -------------Asteroid------------------
    public GameObject asteroidModel;

    // ------------Planet------------------
    public GameObject planetModel;
    public Material[] planetMaterials;

    public Material getPlanetMaterial() {
        int index = Random.Range(0, planetMaterials.Length - 1);
        return planetMaterials[index];
    }

    // ------------Star----------------

    public GameObject starModel;
    public Material smallStarMat;
    public Material mediumStarMat;
    public Material largeStarMat;

    public Material[] getStarMaterials() {
        Material[] materials = new Material[3];
        materials[0] = smallStarMat;
        materials[1] = mediumStarMat;
        materials[2] = largeStarMat;
        return materials;
    }
    // ------------Black Hole----------

    public AudioClip asteroidCollideSFX;
    public AudioClip planetCollideSFX;
    public AudioClip absorbSFX;
}
