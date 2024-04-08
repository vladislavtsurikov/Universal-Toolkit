using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings.ScatterSystem
{
    public class PhysicsWaitingNextFrame : WaitingNextFrame
    {
        private int _maxSimulatedBodyCount;

        public PhysicsWaitingNextFrame(int maxSimulatedBodyCount)
        {
            _maxSimulatedBodyCount = maxSimulatedBodyCount;
        }
        
        public override bool IsWaitForNextFrame()
        {
            if (SimulatedBodyStack.Count < _maxSimulatedBodyCount)
            {
                return false;
            }

            return true;
        }
    }
}