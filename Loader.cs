using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Loader : MonoBehaviour
    {
        // This class loads up a new game.


        //<------- Variables ------->

        public GameObject game;                                         //Game to instantiate

        //<------- Methods ------->

        private void Awake()
        {
            //This checks if a game instance is already running, and if not instantiates one

            if (Game.instance == null)
            {
                Instantiate(game);
            }
        }
    }