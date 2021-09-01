using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script places random asteroids, planets and stars around the player as they explore more chunks
// It also cleans up any chunks and objects that are too far from the player, in order to save computer resources

public class RandomObjectPlacers : MonoBehaviour
{
    public GameObject asteroid;
    public GameObject planet;
    public GameObject star;

    public Transform playerTransform;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private List<Vector3> chunksList = new List<Vector3>();

    private void Start() {
        StartCoroutine("cleanUp");
    }

    // If objects too far away, delete
    // If chunk too far away, clear from chunks List
    public IEnumerator cleanUp() {
        yield return new WaitForSeconds(5);
        while (true) {
            for (int i = 0; i < spawnedObjects.Count; i++) {
                try {
                    GameObject obj = spawnedObjects[i];
                    if ((obj.transform.position - playerTransform.position).sqrMagnitude >= 1000000) {
                        spawnedObjects.Remove(obj);
                        Destroy(obj);
                    }
                } catch (System.Exception e) {

                }
            }
            for (int i = 0; i < chunksList.Count; i++) {
                try {
               
                    if ((chunksList[i] - playerTransform.position).sqrMagnitude >= 1000000) {
                        chunksList.Remove(chunksList[i]);
                    }
                } catch (System.Exception e) {

                }
            }
            yield return new WaitForSeconds(4);
        }
    }

    public void enteredNewChunk(Vector3 chunk) {
        generateChunk(chunk);
        adjacentChunks(chunk);
    }


    // Generates 8 chunks around a center chunk
    public void adjacentChunks(Vector3 chunk) {
        generateChunk( leftChunk(chunk)  );
        generateChunk(rightChunk(chunk));
        generateChunk(upChunk(chunk));
        generateChunk(downChunk(chunk));

        generateChunk(upChunk(leftChunk(chunk)));
        generateChunk(upChunk(rightChunk(chunk)));
        generateChunk(downChunk(leftChunk(chunk)));
        generateChunk(downChunk(rightChunk(chunk)));
    }
    //--------------------------------------------//
    public Vector3 leftChunk(Vector3 chunk) {
        return (chunk + new Vector3(0, 0, 100));
    }

    public Vector3 rightChunk(Vector3 chunk) {
        return (chunk + new Vector3(0, 0, -100));
    }

    public Vector3 upChunk(Vector3 chunk) {
        return (chunk + new Vector3(100, 0, 0));
    }

    public Vector3 downChunk(Vector3 chunk) {
        return (chunk + new Vector3(-100, 0, 0));
    }
    //--------------------------------------------//




    public void generateChunk(Vector3 chunk) {
        if (!chunksList.Contains(chunk)) {
            DrawChunk(chunk, Color.cyan, 5);
            chunksList.Add(chunk);
            chunkObjectSpawner(chunk); // Spawn
        } else {
            DrawChunk(chunk, Color.red, 5);
        }
    }

