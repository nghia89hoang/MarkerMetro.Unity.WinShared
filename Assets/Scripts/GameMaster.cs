﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using MarkerMetro.Unity.WinIntegration.Facebook;
using LitJson;

public class GameMaster : MonoBehaviour {

	public GameObject 	gui_start_;
	public GameObject 	gui_play_;
	public GameObject 	gui_end_;
	public GameObject	gui_store_;
    public GameObject   facebook_image_;
    public GameObject   login_name_;
    public GameObject   number_friends_;

	public GUIText 		gui_matches_;
	public GUIText 		gui_remaining_;
	public GUIText 		gui_result_;

	public GAME_STATE state_;

	private List<GameObject> tiles_ = new List<GameObject>();
	private string[] names_ = {"Keith", "Tony", "Greg", "Nigel", "Ivan", "Chad", "Damian", "Brian" };
	private Tile current_switched_1 = null;
	private Tile current_switched_2 = null;
	private float waiting_timer_ = 0.0f;

	private int max_moves_ = 15;
	private int remaining_moves_ = 0;
	private int number_matches_ = 0;

    private Dictionary<string, Texture2D> facebook_friends_ = new Dictionary<string, Texture2D>();

	const int rows_ = 4;
	const int cols_ = 4;
	const float wait_after_switch_ = 0.5f;

	public enum GAME_STATE
	{
		GS_START,
		GS_PLAYING,
		GS_END,
		GS_WAITING,
		GS_STORE
	};

	// Use this for initialization
	void Start () {

		CreateTiles();
		ChangeState( GAME_STATE.GS_START );

#if UNITY_WINRT
        FB.Init(SetFBInit, "682783485145217", OnHideUnity);
#endif
	}

    void Awake()
    {

    }
	
	// Update is called once per frame
	void Update () {
		if ( state_ == GAME_STATE.GS_WAITING )
		{
			if ( waiting_timer_ > wait_after_switch_ )
			{
				waiting_timer_ = 0.0f;
				ChangeState( GAME_STATE.GS_PLAYING );
			}
			else
			{
				waiting_timer_ += Time.deltaTime;
			}
		}

		if ( state_ == GAME_STATE.GS_PLAYING )
		{
			if ( number_matches_ == tiles_.Count / 2 )
			{
				// Win
				ChangeState( GAME_STATE.GS_END );
				gui_result_.text = "YOU WIN";
			}
			else if ( remaining_moves_ == 0 )
			{
				// Loss
				ChangeState( GAME_STATE.GS_END );
				gui_result_.text = "YOU LOSE";
			}
		}
	}

	// Change the games state
	public void ChangeState( GAME_STATE state )
	{
		switch ( state )
		{
			case GAME_STATE.GS_START:
			{
				gui_start_.SetActive( true );
				gui_end_.SetActive( false );
				gui_play_.SetActive( false );
				gui_store_.SetActive( false );
				state_ = state;

				remaining_moves_ = max_moves_;
				number_matches_ = 0;
				gui_result_.text = "YOU LOSE";

                SetupTiles();
			}
			break;
			case GAME_STATE.GS_PLAYING:
			{
				gui_start_.SetActive( false );
				gui_end_.SetActive( false );
				gui_play_.SetActive( true );
				gui_store_.SetActive( false );
				state_ = state;

				if ( current_switched_1 != null )
				{
					current_switched_1.Rotate();
					current_switched_1 = null;
				}
				if ( current_switched_2 != null )
				{
					current_switched_2.Rotate();
					current_switched_2 = null;
				}

				SetGUIText();
			}
			break;
			case GAME_STATE.GS_END:
			{
				gui_start_.SetActive( false );
				gui_end_.SetActive( true );
				gui_play_.SetActive( false );
				gui_store_.SetActive( false );
				state_ = state;
			}
			break;
			case GAME_STATE.GS_WAITING:
			{
				waiting_timer_ = 0.0f;
				state_ = state;
			}
			break;
			case GAME_STATE.GS_STORE:
			{
				gui_start_.SetActive( false );
				gui_end_.SetActive( false );
				gui_play_.SetActive( false );
				gui_store_.SetActive( true );
				state_ = state;
			}
			break;
			default: break;
		}
	}

	void CreateTiles()
	{
		GameObject tile_base = GameObject.Find("GameTile");
		for ( int x = 0; x < rows_; ++x )
		{
			for ( int y = 0; y < cols_; ++y )
			{
				GameObject new_tile = Instantiate( tile_base, new Vector3( x * 1.5f, y * 1.5f, 0.0f ), Quaternion.identity ) as GameObject;
				tiles_.Add( new_tile );
			}
		}
	}

