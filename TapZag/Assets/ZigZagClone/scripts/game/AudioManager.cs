using System.Collections;
using UnityEngine;
using RC_Projects.ZigZag.Types;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// The audio manager is the class to reference when you want to play a sound clip
		/// </summary>
		public class AudioManager : MonoBehaviour {

			[System.Serializable]
			public class Clip {
				public SoundType soundType;
				public AudioClip clip;
				public AudioSource channel; //provides ability to utilize any number of channels...add from inspector
			}

			public static AudioManager Instance;
			
			public Clip[] clips;

			AudioSource source;
			ProgressManager progress;

			/// <summary>
			/// Singleton Pattern. Only one Audio Manager allowed.
			/// </summary>
			void Awake() {
				if (Instance != null) {
					Destroy(gameObject);
				}
				else {
					Instance = this;
					source = GetComponent<AudioSource>();
					DontDestroyOnLoad(gameObject);
				}
			}

			/// <summary>
			/// Initialize other class instances in Start() to ensure the instance always has a value
			/// Start is always called after Awake - where all instances are created
			/// </summary>
			void Start() {
				progress = ProgressManager.Instance;
			}

			/// <summary>
			/// Only play audio if the sound setting is turned on
			/// Find the clip to play using the sound type argument
			/// set the channel to play the sound on
			/// </summary>
			public void PlaySound(SoundType sound) {
				if (!progress.AudioOn) return;

				for (int i = 0; i < clips.Length; i++) {
					if (clips[i].soundType == sound) {
						source = clips[i].channel;
						source.clip = clips[i].clip;
						source.Play();
					}
				}
			}
		}
	}
}
