using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Editor.WorldSpaceSupport;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.UIElementsUtility.Runtime.WorldSpaceSupport
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class WorldSpaceUIDocument : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField] 
        private UIDocument _uiDocument;
        
        [SerializeField] 
        public Camera MainCamera;
        
        [HideInInspector]
        [SerializeField] 
        private MeshCollider _meshCollider;

#if UNITY_EDITOR
        [HideInInspector]
        [SerializeField]
        public EditorWorldSpaceUIDocumentSupport EditorWorldSpaceUIDocumentSupport = new EditorWorldSpaceUIDocumentSupport();
#endif

        private void OnEnable()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

#if UNITY_EDITOR
            InitializeCamera();
            _meshCollider = GetComponent<MeshCollider>();
            
            EditorWorldSpaceUIDocumentSupport.Setup(this, _uiDocument);
#endif
            
            _uiDocument.panelSettings.SetScreenToPanelSpaceFunction(ConvertScreenSpacePositionToPanelSpacePosition);
        }
        
        public void InitializeCamera()
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }
        }

        private Vector2 ConvertScreenSpacePositionToPanelSpacePosition(Vector2 screenPosition)
        {
            screenPosition.y = Screen.height - screenPosition.y;
            var ray = MainCamera.ScreenPointToRay(screenPosition);

            if (!_meshCollider.Raycast(ray, out var hit, Mathf.Infinity))
            {
                return new Vector2(float.NaN, float.NaN);
            }

            var targetTexture = _uiDocument.panelSettings.targetTexture;
            var textureCoord = hit.textureCoord;
        
            textureCoord.y = 1 - textureCoord.y;
            textureCoord *= new Vector2(targetTexture.width, targetTexture.height);

            return textureCoord;
        }
    }
}