using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : Entity
    {
        // This class controls how the players and enemies move and collide with each other.


        //<------- Variables ------->

        [HideInInspector] public float restartLevelDelay = 1f;                            //Delay time in seconds
        [HideInInspector] public int wallDamage = 1;                                      //How much damage the player does to a wall
        public Text UIText;                                             //UI text of player's stats
        //The players's attack sound effects
        public AudioClip moveSound1;
        public AudioClip moveSound2;
        public AudioClip eatSound1;
        public AudioClip eatSound2;
        public AudioClip drinkSound1;
        public AudioClip drinkSound2;
        public AudioClip gameOverSound;

        private int _heartHalves;                                       //How many hearts the player has stored in halves
        private int _lives;                                             //The number of spare lives the player has
        private int _maxHearts;                                         //The total possible hearts a player can have
        private int _headArmourIndex;                                   //The player's head armour index in the master list                                    
        private int _bodyArmourIndex;                                   //The player's body armour index in the master list
        private int _pendantIndex;                                      //The player's pendant index in the master list
        private int _weaponIndex;                                       //The player's weapon index in the master list
        private Animator _animator;                                     //The player's animator component

        //<------- Methods ------->

        protected override void Start()
        {
            _animator = GetComponent<Animator>();

            //Gets the current player stats and equipment stored in the Game instance between levels
            _heartHalves = Game.instance.playerHearts;

            _maxHearts = Game.instance.maxPlayerHearts;

            _lives = Game.instance.playerLives;

            base.attack = Game.instance.playerAttack;

            base.defense = Game.instance.playerDefense;

            HeadArm = Game.instance.playerHeadArmourIndex;

            BodyArm = Game.instance.playerBodyArmourIndex;

            Pendant = Game.instance.playerPendantIndex;

            Weapon = Game.instance.playerWeaponIndex;

            base.moveTime = 0.1f;

            UIText = GameObject.Find("UIText").GetComponent<Text>();

            //Update the text HUD
            HUDUpdate update = () => UIText.text = "\nATK:" + base.attack + " DFN: " + base.defense + "\nLives: " + _lives;
            update();

            //Calls the base method
            base.Start();
        }

        private void OnDisable()
        {
            //Keeps the number of stats and equipment between levels by assigning it to the Game instance

            Game.instance.playerHearts = _heartHalves;

            Game.instance.maxPlayerHearts = _maxHearts;

            Game.instance.playerLives = _lives;

            Game.instance.playerAttack = base.attack;

            Game.instance.playerDefense = base.defense;

            Game.instance.playerHeadArmourIndex = HeadArm;

            Game.instance.playerBodyArmourIndex = BodyArm;

            Game.instance.playerPendantIndex = Pendant;

            Game.instance.playerWeaponIndex = Weapon;
        }

        private void Update()
        {
            //Gets input about the player move and then updates the Player location

            //Exits if it isn't the player's turn
            if (!Game.instance.playerTurn)
            {
                return;
            }

            //Ensures the player's hearts doesn't exceed the maximum number of hearts
            if (_heartHalves > _maxHearts)
            {
                _heartHalves = _maxHearts;
            }

            //Variables for our move
            int horizontal = 0;
            int vertical = 0;

            //Moves the player
            horizontal = (int)(Input.GetAxisRaw("Horizontal"));

            vertical = (int)(Input.GetAxisRaw("Vertical"));

            if (horizontal != 0)
            {
                vertical = 0;
            }

            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove<Wall>(horizontal, vertical);
                AttemptMove<Equipment>(horizontal, vertical);
                AttemptMove<Enemy>(horizontal, vertical);
            }
        }

        protected override void AttemptMove<T>(int xMove, int yMove)
        {
            //Update UI text display to reflect current information
            HUDUpdate update = () => UIText.text = "\nATK:" + base.attack + " DFN: " + base.defense + "\nLives: " + _lives;
            update();

            base.AttemptMove<T>(xMove, yMove);

            //Moves if the move returns true meaning the space is empty
            if (Move(xMove, yMove, out hit))
            {
                SoundManager.instance.RandomiseSfx(moveSound1, moveSound2);
            }

            //Since the player has moved and may have lost hearts, check if the game has ended
            CheckIfGameOver();

            //Set the playerTurn boolean of Game class to false now that players turn is over
            Game.instance.playerTurn = false;
        }

        protected override void OnBlocked<T>(T component)
        {
            var wall = component as Wall;

            if (wall == null)
            {
                var equipment = component as Equipment;
                if (equipment == null)
                {
                    var enemy = component as Enemy;
                    //Attacks enemies if the player's encounters them
                    enemy.LoseHearts(base.attack);
                    _animator.SetTrigger("playerChop");
                }
                else
                {
                    //Picks up equipment if the player encounters it
                    equipment.PickedUp(this);
                }
            }
            else
            {
                //Damages the wall with the player's wall damage amount
                wall.DamageWall(wallDamage);
                _animator.SetTrigger("playerChop");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //Responds to triggers when the Player collides with an object by checking its tag

            //Used if the player moves onto the exit tile
            if (other.tag == "Exit")
            {

                if (Game.instance.level == 4)
                {
                    Game.instance.Win();
                }
                else
                {
                    //Restarts to the next level after a delay
                    Invoke("Restart", restartLevelDelay);

                    //Disable the player object since level is over.
                    enabled = false;
                }
            }
            //Used if the player picks up a health item: adds hearts and disables the item
            else if (other.tag == "Banana")
            {
                SoundManager.instance.RandomiseSfx(eatSound1, eatSound2);

                int heartsPerItem = 2;

                GainHearts(heartsPerItem);

                other.gameObject.SetActive(false);
            }
            else if (other.tag == "Candy")
            {
                SoundManager.instance.RandomiseSfx(eatSound1, eatSound2);

                int heartsPerItem = 1;

                GainHearts(heartsPerItem);

                other.gameObject.SetActive(false);
            }
            else if (other.tag == "Cookies")
            {
                SoundManager.instance.RandomiseSfx(eatSound1, eatSound2);

                int heartsPerItem = 4;

                GainHearts(heartsPerItem);

                other.gameObject.SetActive(false);
            }
            else if (other.tag == "HeartPotion")
            {
                SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);

                int heartsPerItem = 6;

                GainHearts(heartsPerItem);

                other.gameObject.SetActive(false);
            }
            else if (other.tag == "LifePotion")
            {
                SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);

                //Adds lives
                _lives += 1;

                //Disable the item the player collided with
                other.gameObject.SetActive(false);
            }
        }

        private void Restart()
        {
            /* Reloads the last scene, in this case Main, the only scene in the game. And we load it in 
                * "Single" mode so it replace the existing one and not load all the scene object in the 
                * current scene.*/

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public delegate void HUDUpdate();
        //Updates the part of the HUD controlled by the player, displaying the stats and lives

        public void GainHearts(int hearts)
        {
            //Adds the hearts from the item

            if ((_heartHalves + hearts) >= _maxHearts)
            {
                _heartHalves = _maxHearts;
            }
            else
            {
                _heartHalves += hearts;
            }

            //Update the text HUD
            HUDUpdate update = () => UIText.text = "+" + (float)hearts / 2 + "\nATK:" + base.attack + " DFN: " + base.defense + "\nLives: " + _lives;
            update();

            //Updates the HUD system
            Game.instance.hudSystemStatic.HealDamage(hearts);
        }

        public override void LoseHearts(int damage)
        {
            //Removes hearts when the Player is attacked 

            _heartHalves -= damage;

            //Updates the HUD system
            Game.instance.hudSystemStatic.TakeDamage(damage);

            //Checks if the player has lost too many hearts and died
            CheckIfGameOver();

            //Set the trigger for the player animator to transition to the playerHit animation.
            _animator.SetTrigger("playerHit");


            //Update the text HUD
            HUDUpdate update = () => UIText.text = "-" + (float)damage / 2 + "\nATK:" + base.attack + " DFN: " + base.defense + "\nLives: " + _lives;
            update();
        }

        private void CheckIfGameOver()
        {
            //Checks if the Player has run out of hearts and either ends the game or removes a life

            if (_heartHalves <= 0 && _lives == 0)
            {
                SoundManager.instance.PlaySingle(gameOverSound);
                SoundManager.instance.musicSource.Stop();
                SoundManager.instance.bossMusicSource.Stop();
                Game.instance.GameOver();
            }
            if (_heartHalves <= 0 && _lives >= 1)
            {
                _lives -= 1;
                GainHearts(MaxHearts);
            }
        }

        //<------- Properties ------->

        public int HeadArm
        {
            get => _headArmourIndex;
            set => _headArmourIndex = value;
        }

        public int BodyArm
        {
            get => _bodyArmourIndex;
            set => _bodyArmourIndex = value;
        }

        public int Pendant
        {
            get => _pendantIndex;
            set => _pendantIndex = value;
        }

        public int Weapon
        {
            get => _weaponIndex;
            set => _weaponIndex = value;
        }

        public int HeartHalves
        {
            get => _heartHalves;
        }

        public int MaxHearts
        {
            get => _maxHearts;
            set => _maxHearts = value;
        }

        public int Attack
        {
            get => base.attack;
            set => base.attack = value;
        }

        public int Defense
        {
            get => base.defense;
            set => base.defense = value;
        }
    }