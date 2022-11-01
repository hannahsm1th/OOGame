using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Pendant : Equipment
    {
        // This class controls how the different types of body armour behave


        //<------- Methods ------->

        public override void PickedUp(Player other)
        {
            //Checks if there is already some pendant equipped
            if (other.Pendant != -1)
            {
                other.Attack -= Game.instance.equipmentArray[other.Pendant].attack;
                other.Defense -= Game.instance.equipmentArray[other.Pendant].defense;
                other.MaxHearts -= Game.instance.equipmentArray[other.Pendant].bonusHearts;
                base.DropItem(other, Game.instance.equipmentArray[other.Pendant]);
            }

            //Sets the pendant as this item
            other.Pendant = this.index;

            base.PickedUp(other);
        }
    }