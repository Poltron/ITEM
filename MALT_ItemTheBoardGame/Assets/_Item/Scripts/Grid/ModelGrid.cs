using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelGrid {
    public Cell[][] grid;

    private float distanceBetweenHorizontalCells;
    private float distanceBetweenVerticalCells;

    private int width;
    private int height;

	private Transform parent;

	public ModelGrid(int width, int height, List<Cell> cells)
    {
        this.width = width;
        this.height = height;

        InitGridWithCells(cells);
       }

       public void Reset()
       {
           for (int i = 0; i < grid.Length; ++i)
           {
               for (int j = 0; j < grid[i].Length; ++j)
               {
                   if (grid[i][j] && grid[i][j].ball)
                       grid[i][j].ball = null;
               }
           }

           ResetCellsColor();
       }

       public void ResetCellsColor()
       {
           for (int i = 0; i < grid.Length; ++i)
           {
               for (int j = 0; j < grid[i].Length; j++)
               {
                   if (grid[i][j])
                       grid[i][j].SetHighlightedCell(false);
               }
           }
       }

       public List<Cell> GetAllCellsWithColor(BallColor color = BallColor.Black) {

		List<Cell> colorCells = new List<Cell> ();

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < grid[y].Length; x++)
			{
				if (grid [y] [x]) {
					if (grid [y] [x].HasBall (color)) {
						colorCells.Add (grid [y] [x]);
					}
				}
			}
		}  
		return colorCells;
	}

	public Cell GetCellFromDirection(Cell firstCell, Cell secondCell)
       {
		Cell destinationCell;

		//middle up
		if(firstCell.x  == secondCell.x && (firstCell.y + 2) == secondCell.y) 
		{
			destinationCell = GetCellFromModel (secondCell.x, secondCell.y + 2);
			if (destinationCell)
				return destinationCell;
		}

		//middle down
		if(firstCell.x  == secondCell.x && (firstCell.y - 2) == secondCell.y) 
		{
			destinationCell = GetCellFromModel (secondCell.x, secondCell.y - 2);
			if (destinationCell)
				return destinationCell;
		}

		//middle left
		if(firstCell.x - 1  == secondCell.x && firstCell.y == secondCell.y) 
		{
			destinationCell = GetCellFromModel (secondCell.x - 1, secondCell.y);
			if (destinationCell)
				return destinationCell;
		}

		//middle right
		if(firstCell.x + 1  == secondCell.x && firstCell.y == secondCell.y) 
		{
			destinationCell = GetCellFromModel (secondCell.x + 1, secondCell.y);
			if (destinationCell)
				return destinationCell;
		}


		int offsetFirstCell = firstCell.y % 2;
		int offsetSecondtCell = secondCell.y % 2;


		//top left
		if((firstCell.x - 1 + offsetFirstCell) == secondCell.x && (firstCell.y + 1) == secondCell.y) 
		{
			destinationCell = GetCellFromModel (secondCell.x - 1 + offsetSecondtCell, secondCell.y + 1);
			if (destinationCell)
				return destinationCell;
		}

		//top right
		if((firstCell.x + offsetFirstCell) == secondCell.x && (firstCell.y + 1) == secondCell.y) {
			destinationCell = GetCellFromModel (secondCell.x + offsetSecondtCell, secondCell.y + 1);
			if (destinationCell)
				return destinationCell;
		}

		//bottom left
		if((firstCell.x - 1 + offsetFirstCell) == secondCell.x && (firstCell.y - 1) == secondCell.y) 
		{
			destinationCell = GetCellFromModel (secondCell.x - 1 + offsetSecondtCell, secondCell.y - 1);
			if (destinationCell)
				return destinationCell;
		}

		//bottom right
		if((firstCell.x + offsetFirstCell) == secondCell.x && (firstCell.y - 1) == secondCell.y) {
			destinationCell = GetCellFromModel (secondCell.x + offsetSecondtCell, secondCell.y - 1);
			if (destinationCell)
				return destinationCell;
		}

		return null;
	}

	public Cell GetTopLeftCell(Cell cell) {
		int offset = cell.y % 2;
		return GetCellFromModel(cell.x - 1 + offset, cell.y+1);
	}

	public Cell GetTopRightCell(Cell cell) {
		int offset = cell.y % 2;
		return GetCellFromModel(cell.x + offset, cell.y+1);
	}

	public Cell GetBottomLeftCell(Cell cell) {
		int offset = cell.y % 2;
		return GetCellFromModel(cell.x - 1 + offset, cell.y-1);
	}

	public Cell GetBottomRightCell(Cell cell) {
		int offset = cell.y % 2;
		return GetCellFromModel(cell.x + offset, cell.y-1);
	}


	public List<Cell> GetCellNeighbours(Cell cell) {
		List<Cell> neighbours = new List<Cell> ();

		Cell neighbour;

		//middle up
		neighbour = GetCellFromModel(cell.x, cell.y + 2);

		if (neighbour)
			neighbours.Add (neighbour);

		//middle down
		neighbour = GetCellFromModel(cell.x, cell.y - 2);

		if (neighbour)
			neighbours.Add (neighbour);

		//middle left
		neighbour = GetCellFromModel(cell.x -1, cell.y);

		if (neighbour)
			neighbours.Add (neighbour);

		//middle right
		neighbour = GetCellFromModel(cell.x + 1, cell.y);
		if (neighbour)
			neighbours.Add (neighbour);

		int offset = cell.y % 2;

		//top left
		neighbour = GetCellFromModel(cell.x - 1 + offset, cell.y+1);
		if (neighbour)
			neighbours.Add (neighbour);

		//top right
		neighbour = GetCellFromModel(cell.x + offset, cell.y+1);
		if (neighbour)
			neighbours.Add (neighbour);

		//bottom left
		neighbour = GetCellFromModel(cell.x - 1 + offset, cell.y-1);
		if (neighbour)
			neighbours.Add (neighbour);

		//bottom right
		neighbour = GetCellFromModel(cell.x + offset, cell.y-1);
		if (neighbour)
			neighbours.Add (neighbour);
           
		return neighbours;
	}

    public Cell GetCellFromModel(int x, int y)
    {
        if (x < 0 || x >= GetActualWidth(y) || y < 0 || y >= height)
            return null;

        return grid[y][x];
    }

	public Cell GetCellFromModel(Vector2 pos) {
		return GetCellFromModel ((int)pos.x, (int)pos.y);
	}

    private int GetActualWidth(int y)
    {
        return width - y % 2;
    }

       private void InitGridWithCells(List<Cell> cells)
       {
           grid = new Cell[height][];

           // switch between 4 and 5 balls row
           bool isLargeRow = true;
           for (int i = 0; i < height; i++)
           {
               if (isLargeRow)
                   grid[i] = new Cell[5];
               else
                   grid[i] = new Cell[4];

               isLargeRow = !isLargeRow;
           }

           for (int i = 0; i  < cells.Count; ++i)
           {

               grid[cells[i].y][cells[i].x] = cells[i];
           }
       }
}
