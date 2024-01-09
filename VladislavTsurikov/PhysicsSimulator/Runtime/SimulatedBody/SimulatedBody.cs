using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.PhysicsSimulator.Runtime.Settings;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody
{
    public class SimulatedBody 
    {
        protected readonly GameObject _gameObject;
        private Rigidbody _addedRigidbody;
        private Collider _addedCollider;
        private List<Collider> _nonConvexColliders;
        
        private readonly List<OnDisableSimulatedBodyAction> _onDisablePhysicsActions = new List<OnDisableSimulatedBodyAction>();
        
        protected internal delegate void OnAddToSimulatedBodyStackDelegate();
        protected internal OnAddToSimulatedBodyStackDelegate OnAddToSimulatedBodyStack;

        public Rigidbody Rigidbody => _addedRigidbody;
        public GameObject GameObject => _gameObject;

        public SimulatedBody(GameObject gameObject)
        {
            _gameObject = gameObject;

            ApplyPositionDown applyPositionDown =
                new ApplyPositionDown(PhysicsSimulatorSettings.Instance.AutoPositionDownSettings)
                {
                    SimulatedBody = this
                };

            _onDisablePhysicsActions.Insert(0, applyPositionDown);

            AddPhysicsSupport();
        }
        
        public SimulatedBody(GameObject gameObject, List<OnDisableSimulatedBodyAction> onDisablePhysicsActions)
        {
            _gameObject = gameObject;

            if (onDisablePhysicsActions == null)
            {
                onDisablePhysicsActions = new List<OnDisableSimulatedBodyAction>();
            }

            foreach (var onDisablePhysicsAction in onDisablePhysicsActions)
            {
                onDisablePhysicsAction.SimulatedBody = this;
            }
            
            _onDisablePhysicsActions = onDisablePhysicsActions;
            
            ApplyPositionDown applyPositionDown =
                new ApplyPositionDown(PhysicsSimulatorSettings.Instance.AutoPositionDownSettings)
                {
                    SimulatedBody = this
                };
            
            _onDisablePhysicsActions.Insert(0, applyPositionDown);
            
            AddPhysicsSupport();
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

                if(collider is MeshCollider { convex: false } meshCollider)
                {
                    _nonConvexColliders.Add(meshCollider);

                    meshCollider.convex = true;
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
            
            try
            {
                foreach (var onDisablePhysicsAction in _onDisablePhysicsActions)
                {
                    onDisablePhysicsAction.OnDisablePhysicsInternal();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
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