using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInRange(OverlayTile startingTile, int movePoints, bool forAttack)
    {
        List<OverlayTile> inRangeTile = new List<OverlayTile>();
        int stepCount = 0;

        inRangeTile.Add(startingTile);

        List<OverlayTile> tileForPreviousTiles = new List<OverlayTile>{ startingTile };

        while (stepCount < movePoints)
        {
            List<OverlayTile> surrendingTiles = new List<OverlayTile>();

            foreach (OverlayTile tile in tileForPreviousTiles) 
            {
            surrendingTiles.AddRange(MapManager.Instance.GetNeighbourTiles(tile, new List<OverlayTile>(), forAttack));           
            }
            inRangeTile.AddRange(surrendingTiles);
            tileForPreviousTiles = surrendingTiles.Distinct().ToList();
            stepCount++;
        }
        return inRangeTile.Distinct().ToList();
    }
}
