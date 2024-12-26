using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SymmetryMatters
{
    public class RoomStatWorker_Symmetry : RoomStatWorker
    {
        public override float GetScore(Room room)
        {
            CellRect rect = CellRect.FromCellList(room.BorderCells);

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
            return Mathf.Lerp(lowestScore, highestScore, 0.9f);
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
                inRoomScores.Add(inRoom1 == inRoom2 ? 5f : -25f);

                if (inRoom1 && inRoom2)
                {
                    Building building1 = cell1.GetEdifice(room.Map);
                    Building building2 = cell2.GetEdifice(room.Map);
                    spaceScores.Add((building1 == null) == (building2 == null) ? 2f : -10f);

                    float buildingScore = building1?.def == building2?.def ? 1.5f : -7.5f;
                    buildingScore += building1?.Stuff == building2?.Stuff ? 0.5f : -2.5f;
                    buildingScores.Add(buildingScore);

                    if (!IsWallOrRock(building1) && !IsWallOrRock(building2))
                    {
                        TerrainDef floor1 = cell1.GetTerrain(room.Map);
                        TerrainDef floor2 = cell2.GetTerrain(room.Map);
                        floorScores.Add(floor1 == floor2 ? 1f : -5f);
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
