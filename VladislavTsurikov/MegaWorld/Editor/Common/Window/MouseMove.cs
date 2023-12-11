#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ColliderSystem.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Window
{
    public class MouseMove
    {
        private static int _editorHash = "Editor".GetHashCode();
        
        private bool _isStartDrag;
        
        protected RayHit _prevRaycast;
        
        public delegate void OnMouseDownDelegate();
        public delegate void OnMouseDragDelegate(Vector3 dragPoint);
        public delegate void OnMouseMoveDelegate();
        public delegate void OnRepaintDelegate();
        public delegate void BeforePointDetectionFromMouseDragDelegate();
        
        public OnMouseDownDelegate OnMouseDown;
        public OnMouseDragDelegate OnMouseDrag;
        public OnMouseMoveDelegate OnMouseMove;
        public OnRepaintDelegate OnRepaint;
        public BeforePointDetectionFromMouseDragDelegate BeforePointDetectionFromMouseDrag;

        public GameObject IgnoreGameObjectRaycast;
        
        public float LookAtSize;

        public RayHit Raycast { get; protected set; }
        public Vector3 StrokeDirection { get; protected set; }
        public bool IsStartDrag => _isStartDrag;
        
        protected virtual void OnPointDetectionFromMouseDrag(Func<Vector3, bool> func)
        {
            func.Invoke(Raycast.Point);
        }
        
        protected virtual void OnStartDrag() {}
        
        public void Run()
        {
            int controlID = GUIUtility.GetControlID(_editorHash, FocusType.Passive);
            Event e = Event.current;
            EventType eventType = e.GetTypeForControl(controlID);

            switch (eventType)
            {
                case EventType.MouseDown:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(UpdateRayHitFromMousePosition() == false)
                    {
                        return;
                    }

                    StartDrag();

                    if(Raycast != null)
                    {
                        OnMouseDown?.Invoke();
                    }

                    break;
                }
                case EventType.MouseDrag:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(UpdateRayHitFromMousePosition() == false)
                    {
                        return;
                    }
                    
                    BeforePointDetectionFromMouseDrag?.Invoke();

                    PointDetectionFromMouseDrag(dragPoint =>
                    {
                        OnMouseDrag?.Invoke(dragPoint);

                        return true;
                    });

                    e.Use();

                    break;
                }
                case EventType.MouseMove:
                {
                    if(UpdateRayHitFromMousePosition() == false)
                    {
                        return;
                    }
                    
                    OnMouseMove?.Invoke();

                    e.Use();

                    break;
                }
                case EventType.Repaint:
                {
	                if(Raycast == null)
                    {
                        return;
                    }
                    
                    OnRepaint?.Invoke();

                    break;
                }
                case EventType.Layout:
                {
                    HandleUtility.AddDefaultControl(controlID);
                    break;
                }
                case EventType.KeyDown:
                {
                    switch (e.keyCode)
                    {
                        case KeyCode.F:
                        {
                            if (LookAtSize == 0)
                            {
                                break;
                            }
                            
                            if (Utility.Runtime.GUIUtility.IsModifierDown(EventModifiers.None) && Raycast != null)
                            {
                                SceneView.lastActiveSceneView.LookAt(Raycast.Point, SceneView.lastActiveSceneView.rotation, LookAtSize);
                                e.Use();
                            }
                        }

                        break;
                    }
                    break;
                }
            }
        }

        private void PointDetectionFromMouseDrag(Func<Vector3, bool> func)
        {
            _isStartDrag = false;
            
            Vector3 hitPoint = Raycast.Point;
            Vector3 lastHitPoint = _prevRaycast.Point;

            if(!hitPoint.IsSameVector(lastHitPoint))
            {
                StrokeDirection = (hitPoint - lastHitPoint).normalized;

                OnPointDetectionFromMouseDrag(func);
                
                func.Invoke(hitPoint);
            }
            
            _prevRaycast = Raycast;
        }

        private bool UpdateRayHitFromMousePosition()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            foreach (Group type in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

                if (IgnoreGameObjectRaycast != null)
                {
                    ObjectFilter objectRaycastFilter = new ObjectFilter();
                    List<object> gameObjects = new List<object>(IgnoreGameObjectRaycast.GetAllChildrenAndSelf());
                    objectRaycastFilter.SetIgnoreObjects(gameObjects);
                    objectRaycastFilter.LayerMask = layerSettings.PaintLayers;
                    
                    Raycast = ColliderUtility.Raycast(ray, objectRaycastFilter);
                }
                else
                {
                    Raycast = RaycastUtility.Raycast(ray, layerSettings.GetCurrentPaintLayers(type.PrototypeType));
                }

                if(Raycast != null)
                {
                    return true;
                }
            }

            return false;
        }
        
        private void StartDrag()
        {
            if (Raycast == null)
            {
                return;
            }

            _isStartDrag = true;
            _prevRaycast = Raycast;

            OnStartDrag();
        }
    }
}
#endif