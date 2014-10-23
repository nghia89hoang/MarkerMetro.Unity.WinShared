﻿using UnityEngine;
using System.Collections;

using MarkerMetro.Unity.WinIntegration.Facebook;
public class GUIStart : MonoBehaviour {

	void OnGUI()
	{
		// Make a background box
		int half_width = Screen.width / 2;
		int half_height = Screen.height / 2;

		int box_width = 200;
		int box_height = 330;

		int box_x = half_width - box_width / 2;
		int box_y = half_height - box_height / 2;

        GUI.Box(new Rect( box_x, box_y, box_width, box_height), "Main Menu");

        GameObject game_master = GameObject.Find("GameMaster");
        GameMaster game_script = game_master.GetComponent<GameMaster>();

        int y_modifier = 30;

        // Make the first button. If it is pressed, Start Game
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Start Game"))
        {
            game_script.ChangeState(GameMaster.GAME_STATE.GS_PLAYING);
        }

        y_modifier += 50;

#if UNITY_WP8 && !UNITY_EDITOR
        if (!FBNative.IsLoggedIn)
#else
        if (!FB.IsLoggedIn)
#endif
        {
            // Second Button Login to FB
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Login"))
            {
#if (UNITY_WP8 && !UNITY_EDITOR)
                FBNative.Login("email,publish_actions,user_friends");
#else
                FB.Login("email,publish_actions,user_friends", game_script.FBLoginCallback); // TO DO login callback
#endif           
            }
        }
        else
        {
            // Second Button Logout to FB
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Logout"))
            {
#if (UNITY_WP8 && !UNITY_EDITOR)
                FBNative.Logout();
#else
                FB.Logout();
#endif
                StartCoroutine(game_script.FBLogoutCallback());
            }

            y_modifier += 50;

            // Get Friends That Play the game
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Get Friends"))
            {
                game_script.PopulateFriends();
            }

            y_modifier += 50;

            // Invite friends that don't play
            if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Invite Friends"))
            {
                game_script.InviteFriends();
            }
        }

        y_modifier += 50;

        //Test crash button
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Throw an exception"))
        {
           throw new System.Exception("This is test exception from Unity code");
        }

        y_modifier += 50;

        // Third Button Login to Quit
        if (GUI.Button(new Rect(box_x + 10, box_y + y_modifier, box_width - 20, 40), "Quit"))
        {
            Application.Quit();
        }
    }
}
