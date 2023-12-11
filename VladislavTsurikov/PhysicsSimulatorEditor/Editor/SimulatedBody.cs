#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulatorEditor.Editor
{
    public class SimulatedBody 
    {
        protected readonly GameObject _gameObject;
        private Rigidbody _addedRigidbody;
        private Collider _addedCollider;
        private List<Collider> _nonConvexColliders;

        public Rigidbody Rigidbody => _addedRigidbody;
        public GameObject GameObject => _gameObject;

        protected delegate void OnDisablePhysicsSupportDelegate();
        protected OnDisablePhysicsSupportDelegate OnDisablePhysicsSupport;
        
        protected internal delegate void OnAddToSimulatedBodyStackDelegate();
        protected internal OnAddToSimulatedBodyStackDelegate OnAddToSimulatedBodyStack;

        public SimulatedBody(GameObject gameObject)
        {
            _gameObject = gameObject;

            OnDisablePhysicsSupport += ApplyOffset;

            AddPhysicsSupport();
        }
        
        private void ApplyOffset()
        {
            PhysicsSimulatorSettings.Instance.PositionOffsetSettings.ApplyOffset(_gameObject);
        }

        public bool IsRigidbodyStopping()
        {
            if(_gameObject == null)
            {
                return true;
            }

            if(_addedRigidbody == null || _addedRigidbody.IsSleeping())
            {
                return true;
            }

            return false;
        }

        private void AddPhysicsSupport()
        {
            _nonConvexColliders = new List<Collider>();

            Collider[] colliders = _gameObject.transform.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.hideFlags = HideFlags.HideInHierarchy;

                if(collider is MeshCollider && !((MeshCollider) collider).convex)
                {
                    _nonConvexColliders.Add(collider);

                    ((MeshCollider)collider).convex = true;
                }
            }

            AddСomponentsIfNecessary();
        }

        public void DisablePhysicsSupport()
        {
            if (_gameObject == null || _nonConvexColliders == null)
            {
                return;
            }

            RemoveAddedComponents();

            Collider[] colliders = _gameObject.transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider is MeshCollider && _nonConvexColliders.Contains(collider))
                {
                    ((MeshCollider)collider).convex = false;
                }
            }

            _nonConvexColliders = null;
            
            OnDisablePhysicsSupport?.Invoke();
        }

        public bool HasRigidbody()
        {
            if (_gameObject == null || _nonConvexColliders == null)
            {
                return false;
            }

            if (_gameObject.GetComponent<Rigidbody>())
            {
                return true;
            }

            return false;
        }

        private void AddСomponentsIfNecessary()
        {
            if (!_gameObject.GetComponent<Rigidbody>())
            {
                Rigidbody rigidbody = _gameObject.gameObject.AddComponent<Rigidbody>();

                rigidbody.useGravity = true;
                rigidbody.mass = 1;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                _addedRigidbody = rigidbody;
            }

            if (!_gameObject.GetComponent<Collider>())
            {
                MeshCollider collider = _gameObject.gameObject.AddComponent<MeshCollider>();

                // hide colliders in the hierarchy, they cost performance
                collider.hideFlags = HideFlags.HideInHierarchy;

                collider.convex = true;

                _addedCollider = collider;
            }
        }

        private void RemoveAddedComponents()
        {
            if(_addedRigidbody != null)
            {
                Object.DestroyImmediate(_addedRigidbody);
            }

            if(_addedCollider != null)
            {
                Object.DestroyImmediate(_addedCollider);
            }
        }
    }
}
#endif