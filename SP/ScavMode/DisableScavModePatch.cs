﻿using EFT.UI;
using SIT.Tarkov.Core;
using System.Reflection;
using UnityEngine;

/**
 * ORIGINAL CODE written by MaoMao / TheMaoci. All credit goes to him! His code is closed source and subject to license.
 */

namespace SIT.Core.SP.ScavMode
{
    /// <summary>
    /// 
    /// </summary>
    public class DisableScavModePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.UI.Matchmaker.MatchMakerSideSelectionScreen).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        static void PatchPostfix(
            EFT.UI.Matchmaker.MatchMakerSideSelectionScreen __instance,
            DefaultUIButton ____savagesBigButton,
            DefaultUIButton ____pmcBigButton)
        {
            ____savagesBigButton.transform.parent.gameObject.SetActive(false);
            ____pmcBigButton.transform.parent.transform.localPosition = new Vector3(-220, 520, 0);
            RectTransform tempRectTransform = ____pmcBigButton.GetComponent<RectTransform>();
            tempRectTransform.anchoredPosition = new Vector2(-220, 0);
            tempRectTransform.offsetMax = new Vector2(-220, 0);
            tempRectTransform.offsetMin = new Vector2(-220, 0);
            tempRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);

        }
    }
}