    public void chunkObjectSpawner(Vector3 chunk) {
        bool[,] chunkTable = new bool[10, 10] {  // <10 Lists, 10 per list>
             
           // [x,0] [x,1] [x,2] [x,3] [x,4] [x,5] [x,6] [x,7] [x,8] [x,9]
             {false,false,false,false,false,false,false,false,false,false},   // [0,x]
             {false,false,false,false,false,false,false,false,false,false},   // [1,x]
             {false,false,false,false,false,false,false,false,false,false},   // [2,x]
             {false,false,false,false,false,false,false,false,false,false},   // [3,x]
             {false,false,false,false,false,false,false,false,false,false},   // [4,x]
             {false,false,false,false,false,false,false,false,false,false},   // [5,x]
             {false,false,false,false,false,false,false,false,false,false},   // [6,x]
             {false,false,false,false,false,false,false,false,false,false},   // [7,x] 
             {false,false,false,false,false,false,false,false,false,false},   // [8,x]
             {false,false,false,false,false,false,false,false,false,false}, };// [9,x]

        //int x = Random.Range(0, 9);
        //int z = Random.Range(0, 9);
        //Debug.Log("Relative position of: "+ x + " " + z);
        //Vector3 spawnPos = chunk - new Vector3(x*10,0,z*10);
        //Debug.Log("Real position of: " + spawnPos);

        // Star systems have their own chunk
        // Asteroids and planets can be together

        if (chunk.x == 100 && chunk.z == 100) { // Spawn

            int numbAsteroids = Random.Range(24, 35);
            spawnAsteroidField(spawnPositions(numbAsteroids,chunkTable,chunk));
        } else { // Not spawn
            int rand = Random.Range(1, 100);

            if (rand <= 50) { // Empty chunk
                // Nothing
            } else if (rand > 50 && rand <= 70) { // Lone Asteroid field 
                // Just asteroids
                int numbAsteroids = Random.Range(5, 35);
                spawnAsteroidField(spawnPositions(numbAsteroids, chunkTable, chunk));
            } else if (rand > 75 && rand <= 95) { // Asteroids and planets
                int numbAsteroids = Random.Range(10, 25);
                spawnAsteroidField(spawnPositions(numbAsteroids, chunkTable, chunk));

                int numbPlanets = Random.Range(1, 2);
                spawnPlanets(spawnPositions(numbPlanets, chunkTable, chunk));

                // Asteroids and planets
            } else if (rand > 95 && rand <= 100) { // Star system 
                // Star system
                spawnStarSystem(spawnPositions(1, chunkTable, chunk));
            }

        }
        // Empty chunk
        // Asteroid field
        // Asteroids and planet      
        // Star system
    }


    // This function calculates where in each chunk and object can spawn without it overlapping another object
    public List<Vector3> spawnPositions(int nPositions, bool[,] chunkTable, Vector3 chunk) {
        List<Vector3> spawnPositions = new List<Vector3>();

        for (int i = 0; i < nPositions; i++) {
            bool allowed = false;

            while (allowed == false) {
                int x = Random.Range(0, 9);
                int z = Random.Range(0, 9);
                if (chunkTable[x, z] == false) {
                    chunkTable[x, z] = true;
                    allowed = true;
                    spawnPositions.Add(chunk - new Vector3(x * 10, 0, z * 10));
                }
            }
        }
        return spawnPositions;
    }

    public void spawnAsteroidField(List<Vector3> spawnPositions) { // Spawns asteroid fields per chunk
        foreach (Vector3 pos in spawnPositions) {
            GameObject spawnedAsteroid = Instantiate(asteroid, pos, Quaternion.identity);
            spawnedObjects.Add(spawnedAsteroid);
        }
    }

    public void spawnPlanets(List<Vector3> spawnPositions) {
        foreach (Vector3 pos in spawnPositions) {
            GameObject spawnedPlanet = Instantiate(planet, pos, Quaternion.identity);
            spawnedPlanet.GetComponent<PlanetScript>().MassChanged(Random.Range(30, 60));
            spawnedObjects.Add(spawnedPlanet);
        }
    }

    public void spawnStarSystem(List<Vector3> spawnPositions) {
        foreach (Vector3 pos in spawnPositions) {
            GameObject spawnedStar = Instantiate(star, pos, Quaternion.identity);
            spawnedStar.GetComponent<StarScript>().MassChanged(Random.Range(1100, 2000));
            spawnedObjects.Add(spawnedStar);
        }
    }


    void DrawChunk(Vector3 topLeft, Color color, int time) {
        Debug.DrawLine(topLeft, topLeft + new Vector3(0,0,-100), color, time); // Top
        Debug.DrawLine(topLeft + new Vector3(0, 0, -100), topLeft + new Vector3(-100, 0, -100), color, time); // Right
        Debug.DrawLine(topLeft + new Vector3(-100, 0, -100), topLeft + new Vector3(-100, 0, 0), color, time); // Bottom
        Debug.DrawLine(topLeft + new Vector3(-100, 0, 0), topLeft, color, time); // Left

    }
}
