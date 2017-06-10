using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// Child of ButtonEvent, a custom world space UI event class
		/// </summary>
		public class StartGameEvent : ButtonEvent {

			/// <summary>
			/// If clicked, request to start the game
			/// </summary>
			public override void OnClick() {
				if (!game.GameOver) return;
				base.OnClick();
				game.StartGame();
			}
		}
	}
}
