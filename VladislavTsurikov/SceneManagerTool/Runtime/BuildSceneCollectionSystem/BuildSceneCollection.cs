﻿using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem
{
    public abstract class BuildSceneCollection : Component
    {
        [OdinSerialize]
        private string _name;

        public override string Name
        {
            get => _name;
            set => _name = value;
        }

        protected override void OnCreate()
        {
            _name = GetType().GetAttribute<MenuItemAttribute>().Name.Split('/').Last();

            if (SceneManagerData.Instance.Profile.BuildSceneCollectionStack.ElementList.Count == 1)
            {
                SceneManagerData.Instance.Profile.BuildSceneCollectionStack.ActiveBuildSceneCollection = this;
            }
        }

        public abstract List<SceneReference> GetSceneReferences();  
        public abstract List<SceneCollection> GetStartupSceneCollections();
        
        public abstract List<SceneCollection> GetAllSceneCollections();

        public virtual void DoBuild()
        {

        }
    }
}