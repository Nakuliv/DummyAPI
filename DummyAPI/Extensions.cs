using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using RemoteAdmin;
using UnityEngine;

namespace DummyAPI
{
    public static class Extensions
    {
        public static float WalkSpeed
        {
            get => ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier;
            set => ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier = value;
        }

        public static float SprintSpeed
        {
            get => ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier;
            set => ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier = value;
        }

        public static bool IsDummy(this GameObject p)
        {
            return Dummies.ContainsKey(p);
        }

        public static bool IsDummy(this Player p)
        {
            return p.GameObject.IsDummy();
        }

        public static bool IsDummy(this ReferenceHub p)
        {
            return p.gameObject.IsDummy();
        }

        public static Dummy AsDummy(this Player p)
        {
            return p.IsDummy() ? Dummies[p.GameObject] : null;
        }

        public static Dummy AsDummy(this GameObject p)
        {
            return p.IsDummy() ? Dummies[p] : null;
        }

        public static Dummy AsDummy(this ReferenceHub p)
        {
            return AsDummy(p.gameObject);
        }

        public static AnimationController AnimationController(this Player p) => p.ReferenceHub.animationController;
        public static PlayerMovementSync PlayerMovementSync(this Player p) => p.ReferenceHub.playerMovementSync;
        public static NicknameSync NicknameSync(this Player p) => p.ReferenceHub.nicknameSync;
        public static QueryProcessor QueryProcessor(this Player p) => p.ReferenceHub.queryProcessor;
        public static CharacterClassManager ClassManager(this Player p) => p.ReferenceHub.characterClassManager;

        public static Dictionary<GameObject, Dummy> Dummies { get; } = new Dictionary<GameObject, Dummy>();
    }
}
