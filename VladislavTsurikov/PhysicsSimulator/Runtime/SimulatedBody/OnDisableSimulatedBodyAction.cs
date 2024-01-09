namespace VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody
{
    public abstract class OnDisableSimulatedBodyAction
    {
        protected internal SimulatedBody SimulatedBody;

        internal void OnDisablePhysicsInternal()
        {
            OnDisablePhysics();
        }

        protected abstract void OnDisablePhysics();
    }
}