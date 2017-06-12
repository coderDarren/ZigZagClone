using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// The camera controller is in charge of panning upwards to the play position..
		/// ..and rotating around the player on the lose state
		/// This class is controlled by GameManager.cs
		/// </summary>
		public class CameraController : MonoBehaviour {

			public Transform targetEntry;
			public float entryTime;
			public float loseOrbitSpeed = 5;

			GameManager game;

			/// <summary>
			/// Initialize other class instances in Start() to ensure the instance always has a value
			/// Start is always called after Awake - where all instances are created
			/// </summary>
			void Start() {
				game = GameManager.Instance;
			}

			/// <summary>
			/// Start the camera off above the launch pad
			/// </summary>
			public void StartAtEntry() {
				transform.position = targetEntry.position;
			}

			/// <summary>
			/// Run the coroutine to pan the camera upward to the launch pad
			/// </summary>
			public void StartEntrySequence() {
				StopAllCoroutines();
				StartCoroutine("RunEntrySequence");
			}

			/// <summary>
			/// While the current position is not close to the target entry position..continue moving up
			/// </summary>
			IEnumerator RunEntrySequence() {
				Vector3 entrySpeed = Vector3.zero;
				float dist = Vector3.Distance(transform.position, targetEntry.position);
				while (dist > 0.02f) {
					transform.position = Vector3.SmoothDamp(transform.position, targetEntry.position, ref entrySpeed, entryTime);
					dist = Vector3.Distance(transform.position, targetEntry.position);
					yield return null;
				}
				transform.position = targetEntry.position;
				game.Init();
			}

			/// <summary>
			/// Run the coroutine to rotate around the player on the lose state
			/// </summary>
			public void StartLoseSequence() {
				StopAllCoroutines();
				StartCoroutine("RunLoseSequence");
			}

			/// <summary>
			/// Rotate around the player eternally
			/// Only stopped when the match restarts
			/// </summary>
			IEnumerator RunLoseSequence() {
				Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
				Vector3 dir = player.position - transform.position;
				Quaternion targetRotation = Quaternion.LookRotation(dir);

				while (true) {
					transform.RotateAround(player.position, Vector3.up, loseOrbitSpeed * Time.deltaTime);

					dir = player.position - transform.position;
					targetRotation = Quaternion.LookRotation(dir);
					transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1 * Time.deltaTime);

					yield return null;
				}
			}
		}
	}
}
