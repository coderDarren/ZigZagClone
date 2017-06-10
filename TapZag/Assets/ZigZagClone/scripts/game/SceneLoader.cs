using UnityEngine;
using UnityEngine.SceneManagement;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// The Scene Loader is in charge of reloading the only scene in this game
		/// ..and tracking load status
		/// </summary>
		public class SceneLoader : MonoBehaviour {

			public static SceneLoader Instance;

			bool isLoading;
			
			public bool IsLoading { get { return isLoading; } }

			/// <summary>
			/// Singleton Pattern. Only one Scene Loader Allowed.
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
			/// Subscribe to the Unity SceneManager event, sceneLoaded
			/// </summary>
			void OnEnable() {
				SceneManager.sceneLoaded += sceneLoaded;
			}

			/// <summary>
			/// Unsubscribe to the Unity SceneManager event, sceneLoaded
			/// </summary>
			void OnDisable() {
				SceneManager.sceneLoaded -= sceneLoaded;
			}

			/// <summary>
			/// Track the isLoading member
			/// </summary>
			void sceneLoaded(Scene scene, LoadSceneMode mode) {
				isLoading = false;
			}

			/// <summary>
			/// Set the isLoading member and reload the scene
			/// </summary>
			public void ReloadScene() {
				isLoading = true;
				SceneManager.LoadScene("game");
			}
		}
	}
}