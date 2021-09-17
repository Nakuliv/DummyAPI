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
        public string Command { get; } = "spawndummy";

        public string[] Aliases { get; } = new string[]
        {
            "spdummy"
        };

        public string Description { get; } = "Spawn Dummy";

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

            var _dummy = new Dummy(player.Position, new Quaternion(), RoleType.Tutorial);
            Timing.RunCoroutine(Walk(_dummy, player));
            response = $"Spawned Dummy!";
            return true;
        }

        private IEnumerator<float> Walk(Dummy _dummy, Player Owner)
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(0.1f);

                if (Owner == null) _dummy.Destroy();
                if (_dummy.GameObject == null) yield break;
                _dummy.RotateToPosition(Owner.Position);

                var distance = Vector3.Distance(Owner.Position, _dummy.Position);

                if ((PlayerMovementState)Owner.AnimationController().Network_curMoveState == PlayerMovementState.Sneaking) _dummy.Movement = PlayerMovementState.Sneaking;
                else _dummy.Movement = PlayerMovementState.Sprinting;

                if (_dummy.Movement == PlayerMovementState.Sneaking)
                {
                    if (distance > 5f) _dummy.Position = Owner.Position;

                    else if (distance > 1f) _dummy.Direction = MovementDirection.Forward;

                    else if (distance <= 1f) _dummy.Direction = MovementDirection.Stop;

                    continue;
                }

                if (distance > 10f)
                    _dummy.Position = Owner.Position;

                else if (distance > 2f)
                    _dummy.Direction = MovementDirection.Forward;

                else if (distance <= 1.25f)
                    _dummy.Direction = MovementDirection.Stop;

            }
        }
    }
}
