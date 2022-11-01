using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
    {
        // This class is responsible for managing everything about the level construction


        //<------- Variables ------->

        //These variables are public so they can be seen in the Unity inspector
        public int width = 8;                                           //The width of the level
        public int height = 8;                                          //The height of the level
        public GameObject exit;                                         //Prefab to spawn for exit
        public GameObject[] floorTiles;                                 //Array of floor tiles
        public GameObject[] wallTiles;                                  //Array of wall tiles
        public GameObject[] itemTiles;                                  //Array of item tiles
        public GameObject[] enemyTiles;                                 //Array of enemy tiles
        public GameObject[] bossTiles;                                  //Array of boss tiles
        public GameObject[] outerWallTiles;                             //Array of outer wall tiles
        public Count wallCount = new Count(5, 9);                       //Lower and upper limit for our random number of walls
        public Count itemCount = new Count(1, 2);                       //Lower and upper limit for our random number of items

        private Transform _level;                                       //The reference to the Transform of the level
        private List<Vector3> _gridPositions = new List<Vector3>();     //A list of possible locations to place tiles.

        //<------- Methods ------->

        private void InitialiseList()
        {
            //Clears out the old list of grid positions to make a new level

            _gridPositions.Clear();

            //Add a new Vector3 to each position in the list with the coordinates of that location
            for (int x = 1; x < height - 1; x++)
            {
                for (int y = 1; y < width - 1; y++)
                {
                    _gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }

        private void LevelSetup()
        {
            //Makes a new level and sets up the outer walls and floor

            _level = new GameObject("Level").transform;

            //Filling in the walls and floor
            for (int x = -1; x < height + 1; x++)
            {
                for (int y = -1; y < height + 1; y++)
                {
                    //Chooses a random floor tile to use
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                    //If we are at the outer edge, place an outer wall
                    if (x == -1 || x == height || y == -1 || y == height)
                    {
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    }

                    //Create a new game object instance for each tile
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                    //Setting it correctly in the hierarchy
                    instance.transform.SetParent(_level);
                }
            }
        }

        private Vector3 RandomPosition()
        {
            //This is used to choose a random position for the inner walls, enemies and items in the list

            int randomIndex = Random.Range(0, _gridPositions.Count);

            Vector3 randomPosition = _gridPositions[randomIndex];

            _gridPositions.RemoveAt(randomIndex);

            return randomPosition;
        }

        private void RandomPlace(GameObject[] tileArray, int minimum, int maximum)
        {
            //Choose a random selection of the array it is passed to distribute throughout the level

            //Choose a random number of objects
            int objectCount = Random.Range(minimum, maximum + 1);

            //Places random objects around the level
            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }

        public void SetupScene(int level)
        {
            //This sets up the level using the other functions in this class

            //Creates the outer walls and floor
            LevelSetup();

            //Reset our the grid positions
            InitialiseList();

            //Places a random number of inner wall
            RandomPlace(wallTiles, wallCount.minimum, wallCount.maximum);

            //Place a random number of items
            RandomPlace(itemTiles, itemCount.minimum, itemCount.maximum);

            //Creates the boss level, or a regular level
            if (level == 4)
            {
                SoundManager.instance.musicSource.Stop();
                SoundManager.instance.bossMusicSource.Play();
                Instantiate(bossTiles[Random.Range(0, bossTiles.Length)], RandomPosition(), Quaternion.identity);
            }
            else
            {
                //Determine number of enemies based on current level number
                int enemyCount = level * 2;

                //Create the specified number of enemies
                RandomPlace(enemyTiles, level, enemyCount);
            }

            for (int i = 0; i < Random.Range(1, 2); i++)
            {
                int randomEquipmentIndex = Random.Range(0, Game.instance.equipmentList.Count);
                Instantiate(Game.instance.equipmentArray[Game.instance.equipmentList[randomEquipmentIndex]], RandomPosition(), Quaternion.identity);
                Game.instance.equipmentList.Remove(Game.instance.equipmentList[randomEquipmentIndex]);
            }

            //Place the exit tile in the upper right hand corner of our game board
            Instantiate(exit, new Vector3(width - 1, height - 1, 0f), Quaternion.identity);
        }

        [Serializable]
        public class Count
        {
            //An encapsulated class used to determine the minimum and maximum for tile arrays


            //<------- Variables ------->

            public int minimum;                                         //Minimum value for our Count class
            public int maximum;                                         //Maximum value for our Count class

            //<------- Methods ------->

            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }
    }