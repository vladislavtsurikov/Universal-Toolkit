namespace VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody
{
    public abstract class OnDisableSimulatedBodyEvent
    {
        protected internal SimulatedBody SimulatedBody;

        internal void OnDisablePhysicsInternal()
        {
            OnDisablePhysics();
        }

        protected abstract void OnDisablePhysics();
    }
}