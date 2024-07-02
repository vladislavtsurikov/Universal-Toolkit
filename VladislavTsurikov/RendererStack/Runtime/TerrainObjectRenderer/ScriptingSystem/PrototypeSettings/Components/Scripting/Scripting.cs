﻿using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting
{
    [Name("Scripting")]
    public class Scripting : PrototypeComponent
    {
        [SerializeField]
        private float _maxDistance = 50;
        public float MaxDistance 
        {
            get => _maxDistance;
            set
            {
                if(value < 1)
                {
                    _maxDistance = 1;
                }
                else
                {
                    _maxDistance = value;
                }
            }
        }
        
        [OdinSerialize]
        public ComponentStackSupportSameType<Script> ScriptStack = new ComponentStackSupportSameType<Script>();

        protected override void SetupPrototypeComponent()
        {
            ScriptStack.Setup();
        }

        protected override void OnDeleteElement()
        { 
            TerrainObjectRenderer.Instance.ScriptingSystem.Setup();
        }

        protected override void OnCreate()
        {
            TerrainObjectRenderer.Instance.ScriptingSystem.Setup();
        }

        protected override void OnChangeActive()
        {
            TerrainObjectRenderer.Instance.ScriptingSystem.Setup();
        }
    }
}