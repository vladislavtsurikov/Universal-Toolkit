using UnityEngine;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.ShortcutCombo.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    public class PrecisePlaceToolAllShortcutCombos : DataTypeSingleton<PrecisePlaceToolAllShortcutCombos>
    {
        private ShortcutCombo.Editor.ShortcutCombo _restore;

        public ShortcutCombo.Editor.ShortcutCombo Restore => _restore;

        public PrecisePlaceToolAllShortcutCombos()
        {
            CreateCombos();
        }
        
        private void CreateCombos()
        {
            _restore = new ShortcutCombo.Editor.ShortcutCombo();
            _restore.AddKey(KeyCode.Tab);
        }
    }
}