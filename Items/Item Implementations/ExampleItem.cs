using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// STEP ONE: Your class must inherit from UsableItem
//  -- Once you do so, Unity will complain until you implement
//     the Use method. (Press Ctrl + . while you have the name
//     of your class selected, then press enter to automatically
//     implement the method.)

public sealed class ExampleItem : UsableItem
{
    private readonly float healthGainAmount = 30f;

    public override void Use(PlayerController pc)
    {
        // STEP TWO: If you let Unity generate the method for you,
        // it will have a defaul System.NotImplementedException();
        //      -- We don't need that, since you will be implementing 
        //      the method! Get rid of that shit.

        // STEP THREE: Implement the actual functionality of the item
        //      -- Should be self explanatory.
        pc.GetComponent<PlayerHealth>().GainHealth(healthGainAmount);

        // STEP FOUR: You MUST and I mean MUST call the base method.
        base.Use(pc);
    }
}

// STEP FIVE: Adding the Item to the System
//  -- Create an empty (absolutely empty) GameObject in the scene.
//     Ensure that the transform is reset (All Zeros baby).
//     Give the object a sweet ass name
//     Add your script to the empty GameObject
//
//     (ONLY IF STEFAN HAS MADE UI ART) Add the correct sprite to the public field. 
//      
//
//     Add the object to the Items Prefab Folder
//     Delete the item from the scene.
//     Add your prefab item to the Item Box prefab in the same folder

// STEP SIX: Check that everything works.
