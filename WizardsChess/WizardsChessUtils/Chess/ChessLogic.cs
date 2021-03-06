﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WizardsChess.Chess;
using WizardsChess.Chess.Pieces;
using WizardsChess.Movement;

namespace WizardsChess.Chess
{
	public class ChessLogic
	{
		public ChessLogic ()
		{
			board = new ChessBoard();
		}

		//Checks for pieces of a certain type that can move to 
		public ISet<Position> FindPotentialPiecesForMove(PieceType piece, Position destination)
		{
			var pieceLocationList = board.PieceLocationsByType[piece];

			var potentialPiecePositions = new HashSet<Position>();

			foreach (var location in pieceLocationList)
			{
				if (IsMoveValid(new Position(location), destination))
				{
					potentialPiecePositions.Add(new Position(location));
				}
			}

			return potentialPiecePositions;
		}

		/// <summary>
		/// Moves the piece from startPosition to endPosition. Kills the piece at endPosition if it exists.
		/// Writes "Performing illegal move." to debug if this is an invalid move.
		/// </summary>
		public void MovePiece (Position startPosition, Position endPosition)
		{
			if (!IsMoveValid(startPosition, endPosition))
			{
				System.Diagnostics.Debug.WriteLine($"Performing illegal move.");
				//throw new InvalidOperationException($"Cannot complete invalid move from {startPosition} to {endPosition}");
			}

			board.MovePiece(startPosition, endPosition);
		}

		public void Castle(Point2D rookPos)
		{
			board.Castle(rookPos);
		}

		public void UndoMove ()
		{
			if (board.PastMoves.Count == 0)
			{
				System.Diagnostics.Debug.WriteLine($"No previous moves to undo.");
				//throw new InvalidOperationException($"No previous moves to undo.");
			}
			else
			{
				board.UndoMove();
			}
		}

		/// <summary>
		/// Checks if the move from startPosition to endPosition is valid.
		/// Assumes that startPosition and endPosition are valid parameters.
		/// Returns false if there is no piece at startPosition, or the piece otherwise
		/// cannot complete the requested move.
		/// </summary>
		/// <param name="startPosition">The position of the piece to move.</param>
		/// <param name="endPosition">The destination of the piece.</param>
		/// <returns></returns>
		public bool IsMoveValid(Position startPosition, Position endPosition)
		{
			// Get piece at input location
			ChessPiece startPiece = board.PieceAt(startPosition);
			ChessPiece endPiece = board.PieceAt(endPosition);

			// If there is no piece at the requested start position, return false
			if (startPiece == null)
			{
				return false;
			}

			// Piece can't move if it's not this pieces turn to move
			if (startPiece.Team != board.WhoseTurn)
			{
				return false;
			}
			
			//get places this piece could move ignoring obstructions
			IReadOnlyList<Vector2D> pieceMovementVectors;
			if (endPiece == null)	//replace with DoesMoveCapture when implementing En Passant
			{
				pieceMovementVectors = startPiece.GetAllowedMotionVectors();
			}
			else
			{
				// If there is a piece in the way and it is a friendly piece, then we can't move there
				if (endPiece.Team == startPiece.Team)
				{
					return false;
				}
				pieceMovementVectors = startPiece.GetAttackMotionVectors();
			}

			//checks if destination is in one of the positions this piece could move (ignoring obsturctions for now)
			var requestedMoveVector = (Point2D)endPosition - (Point2D)startPosition;
			bool onPieceVector = false;
			foreach(var v in pieceMovementVectors)	//TODO: make more efficient
			{
				if (v == requestedMoveVector)
				{
					onPieceVector = true;
					break;
				}
			}
			if (!onPieceVector)
			{
				System.Diagnostics.Debug.WriteLine("Destination not on this piece's vectors.");
				return false;
			}

			/*try
			{
				var matchingMove = pieceMovementVectors.Single(v => v == requestedMoveVector);
			}
			catch (InvalidOperationException)
			{
				// Could not retrieve a matching vector from the allowed moves
				return false;
			}*/

			// If the piece can jump, it doesn't matter if something is in the way
			if (startPiece.CanJump)
			{
				return true;
			}

			return isPathClear(startPosition, endPosition);
		}

		//TODO: overload of: public bool IsMoveValid(Point2D, Point2D)

		public bool DoesMoveCapture(Position start, Position end)	//TODO: make MovePiece use DoesMoveCapture and CaptureLocation methods
		{
			if (board.PieceAt(end) != null)
			{
				return true;
			}
			//TODO: add en passant conditions
			else
			{
				return false;
			}
		}

		public Position CaptureLocation(Position start, Position end)
		{
			return end;	//remove this to implement en passant
			/*if (board.PieceAt(end)!=null)
			{
				return end;
			}
			else
			{
				//en passant location
			}*/
			
		}

