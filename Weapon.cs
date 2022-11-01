using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Weapon : Equipment
    {
        // This class controls how the different types of Weapon behave


        //<------- Methods ------->

        public override void PickedUp(Player other)
        {

            other.Attack -= Game.instance.equipmentArray[other.Weapon].attack;

            other.Defense -= Game.instance.equipmentArray[other.Weapon].defense;

            other.MaxHearts -= Game.instance.equipmentArray[other.Weapon].bonusHearts;

            base.DropItem(other, Game.instance.equipmentArray[other.Weapon]);

            //Sets the weapon as this item
            other.Weapon = this.index;

            base.PickedUp(other);
        }
    }