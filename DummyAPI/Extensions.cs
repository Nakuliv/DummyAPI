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

        public static AnimationController AnimationController(this Player p) => p.ReferenceHub.animationController;
        public static PlayerMovementSync PlayerMovementSync(this Player p) => p.ReferenceHub.playerMovementSync;
        public static Transform CameraReference(this Player p) => p.ReferenceHub.PlayerCameraReference;
        public static NicknameSync NicknameSync(this Player p) => p.ReferenceHub.nicknameSync;
        public static QueryProcessor QueryProcessor(this Player p) => p.ReferenceHub.queryProcessor;
        public static CharacterClassManager ClassManager(this Player p) => p.ReferenceHub.characterClassManager;
        public static bool IsDummy { get; internal set; } = false;
        public static List<Dummy> Dummies = new List<Dummy>();
    }
}
