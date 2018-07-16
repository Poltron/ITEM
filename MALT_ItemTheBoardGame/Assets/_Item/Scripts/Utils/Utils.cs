using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;
using System;

namespace AppAdvisory.Item {

	public static class Utils {

		#region WIN
		private static int nBallToAlignVertically = 5;
		private static int nBallToAlignHorizontally = 5;



		public static bool CheckWin(ModelGrid grid, Cell cell, bool mustHighlight = true)
		{
			return (CheckWinDiagonalBottomLeftToTopRight(grid, cell, mustHighlight) ||
				CheckWinDiagonalBottomRightToTopLeft(grid, cell, mustHighlight) ||
				CheckWinHorizontal (grid, cell, mustHighlight) ||
				CheckWinVertical (grid, cell, mustHighlight) ||
				CheckWinCrossNormal (grid, cell, mustHighlight) ||
				CheckWinCrossDiagonal (grid, cell, mustHighlight));
		}

		public static bool CheckWinIA(ModelGrid grid, Cell cell) 
		{
			return (CheckWinDiagonalBottomLeftToTopRightIA(grid, cell) ||
				CheckWinDiagonalBottomRightToTopLeftIA(grid, cell) ||
				CheckWinHorizontalIA (grid, cell) ||
				CheckWinVerticalIA (grid, cell) ||
				CheckWinCrossNormalIA (grid, cell) ||
				CheckWinCrossDiagonalIA (grid, cell));
		}
			
		public static List<Cell> GetHorizontalCells(ModelGrid grid, Cell cell) 
		{
			Cell neighbour;
			List<Cell> horizontalCells = new List<Cell> ();

			for (int i = 0; i < nBallToAlignVertically; i++) {
				neighbour = grid.GetCellFromModel(i, cell.y);

				if (!neighbour)
					continue;

				horizontalCells.Add (neighbour);
			}

			return horizontalCells;
		}

		public static List<Cell> GetVerticalCells(ModelGrid grid, Cell cell) 
		{
			Cell neighbour;
			List<Cell> verticalCells = new List<Cell> ();

			if (cell.y % 2 == 1)
				return verticalCells;

			for (int i = 0; i < nBallToAlignVertically; i++) {
				neighbour = grid.GetCellFromModel (cell.x, i * 2);

				if (!neighbour)
					continue;

				verticalCells.Add (neighbour);
			}

			return verticalCells;
		}

		public static List<Cell> GetCrossNormalCells(ModelGrid grid, Cell cell) 
		{
			Cell neighbour;
			List<Cell> crossNormalCells = new List<Cell> ();

			if (!cell)
				return crossNormalCells;

			crossNormalCells.Add (cell);

			//left
			neighbour = grid.GetCellFromModel(cell.x-1, cell.y);
			if (neighbour) {
				crossNormalCells.Add (neighbour);
			}

			//right
			neighbour = grid.GetCellFromModel(cell.x + 1, cell.y);
			if (neighbour) {
				crossNormalCells.Add (neighbour);
			}

			//up
			neighbour = grid.GetCellFromModel(cell.x, cell.y + 2);
			if (neighbour) {
				crossNormalCells.Add (neighbour);
			}

			//down
			neighbour = grid.GetCellFromModel(cell.x, cell.y - 2);
			if (neighbour) {
				crossNormalCells.Add (neighbour);
			}
			return crossNormalCells;
		}

		public static List<Cell> GetCrossDiagonalCells(ModelGrid grid, Cell cell) 
		{
			Cell neighbour;
			List<Cell> crossDiagonal = new List<Cell> ();

			if (!cell)
				return crossDiagonal;

			int offset = cell.y % 2; 
		
			crossDiagonal.Add (cell);


			neighbour = grid.GetCellFromModel(cell.x-1 + offset, cell.y+1);
			if (neighbour)
				crossDiagonal.Add (neighbour);

			//top right
			neighbour = grid.GetCellFromModel(cell.x + offset, cell.y+1);
			if (neighbour)
				crossDiagonal.Add (neighbour);
			
			//bottom left
			neighbour = grid.GetCellFromModel(cell.x-1 + offset, cell.y - 1);
			if (neighbour)
				crossDiagonal.Add (neighbour);

			//bottom right
			neighbour = grid.GetCellFromModel(cell.x + offset, cell.y - 1);

			if (neighbour)
				crossDiagonal.Add (neighbour);

			return crossDiagonal;
		}

