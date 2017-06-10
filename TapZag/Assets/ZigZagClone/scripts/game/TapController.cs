using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RC_Projects.ZigZag.Types;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// This is the "player controller". The actual sphere player in game..
		/// </summary>
		public class TapController : MonoBehaviour {

			public delegate void PlayerDelegate();
			public static event PlayerDelegate OnLose;
			public static event PlayerDelegate OnPickup;

			public float speed = 5;

			enum Direction {
				Left,
				Right
			}
			Direction dir;
			Rigidbody rbody;
			GameManager game;
			Material mat;
			AudioManager audio;

			/// <summary>
			/// Initialize other class instances in Start() to ensure the instance always has a value
			/// Start is always called after Awake - where all instances are created
			/// </summary>
			void Start() {
				game = GameManager.Instance;
				rbody = GetComponent<Rigidbody>();
				mat = GetComponent<MeshRenderer>().material;
				mat.SetColor("_Color", Color.white);
				audio = AudioManager.Instance;
			}

			/// <summary>
			/// If game over, do nothing
			/// Get mouse clicks or screen taps to switch the player's direction
			/// </summary>
			void Update() {
				if (game.GameOver) return;
				
				if (Input.GetMouseButtonDown(0)) {
					audio.PlaySound(SoundType.Tap);
					SwitchDirection();
				}
				transform.position += ForwardVector() * speed * Time.deltaTime;
			}

			/// <summary>
			/// If moving right, turn left..
			/// and vice versa
			/// </summary>
			void SwitchDirection() {
				dir = dir == Direction.Right ? Direction.Left : Direction.Right;
			}

			/// <summary>
			/// Returns a direction vector used for movement. See Update()
			/// </summary>
			Vector3 ForwardVector() {
				return dir == Direction.Right ? Vector3.right : Vector3.left;
			}

			/// <summary>
			/// When WallBounds are hit, the player has lost.
			/// When Pickups are hit, the player gets some points. 
			/// OnLose and OnPickup processing can be seen from GameManager.cs
			/// </summary>
			void OnTriggerEnter(Collider col) {
				if (col.gameObject.tag.Equals("WallBounds")) {
					OnLose(); //event sent to GameManager
					rbody.useGravity = false;
					rbody.velocity = Vector3.zero;
					mat.SetColor("_Color", Color.red);
					audio.PlaySound(SoundType.Die);
				}
				if (col.gameObject.tag.Equals("Pickup")) {
					OnPickup(); //event sent to GameManager
					audio.PlaySound(SoundType.Pickup);
				}
			}
		}
	}
}
