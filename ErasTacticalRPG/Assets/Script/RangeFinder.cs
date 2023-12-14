using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInRange(OverlayTile startingTile, int range)
    {
        var inRangeTile = new List<OverlayTile>();
        int stepCount = 0;

        inRangeTile.Add(startingTile);

        var tileForPreviousTiles = new List<OverlayTile>{ startingTile };

        while (stepCount < range)
        {
            var surrendingTiles = new List<OverlayTile>();

            foreach (var tile in tileForPreviousTiles) 
            {
            surrendingTiles.AddRange(MapManager.Instance.GetNeighbourTiles(tile, new List<OverlayTile>()));           
            }

            inRangeTile.AddRange(surrendingTiles);
            tileForPreviousTiles = surrendingTiles.Distinct().ToList();
            stepCount++;
        }
        return inRangeTile.Distinct().ToList();
    }
}
