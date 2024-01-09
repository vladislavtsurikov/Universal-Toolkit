using System.Diagnostics;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public class DefaultWaitingNextFrame : WaitingNextFrame
    {
        private Stopwatch _stopwatch;
        
        public float SecondsUntilNextFrame = 5;
        public float MillisecondsUntilNextFrame => SecondsUntilNextFrame * 1000;

        public DefaultWaitingNextFrame(float secondsUntilNextFrame)
        {
            SecondsUntilNextFrame = secondsUntilNextFrame;
        }

        public override bool IsWaitForNextFrame()
        {
            if (_stopwatch == null)
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
                return false;
            }
		    
            if (_stopwatch.ElapsedMilliseconds >= MillisecondsUntilNextFrame)
            {
                _stopwatch.Restart();
                return true;
            }

            return false;
        }
    }
}