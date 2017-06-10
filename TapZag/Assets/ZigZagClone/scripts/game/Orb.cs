using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// This is a pooled object. It is always available - never instantiated, never destroyed
		/// </summary>
		public class Orb : MonoBehaviour {

			public float speed = 5;
			public float acceleration = 0.01f;
			public float shrinkRate = 1;

			Pool orbPool;
			Transform player;

			//called by pickup
			/// <summary>
			/// When needed, Pickup.cs will move the position of an Orb and start the TravelToPlayer coroutine
			/// </summary>
			public void Init() {
				orbPool = transform.parent.GetComponent<Pool>();
				StopCoroutine("TravelToPlayer");
				StartCoroutine("TravelToPlayer");
			}

			/// <summary>
			/// This coroutine handles resizing overtime, and moving toward the player position over time
			/// Overtime, the speed of the orb increases to ensure this coroutine exits and the orb is made available to the
			/// ..orb pool in a timely manner
			/// </summary>
			IEnumerator TravelToPlayer() {
				player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
				Vector3 vel = new Vector3(Random.Range(-1, 1), Random.Range(1, 2), Random.Range(-1, 1));
				Vector3 goalDir = player.position - transform.position;
				float dist = Vector3.Distance(transform.position, player.position);
				float currentSpeed = speed;
				transform.localScale = Vector3.one * 0.2f;

				while (dist > 0.25f) {
					if (currentSpeed < 20) {
						currentSpeed += acceleration;
					}
					transform.position += vel * currentSpeed * Time.deltaTime;
					vel = Vector3.Lerp(vel, Vector3.Normalize(goalDir), currentSpeed * Time.deltaTime);

					transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkRate * Time.deltaTime);

					goalDir = player.position - transform.position;
					dist = Vector3.Distance(transform.position, player.position);
					yield return null;
				}
				
				orbPool.Dispose(transform);
			}
		}
	}
}
