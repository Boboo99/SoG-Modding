using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoG.GrindScript
{
    
    public abstract class ConvertedObject
    {
        protected dynamic _originalObject;

        protected ConvertedObject(object originalObject)
        {
            _originalObject = originalObject;
        }

        public dynamic Original => _originalObject;
    }

    public class Inventory : ConvertedObject 
    {
        public Inventory(object orignalType) : base(orignalType)
        {
        }

        public void AddItem(ItemTypes type, int amount)
            => _originalObject.AddItem(type, amount);

        public int GetItemAmount(ItemTypes type)
            => _originalObject.GetAmount(type);

        public void SetItemAmount(ItemTypes type, int amount)
            => _originalObject.SetItemAmount(type, amount);

        public void AddMoney(int amount)
            => _originalObject.AddMoney(amount);

        public int GetMoneyAmount()
            => _originalObject.GetMoney();

        public void SetMoneyAmount(int amount)
            => _originalObject.SetMoney(amount);

    }


    public class Equipment : ConvertedObject
    {
        public Equipment(object originalType) : base(originalType)
        {
        }
    }

    /*
    public class Journal : ConvertedObject
    {
        public Journal(object originalType) : base(originalType)
        {
        }

        public void AddQuest(Quest quest)
        {
            dynamic qDescription = Activator.CreateInstance(Utils.GetGameType("Quests.QuestDescription"));
            qDescription.sQuestNameReference = quest.Name;
            qDescription.sSummaryReference = quest.Summary;
            qDescription.iIntendedLevel = 1;
            qDescription.xReward = Activator.CreateInstance(Utils.GetGameType("Quests.QuestReward"));

            dynamic q = Activator.CreateInstance(Utils.GetGameType("Quests.Quest"));
            q.xDescription = qDescription;
            q.enQuestID = 44000;




            _originalObject.xQuestLog.AddQuest(q);
        }

    }
    */

    public class Player : ConvertedObject
    {
        public Inventory Inventory;
        public Equipment Equipment;
        //public Journal Journal;

        public Player(object originalType) : base(originalType)
        {
            Inventory = new Inventory(_originalObject.xInventory);
            Equipment = new Equipment(_originalObject.xEquipment);
            //Journal = new Journal(_originalObject.xJournalInfo);
        }

        /* Player Level Members */
        public ushort Level
        {
            get { return _originalObject.xViewStats.iLevel; }
            set { _originalObject.xViewStats.iLevel = value;  }
        }

        public ushort SilverSkillPoints
        {
            get { return _originalObject.xViewStats.iSkillPointsSilver; }
            set { _originalObject.xViewStats.iSkillPointsSilver = value; }
        }

        public ushort GoldSkillPoints
        {
            get { return _originalObject.xViewStats.iSkillPointsGold; }
            set { _originalObject.xViewStats.iSkillPointsGold = value; }
        }

        public ushort TalentPoints
        {
            get { return _originalObject.xViewStats.iTalentPoints; }
            set { _originalObject.xViewStats.iTalentPoints = value; }
        }

        public void SetSkillLevel(SpellTypes type, byte level)
        {
            var spellTypes = Utils.GetGameType("SoG.SpellCodex").GetDeclaredNestedType("SpellTypes");
            var spellGameType = Enum.Parse(spellTypes, type.ToString());

            Utils.GetGameType("SoG.PlayerViewStats")
                 .GetPublicInstanceOverloadedMethods("SetSkillLevel").Single()
                 .Invoke((object) _originalObject.xViewStats, new[] { spellGameType, level });
        }

        /* Player Stat Members */
        public int Health
        {
            get { return _originalObject.xEntity.xBaseStats.iHP; }
            set { _originalObject.xEntity.BaseStats.iHP = value; }
        }

        public int EP
        {
            get { return _originalObject.xEntity.xBaseStats.iEP; }
            set { _originalObject.xEntity.BaseStats.iEP = value; }
        }

        public string Name
        {
            get { return _originalObject.sNetworkNickname; }
        }
    }
}
