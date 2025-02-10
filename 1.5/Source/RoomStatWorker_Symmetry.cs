using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SymmetryMatters
{
    public class RoomStatWorker_Symmetry : RoomStatWorker
    {
        private static Dictionary<Room, Tuple<float, int>> cache = new Dictionary<Room, Tuple<float, int>>();

        public override float GetScore(Room room)
        {
            CellRect rect = CellRect.FromCellList(room.BorderCells);
            bool cachingEnabled = SymmetryMattersSettings.LargeRoomCacheExpiryTicks > 0 && rect.Area > 625;

            if (cachingEnabled)
            {
                Tuple<float, int> cacheValue = cache.ContainsKey(room) ? cache[room] : null;
                if (cacheValue != null && cacheValue.Item2 > Find.TickManager.TicksGame)
                {
                    Symmetry.ForceRoomRecalcAt(room, cacheValue.Item2);
                    return cacheValue.Item1;
                }
            }

            float centerZ = (rect.minZ + rect.maxZ) / 2f;
            List<IntVec3> top = rect.Cells.Where(i => i.z < centerZ).OrderBy(c => c.z).ThenBy(c => c.x).ToList();
            List<IntVec3> bottom = rect.Cells.Where(i => i.z > centerZ).OrderByDescending(c => c.z).ThenBy(c => c.x).ToList();
            float horizontalScore = GetScore(top, bottom, room);

            float centerX = (rect.minX + rect.maxX) / 2f;
            List<IntVec3> left = rect.Cells.Where(i => i.x < centerX).OrderBy(c => c.x).ThenBy(c => c.z).ToList();
            List<IntVec3> right = rect.Cells.Where(i => i.x > centerX).OrderByDescending(c => c.x).ThenBy(c => c.z).ToList();
            float verticalScore = GetScore(left, right, room);

            float highestScore = Mathf.Max(horizontalScore, verticalScore);
            float lowestScore = Mathf.Min(horizontalScore, verticalScore);
            float score = Mathf.Lerp(lowestScore, highestScore, 0.9f);
            if (cachingEnabled)
            {
                cache[room] = new Tuple<float, int>(score, Find.TickManager.TicksGame + 600);
            }
            return score;
        }

        private float GetScore(List<IntVec3> side1, List<IntVec3> side2, Room room)
        {
            List<float> inRoomScores = new List<float>();
            List<float> spaceScores = new List<float>();
            List<float> buildingScores = new List<float>();
            List<float> floorScores = new List<float>();

            IEnumerable<IntVec3> allCells = room.Cells.Union(room.BorderCells.Where(c => IsWallOrRockOrDoor(c.GetEdifice(room.Map))));

            for (int i = 0; i < side1.Count; i++)
            {
                IntVec3 cell1 = side1[i];
                IntVec3 cell2 = side2[i];

                bool inRoom1 = allCells.Contains(cell1);
                bool inRoom2 = allCells.Contains(cell2);
                inRoomScores.Add(inRoom1 == inRoom2 ? 5f : -5f * SymmetryMattersSettings.AsymmetryPunishFactor);

                if (inRoom1 && inRoom2)
                {
                    Building building1 = cell1.GetEdifice(room.Map);
                    if (building1 != null && building1.GetStatValue(StatDefOf.WorkToBuild) == 0f)
                    {
                        building1 = null;
                    }
                    Building building2 = cell2.GetEdifice(room.Map);
                    if (building2 != null && building2.GetStatValue(StatDefOf.WorkToBuild) == 0f)
                    {
                        building2 = null;
                    }
                    spaceScores.Add((building1 == null) == (building2 == null) ? 2f : -2f * SymmetryMattersSettings.AsymmetryPunishFactor);

                    float buildingScore = building1?.def == building2?.def ? 1.5f : -1.5f * SymmetryMattersSettings.AsymmetryPunishFactor;
                    if (building1?.PaintColorDef != null && building2?.PaintColorDef != null)
                    {
                        buildingScore += building1.PaintColorDef == building2.PaintColorDef ? 0.5f : -0.5f * SymmetryMattersSettings.AsymmetryPunishFactor;
                    }
                    else
                    {
                        buildingScore += building1?.Stuff == building2?.Stuff ? 0.5f : -0.5f * SymmetryMattersSettings.AsymmetryPunishFactor;
                    }
                    buildingScores.Add(buildingScore);

                    if (!IsWallOrRock(building1) && !IsWallOrRock(building2))
                    {
                        TerrainDef floor1 = cell1.GetTerrain(room.Map);
                        TerrainDef floor2 = cell2.GetTerrain(room.Map);
                        float floorScore = floor1 == floor2 ? 0.8f : -0.8f * SymmetryMattersSettings.AsymmetryPunishFactor;
                        ColorDef floorColor1 = room.Map.terrainGrid.ColorAt(cell1);
                        ColorDef floorColor2 = room.Map.terrainGrid.ColorAt(cell2);
                        floorScore += floorColor1 == floorColor2 ? 0.2f : -0.2f * SymmetryMattersSettings.AsymmetryPunishFactor;
                        floorScores.Add(floorScore);
                    }
                }
            }

            float inRoomAverage = inRoomScores.Empty() ? 0f : inRoomScores.Average();
            float spaceAverage = spaceScores.Empty() ? 0f : spaceScores.Average();
            float buildingAverage = buildingScores.Empty() ? 0f : buildingScores.Average();
            float floorAverage = floorScores.Empty() ? 0f : floorScores.Average();
            return inRoomAverage + spaceAverage + buildingAverage + floorAverage;
        }

        private bool IsWallOrRock(Building building)
        {
            return building != null && (building.def == ThingDefOf.Wall || building.def.building.isNaturalRock);
        }

        private bool IsWallOrRockOrDoor(Building building)
        {
            return building != null && (IsWallOrRock(building) || building is Building_Door);
        }
    }
}
