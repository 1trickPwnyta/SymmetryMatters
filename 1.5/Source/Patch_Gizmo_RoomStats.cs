using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace SymmetryMatters
{
    [HarmonyPatch(typeof(Gizmo_RoomStats))]
    [HarmonyPatch(nameof(Gizmo_RoomStats.GizmoOnGUI))]
    public static class Patch_Gizmo_RoomStats
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 7f)
                {
                    instruction.operand = 0f;
                }

                yield return instruction;
            }
        }
    }
}
