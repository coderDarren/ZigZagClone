using System.Collections;
using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// Child of ButtonEvent, a custom world space UI event class
		/// The audio toggle just needs to toggle the sprite and the audio setting when clicked
		/// </summary>
		public class AudioToggleEvent : ButtonEvent {

			public Sprite toggleOn;
			public Sprite toggleOff;

			ProgressManager progress;

			/// <summary>
			/// Set the button image sprite and initialize
			/// </summary>
			public override void Init() {
				base.Init();
				progress = ProgressManager.Instance;
				StartCoroutine("SetSprite");
			}

			/// <summary>
			/// When clicked, the audio will be toggled. See ProgressManager.cs
			/// </summary>
			public override void OnClick() {
				if (!game.GameOver) return;
				base.OnClick();
				progress.ToggleAudio();
				SetSprite();
			}

			/// <summary>
			/// We must wait until the player data has been loaded before determining if audio is on or off
			/// </summary>
			IEnumerator WaitToSetSprite() {
				while (DataStorage.LOADING_USER) {
					yield return null;
				}
				SetSprite();
			}

			/// <summary>
			/// Set the image sprite based on the current audio setting
			/// </summary>
			void SetSprite() {
			#if UNITY_WEBGL
				img.enabled = false;
				return;
			#endif
				switch (progress.AudioOn) {
					case true: 
						if (toggleOn != null) img.sprite = toggleOn; break;
					case false: 
						if (toggleOff != null) img.sprite = toggleOff; break;
				}
			}
		}
	}
}