		//Checks to see if all the squares between two points are clear along their connecting vector
		//TODO: see if this code can be simplified, I think it can be
		private bool isPathClear(Point2D startPosition, Point2D endPosition)
		{
			var requestedMoveVector = endPosition - startPosition;

			// Increment from the startPosition to the endPosition, checking nothing is in the way
			var unitVector = requestedMoveVector.GetUnitVector();
			var nextPosition = startPosition + unitVector;
			while (nextPosition != endPosition)
			{
				if (board.PieceAt(nextPosition) != null)
				{
					System.Diagnostics.Debug.WriteLine("Obstruction on path.");
					return false;
				}
				nextPosition = nextPosition + unitVector;
			}

			return true;
		}

		//TODO: detect Checkmate

		//return whether or not castling on the short side for the current turn is legal. Directions are hard coded.
		public bool shortCastleLegal()
		{
			if(board.WhoseTurn == ChessTeam.White &&	//white's turn 
				board.PieceAt(board.GetKingCol(), board.GetWhiteBackRow()).HasMoved == false &&		//king is unmoved
				board.PieceAt(board.GetLeftRookCol(), board.GetWhiteBackRow()).HasMoved == false &&	//rook is unmoved
				isPathClear(new Point2D(board.GetKingCol(), board.GetWhiteBackRow()), new Point2D(board.GetLeftRookCol(), board.GetWhiteBackRow())))	//path is clear
			{
				if(pathAlongRowInCheck(board.GetKingCol() - 2, board.GetKingCol(), board.GetWhiteBackRow()))    //checks if any squares the king traverses are in check
				{
					return false;
				}
				return true;
			}
			else if(board.PieceAt(board.GetKingCol(), board.GetBlackBackRow()).HasMoved == false &&	//king is unmoved, also it's black turn since it's not white
				board.PieceAt(board.GetLeftRookCol(), board.GetBlackBackRow()).HasMoved == false &&	//rook is unmoved
				isPathClear(new Point2D(board.GetKingCol(), board.GetBlackBackRow()), new Point2D(board.GetLeftRookCol(), board.GetBlackBackRow())))	//path is clear
			{
				if (pathAlongRowInCheck(board.GetKingCol() - 2, board.GetKingCol(), board.GetBlackBackRow()))   //checks if any squares the king traverses are in check
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public bool longCastleLegal()
		{
			if (board.WhoseTurn == ChessTeam.White &&   //white's turn 
				board.PieceAt(board.GetKingCol(), board.GetWhiteBackRow()).HasMoved == false &&         //king is unmoved
				board.PieceAt(board.GetRightRookCol(), board.GetWhiteBackRow()).HasMoved == false &&    //rook is unmoved
				isPathClear(new Point2D(board.GetKingCol(), board.GetWhiteBackRow()), new Point2D(board.GetRightRookCol(), board.GetWhiteBackRow())))    //path is clear
			{
				if (pathAlongRowInCheck(board.GetKingCol(), board.GetKingCol() + 2, board.GetWhiteBackRow()))  //checks if any squares the king traverses are in check
				{
					return false;
				}
				return true;
			}
			else if (board.PieceAt(board.GetKingCol(), board.GetBlackBackRow()).HasMoved == false &&    //king is unmoved, also it's black turn since it's not white
				board.PieceAt(board.GetRightRookCol(), board.GetBlackBackRow()).HasMoved == false &&    //rook is unmoved
				isPathClear(new Point2D(board.GetKingCol(), board.GetBlackBackRow()), new Point2D(board.GetRightRookCol(), board.GetBlackBackRow())))    //path is clear, note the
			{
				if (pathAlongRowInCheck(board.GetKingCol(), board.GetKingCol() + 2, board.GetBlackBackRow()))	//checks if any squares the king traverses are in check
				{
					return false;
				}
				return true;
			}
			else
				return false;
		}

		//used by left/right castling legality checkers to see if any squares specified, inclusive, are in check
		public bool pathAlongRowInCheck(int leftCol, int rightCol, int row)
		{
			for (int i = leftCol; i <= rightCol; i++)
			{
				if(inCheck(new Point2D(i, row), board.WhoseTurn))
				{
					return true;
				}
			}
			return false;
		}

		public Point2D shortCastleRookPos()
		{
			if (board.WhoseTurn == ChessTeam.White)
			{
				return new Point2D(board.GetLeftRookCol(), board.GetWhiteBackRow());
			}
			else //black's turn
			{
				return new Point2D(board.GetLeftRookCol(), board.GetBlackBackRow());
			}
		}

		public Point2D longCastleRookPos()
		{
			if(board.WhoseTurn == ChessTeam.White)
			{
				return new Point2D(board.GetRightRookCol(), board.GetWhiteBackRow());
			}
			else //black's turn
			{
				return new Point2D(board.GetRightRookCol(), board.GetBlackBackRow());
			}
		}

		//Unused in current build with Left/Right Castling differentiation
		public List<Point2D> validRookLocationsForCastling()
		{
			List<Point2D> validRookLocations = new List<Point2D>();
			var kingLocations = board.PieceLocationsByType[PieceType.King];

			Point2D kingLocation = new Point2D();
			ChessPiece king;

			foreach (var aKingLocation in kingLocations)
			{
				if (board.PieceAt(aKingLocation).Team == board.WhoseTurn)
				{
					kingLocation = aKingLocation;
				}
			}
			if (kingLocation == null)
			{
				throw new InvalidOperationException($"missing king");
			}
			king = board.PieceAt(kingLocation);
			if (king.HasMoved == true || inCheck(kingLocation, board.WhoseTurn))	//if the king has moved or is in check, castling is not legal
			{
				return validRookLocations;
			}
			var allRookLocations = board.PieceLocationsByType[PieceType.Rook];
			int x;
			int y;
			int kingToRookDir;
			bool thisSideOkay;

			foreach (var location in allRookLocations)
			{
				var rook = board.PieceAt(location);
				//if the rook in question is on the team that isn't moving, has move, it cannot provide a legal castle
				if (rook.Team == board.WhoseTurn && rook.HasMoved == false && isPathClear(kingLocation, location))
				{
					thisSideOkay = true;
					y = kingLocation.Y;
					kingToRookDir = Math.Sign(location.X - kingLocation.X);
					for (x = kingLocation.X + kingToRookDir; x != location.X; x += kingToRookDir) {
						if (board.PieceAt(x,y) != null)	//if any squares between the king and the rook contain piece, that castle is illegal
						{
							thisSideOkay = false;
							break;	//could be replaced with a double break of sorts, and then the valid rook adding could be without an if statement
						}
						if (Math.Abs(kingLocation.X-x) <= 2) //if the square is 1 or two over from the king, the king will pass through it, so it must not be in check
						{
							if (inCheck(new Point2D(x, y), board.WhoseTurn))
							{
								thisSideOkay = false;
								break;  //could be replaced with a double break of sorts, and then the valid rook adding could be without an if statement
							}
						}
					}
					if (thisSideOkay)
					{
						validRookLocations.Add(location);
					}
				}
			}
			return validRookLocations;
		}

		//TODO: en passant (this might be implemented through other methods)

		// Checks if the player to move's king is in check
		public bool inCheck()
		{
			var kingLocations = board.PieceLocationsByType[PieceType.King];
			Point2D kingLocation = new Point2D();
			foreach (var aKingLocation in kingLocations)
			{
				if (board.PieceAt(aKingLocation).Team == board.WhoseTurn)
				{
					kingLocation = aKingLocation;
				}
			}
			if (kingLocation == null)
			{
				System.Diagnostics.Debug.WriteLine("King missing somehow.");
				throw new InvalidOperationException($"missing king");
			}
			return inCheck(kingLocation, board.WhoseTurn);
		}

		// Checks if the specified location is in check for the specified team 
		public bool inCheck(Point2D checkPoint, ChessTeam Turn)
		{
			int i, j;
			for (i = 0; i < ChessBoard.Size; i++)
			{
				for (j = 0; j < ChessBoard.Size; j++)
				{
					var piece = board.PieceAt(i, j);
					if (piece != null && piece.Team != Turn)
					{
						if (IsCheckMoveValid(new Point2D(i,j), checkPoint))
						{
							return true;
						}
					}
				}
			}
			return false;	//TODO: remove this
		}

		public bool IsCheckMoveValid(Point2D startPoint, Point2D endPoint)
		{
			// Get piece at input location
			ChessPiece startPiece = board.PieceAt(startPoint);
			ChessPiece endPiece = board.PieceAt(endPoint);

			// If there is no piece at the requested start position, return false
			if (startPiece == null)
			{
				return false;
			}

			IReadOnlyList<Vector2D> pieceMovementVectors;
			if (endPiece == null)
			{
				pieceMovementVectors = startPiece.GetAllowedMotionVectors();
			}
			else
			{
				// If there is a piece in the way and it is a friendly piece, then we can't move there
				if (endPiece.Team == startPiece.Team)
				{
					return false;
				}
				pieceMovementVectors = startPiece.GetAttackMotionVectors();
			}

			var requestedMoveVector = endPoint - startPoint;

			bool matchingVector = false;	// true if requested MoveVector matches a movement vector in pieceMovementVectors
			foreach(var moveVector in pieceMovementVectors)
			{
				if (requestedMoveVector == moveVector)
				{
					matchingVector = true;
					break;
				}
			}
			if(!matchingVector)
			{
				System.Diagnostics.Debug.WriteLine("Move illegal because it no piece vectors match that vector");
				return false;
			}
			/*try
			{
				var matchingMove = pieceMovementVectors.Single(v => v == requestedMoveVector);
			}
			catch (InvalidOperationException)
			{
				// Could not retrieve a matching vector from the allowed moves
				return false;
			}*/	//TODO: Remove this code

			// If the piece can jump, it doesn't matter if something is in the way
			if (startPiece.CanJump)
			{
				return true;
			}

			return isPathClear(startPoint, endPoint);
		}

		//piece accessor by x and y indexes
		public ChessPiece PieceAt(int x, int y)
		{
			return board.PieceAt(x, y);
		}

		//piece accessor by Point2D
		public ChessPiece PieceAt(Point2D point)
		{
			return board.PieceAt(point);
		}

		//piece accessor by Position
		public ChessPiece PieceAt(Position pos)
		{
			return board.PieceAt(pos);
		}

		private ChessBoard board;
		public IChessBoard Board
		{
			get
			{
				return board;
			}
		}
	}
}
