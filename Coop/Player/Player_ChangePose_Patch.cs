﻿using EFT.InventoryLogic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIT.Coop.Core.Web;
using SIT.Tarkov.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Core.Coop.Player
{
    public class Player_ChangePose_Patch : ModuleReplicationPatch
    {
        public override Type InstanceType => typeof(EFT.Player);
        public override string MethodName => "ChangePose";
        public override bool DisablePatch => true;

        protected override MethodBase GetTargetMethod()
        {
            var method = PatchConstants.GetMethodForType(InstanceType, MethodName);
            //Logger.LogInfo($"Player_ChangePose_Patch:{InstanceType.Name}:{method.Name}");

            return method;
        }

        public static Dictionary<string, bool> CallLocally
            = new Dictionary<string, bool>();

        [PatchPrefix]
        public static bool PrePatch(EFT.Player __instance)
        {
            var result = false;
            if (CallLocally.TryGetValue(__instance.Profile.AccountId, out var expecting) && expecting)
                result = true;

            return result;
        }

        [PatchPostfix]
        public static void PostPatch(
           EFT.Player __instance,
           float poseDelta
            )
        {
            var player = __instance;

            if (CallLocally.TryGetValue(player.Profile.AccountId, out var expecting) && expecting)
            {
                CallLocally.Remove(player.Profile.AccountId);
                return;
            }

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("t", DateTime.Now.Ticks);
            dictionary.Add("d", poseDelta.ToString());
            dictionary.Add("m", "ChangePose");
            ServerCommunication.PostLocalPlayerData(player, dictionary);
        }

        private static List<long> ProcessedCalls = new List<long>();

        public override void Replicated(EFT.Player player, Dictionary<string, object> dict)
        {
            var timestamp = long.Parse(dict["t"].ToString());
            if (!ProcessedCalls.Contains(timestamp))
                ProcessedCalls.Add(timestamp);
            else
            {
                ProcessedCalls.RemoveAll(x => x <= DateTime.Now.AddMinutes(-5).Ticks);
                return;
            }
            
            try
            {
                
                var poseDelta = float.Parse(dict["d"].ToString());
                CallLocally.Add(player.Profile.AccountId, true);
                player.ChangePose(poseDelta);
            }
            catch (Exception e)
            {
                Logger.LogInfo(e);
            }
        }
    }
}
