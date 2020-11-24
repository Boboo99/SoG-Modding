using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoG.GrindScript
{

    public abstract class Reward : ConvertedObject
    {
        protected Reward(object originalObject) : base(originalObject)
        {
        }
    }

    public class ExperienceReward : Reward
    {
        public ExperienceReward(object originalObject) : base(originalObject)
        {
        }

        public static ExperienceReward CreateReward(int amount, List<ItemTypes> items)
        {
            dynamic newReward = Utils.GetGameType("Quests.ExpItemReward").GetConstructor(new[] { typeof(int) })
                .Invoke(new[] { (object)amount });

            var addToReward = Utils.GetGameType("Quests.ExpItemReward").GetMethod("AddItem");


            foreach (var item in items)
            {
                addToReward.Invoke(newReward,
                    new[] { Enum.ToObject(Utils.GetGameType("SoG.ItemCodex+ItemTypes"), item), (ushort)1}); //TODO add an option to have more than just one "amount" per item ^^
            }

            return new ExperienceReward(newReward);
        }
    }

    public abstract class Objective : ConvertedObject
    {
        protected Objective(object originalObject) : base(originalObject)
        {
        }
    }

    public class KillEnemiesObject : Objective
    {
        public KillEnemiesObject(object originalObject) : base(originalObject)
        {
        }

        public static KillEnemiesObject CreateObjectiveIn(LocalGame game, EnemyTypes enemy, ushort amount, string description)
        {
            var constructors = Utils.GetGameType("Quests.Objective_KillEnemies").GetConstructors();
            dynamic newObjective = null;

            Ui.AddMiscTextTo(game, "Quests", description.Trim() + "_ObjectiveDescription", description, MiscTextTypes.Default);

            foreach (var constructor in constructors)
            {
                if (constructor.GetParameters().Count() == 3)
                    newObjective = constructor.Invoke(new[] { Enum.ToObject(Utils.GetGameType("SoG.EnemyCodex+EnemyTypes"), enemy), amount, description.Trim() + "_ObjectiveDescription" });
            }

            if (newObjective == null)
                throw new Exception("Fuck  me sideways!");

            return new KillEnemiesObject(newObjective);
        }
    }

    public class Quest : ConvertedObject
    {
        public Quest(object originalObject) : base(originalObject)
        {
        }

        public static Quest AddQuestTo(LocalGame game, Player player, string name, string description, QuestType type, Reward reward, List<Objective> questObjectives)
        {
            dynamic newQuestDescription = Activator.CreateInstance(Utils.GetGameType("Quests.QuestDescription"));
            dynamic newQuest = Activator.CreateInstance(Utils.GetGameType("Quests.Quest"));

            Ui.AddMiscTextTo(game, "Quests", name.Trim() + "_QuestName", name, MiscTextTypes.Default);
            Ui.AddMiscTextTo(game, "Quests", description.Trim() + "_QuestDescription", description, MiscTextTypes.Default);

            newQuestDescription.sQuestNameReference = name.Trim() + "_QuestName";
            newQuestDescription.sSummaryReference = description.Trim() + "_QuestDescription";
            newQuestDescription.iIntendedLevel = 0;
            newQuestDescription.enType = (dynamic)Enum.ToObject(Utils.GetGameType("Quests.QuestDescription+QuestType"), type);
            newQuestDescription.xReward = reward.Original;

            newQuest.xDescription = newQuestDescription;
            newQuest.xReward = newQuestDescription.xReward;

            newQuest.enQuestID = (dynamic)Enum.ToObject(Utils.GetGameType("Quests.QuestCodex+QuestID"),(ushort)(4400+1));


            var addToObjectives = Utils.GetGameType("Quests.Quest").GetField("lxObjectives").FieldType.GetMethod("Add");
            var addToObjectiveGroup = Utils.GetGameType("Quests.Quest").GetField("liObjectiveGroups").FieldType.GetMethod("Add");
            var addToQuestLog = Utils.GetGameType("Quests.QuestLog").GetField("lxActiveQuests").FieldType
                .GetMethod("Add");


            foreach (var objective in questObjectives)
            {
                addToObjectives.Invoke(newQuest.lxObjectives, new []{ (object)objective.Original});
            }

            addToObjectiveGroup.Invoke(newQuest.liObjectiveGroups, new[] { (object)questObjectives.Count()});

            addToQuestLog.Invoke(player.Original.xJournalInfo.xQuestLog.lxActiveQuests, new[] {(object) newQuest});

            Console.WriteLine("Added the Quest " + name + "...");

            return new Quest(newQuest);
        }
    }


}
