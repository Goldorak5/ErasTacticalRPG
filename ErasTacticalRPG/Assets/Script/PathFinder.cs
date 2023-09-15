using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder
{
  
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> limitList)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closeList = new List<OverlayTile>();

        openList.Add(start);

        while (openList.Count > 0)
        {

            //takes the smaller F of the neighbor tiles that mean the tiles that cost the less to move
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentOverlayTile);
            closeList.Add(currentOverlayTile);

            if (currentOverlayTile == end)
            {
            //finalize Path
            return GetFinishList(start, end);
            }

            var neighbourTiles = MapManager.Instance.GetNeighbourTiles(currentOverlayTile, limitList);

            foreach (var neighbour in neighbourTiles) 
            {
                
            if (neighbour.isBlocked || closeList.Contains(neighbour))
                   {
                    continue;
                   }
                // G = distance from starting node and where you are
                neighbour.G = GetManhattenDistance(start, neighbour); 

                //H = remaining distance between the end and where your are 
                neighbour.H = GetManhattenDistance(end, neighbour);

                //previousTile assignment
                neighbour.previousTile = currentOverlayTile;

                if(!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }
        return new List<OverlayTile>();
    }

    private int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

    private List<OverlayTile> GetFinishList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishList = new List<OverlayTile>();

        OverlayTile currentTile = end;

        while (currentTile != start) 
        {
            finishList.Add(currentTile);
            currentTile = currentTile.previousTile;
        }

        finishList.Reverse();

        return finishList;
    }

    
}
