using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoG.GrindScript
{
    public class CustomItem : ConvertedObject
    {
        public int Id { get; set; }

        public int EnType
        {
            get => (int)_originalObject.enType;
            set => _originalObject.enType = Enum.ToObject(Utils.GetGameType("SoG.ItemCodex+ItemTypes"), value);
        }

        public bool IsVanillaItem 
            => Enum.IsDefined(Utils.GetGameType("SoG.ItemCodex+ItemTypes"), EnType);

        public CustomItem(object originalObject) : base(originalObject)
        {
            
        }

        public static bool ValueIsVanillaItem(int enType)
        {
            return Enum.IsDefined(Utils.GetGameType("SoG.ItemCodex+ItemTypes"), enType);
        }

        public static CustomItem AddCustomItemTo(LocalGame game, string name, string description, string appearance, int value)
        {
            int newId = 1;
            if (BaseScript.CustomItems.Count > 0)
                newId = BaseScript.CustomItems.Count + 1;


            dynamic newItem = Utils.GetGameType("SoG.ItemDescription").GetConstructor(Type.EmptyTypes).Invoke(null);

            Ui.AddMiscTextTo(game, "Items", name.Trim() + "_Name", name, MiscTextTypes.GenericItemName);
            Ui.AddMiscTextTo(game,"Items", description.Trim() + "_Description", description, MiscTextTypes.GenericItemDescription);

            newItem.sFullName = name;
            newItem.txDisplayImage = game.GetContentManager().Load<Texture2D>(appearance);
            newItem.lenCategory.Add((dynamic)Enum.ToObject(ModCodex.SoGType.ItemCategories, 0));
            newItem.sNameLibraryHandle = name.Trim() + "_Name";
            newItem.sDescriptionLibraryHandle = description.Trim() + "_Description";
            newItem.sCategory = "Misc";
            newItem.iInternalLevel = 1;
            newItem.iValue = value;

            dynamic items = Utils.GetGameType("SoG.ItemCodex").GetPublicStaticField("denxLoadedDescriptions");


            newItem.enType = (dynamic)Enum.ToObject(Utils.GetGameType("SoG.ItemCodex+ItemTypes"), 400000 + newId);


            items[newItem.enType] = newItem;

            Console.WriteLine("Added the custom item called " + name + " with the id " + 400000 + newId + "...");

            var customItem = new CustomItem(newItem) {Id = newId};

            return customItem;
        }

        public void SpawnOn(LocalGame game,Player player)
        {
            // While the Player fix is coming, this should work for now
            dynamic playaa = ((dynamic)Utils.GetGameType("SoG.Program").GetMethod("GetTheGame")?.Invoke(null, null)).xLocalPlayer;

            var function = Utils.GetGameType("SoG.Game1").GetDeclaredMethods("_EntityMaster_AddItem").First();
            function.Invoke(game.GetUnderlayingGame(), new[] { EnType, playaa.xEntity.xTransform.v2Pos, playaa.xEntity.xRenderComponent.fVirtualHeight, playaa.xEntity.xCollisionComponent.ibitCurrentColliderLayer, Vector2.Zero });
        }

        #region GetItemInstance patch

        private static bool OnGetItemInstancePrefix(ref dynamic __result, ref int enType)
        {
            if (CustomItem.ValueIsVanillaItem(enType))
            {
                // Continue with original
                return true;
            }

            dynamic items = Utils.GetGameType("SoG.ItemCodex").GetPublicStaticField("denxLoadedDescriptions");
            dynamic enumType = Enum.ToObject(Utils.GetGameType("SoG.ItemCodex+ItemTypes"), enType);

            if (!items.ContainsKey(enumType))
            {
                // No such item? Continue with original anyway...
                return true;
            }

            // Create new instance and return it, skipping original code
            __result = Activator.CreateInstance(Utils.GetGameType("SoG.Item"));

            // The "Regional" Content Manager is used here. Default shadow seems to be the rabby feet's shadow
            __result.enType = enumType;
            __result.xRenderComponent.txTexture = items[enumType].txDisplayImage;
            __result.xRenderComponent.txShadowTexture = ((dynamic)Utils.GetGameType("SoG.Program").GetMethod("GetTheGame")?.Invoke(null, null)).xLevelMaster.contRegionContent.Load<Texture2D>("Items/DropAppearance/hartass02");
            __result.sFullName = items[enumType].sFullName;

            if (items[enumType].lenCategory.Contains((dynamic)Enum.ToObject(Utils.GetGameType("SoG.ItemCodex+ItemCategories"), (int)ItemCategories.GrantToServer)))
            {
                __result.bGiveToServer = true;
            }
            __result.xCollisionComponent.xMovementCollider = Activator.CreateInstance(Utils.GetGameType("SoG.SphereCollider"), 10f, Vector2.Zero, __result.xTransform, 1f, __result);
            __result.xCollisionComponent.xMovementCollider.bCollideWithFlat = true;

            // Skip original code
            return false;
        }

        #endregion
    }
}
