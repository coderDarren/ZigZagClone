using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// This is a pooled object. It is always available - never instantiated, never destroyed
		/// </summary>
		public class Wall : MonoBehaviour {

			Pool pool;
			WallSpawner wallSpawner;

			/// <summary>
			/// It is here that wall pieces are recycled from the bottom of the screen to the top
			/// When a wall falls off screen, tell the wall spawner to spawn a new wall. 
			/// This removes the need to "spawn" walls on a timer, thus removing any possibility of wall gaps!
			/// </summary>
			IEnumerator Die() {
				pool = GameObject.FindWithTag("WallPool").GetComponent<Pool>();
				wallSpawner = GameObject.FindObjectOfType<WallSpawner>();
				while (transform.position.y > -5) {
					transform.position -= Vector3.up * 5 * Time.deltaTime;
					yield return null;
				}
				bool success = pool.Dispose(transform);
				if (success) {
					wallSpawner.SpawnWall();
				} else {
					Destroy(gameObject);
				}
			}

			/// <summary>
			/// When this wall reaches the bottom bounds, "die"
			/// </summary>
			void OnTriggerEnter(Collider col) {
				if (col.gameObject.tag.Equals("BottomBounds")) {
					StopCoroutine("Die");
					StartCoroutine("Die");
				}
			}
		}
	}
}
