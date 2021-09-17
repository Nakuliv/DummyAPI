using Mirror;
using HarmonyLib;
using Exiled.API.Features;
using DummyAPI;

namespace Runtime
{
    [HarmonyPatch(typeof(NetworkBehaviour), nameof(NetworkBehaviour.SendTargetRPCInternal))]
    internal static class MirrorPatch
    {
        [HarmonyPrefix]
        private static bool TargetRPC(NetworkBehaviour __instance)
        {
            var player = Player.Get(__instance.gameObject);
            if (player != null && player.IsDummy()) return false;
            return true;
        }
    }
}
