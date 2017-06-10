using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// Stat Text goes on a text component that is either a high score, current score, or # of attempts
		/// </summary>
		public class StatText : MonoBehaviour {

			public enum StatType {
				HighScore,
				Attempts,
				Score
			}
			public StatType statType;
			public string prefix;

			Text txt;
			ProgressManager progress;

			/// <summary>
			/// Initialize required variables
			/// </summary>
			void Start() {
				txt = GetComponent<Text>();
				progress = ProgressManager.Instance;

				StartCoroutine("WaitToDisplay");
			}

			/// <summary>
			/// Wait to fill in any text until the player data has been loaded
			/// </summary>
			IEnumerator WaitToDisplay() {
				while (DataStorage.LOADING_USER) {
					yield return null;
				}

			#if UNITY_WEBGL
				txt.text = string.Empty;

			#elif !UNITY_WEBGL
				switch (statType) {
					case StatType.HighScore:
						txt.text = prefix + progress.HighScore.ToString();
						break;
					case StatType.Attempts:
						txt.text = prefix + progress.Attempts.ToString();
						break;
				}
			#endif
			}

			/// <summary>
			/// Update score text while the player zigs and zags
			/// </summary>
			void Update() {
				if (statType != StatType.Score) return;
				txt.text = prefix + progress.Score.ToString();
			}
		}
	}
}
