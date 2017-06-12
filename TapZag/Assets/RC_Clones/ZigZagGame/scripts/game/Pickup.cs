using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// This is a pooled object. It is always available - never instantiated, never destroyed
		/// </summary>
		public class Pickup : MonoBehaviour {

			public float bounceSpeed = 2;
			public float bounceHeight = 0.004f;
			public float turnSpeed = 2;
			public float turnFactor = 10;
			public int orbOutput = 4;

			Vector3 pos;
			Vector3 angle;

			Pool orbPool;
			Pool pool;

			/// <summary>
			/// Initialize necessary pool references
			/// </summary>
			void Start() {
				pool = transform.parent.GetComponent<Pool>();
				orbPool = GameObject.FindWithTag("OrbPool").GetComponent<Pool>();
			}

			/// <summary>
			/// The pickup will bounce and turn in a continuous oscillation phase
			/// </summary>
			void Update() {
				Bounce();
				Turn();
			}

			/// <summary>
			/// Bounce the position overtime with cosine
			/// </summary>
			void Bounce() {
				pos = transform.position;
				pos.y += Mathf.Cos(Time.time * bounceSpeed) * bounceHeight;	
				transform.position = pos;
			}

			/// <summary>
			/// Turn the rotation overtime with cosine
			/// </summary>
			void Turn() {
				angle = transform.eulerAngles;
				angle.y += Mathf.Cos(Time.time * turnSpeed) * turnFactor;
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(angle), 5 * Time.deltaTime);
			}

			/// <summary>
			/// When the player is detected, "die"
			/// When the bounds at the bottom of the screen are detected, fall and "die"
			/// </summary>
			void OnTriggerEnter(Collider col) {
				if (col.gameObject.tag.Equals("Player")) {
					Die();
				}
				if (col.gameObject.tag.Equals("BottomBounds")) {
					StopCoroutine("Fall");
					StartCoroutine("Fall");
				}
			}

			/// <summary>
			/// Request 'orbOutput' number of orbs from the orbPool and initialize them
			/// Use the pickupPool to dispose this pickup. See Pool.cs
			/// </summary>
			void Die() {
				for (int i = 0; i < orbOutput; i++) {
					Transform orb = orbPool.GetFirstAvailable();
					orb.GetComponent<Orb>().Init();
					orb.position = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1), Random.Range(-0.5f, 0.5f));
				}
				pool.Dispose(transform);
			}

			/// <summary>
			/// While position.y is greater than -5, the pickup will fall and then be disposed
			/// </summary>
			IEnumerator Fall() {
				while (transform.position.y > -5) {
					transform.position -= Vector3.up * 5 * Time.deltaTime;
					yield return null;
				}
				pool.Dispose(transform);
			}
		}
	}
}
