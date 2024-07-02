using UnityEngine;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    public class PrecisePlaceToolAllShortcutCombos : DataTypeSingleton<PrecisePlaceToolAllShortcutCombos>
    {
        public ShortcutCombo Restore { get; private set; }

        public PrecisePlaceToolAllShortcutCombos()
        {
            CreateCombos();
        }
        
        private void CreateCombos()
        {
            Restore = new ShortcutCombo();
            Restore.AddKey(KeyCode.Tab);
        }
    }
}