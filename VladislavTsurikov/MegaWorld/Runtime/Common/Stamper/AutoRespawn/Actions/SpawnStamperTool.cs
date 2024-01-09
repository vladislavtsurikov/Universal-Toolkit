namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn.Actions
{
    public class SpawnStamperTool : Respawn
    {
        public SpawnStamperTool(StamperTool stamperToolTool) : base(stamperToolTool)
        {
        }

        public override void OnRespawn()
        {
            StamperTool.StamperSpawn();
        }
    }
}