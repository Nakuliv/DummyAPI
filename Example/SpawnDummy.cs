using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using DummyAPI;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using UnityEngine;

namespace Example
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class SpawnDummy : ICommand
    {
        private Dummy _dummy;
        public string Command { get; } = "spawnscp372";

        public string[] Aliases { get; } = new string[]
        {
            "spawndummy",
            "spdummy",
        };

        public string Description { get; } = "Spawn SCP-372";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));
            if (!Permissions.CheckPermission((CommandSender)sender, "dummy.spawn"))
            {
                response = "You do not have permissions to run this command!";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: spawndummy (player id / name)";
                return false;
            }

            if (player == null)
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            _dummy = new Dummy(player.Position, new Quaternion(), RoleType.Tutorial);
            Timing.RunCoroutine(Walk());
            response = $"Spawned Dummy!";
            return true;
        }

        private IEnumerator<float> Walk()
        {
            for (; ; )
            {
                var _dood = Player.Get(2);
                yield return Timing.WaitForSeconds(0.1f);

                _dummy.Movement = PlayerMovementState.Walking;
                _dummy.Direction = MovementDirection.Forward;
                _dummy.RotateToPosition(_dood.Position);
                float distance = Vector3.Distance(_dood.Position, _dummy.Position);

                if (distance <= 1.25f)
                    _dummy.Direction = MovementDirection.Stop;
            }
        }
    }
}