	void SetupTiles()
	{
		int number_tiles = tiles_.Count;
		for ( int i = 0; i < number_tiles; ++i )
		{
			var temp = tiles_[i];
			int random_index = UnityEngine.Random.Range( i, tiles_.Count );
			tiles_[i] = tiles_[ random_index ];
			tiles_[ random_index ] = temp;
		}

		for ( int i = 0; i < number_tiles / 2; ++i )
		{
            string name = names_[i];
            Texture2D texture = Resources.Load(name) as Texture2D;

            if ( facebook_friends_.Count > i )
            {
                int count = 0;
                foreach ( var item in facebook_friends_ )
                {
                    if ( count == i )
                    {
                        name = item.Key;
                        texture = item.Value;
                        break;
                    }
                    ++count;
                }
            }

			Tile script_1 = tiles_[i * 2].GetComponent<Tile>();
			script_1.SetImage( names_[i], texture );
			Tile script_2 = tiles_[i * 2 + 1].GetComponent<Tile>();
			script_2.SetImage( names_[i], texture );
		}
	}

	public void OnTileSwitch( Tile script )
	{
		if ( current_switched_1 == null )
		{
			current_switched_1 = script;
		}
		else
		{
			--remaining_moves_;
			if ( current_switched_1.name_ == script.name_ )
			{
				// match
				current_switched_1 = null;
				++number_matches_;
			}
			else
			{
				current_switched_2 = script;
				ChangeState( GAME_STATE.GS_WAITING );
			}
		}

		SetGUIText();
	}

	void SetGUIText()
	{
		gui_matches_.text = "Matches: " + number_matches_.ToString();
		gui_remaining_.text = "Moves Remaining: " + remaining_moves_.ToString();
	}

	public void AddMoves()
	{
		remaining_moves_ += 5;
		SetGUIText();
	}


    //
    // Facebook Test Functions
    //
    private void SetFBInit()
    {
        Debug.Log("Set FB Init");
        if ( FB.IsLoggedIn )
        {
            Debug.Log("Already logged in to FB");
            StartCoroutine(RefreshFBStatus());
        }
    }

    private void OnHideUnity( bool hide_unity )
    {
        Debug.Log("OnHideUnity");
    }

    public void FBLoginCallback( FBResult result )
    {
        Debug.Log("LoginCallback");
        if ( FB.IsLoggedIn )
        {
            StartCoroutine(RefreshFBStatus());
        }
    }

    public IEnumerator FBLogoutCallback()
    {
        while ( FB.IsLoggedIn )
        {
            yield return null;
        }

        StartCoroutine(RefreshFBStatus());
    }

    private IEnumerator RefreshFBStatus()
    {
        TextMesh text = (TextMesh)login_name_.GetComponent<TextMesh>();
        Renderer renderer = facebook_image_.GetComponent<MeshRenderer>().renderer;
        if ( FB.IsLoggedIn )
        {
            text.text = FB.UserName;
            Texture2D texture = new Texture2D(128, 128, TextureFormat.DXT1, false);

            yield return StartCoroutine(GetFBPicture(FB.UserId, texture));

            renderer.material.mainTexture = texture;
        }
        else
        {
            text.text = "Not Logged In";
            renderer.material.mainTexture = null;
            TextMesh number_text = (TextMesh)number_friends_.GetComponent<TextMesh>();
            number_text.text = "No Friends";
        }
    }


    public void PopulateFriends()
    {
        if ( FB.IsLoggedIn )
        {
            // Get the friends
            FB.API("/me/friends", HttpMethod.GET, GetFriendsCallback);
        }
    }

    private void GetFriendsCallback( FBResult result )
    {
        if (result.Error != null)
        {
            Debug.Log("Failed to get FB Friends");
            return;
        }

        try
        {
            facebook_friends_.Clear();
            JsonData data = JsonMapper.ToObject(result.Text);

            JsonData friends = data["data"];
            for (int i = 0; i < friends.Count; ++i)
            {
                JsonData friend = friends[i];
                string name = (string)friend["name"];
                string id = (string)friend["id"];
                Texture2D texture = new Texture2D(128, 128, TextureFormat.DXT1, false);
                StartCoroutine(GetFBPicture(id, texture));

                facebook_friends_.Add(name, texture);
            }

            TextMesh text = (TextMesh)number_friends_.GetComponent<TextMesh>();
            text.text = "Friends: " + facebook_friends_.Count;
            SetupTiles();
        }
        catch( Exception e )
        {
            Debug.Log(e.Message);
        }
    }

    private IEnumerator GetFBPicture(string id, Texture2D texture)
    {     
        WWW url = new WWW("https" + "://graph.facebook.com/" + id + "/picture?type=large");

        while (!url.isDone)
        {
            yield return null;
        }
  
        url.LoadImageIntoTexture(texture);
    }
}
