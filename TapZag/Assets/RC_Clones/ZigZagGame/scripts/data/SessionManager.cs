using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Events;
#endif

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// The Session Manager is the entry point to the game
		/// </summary>
		public class SessionManager : MonoBehaviour {

			public static SessionManager Instance;

			public string userId { get; private set; }
			public bool validUser { 
				get { 
				#if UNITY_ANDROID
					return PlayGamesPlatform.Instance.IsAuthenticated(); 
				#elif UNITY_IOS
					//return //ios implementation...if different
				#endif
					return false;
				} 
			}

			GameManager game;

			/// <summary>
			/// Singleton pattern. Only one Session Manager allowed
			/// </summary>
			void Awake() {
				if (Instance != null) {
					Destroy(gameObject);
				}
				else {
					Instance = this;
					DontDestroyOnLoad(gameObject);
				}
			}

			/// <summary>
			/// Begin configuring google play
			/// </summary>
			void Start() {
				game = GameManager.Instance;
				ConfigureGooglePlay();
			}

			/// <summary>
			/// Standard Google Play Initialization function calls
			/// Unity social class will attempt to log the player in using a callback function 'ProcessAuthentication'
			/// </summary>
			void ConfigureGooglePlay() {
		#if UNITY_ANDROID
				PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
				PlayGamesPlatform.InitializeInstance(config);
				PlayGamesPlatform.DebugLogEnabled = true;
				PlayGamesPlatform.Activate();
		#elif UNITY_IOS
				//ios implementation...if different
		#endif
				Social.localUser.Authenticate(ProcessAuthentication);
			}

			/// <summary>
			/// Called at the end of authentication attempt
			/// If success, the player successfully logged in 
			/// Regardless, data storage will load some user (default user or not) and save data to local player prefs
			/// </summary>
			void ProcessAuthentication(bool success) {
				if (success) {
					userId = Social.localUser.id;
					Debug.Log("Google Play Authentication Success!");
				} else {
					userId = string.Empty;
					Debug.Log("Google Play Authentication Failure!");
				}
				DataStorage.LoadUser(userId, false);
				StartCoroutine("WaitToStart");
			}

			/// <summary>
			/// Initiated at the end of authentication
			/// Wait until all player data has been loaded before starting the game
			/// </summary>
			IEnumerator WaitToStart() {
				while (DataStorage.LOADING_USER) {
					yield return null;
				}
				game.StartApp();
			}

			public void ShowAchievements() {
		#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ShowAchievementsUI();
		#elif UNITY_IOS
				//ios implementation...if different
		#endif
			}

			public void ShowLeaderboard() {
		#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ShowLeaderboardUI();
		#elif UNITY_IOS
				//ios implementation...if different
		#endif
			}
		}
	}
}
