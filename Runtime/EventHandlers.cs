using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DummyAPI;

namespace Runtime
{
        public class EventHandlers
        {

            public void OnWaitingForPlayers()
            {
                ServerConsole.singleton.NameFormatter.Commands["player_count"] = delegate (List<string> args)
                {
                    return (PlayerManager.players.Count - Extensions.Dummies.Count).ToString();
                };
                ServerConsole.singleton.NameFormatter.Commands["full_player_count"] = delegate (List<string> args)
                {
                    int count = PlayerManager.players.Count - Extensions.Dummies.Count;
                    if (count != CustomNetworkManager.TypedSingleton.ReservedMaxPlayers)
                    {
                        return string.Format("{0}/{1}", count, CustomNetworkManager.TypedSingleton.ReservedMaxPlayers);
                    }
                    int count2 = args.Count;
                    if (count2 == 1)
                    {
                        return "FULL";
                    }
                    if (count2 != 2)
                    {
                        throw new ArgumentOutOfRangeException("args", args, "Invalid arguments. Use: full_player_count OR full_player_count,[full]");
                    }
                    return ServerConsole.singleton.NameFormatter.ProcessExpression(args[1]);
                };
            }

            public void OnRoundStart()
            {

            }

            public void OnRoundEnd(RoundEndedEventArgs ev)
            {
                foreach (Dummy npc in Extensions.Dummies.Values)
                {
                    npc.Destroy();
                }
            }

            public void OnDeath(DiedEventArgs ev)
            {
                if (ev.Target.IsDummy())
                {
                    ev.Target.AsDummy().Destroy();
                }
            }
        }
}