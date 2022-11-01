using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BodyArmour : Equipment
    {
        // This class controls how the different types of body armour behave


        //<------- Methods ------->
        public override void PickedUp(Player other)
        {
            //Checks if there is already some armour equipped
            if (other.BodyArm != -1)
            {
                other.Attack -= Game.instance.equipmentArray[other.BodyArm].attack;
                other.Defense -= Game.instance.equipmentArray[other.BodyArm].defense;
                other.MaxHearts -= Game.instance.equipmentArray[other.BodyArm].bonusHearts;
                base.DropItem(other, Game.instance.equipmentArray[other.BodyArm]);
            }

            //Sets the body armour as this item
            other.BodyArm = this.index;

            base.PickedUp(other);
        }
    }