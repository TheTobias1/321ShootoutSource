using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips : MonoBehaviour
{
    public Text tipText;
    private string[] gameTips = {
        "You can still look around while shooting",
        "You're outnumbered. Pick your time and angle of attack wisely",
        "Energy (purple) weapons can penetrate shields",
        "Watch your ammo count!",
        "Knowing when to back off and where to back off to is very important",
        "All weapons have an intended range. Position yourself accordingly",
        "The LMGs spread reduces with sustained fire, making it effective for crowds at any range",
        "The SMG is best up close, but with good spread control it can eliminate targets at range",
        "The Assault rifle needs to be reloaded often",
        "The DMR is good a picking off targets at range, but its fire rate is a little low for close range firefights",
        "Don't waste your Raygun shots",
        "Did you know that the Shotgun can't hit things from far away?",
        "Energy weapons don't need reloading, but have limited ammo",
        "You can look around before the shootout starts. Look for weapons and health",
        "The starting pistol can only serve you for so long, find something else",
        "You can carry 2 weapons at once",
        "No camping! Unless you like grenades",
        "DO NOT let the enemy surround you. Move!",
        "Stay stocked up on health packs, you never know when you'll get outplayed",
        "Getting shot will cancel your heal. Take cover first",
        "Healing will slow your movement. Make sure its safe",
        "You can cancel a heal yourself by pressing the shoot button",
        "Incase you didn't complete the tutorial, you can heal by pressing the health icon in the top left (on default controls)",
        "Hold the center of the screen to switch weapons",
        "Don't like the default touch controls? You can edit them in the main menu",
        "The extended mag perk is pretty awesome",
        "Stash is usually better than Multiplier, but Multiplier lets you buy other perks",
        "Next time you die...just know that it wasn't personal...but I did enjoy it",
        "Hey, I really appreciate that you're playing this game. If you're enjoying it consider leaving a review. It helps a lot",
        "There's a metaphor for the meaning of life somewhere in this game...probably",
        "The Swift Healthpacks perk lets you move at normal speed while healing. Very useful when you're struggling to find cover",
        "Extra ammo isn't that special, but we all need a little less stress in our lives",
        "If the enemy occupies too much of the map, then it's gonna get difficult",
        "I don't know who needs to here this...But how long have you been on this bus...Or train",
        "Don't like ads? You can remove them permenetly for a small fee (+ it helps support the developer :D )",
        "This game supports gamepads (should work automatically)",
        "For your convenience, all weapons are fully automatic",
        "Checkout the achievements. Once your done with them...Checkout the leaderboards",
        "This game takes practice, you're probably going to die... a lot",
        "Enemies will constantly try to flank you. Keep an eye on that radar",
        "Stash will save your cash till next round... If you can survive the next round",
        "You can tweak the graphics in the Settings menu",
        "You can enable HD in the Settings menu"
    };

    void Start()
    {
        if(tipText != null)
        {
            tipText.text = "Tip: " + gameTips[Random.Range(0, gameTips.Length)];
        }
    }
}
