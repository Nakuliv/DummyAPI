using Exiled.API.Features;
using System;
using Handlers = Exiled.Events.Handlers;

namespace Example
{
    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "Naku";
        public override string Name { get; } = "Example";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(3, 0, 0);

        public override void OnEnabled()
        {
            Plugin.Singleton = this;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {

            base.OnDisabled();
        }


        public static Plugin Singleton;
    }
}
