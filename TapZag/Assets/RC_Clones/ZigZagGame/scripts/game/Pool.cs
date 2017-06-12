using System.Collections;
using UnityEngine;

namespace RC_Projects {
	namespace ZigZag {

		/// <summary>
		/// The pool class is in charge of initializing, storing, and provisioning of gameObjects to the game
		/// </summary>
		public class Pool : MonoBehaviour {

			public static Hashtable Instances;
			public int instanceId; //there will be multiple pools

			class PoolObject { 
				public Transform transform;
				bool inUse;
				public bool InUse { get { return inUse; } }
				public PoolObject(Transform t) { transform = t; }
				public void Use() { inUse = true; }
				public void Dispose() { inUse = false; }
			}

			public int poolSize;
			public GameObject obj;
			
			[HideInInspector]
			public bool configured;

			PoolObject[] poolObjects;
			Hashtable poolHash;
			Vector3 offscreen = new Vector3(-10000,-10000,-10000);
			
			/// <summary>
			/// Singleton Pattern (with a twist). Only One Pool Instance With THIS Id is Allowed. See instanceId above (or in the inspector)
			/// This is because there are multiple Pool Containers in the scene (Wall, Orb, & Pickup)
			/// A Pool is only configured once for the lifetime of gameplay
			/// </summary>
			void Awake() {
				if (Instances == null) {
					Instances = new Hashtable();
				}

				if (Instances.ContainsKey(instanceId)) {
					Destroy(gameObject);
				}
				else if (!configured) {
					Instances.Add(instanceId, this);
					DontDestroyOnLoad(gameObject);
					Configure();

				}
			}

			/// <summary>
			/// Instantiate predetermined number of pool objects to this container.
			/// Called once per game session (not per game round)
			/// </summary>
			void Configure() {
				configured = true;
				poolObjects = new PoolObject[poolSize];
				poolHash = new Hashtable();
				for (int i = 0; i < poolObjects.Length; i++) {
					GameObject go = Instantiate(obj) as GameObject;
					Transform t = go.transform;
					t.position = offscreen;
					t.SetParent(transform);
					//t.gameObject.SetActive(false);
					poolObjects[i] = new PoolObject(t);
					poolHash.Add(t, poolObjects[i]);
				}
			}

			/// <summary>
			/// Mark the pool object 't' as available and move it far off screen
			/// </summary>
			public bool Dispose(Transform t) {
				if (poolHash.ContainsKey(t)) {
					PoolObject obj = (PoolObject)poolHash[t];
					obj.transform.position = offscreen;
					obj.transform.SetParent(transform);
					//obj.transform.gameObject.SetActive(false);
					obj.Dispose();
					return true;
				}
				else { 
					return false;
				}
			}

			/// <summary>
			/// Retrieve the first available pool object's transform
			/// </summary>
			public Transform GetFirstAvailable() {
				for (int i = 0; i < poolObjects.Length; i++) {
					if (!poolObjects[i].InUse) {
						poolObjects[i].Use();
						//poolObjects[i].transform.gameObject.SetActive(true);
						return poolObjects[i].transform;
					}
				}
				return null;
			}
		}
	}
}
