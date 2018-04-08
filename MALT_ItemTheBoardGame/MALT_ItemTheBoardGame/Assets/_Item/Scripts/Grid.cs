using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AppAdvisory.Item {

	public class Grid {

	    public Cell[][] grid;

	    private float distanceBetweenHorizontalCells;
	    private float distanceBetweenVerticalCells;

	    private int width;
	    private int height;

		private Transform parent;

		public Grid(int width, int height, Transform parent, float distanceBetweenHorizontalCells = 1, float distanceBetweenVerticalCells = 0.5f)
	    {
	        this.width = width;
	        this.height = height;
	        this.distanceBetweenHorizontalCells = distanceBetweenHorizontalCells;
	        this.distanceBetweenVerticalCells = distanceBetweenVerticalCells;
			this.parent = parent;
	        InitGrid();
	        DisplayCells();
	    }


		public Cell GetRandomEmptyCell() {
			List<Cell> emptyCells = new List<Cell> ();

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < grid[y].Length; x++)
				{
					if (grid [y] [x]) {
						if (!grid [y] [x].HasBall ()) {
							emptyCells.Add (grid [y] [x]);
						}
					}
				}
			}  
				
			return emptyCells [Random.Range (0, emptyCells.Count - 1)];
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

		public Cell GetCellFromDirection(Cell firstCell, Cell secondCell) {
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

	    private void InitGrid()
	    {
	        int centerX = Mathf.FloorToInt(width / 2);
	        int centerY = Mathf.FloorToInt(height / 2);

	        int actualWidth;
	        int count = 0;
	        grid = new Cell[height][];
	        for (int y = 0; y < height; y++)
	        {
	            actualWidth = GetActualWidth(y);
	            grid[y] = new Cell[actualWidth];
	            for(int x = 0; x < actualWidth; x++)
	            {
	                if (x == centerX && y == centerY)
	                    continue;

	                grid[y][x] = Object.Instantiate(Resources.Load<Cell>("Cell"));
	                grid[y][x].SetModelPosition(x, y);
					grid[y][x].name = "Cell" + x + "_" + y;
					grid[y][x].transform.SetParent (parent);
	                count++;
	            }
	        }
	    }




	    private void DisplayCells()
	    {
	        
	        for (int y = 0; y < height; y++)
	        {
	            for (int x = 0; x < grid[y].Length; x++)
	            {
	                if(grid[y][x])
	                    grid[y][x].transform.localPosition = new Vector3(x * distanceBetweenHorizontalCells + (y % 2) * distanceBetweenHorizontalCells / 2, y * distanceBetweenVerticalCells, 0);
	            }
	        }  
	    }
	}

}
