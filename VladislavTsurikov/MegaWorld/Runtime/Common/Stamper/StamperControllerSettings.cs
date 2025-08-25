﻿using System;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    [Name("Stamper Tool Controller")]
    public class StamperControllerSettings : Component
    {
        public bool AutoRespawn;
        public float DelayAutoRespawn = 0.1f;

        [NonSerialized]
        public StamperTool StamperTool;

        public bool Visualisation = true;

        protected override UniTask SetupComponent(object[] setupData = null)
        {
            StamperTool = (StamperTool)setupData[0];
            return UniTask.CompletedTask;
        }
    }
}
