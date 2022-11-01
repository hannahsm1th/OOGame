using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
    {
        // This class controls the game, starting a new game and moving the entities around
        // It is a singleton


        //<------- Variables ------->

        public float levelStartDelay = 0.5f;                            //The delay between levels
        public float turnDelay = 0.1f;                                 //The delay between turns
        public static Game instance = null;                             //Static instance of this class
        public HUDSystem hudSystemStatic;                               //This is the HUD System for the game
        public Equipment[] equipmentArray;                              //This is the master list of equipment
        public List<int> equipmentList;                                 //This list keeps track of what equpment has already been generated so we don't get duplicates
                                                                        //The player stats stored in the game so they remain consistent between levels
        public int playerHearts = 6;                                    //The player starts with 3 hearts or 6 heart halves
        public int maxPlayerHearts = 6;                                 //The player also starts with a maximum of 3 hearts
        public int playerLives = 0;                                     //The player's bonus lives
        public int playerAttack = 1;                                    //The player starts with an attack of 1
        public int playerDefense;
        //The player's equipment is stored as the indexes of the master list
        //-1 refers to an empty slot
        public int playerHeadArmourIndex = -1;                          //The player's head armour
        public int playerBodyArmourIndex = -1;                          //The player's body armour
        public int playerPendantIndex = -1;                             //The player's pendant
        public int playerWeaponIndex = 7;                               //The player's weapon (defaults to dagger)		
        [HideInInspector] public int level = 1;                         //Current level number
        [HideInInspector] public bool playerTurn = true;                //Boolean to check if it's players turn

        private Text _levelText;                                        //Text to display current level number
        private GameObject _levelImage;                                 //The image that displays when a level loads
        private Level _levelScript;                                     //Store a reference to the Level class
        private List<Enemy> _enemies;                                   //List of all Enemy units
        private bool _enemiesMoving;                                    //Boolean to check if enemies are moving.
        private bool _doingSetup;                                       //Boolean to check if we're setting up board, prevent Player from moving during setup.

        //<------- Methods ------->

        private void Awake()
        {
            //Check if instance already exists, to enforce the singleton pattern
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            //Keeps this instance persistant between levels
            DontDestroyOnLoad(gameObject);

            //The list of enemies
            _enemies = new List<Enemy>();

            //Populates the equipment list with all the equipment possibilities except the default weapon
            equipmentList = new List<int>();

            for (int i = 0; i < equipmentArray.Length; i++)
            {
                if (i == 7)
                {
                    continue;
                }
                equipmentList.Add(i);
            }

            //Attaches the component to the level script
            _levelScript = GetComponent<Level>();

            //Starts a new game
            StartGame();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //Register the callback to be called every time the next level is loaded
            SceneManager.sceneLoaded += OnLevelWasLoaded;
        }

        static private void OnLevelWasLoaded(Scene arg0, LoadSceneMode arg1)
        {
            instance.level++;

            instance.StartGame();
        }

        private void StartGame()
        {
            //this prevents the player from moving while the title card is displaying
            _doingSetup = true;

            //Gets the references for the title card
            _levelImage = GameObject.Find("LevelImage");

            _levelText = GameObject.Find("LevelText").GetComponent<Text>();

            //Displays the level number
            _levelText.text = "Level " + level;

            //Blocks out the screen with the level image for the specified delay time
            _levelImage.SetActive(true);

            Invoke("HideLevelImage", levelStartDelay);

            //Clear any Enemy objects in our List
            _enemies.Clear();

            //Call the SetupScene function of the Level script, pass it current level number
            _levelScript.SetupScene(level);
        }

        private void HideLevelImage()
        {
            //Disables the title card to reveal the game
            _levelImage.SetActive(false);

            //Set doingSetup to false allowing player to move again
            _doingSetup = false;
        }

        private void Update()
        {
            //If any of these are true, return and do not start MoveEnemies
            if (playerTurn || _enemiesMoving || _doingSetup)
            {
                return;
            }

            //Start moving enemies
            StartCoroutine(MoveEnemies());
        }

        public void AddEnemyToList(Enemy script)
        {
            //Add a new enemy to the list of enemies
            _enemies.Add(script);
        }

        public void RemoveEnemyFromList(Enemy script)
        {
            //Removes an enemy from the list of enemies
            _enemies.Remove(script);
        }

        public void GameOver()
        {
            //This is called when the player runs out of hearts

            //Sets and displays he text for the game over message
            _levelText.text = "GAME OVER!";

            _levelImage.SetActive(true);

            //Disable this Game
            enabled = false;

            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }

        public void Win()
        {
            //This is called when the player defeats the final level

            //Sets and displays the text for the win message
            _levelText.text = "Congratulations!\nYou finished the game!";

            _levelImage.SetActive(true);

            //Disable this Game
            enabled = false;
        }

        private IEnumerator MoveEnemies()
        {
            //This method handles enemy movement, allowing them all to move in sequence

            _enemiesMoving = true;

            //Wait for turnDelay seconds, default value is .1 (100 ms)
            yield return new WaitForSeconds(turnDelay);

            //If there are no enemies on the board (if they are all dead) still wait the turnDelay between player moves:
            if (_enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }

            //Loop through list of enemies and calls on them to move in sequence
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].MoveEnemy();
                yield return new WaitForSeconds(_enemies[i].moveTime);
            }

            //Allows the player to move again
            playerTurn = true;

            _enemiesMoving = false;
        }
    }