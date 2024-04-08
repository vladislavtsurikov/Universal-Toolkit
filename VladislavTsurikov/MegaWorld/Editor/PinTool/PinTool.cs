#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ColliderSystem.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.PinTool.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.Undo.Editor.Actions.GameObject;
using VladislavTsurikov.Undo.Editor.Actions.TerrainObjectRenderer;
using VladislavTsurikov.Utility.Runtime.Extensions;
using DrawHandles = VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint.DrawHandles;
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.PinTool
{
    [ComponentStack.Runtime.Attributes.MenuItem("Happy Artist/Pin")]
    [AddToolComponents(new []{typeof(PinToolSettings)})]
    [AddGlobalCommonComponents(new []{typeof(TransformSpaceSettings), typeof(LayerSettings)})]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainObject), typeof(PrototypeGameObject)})]
    public class PinTool : ToolWindow
    {
        private PlacedObjectData _placedObjectData;

        private float _scaleFactor;
        private float _angle;

        private Vector3 _point;
        private Vector3 _right;
        private Vector3 _upwards;
        private Vector3 _forward;

        private RayHit _currentRayHit;

        protected override void DoTool()
        {                        
            PinToolSettings settings = (PinToolSettings)ToolsComponentStack.GetElement(typeof(PinTool), typeof(PinToolSettings));
            
            Group group = WindowData.Instance.SelectionData.SelectedData.SelectedGroup;

            Event e = Event.current;
            int controlID = UnityEngine.GUIUtility.GetControlID(EditorHash, FocusType.Passive);

            switch (e.GetTypeForControl(controlID))
            {
            case EventType.MouseDown:
                if (e.button == 0 && !e.alt)
                {
                    _currentRayHit = Raycast();

                    if (_currentRayHit != null)
                    {
                        _placedObjectData = PlaceObject.Place(group, _currentRayHit);
                        if (_placedObjectData == null)
                        {
                            return;
                        }

                        _point = _currentRayHit.Point;
                        Vector3Ex.GetOrientation(_currentRayHit.Normal, settings.FromDirection, settings.WeightToNormal, out _upwards, out _right, out _forward);

                        if (settings.RotationTransformMode == TransformMode.Fixed)
                        {
                            _placedObjectData.GameObject.transform.rotation = GetRotation(settings.FixedRotationValue);
                        }
                        else
                        {
                            _placedObjectData.GameObject.transform.rotation = GetRotation(new Vector3(0, _angle, 0));
                        }

                        _scaleFactor = GetObjectScaleFactor(_placedObjectData.GameObject);

                        _placedObjectData.GameObject.transform.localScale = new Vector3(0, 0f, 0f);
                        _placedObjectData.GameObject.transform.position += new Vector3(0, settings.Offset, 0);
                    }

                    UnityEngine.GUIUtility.hotControl = controlID;
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
            if (UnityEngine.GUIUtility.hotControl == controlID && e.button == 0 && _placedObjectData != null)
            {
                if (IntersectsHitPlane(HandleUtility.GUIPointToWorldRay(e.mousePosition), out _point))
                {
                    Vector3 vector = _point - _placedObjectData.GameObject.transform.position;
                    float vectorLength = vector.magnitude;

                    if (vectorLength < 0.01f)
                    {
                        vector = Vector3.up * 0.01f;
                        vectorLength = 0.01f;
                    }

                    _angle = Vector3.Angle(_forward, vector.normalized);
                    if (Vector3.Dot(vector.normalized, _right) < 0.0f)
                    {
                        _angle = -_angle;
                    }

                    float scale = 2.0f * vectorLength * _scaleFactor;
                            
                    switch (settings.RotationTransformMode)
                    {
                        case TransformMode.Free:
                        {
                            _placedObjectData.GameObject.transform.rotation = GetRotation(new Vector3(0, _angle, 0));

                            break;
                        }
                        case TransformMode.Snap:
                        {				
                            if (settings.SnapRotationValue > 0)
                            {
                                _angle = Mathf.Round(_angle / settings.SnapRotationValue) * settings.SnapRotationValue;
                            }

                            _placedObjectData.GameObject.transform.rotation = GetRotation(new Vector3(0, _angle, 0));

                            break;
                        }
                    }

                    switch (settings.ScaleTransformMode)
                    {
                        case TransformMode.Free:
                        {
                            _placedObjectData.GameObject.transform.localScale = new Vector3(scale, scale, scale);

                            break;
                        }
                        case TransformMode.Snap:
                        {				
                            if (settings.SnapScaleValue > 0)
                            {
                                scale = Mathf.Round(scale / settings.SnapScaleValue) * settings.SnapScaleValue;
                                scale = Mathf.Max(scale, 0.01f);
                            }

                            _placedObjectData.GameObject.transform.localScale = new Vector3(scale, scale, scale);

                            break;
                        }
                        case TransformMode.Fixed:
                        {
                            _placedObjectData.GameObject.transform.localScale = settings.FixedScaleValue;

                            break;
                        }
                    }
                }

                e.Use();
            }
            break;
            case EventType.MouseUp:
                if (UnityEngine.GUIUtility.hotControl == controlID && e.button == 0)
                {
                    if (_placedObjectData != null)
                    {
                        if (group.PrototypeType == typeof(PrototypeGameObject))
                        {
                            if (settings.ScaleTransformMode != TransformMode.Fixed)
                            {
                                Vector2 placeScreenPoint = HandleUtility.WorldToGUIPoint(_placedObjectData.GameObject.transform.position);

                                if ((e.mousePosition - placeScreenPoint).magnitude < 5f)
                                {
                                    Object.DestroyImmediate(_placedObjectData.GameObject);
                                }
                                else
                                {
                                    GameObjectCollider.Runtime.GameObjectCollider.RegisterGameObjectToCurrentScene(_placedObjectData.GameObject);
                                    Undo.Editor.Undo.RecordUndo(new CreatedGameObject(_placedObjectData.GameObject));
                                }
                            }
                        }
#if RENDERER_STACK
                        else if (group.PrototypeType == typeof(PrototypeTerrainObject))
                        {
                            if (settings.ScaleTransformMode != TransformMode.Fixed)
                            {
                                Vector2 placeScreenPoint = HandleUtility.WorldToGUIPoint(_placedObjectData.GameObject.transform.position);

                                if ((e.mousePosition - placeScreenPoint).magnitude < 5f)
                                {
                                    Object.DestroyImmediate(_placedObjectData.GameObject);
                                }
                                else
                                {
                                    PrototypeTerrainObject proto = (PrototypeTerrainObject)_placedObjectData.Proto;
                                    Transform transform = new Transform(_placedObjectData.GameObject);

                                    TerrainObjectInstance terrainObjectInstance = TerrainObjectRendererAPI.AddInstance(proto.RendererPrototype, transform.Position, transform.Scale, transform.Rotation);
                                    Undo.Editor.Undo.RecordUndo(new CreatedTerrainObject(terrainObjectInstance));

                                    Object.DestroyImmediate(_placedObjectData.GameObject);
                                }
                            }
                        }
#endif

                        _placedObjectData = null;
                    }

                    UnityEngine.GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
            case EventType.MouseMove:
            {
                _currentRayHit = Raycast();

                e.Use();
            }
                break;
            case EventType.Repaint:
                if (_currentRayHit != null)
                {
                    DrawPinToolHandles();
                }
                break;
            case EventType.Layout:
                HandleUtility.AddDefaultControl(controlID);
                break;
            case EventType.KeyDown:
                switch (e.keyCode)
                {
                    case KeyCode.F:
                        // F key - Frame camera on brush hit point
                        if (GUIUtility.IsModifierDown(EventModifiers.None) && _currentRayHit != null)
                        {
                            SceneView.lastActiveSceneView.LookAt(_currentRayHit.Point, SceneView.lastActiveSceneView.rotation, 15);
                            e.Use();
                        }
                        break;
                }
                break;
            }
        }

        protected override void OnDisableElement()
        {
            if(_placedObjectData != null)
            {
                Object.DestroyImmediate(_placedObjectData.GameObject);
                _placedObjectData = null;
            }
        }

        private void DrawPinToolHandles()
        {
            PinToolSettings settings = (PinToolSettings)ToolsComponentStack.GetElement(typeof(PinTool), typeof(PinToolSettings));

            Vector3Ex.GetOrientation(_currentRayHit.Normal, settings.FromDirection, settings.WeightToNormal,
                out var upwards, out var right, out var forward);

            if (_placedObjectData == null)
            {
                DrawHandles.DrawXYZCross(_currentRayHit, upwards, right, forward);

                return;
            }
            else
            {
                Handles.DrawDottedLine(_currentRayHit.Point, _point, 4.0f);
            }
            
            DrawHandles.DrawXYZCross(_currentRayHit, upwards, right, forward);
        }

        private Quaternion GetRotation(Vector3 euler)
        {
            Quaternion placeOrientation = Quaternion.LookRotation(_forward, _upwards);
            return placeOrientation * Quaternion.Euler(euler);
        }

        private float GetObjectScaleFactor(GameObject gameObject)
    	{
    		Bounds bounds = gameObject.GetObjectWorldBounds();
    		Vector3 localScale = gameObject.transform.localScale;

    		float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

    		if (size != 0.0f)
    			size = 1.0f / size;
    		else
    			size = 1.0f;

    		return new Vector3(localScale.x * size, localScale.y * size, localScale.z * size).x;
    	}

        private bool IntersectsHitPlane(Ray ray, out Vector3 hitPoint)
        {
            Plane plane = new Plane(_upwards, _point);
            if (plane.Raycast(ray, out var rayDistance))
            {
                hitPoint = ray.GetPoint(rayDistance);
                return true;
            }
            hitPoint = Vector3.zero;
            return false;
        }

        private static RayHit Raycast()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            RayHit rayHit = ColliderUtility.Raycast(ray, layerSettings.PaintLayers);

            return rayHit;
        }
    }
}
#endif