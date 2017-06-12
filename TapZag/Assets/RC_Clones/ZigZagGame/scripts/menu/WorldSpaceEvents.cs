using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// This is a utility class to accommodate screen taps and button events on world space UI events
		/// (Not natively supported by Unity)
		/// </summary>
		public class WorldSpaceEvents : MonoBehaviour {

			List<ButtonEvent> clickedButtons;
			List<RaycastResult> results;
			PointerEventData ped;

			/// <summary>
			/// Initialize required variables for handling screen space raycasts to UI elements
			/// </summary>
			void Start() {
				clickedButtons = new List<ButtonEvent>();
				results = new List<RaycastResult>();
				ped = new PointerEventData(EventSystem.current);
			}

			/// <summary>
			/// When a tap occurs, a UI EventSystem raycast is sent out and results are returned.
			/// We check the results to see if a ButtonEvent component exists on one of objects caught in the raycast
			/// If a ButtonEvent is found, invoke a 'click' event
			/// If a tap ends, look through clicked buttons and invoke an 'up' event
			/// </summary>
			void Update() {
				if (Input.GetMouseButtonDown(0)) {
					ped.position = Input.mousePosition;
					EventSystem.current.RaycastAll(ped, results);
					foreach(RaycastResult result in results) {
						if (result.gameObject.GetComponent<ButtonEvent>()) {
							result.gameObject.GetComponent<ButtonEvent>().ButtonClickEvent();
							clickedButtons.Add(result.gameObject.GetComponent<ButtonEvent>());
						}
					}
				}

				if (Input.GetMouseButtonUp(0)) {
					foreach(ButtonEvent button in clickedButtons) {
						button.ButtonUpEvent();
					}
					clickedButtons.Clear();
				}

			}
		}
	}
}
