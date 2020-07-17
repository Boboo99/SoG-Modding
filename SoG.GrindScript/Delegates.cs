﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoG.GrindScript
{
    public delegate void OnDrawPrototype();

    public delegate void OnPlayerTakeDamagePrototype(ref int damage, ref byte type);

    public delegate void OnPlayerKilledPrototype();

    public delegate void OnEnemyDamagedPrototype(Enemy enemy, ref int damage, ref byte type);

    public delegate void OnNPCDamagedPrototype(NPC npc, ref int damage, ref byte type);

    public delegate void OnNPCInteractionPrototype(NPC npc);






}
