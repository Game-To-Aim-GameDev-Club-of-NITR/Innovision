using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Components
{
    /**
     * Input component is responsible for setting up mappings for actions and axes input.
     * It allows for easy configuration of input actions and axis mappings, similar to unreal
     * 
     * Date Created: 24-06-2025
     * Created By: Prayas Bharadwaj
     *
     * Date Modified: "Latest Modification Date must be mentioned"
     * Modified By: "Latest Modification Author name"
    */

    public class InputComponent : BaseComponent
    {
        public Dictionary<string, Action> ActionMappings { get; private set; } = new ();
        public Dictionary<string, Action<float>> AxisMappings { get; private set; } = new();

        public Dictionary<string, Action<Vector3>> Vector3Mappings { get; private set; } = new();

        protected override void Start()
        {
            base.Start();
        }
    }
}