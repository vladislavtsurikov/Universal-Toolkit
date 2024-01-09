namespace VladislavTsurikov.PhysicsSimulator.Runtime.DisablePhysics
{
    public abstract class DisablePhysics
    {
        internal abstract void DisablePhysicsSupport();
        internal virtual void OnRegisterSimulatedBody(SimulatedBody.SimulatedBody simulatedBody){}
        internal virtual void OnUnregisterSimulatedBody(SimulatedBody.SimulatedBody simulatedBody){}
        internal virtual void OnActivate(){}
        internal virtual void OnDisable(){}
    }
}