using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using VanillaStorytellersExpanded;
using RimWorld.QuestGen;

namespace OuterRimCore
{
    public class QuestCurrency_Silver : QuestCurrency
    {
        public int minimumGoodwillRequirement = -100;

        public override bool Allows(QuestGiverManager questGiverManager, Quest quest, Slate slate, out QuestInfo questInfo)
        {
            Pawn pawn = slate.Get<Pawn>("asker");
            if (pawn?.Faction != null && pawn.Faction.GoodwillWith(Faction.OfPlayer) >= minimumGoodwillRequirement && TradeUtility.ColonyHasEnoughSilver(Find.CurrentMap, (int)questGiverManager.def.currency.costToAcceptQuest))
            {
                CurrencyInfo_Silver currencyInfo = new CurrencyInfo_Silver();
                currencyInfo.amount = questGiverManager.def.currency.costToAcceptQuest;
                questInfo = new QuestInfo(quest, pawn.Faction, currencyInfo, questGiverManager.def.onlyOneReward ? true : false);
                return true;
            }
            questInfo = null;
            return false;
        }
    }

    public class CurrencyInfo_Silver : QuestCurrencyInfo
    {
        public override string GetCurrencyInfo()
        {
            return "OuterRim.SilverCurrencyCost".Translate(amount);
        }

        public override void Buy(QuestInfo questInfo)
        {
            base.Buy(questInfo);
            TradeUtility.LaunchSilver(Find.CurrentMap, (int)amount);
        }
    }
}