		public static List<Cell> GetDiagonalBottomRightToTopLeftCells(ModelGrid grid, Cell cell) {
			Cell neighbour;
			List<Cell> diagonalCells = new List<Cell> ();

			diagonalCells.Add (cell);

			int offset = cell.y % 2;

			if (offset == 0) {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - Mathf.CeilToInt(i /2f), cell.y+i);

					if (!neighbour)
						continue;

					diagonalCells.Add (neighbour);
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + i / 2, cell.y-i);

					if (!neighbour)
						continue;

					diagonalCells.Add (neighbour);
				}


			} else {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - i/2, cell.y+i);

					if (!neighbour)
						continue;

					diagonalCells.Add (neighbour);
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + Mathf.CeilToInt(i /2f), cell.y-i);

					if (!neighbour)
						continue;

					diagonalCells.Add (neighbour);
				}

			}
			return diagonalCells;
		}


		public static List<Cell> GetDiagonalBottomLeftToTopRightCells(ModelGrid grid, Cell cell) {
			Cell neighbour;
			List<Cell> diagonalCells = new List<Cell> ();
			int offset = cell.y % 2;

			diagonalCells.Add (cell);

			if (offset == 0) {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + i /2, cell.y+i);

					if (!neighbour)
						continue;

					diagonalCells.Add (neighbour);
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - Mathf.CeilToInt(i /2f), cell.y-i);

					if (!neighbour)
						continue;

					diagonalCells.Add (neighbour);
				}


			} else {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + Mathf.CeilToInt(i /2f), cell.y+i);

					if (!neighbour)
						continue;

					diagonalCells.Add (neighbour);
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - i /2, cell.y-i);

					if (!neighbour)
						continue;

					diagonalCells.Add (neighbour);
				}

			}
			return diagonalCells;
		}

		public static int GetCellToWinCountDiagonal(ModelGrid grid, List<Cell> cells, BallColor color = BallColor.Black) {
			int count = 0;

			if (cells.Count < 5)
				return -1;

			Cell cell;
			for (int i = 0; i < cells.Count; i++) {
				cell = cells [i];
				if (cell.HasBall (color)) {
					count++;
				} 
//				if (cell.HasBall(BallColor.White)) {
//					return -1;
//				}
//
//				if(cell.HasBall(BallColor.Black)) {
//					count++;
//				}
			}
			return count;
		}

		public static int GetCellToWinCount(ModelGrid grid, List<Cell> cells, BallColor color = BallColor.Black) {
			int count = 0;

			if (cells.Count < 5)
				return -1;

			Cell cell;
			for (int i = 0; i < cells.Count; i++) {
				cell = cells [i];
				if (cell.HasBall (color)) {
					count++;
				} else if(cell.HasBall()) {
					return -1;
				}

				//				if (cell.HasBall(BallColor.White)) {
				//					return -1;
				//				}
				//
				//				if(cell.HasBall(BallColor.Black)) {
				//					count++;
				//				}
			}
			return count;
		}


		private static bool CheckWinHorizontal(ModelGrid grid, Cell cell, bool mustHighlight = true)
		{
			Cell neighbour;

			BallColor color;
			if (!cell.ball)
				color = BallColor.White;
			else 
				color = cell.ball.Color;
			
			int count = 1;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);


			// left
			for(int i = 1; i < nBallToAlignHorizontally; i++)
			{
				neighbour = grid.GetCellFromModel(cell.x - i, cell.y);

				if (!neighbour)
					break;

				if (neighbour.HasBall (color)) {
					patternCells.Add (neighbour);
					count++;
				}
				else
					break;
			}

			//right
			for(int i = 1; i < nBallToAlignHorizontally; i++)
			{
				neighbour = grid.GetCellFromModel(cell.x + i, cell.y);

				if (!neighbour)
					break;

				if (neighbour.HasBall (color)) {
					patternCells.Add (neighbour);
					count++;
				}
				else
					break;
			}

			/*if(count >= nBallToAlignHorizontally && mustHighlight)
				HighlighCells (patternCells, Color.red);*/

			return count >= nBallToAlignHorizontally;
		}
			

		private static bool CheckWinDiagonalBottomLeftToTopRight(ModelGrid grid, Cell cell, bool mustHighlight = true) {
			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.White;
			else 
				color = cell.ball.Color;
			int count = 1;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);


			int offset = cell.y % 2;

			if (offset == 0) {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + i /2, cell.y+i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					} else
						break;
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - Mathf.CeilToInt(i /2f), cell.y-i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					} else
						break;
				}


			} else {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + Mathf.CeilToInt(i /2f), cell.y+i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					} else
						break;
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - i /2, cell.y-i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					} else
						break;
				}

			}
				
			/*if(count >= nBallToAlignVertically && mustHighlight)
				HighlighCells (patternCells, Color.red);*/


			return count >= nBallToAlignVertically;
		}
			
		private static bool CheckWinDiagonalBottomLeftToTopRightIA(ModelGrid grid, Cell cell) {
			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.Black;
			else 
				color = cell.ball.Color;
			int count = 1;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);


			int offset = cell.y % 2;

			if (offset == 0) {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + i /2, cell.y+i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					} else
						break;
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - Mathf.CeilToInt(i /2f), cell.y-i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					} else
						break;
				}


			} else {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + Mathf.CeilToInt(i /2f), cell.y+i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					} else
						break;
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - i /2, cell.y-i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					} else
						break;
				}

			}

			/*if(count >= nBallToAlignVertically)
				HighlighCells (patternCells, Color.red);*/


			return count >= nBallToAlignVertically;
		}


		private static bool CheckWinDiagonalBottomRightToTopLeft(ModelGrid grid, Cell cell, bool mustHighlight = true) {
			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.White;
			else 
				color = cell.ball.Color;
			int count = 1;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);

			int offset = cell.y % 2;

			if (offset == 0) {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - Mathf.CeilToInt(i /2f), cell.y+i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					}
					else
						break;
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + i / 2, cell.y-i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					}
					else
						break;
				}


			} else {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - i/2, cell.y+i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					}
					else
						break;
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + Mathf.CeilToInt(i /2f), cell.y-i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					}
					else
						break;
				}

			}

			/*if(count >= nBallToAlignHorizontally && mustHighlight)
				HighlighCells (patternCells, Color.red);*/

			return count >= nBallToAlignHorizontally;

		}

		private static bool CheckWinDiagonalBottomRightToTopLeftIA(ModelGrid grid, Cell cell) {
			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.Black;
			else 
				color = cell.ball.Color;
			int count = 1;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);

			int offset = cell.y % 2;

			if (offset == 0) {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - Mathf.CeilToInt(i /2f), cell.y+i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					}
					else
						break;
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + i / 2, cell.y-i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					}
					else
						break;
				}


			} else {
				//top
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x - i/2, cell.y+i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					}
					else
						break;
				}

				//bottom
				for (int i = 1; i < nBallToAlignVertically; i++) {
					neighbour = grid.GetCellFromModel(cell.x + Mathf.CeilToInt(i /2f), cell.y-i);

					if (!neighbour)
						break;

					if (neighbour.HasBall (color)) {
						patternCells.Add (neighbour);
						count++;
					}
					else
						break;
				}

			}

			/*if(count >= nBallToAlignHorizontally)
				HighlighCells (patternCells, Color.red);*/

			return count >= nBallToAlignHorizontally;

		}

		private static bool CheckWinVertical(ModelGrid grid, Cell cell, bool mustHighlight = true)
		{
			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.White;
			else 
				color = cell.ball.Color;
			int count = 1;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);

			// down
			for (int i = 1; i < nBallToAlignVertically; i++)
			{
				neighbour = grid.GetCellFromModel(cell.x, cell.y - i * 2);

				if (!neighbour)
					break;

				if (neighbour.HasBall (color)) {
					count++;
					patternCells.Add (neighbour);
				}

				else
					break;
			}

			//up
			for (int i = 1; i < nBallToAlignVertically; i ++)
			{
				neighbour = grid.GetCellFromModel(cell.x, cell.y + i * 2);

				if (!neighbour)
					break;

				if (neighbour.HasBall (color)) {
					count++;
					patternCells.Add (neighbour);
				}

				else
					break;
			}

			/*if(count >= nBallToAlignVertically && mustHighlight)
				HighlighCells (patternCells, Color.red);*/


			return count >= nBallToAlignVertically;
		}
			
		private static bool CheckWinHorizontalIA(ModelGrid grid, Cell cell) {
			Cell neighbour;

			BallColor color = BallColor.Black;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);
			int count = 1;

			// left
			for(int i = 1; i < nBallToAlignHorizontally; i++)
			{
				neighbour = grid.GetCellFromModel(cell.x - i, cell.y);

				if (!neighbour)
					break;

				if (neighbour.HasBall (color)) {
					patternCells.Add (neighbour);
					count++;
				}
				else
					break;
			}

			//right
			for(int i = 1; i < nBallToAlignHorizontally; i++)
			{
				neighbour = grid.GetCellFromModel(cell.x + i, cell.y);

				if (!neighbour)
					break;

				if (neighbour.HasBall (color)) {
					patternCells.Add (neighbour);
					count++;
				}
				else
					break;
			}

			/*if(count >= nBallToAlignHorizontally)
				HighlighCells (patternCells, Color.red);*/

			return count >= nBallToAlignHorizontally;
		}
			
		private static bool CheckWinVerticalIA(ModelGrid grid, Cell cell)
		{
			Cell neighbour;
			BallColor color = BallColor.Black;
			int count = 1;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);

			// down
			for (int i = 1; i < nBallToAlignVertically; i++)
			{
				neighbour = grid.GetCellFromModel(cell.x, cell.y - i * 2);

				if (!neighbour)
					break;

				if (neighbour.HasBall (color)) {
					count++;
					patternCells.Add (neighbour);
				}

				else
					break;
			}

			//up
			for (int i = 1; i < nBallToAlignVertically; i ++)
			{
				neighbour = grid.GetCellFromModel(cell.x, cell.y + i * 2);

				if (!neighbour)
					break;

				if (neighbour.HasBall (color)) {
					count++;
					patternCells.Add (neighbour);
				}

				else
					break;
			}

			/*if(count >= nBallToAlignVertically)
				HighlighCells (patternCells, Color.red);*/


			return count >= nBallToAlignVertically;
		}

		private static bool CheckWinCrossNormalIA(ModelGrid grid, Cell cell) {
			return (CheckCrossNormalIA (grid, cell) || 
				CheckCrossNormalIA (grid, grid.GetCellFromModel (cell.x - 1, cell.y)) || 
				CheckCrossNormalIA (grid, grid.GetCellFromModel (cell.x + 1, cell.y)) || 
				CheckCrossNormalIA (grid, grid.GetCellFromModel (cell.x, cell.y + 2)) ||
				CheckCrossNormalIA (grid, grid.GetCellFromModel (cell.x, cell.y - 2)));
		}

		private static bool CheckWinCrossNormal(ModelGrid grid, Cell cell,  bool mustHighlight = true) {
			return (CheckCrossNormal (grid, cell, mustHighlight) || 
				CheckCrossNormal (grid, grid.GetCellFromModel (cell.x - 1, cell.y), mustHighlight) || 
				CheckCrossNormal (grid, grid.GetCellFromModel (cell.x + 1, cell.y), mustHighlight) || 
				CheckCrossNormal (grid, grid.GetCellFromModel (cell.x, cell.y + 2), mustHighlight) ||
				CheckCrossNormal (grid, grid.GetCellFromModel (cell.x, cell.y - 2), mustHighlight));
		}

		private static bool CheckWinCrossDiagonal(ModelGrid grid, Cell cell,  bool mustHighlight = true) {
			int offset = cell.y % 2; 

			return (CheckCrossDiagonal (grid, cell, mustHighlight) || 
				CheckCrossDiagonal(grid, grid.GetCellFromModel(cell.x-1 + offset, cell.y+1), mustHighlight) || 
				CheckCrossDiagonal(grid, grid.GetCellFromModel(cell.x + offset, cell.y + 1), mustHighlight) || 
				CheckCrossDiagonal(grid, grid.GetCellFromModel(cell.x - 1 + offset, cell.y - 1), mustHighlight) ||
				CheckCrossDiagonal(grid, grid.GetCellFromModel(cell.x + offset, cell.y - 1), mustHighlight));
		}

		private static bool CheckWinCrossDiagonalIA(ModelGrid grid, Cell cell) {
			int offset = cell.y % 2; 

			return (CheckCrossDiagonalIA (grid, cell) || 
				CheckCrossDiagonalIA(grid, grid.GetCellFromModel(cell.x-1 + offset, cell.y+1)) || 
				CheckCrossDiagonalIA(grid, grid.GetCellFromModel(cell.x + offset, cell.y + 1)) || 
				CheckCrossDiagonalIA(grid, grid.GetCellFromModel(cell.x - 1 + offset, cell.y - 1)) ||
				CheckCrossDiagonalIA(grid, grid.GetCellFromModel(cell.x + offset, cell.y - 1)));
		}

			
		private static bool CheckCrossNormal(ModelGrid grid, Cell cell, bool mustHighlight = true)
		{
			if (!cell)
				return false;


			if (!cell.HasBall ())
				return false;

			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.White;
			else 
				color = cell.ball.Color;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);

			//left
			neighbour = grid.GetCellFromModel(cell.x-1, cell.y);

				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;

			
			patternCells.Add (neighbour);

			//right
			neighbour = grid.GetCellFromModel(cell.x + 1, cell.y);

				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;


			patternCells.Add (neighbour);

			//up
			neighbour = grid.GetCellFromModel(cell.x, cell.y + 2);
	
				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;
	

			patternCells.Add (neighbour);

			//down
			neighbour = grid.GetCellFromModel(cell.x, cell.y - 2);
	
				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;
		

			patternCells.Add (neighbour);

			/*if(mustHighlight)
				HighlighCells (patternCells, Color.red);*/

			return true;
		}
			

		private static bool CheckCrossNormalIA(ModelGrid grid, Cell cell) {
			if (!cell)
				return false;


			if (!cell.HasBall ())
				return false;

			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.White;
			else 
				color = cell.ball.Color;

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);

			//left
			neighbour = grid.GetCellFromModel(cell.x-1, cell.y);

				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;

			patternCells.Add (neighbour);

			//right
			neighbour = grid.GetCellFromModel(cell.x + 1, cell.y);

				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;


			patternCells.Add (neighbour);

			//up
			neighbour = grid.GetCellFromModel(cell.x, cell.y + 2);

				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;


			patternCells.Add (neighbour);

			//down
			neighbour = grid.GetCellFromModel(cell.x, cell.y - 2);

				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;

			patternCells.Add (neighbour);

			//HighlighCells (patternCells, Color.red);

			return true;
		}

		private static bool CheckCrossDiagonal(ModelGrid grid, Cell cell, bool mustHighlight = true)
		{
			if (!cell)
				return false;
{
			if (!cell.HasBall ())
				return false;
			}

			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.White;
			else 
				color = cell.ball.Color;
			int offset = cell.y % 2; 

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);

			//top left


			neighbour = grid.GetCellFromModel(cell.x-1 + offset, cell.y+1);


				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;
	
			patternCells.Add (neighbour);

			//top right
			neighbour = grid.GetCellFromModel(cell.x + offset, cell.y+1);


				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;


			patternCells.Add (neighbour);

			//bottom left
			neighbour = grid.GetCellFromModel(cell.x-1 + offset, cell.y - 1);


				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;
	

			patternCells.Add (neighbour);

			//bottom right
			neighbour = grid.GetCellFromModel(cell.x + offset, cell.y - 1);

				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;
			
			patternCells.Add (neighbour);

			/*if(mustHighlight)
				HighlighCells (patternCells, Color.red);*/

			return true;
		}

		private static bool CheckCrossDiagonalIA(ModelGrid grid, Cell cell)
		{
			if (!cell)
				return false;
			{
				if (!cell.HasBall ())
					return false;
			}

			Cell neighbour;
			BallColor color;
			if (!cell.ball)
				color = BallColor.White;
			else 
				color = cell.ball.Color;
			int offset = cell.y % 2; 

			List<Cell> patternCells = new List<Cell> ();
			patternCells.Add (cell);

			//top left


			neighbour = grid.GetCellFromModel(cell.x-1 + offset, cell.y+1);


				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;

			patternCells.Add (neighbour);

			//top right
			neighbour = grid.GetCellFromModel(cell.x + offset, cell.y+1);


				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;


			patternCells.Add (neighbour);

			//bottom left
			neighbour = grid.GetCellFromModel(cell.x-1 + offset, cell.y - 1);

				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;

			patternCells.Add (neighbour);

			//bottom right
			neighbour = grid.GetCellFromModel(cell.x + offset, cell.y - 1);


				if (!neighbour)
					return false;

				if (!neighbour.HasBall (color))
					return false;

			patternCells.Add (neighbour);

			//HighlighCells (patternCells, Color.red);

			return true;
		}

		#endregion

		#region CELL COLOR
        

		public static void ResetCells(ModelGrid grid) {
			foreach (Cell[] cells in grid.grid) {
				foreach(Cell cell in cells) {
					if (cell) {
						if (cell.ball) {
							UnityEngine.Object.Destroy (cell.ball.gameObject);
							cell.ball = null;
						}
					}
				}
			}
		}

		/*public static void HighlighCells(List<Cell> cells, Color color) 
		{
			float delay = 0;
			foreach (Cell cell in cells)
			{
				delay += 0.05f;
				HighlighCell(cell, color, delay);
			}
		}

		public static void HighlighCell(Cell cell, Color color, float delay = 0)
		{
			if (cell) 
			{
				cell.GetComponent<SpriteRenderer> ().DOColor (color, 0.05f).SetDelay (delay);
				if(cell.ball != null)
					cell.ball.transform.localScale *= 2.5f;
			}
		}*/
		#endregion

		public static void Place(this Ball ball, Cell pickedCell) {

            pickedCell.ball = ball;
			ball.transform.position = pickedCell.transform.position;
			ball.owner = pickedCell;
			//ball.SetStartPosition ();
		}

        public static void DOPlace(this Ball ball, Cell pickedCell) {
            pickedCell.ball = ball;

            Move move = new Move();
            move.fromX = -1;
            move.fromY = -1;
            move.toX = pickedCell.y;
            move.toY = pickedCell.x;
            move.color = (CellColor)ball.Color;

            GameObject.FindObjectOfType<GridManager>().OptiGrid.DoMove(move);

            if (ball.isPickedUp)
                ball.GetComponent<Animator>().SetTrigger("PlaceBall");
            else
                ball.GetComponent<Animator>().SetTrigger("Move");

            ball.isPickedUp = false;
            ball.FixSortingLayer(true);

            ball.transform.DOMove (pickedCell.transform.position, 1.0f).OnComplete (() =>  {
				ball.transform.position = pickedCell.transform.position;
				ball.owner = pickedCell;
                ball.FixSortingLayer(false);
            });
		}

		public static IEnumerator LoadSpriteFromURL(string url, Action<Sprite> callback)
		{
			if (string.IsNullOrEmpty(url))
				yield break;

			WWW www = new WWW(url);
			yield return www;

			Texture2D tex = www.textureNonReadable;
			Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);

			callback(sprite);
		}

		public static void Shuffle<T>(this IList<T> ts) {
			var count = ts.Count;
			var last = count - 1;
			for (var i = 0; i < last; ++i) {
				var r = UnityEngine.Random.Range(i, count);
				var tmp = ts[i];
				ts[i] = ts[r];
				ts[r] = tmp;
			}
		}
	}
}