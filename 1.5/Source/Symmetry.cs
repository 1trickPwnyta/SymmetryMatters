using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace SymmetryMatters
{
    public class Symmetry : GameComponent
    {
        private readonly Dictionary<int, HashSet<Room>> roomRecalcTick = new Dictionary<int, HashSet<Room>>();

        public Symmetry(Game _) { }

        public static void ForceRoomRecalcAt(Room room, int tick)
        {
            Dictionary<int, HashSet<Room>> ticks = Current.Game.GetComponent<Symmetry>().roomRecalcTick;
            if (!ticks.ContainsKey(tick))
            {
                ticks[tick] = new HashSet<Room>();
            }
            ticks[tick].Add(room);
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            int tick = Find.TickManager.TicksGame;
            if (roomRecalcTick.ContainsKey(tick))
            {
                foreach (Room room in roomRecalcTick[tick])
                {
                    DefMap<RoomStatDef, float> stats = typeof(Room).Field("stats").GetValue(room) as DefMap<RoomStatDef, float>;
                    RoomStatDef symmetryStat = DefDatabase<RoomStatDef>.GetNamed("Symmetry");
                    stats[symmetryStat] = symmetryStat.Worker.GetScore(room);
                    RoomStatDef impressivenessStat = DefDatabase<RoomStatDef>.GetNamed("Impressiveness");
                    stats[impressivenessStat] = impressivenessStat.Worker.GetScore(room);
                }
                roomRecalcTick.Remove(tick);
            }
        }
    }
}
