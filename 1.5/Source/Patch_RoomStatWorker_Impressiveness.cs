using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace SymmetryMatters
{
    [HarmonyPatch(typeof(RoomStatWorker_Impressiveness))]
    [HarmonyPatch(nameof(RoomStatWorker_Impressiveness.GetScore))]
    public static class Patch_RoomStatWorker_Impressiveness
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            bool calculatedSymmetry = false;
            bool foundAverage = false;

            LocalBuilder symmetryFactorLocal = il.DeclareLocal(typeof(float));

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Stloc_2)
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(PatchUtility_RoomStatWorker_Impressiveness).Method(nameof(PatchUtility_RoomStatWorker_Impressiveness.GetSymmetryStat)));
                    yield return new CodeInstruction(OpCodes.Call, typeof(RoomStatWorker_Impressiveness).Method("GetFactor"));
                    yield return new CodeInstruction(OpCodes.Stloc_S, symmetryFactorLocal);
                    calculatedSymmetry = true;
                    continue;
                }

                if (calculatedSymmetry && !foundAverage && instruction.opcode == OpCodes.Ldc_R4)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, symmetryFactorLocal);
                    yield return new CodeInstruction(OpCodes.Add);
                    instruction.operand = (float)instruction.operand + 1f;
                    foundAverage = true;
                }

                if (foundAverage && instruction.opcode == OpCodes.Ldloc_2)
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldloc_S, symmetryFactorLocal);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Mathf).Method(nameof(Mathf.Min), new[] { typeof(float), typeof(float) }));
                    continue;
                }

                yield return instruction;
            }
        }
    }

    public static class PatchUtility_RoomStatWorker_Impressiveness
    {
        public static float GetSymmetryStat(Room room)
        {
            return room.GetStat(DefDatabase<RoomStatDef>.GetNamed("Symmetry")) * SymmetryMattersSettings.RoomImpressivenessEffect;
        }
    }
}
