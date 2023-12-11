using UnityEngine;

namespace VladislavTsurikov.BVH.Runtime
{
    public class BVHNodeRayHit<TNodeData>
        where TNodeData : class
    {
        private BVHNode<TNodeData> _hitNode;
        private float _distance;
        private Vector3 _hitPoint;

        public BVHNode<TNodeData> HitNode => _hitNode;
        public float Distance => _distance;
        public Vector3 HitPoint => _hitPoint;

        public BVHNodeRayHit(Ray ray, BVHNode<TNodeData> hitNode, float distance)
        {
            _hitNode = hitNode;
            _distance = distance;
            _hitPoint = ray.GetPoint(_distance);
        }
    }
}