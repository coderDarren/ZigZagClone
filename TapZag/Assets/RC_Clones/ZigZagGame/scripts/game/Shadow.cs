using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		public class Shadow : MonoBehaviour {

			public Transform plane;

			RaycastHit hit;

			void Update() {
				if (Physics.Raycast(transform.position, Vector3.down, out hit, 50)) {
					plane.position = hit.point + Vector3.up * 0.02f;
					plane.gameObject.SetActive(true);
				}
				else {
					plane.gameObject.SetActive(false);
				}
			}
		}
	}
}
