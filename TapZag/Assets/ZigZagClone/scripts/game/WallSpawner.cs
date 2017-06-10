using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// This is what moves the wall pieces. (The camera never moves)
		/// Furthermore, only one transform moves. All wall pieces are a child of the wall spawner
		/// </summary>
		public class WallSpawner : MonoBehaviour {

			[System.Serializable]
			public struct Range {
				public float min;
				public float max;
			}

			public float shiftSpeed;
			public Range xRange;
			public Range zRange;
			public float pickupProbability;

			Pool wallPool;
			Pool pickupPool;
			Transform lastSpawnedWall;
			Vector3 leftSlot = new Vector3(-0.70710678118f, 0, 0.70710678118f);
			Vector3 rightSlot = new Vector3(0.70710678118f, 0, 0.70710678118f);

			GameManager game;

			/// <summary>
			/// Initialize other class instances in Start() to ensure the instance always has a value
			/// Start is always called after Awake - where all instances are created
			/// </summary>
			void Start() {
				game = GameManager.Instance;
				wallPool = GameObject.FindWithTag("WallPool").GetComponent<Pool>();
				pickupPool = GameObject.FindWithTag("PickupPool").GetComponent<Pool>();
				Prewarm();
			}

			/// <summary>
			/// This works similarly to how the Shuriken particle system works.
			/// When the game restarts, there needs to be some walls placed from the launch pad and all the way off the top of the screen
			/// </summary>
			void Prewarm() {
				lastSpawnedWall = wallPool.GetFirstAvailable();
				lastSpawnedWall.SetParent(transform);
				lastSpawnedWall.position = Vector3.right * 0.7071068118f;
				while (lastSpawnedWall.position.z < zRange.max * 2) {
					SpawnWall();
				}
			}

			/// <summary>
			/// If the game is over, do nothing
			/// otherwise shift down
			/// </summary>
			void Update() {
				if (game == null) return;
				if (game.GameOver) return;

				Shift();
			}

			/// <summary>
			/// Move the position of this object downward
			/// </summary>
			void Shift() {
				transform.position += -Vector3.forward * shiftSpeed * Time.deltaTime;
			}

			/// <summary>
			/// Simple procedural function (randomization with rules)
			/// Choose a number (0 or 1) and that determines the direction of the next wall (left or right)
			/// If the next spawn position is offscreen, switch the number (0 to 1 OR 1 to 0)
			/// Find a wall from the wall pool and set that position to the newly calculated position (nextPos)
			/// Store the last spawned wall in a variable 'lastSpawnedWall'
			/// Randomly spawn a pickup item on this wall
			/// </summary>
			public void SpawnWall() {
				Vector3 nextPos = lastSpawnedWall.position;
				int random = Random.Range(0, 2);
				if (random == 0) {
					if (nextPos.x + leftSlot.x < xRange.min ||
						nextPos.x + leftSlot.x > xRange.max) {
						//cannot use leftSlot
						nextPos += rightSlot;
					} else {
						nextPos += leftSlot;
					}
				}
				if (random == 1) {
					if (nextPos.x + rightSlot.x < xRange.min ||
						nextPos.x + rightSlot.x > xRange.max) {
						//cannot use rightSlot
						nextPos += leftSlot;
					}
					else {
						nextPos += rightSlot;
					}
				}
				Transform t = wallPool.GetFirstAvailable();
				t.SetParent(transform);
				nextPos.y = 0;
				t.position = nextPos;
				lastSpawnedWall = t;

				TrySpawnPickup();
			}

			/// <summary>
			/// Called when the game restarts. All wall pieces child of the Wall Spawner need..
			/// ..to be moved back to the persistent, undying wallPool object
			/// Same goes for pickup objects
			/// </summary>
			public void ReleasePoolObjects() {
				Wall[] walls = GetComponentsInChildren<Wall>();
				Pickup[] pickups = GetComponentsInChildren<Pickup>();
				foreach (Wall wall in walls) {
					wallPool.Dispose(wall.gameObject.transform);
				}
				foreach (Pickup pickup in pickups) {
					pickupPool.Dispose(pickup.gameObject.transform);
				}
			}

			/// <summary>
			/// Randomly place a pickup above the last spawned wall
			/// </summary>
			void TrySpawnPickup() {
				float random = Random.Range(0.0f, 1.0f);
				if (random <= pickupProbability) {
					Transform t = pickupPool.GetFirstAvailable();
					t.SetParent(transform);
					t.position = lastSpawnedWall.position + Vector3.up * 1.7f;
				}
			}
		}
	}
}
