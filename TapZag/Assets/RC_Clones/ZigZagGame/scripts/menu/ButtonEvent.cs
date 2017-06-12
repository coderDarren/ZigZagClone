using UnityEngine.UI;
using UnityEngine;
using RC_Projects.ZigZag.Types;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// Button Event is a utility class to assist world space UI tap events
		/// See WorldSpaceEvents.cs 
		/// </summary>
		public class ButtonEvent : MonoBehaviour {

			public Color imgRestColor;
			public Color imgClickColor;
			public Color txtRestColor;
			public Color txtClickColor;
			public Image img;
			public Text txt;

			protected GameManager game;
			protected bool interactable = true;

			AudioManager audio;

			/// <summary>
			/// Initialize in Start
			/// Children classes will just need to override Init() and won't need to call Start()
			/// </summary>
			void Start() {
				Init();
			}

			/// <summary>
			/// Initialize image and text components (if they exist) and get persistent references for children inheritors
			/// </summary>
			public virtual void Init() {
				if (GetComponent<Image>())
					img = GetComponent<Image>();
				if (GetComponent<Text>())
					txt = GetComponent<Text>();
				
				game = GameManager.Instance;
				audio = AudioManager.Instance;
			}

			/// <summary>
			/// Invoked during a click event
			/// </summary>
			public virtual void OnClick() {
				SetColor(txtClickColor, imgClickColor);
				audio.PlaySound(SoundType.Click);
			}

			/// <summary>
			/// Invoked during an up event
			/// </summary>
			public virtual void OnUp() {
				SetColor(txtRestColor, imgRestColor);
			}
			
			/// <summary>
			/// Called when a click event is invoked by WorldSpaceEvents.cs
			/// </summary>
			public void ButtonClickEvent() {
				if (!interactable) return;
				OnClick();
			}

			/// <summary>
			/// Called when an up event is invoked by WorldSpaceEvents.cs
			/// </summary>
			public void ButtonUpEvent() {
				if (!interactable) return;
				OnUp();
			}

			/// <summary>
			/// Set the color of the image or text component if a reference exists
			/// </summary>
			void SetColor(Color txtColor, Color imgColor) {
				if (img)
					img.color = imgColor;
				if (txt)
					txt.color = txtColor;
			}
		}
	}
}
