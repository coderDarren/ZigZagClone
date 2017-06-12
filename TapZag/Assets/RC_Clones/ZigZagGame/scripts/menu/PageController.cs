using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RC_Projects.ZigZag.Types;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// Managed by PageManager.cs
		/// Responsible for executing entry and exit animations for this page
		/// </summary>
		public class PageController : MonoBehaviour {

			public PageType pageType;
			
			bool isOn = false;
			CanvasGroup canvas;
			Animator anim;

			public bool IsOn { get { return isOn; } }

			/// <summary>
			/// Initialize
			/// </summary>
			void Start() {
				anim = GetComponent<Animator>();
				canvas = GetComponent<CanvasGroup>();
				SetInteractability(false);
			}

			/// <summary>
			/// Run the entry animation sequence
			/// </summary>
			public void TurnOn() {
				if (isOn) return;
				StopAllCoroutines();
				StartCoroutine("RunEntrySequence");
			}

			/// <summary>
			/// Make the page interactable and tell the page to animate into the game
			/// Do not set isOn to true until the page animation is done
			/// </summary>
			IEnumerator RunEntrySequence() {
				SetInteractability(true);
				SetAnimState(true, false);

				yield return new WaitForSeconds(0.1f);
				while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || anim.IsInTransition(0)) { //is the animation over?
					yield return null;
				}

				isOn = true;
			}

			/// <summary>
			///
			/// </summary>
			public void TurnOff() {
				StopAllCoroutines();
				StartCoroutine("RunExitSequence");
			}

			/// <summary>
			/// Disable interactability with this page and tell the page to animate out of the game
			/// Do not set isOn to false until the page animation is done
			/// </summary>
			IEnumerator RunExitSequence() {
				SetInteractability(false);
				SetAnimState(false, true);

				yield return new WaitForSeconds(0.1f);
				while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || anim.IsInTransition(0)) { //is the animation over?
					yield return null;
				}

				canvas.alpha = 0;
				isOn = false;
			}

			/// <summary>
			/// Set animator parameters to determine the animation state
			/// </summary>
			void SetAnimState(bool enterStatus, bool exitStatus) {
				anim.SetBool("Enter", enterStatus);
				anim.SetBool("Exit", exitStatus);
			}

			/// <summary>
			/// Set the page interaction state. This will determine if the page listens for any tap events
			/// </summary>
			void SetInteractability(bool canInteract) {
				canvas.interactable = canInteract;
				canvas.blocksRaycasts = canInteract;
			}
		}
	}
}
