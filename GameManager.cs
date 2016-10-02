using System;
using System.Collections.Generic;

// To control the motions
namespace WizardChess{
	
	class GameManager{
		// To test only
		public static void Main(){
			
			// Initiialize a game
			GameManager game = new GameManager();
			game.initialize();
			game.printNodes();


			while(true){
				Console.WriteLine("Enter Start Location");
				string Start = Console.ReadLine();
				Console.WriteLine("Enter End Location");
				string End = Console.ReadLine();

				int[] startCoordinates = game.getFormattedCoordinate(Start);
				int[] endCoordinates = game.getFormattedCoordinate(End);

				bool status = game.checkMoveValidity(startCoordinates,endCoordinates);

				Console.WriteLine("Move from "+Start+" to "+End+" is: ");

				if(status){
					Console.WriteLine("Valid!");
					game.movePiece(startCoordinates[0],startCoordinates[1],endCoordinates[0],endCoordinates[1]);
					game.printNodes();
				}else{
					Console.WriteLine("Invalid!");
				}

			}
			

			//string Start = "G1";
			//string End = "F3";

			

			// Get what piece is at the input location
			//string pieceName = (grid[startCoordinates[0],startCoordinates[1]]).getName();
			//Console.WriteLine(startCoordinates[0]+" "+startCoordinates[1]);
			//Console.WriteLine(endCoordinates[0]+" "+endCoordinates[1]);
			

			// We get an input such as "A3 --> A5". Ex. A8 Sequence is (8,A)--> 8,0

		}

		// To see if move valid
		public bool checkMoveValidity(int[] startCoordinates, int[] endCoordinates){
			bool isValidMove = false;

			// Get piece at input location

			ChessPiece startPiece = grid[startCoordinates[0],startCoordinates[1]];
			ChessPiece endPiece = grid[endCoordinates[0],endCoordinates[1]];
			

		
			// To hold object values
			string startPieceName;
			string startPieceTeam;
			string endPieceTeam;
			bool attemptMoveCheck = false;

			// To hold if possible to move there or not
			bool placementValid = false;

			// To break if objects in the way
			bool objectsInWay = false;

			// To hold onto movement vectors
			int vectorX;
			int vectorY;
			

			List<int[]> allowedMoveSets = new List<int[]>();
			List<int[]> specialMoveSets = new List<int[]>();

			// // check if valid move then check if anything blocking seperatly
			if (startPiece != null){
				startPieceName = startPiece.getName();
				startPieceTeam = startPiece.getTeamName();


				// If there is a piece there we need to check if friendly or not
				if(endPiece != null){
					// if is not a friently piece can move there
					endPieceTeam = endPiece.getTeamName();
					if(endPieceTeam!=startPieceName){
						attemptMoveCheck = true;
					}

				}else{
					attemptMoveCheck = true;
				}
				if(attemptMoveCheck){
					// This tile is empty, we can look into moving here
					List<int[]> allowedMoveVectors = startPiece.getAllowedMotionVector();

					// generate a list of allowed moves
					List<int[]>  allowedTiles = new List<int[]>();

					// given start and end coordinates, let us subtract to get vector
					int xVector = endCoordinates[0]-startCoordinates[0];
					int yVector = endCoordinates[1]-startCoordinates[1];

					// Check to see if move is possible in constraints of piece movement allowment
					foreach (int[] allocatedMoveSet in allowedMoveVectors){
						// if pawn, knight, king, need exact
						if(startPieceName=="Pawn" || startPieceName=="Knight" || startPieceName=="King"){
							foreach (int[] vector in allowedMoveVectors){
								if(vector[0]==xVector && vector[1]==yVector){
									isValidMove = true;
								}
							}
						}else{
							// All of these can have variants of allowed movement vectors
							foreach (int[] vector in allowedMoveVectors){
								// Check to make sure coordinates are multiples 
								//Console.WriteLine(xVector+" "+yVector);
								//Console.WriteLine(vector[0]+" "+vector[1]);
								//Console.WriteLine("mod "+5%3+" "+vector[1]%yVector);

								if(startPieceName=="Bishop"){
									if(xVector!=0 && yVector !=0){
										if(xVector%vector[0]==yVector%vector[1]){
											// Check for collisions here
											if(checkCollisions(startCoordinates,endCoordinates,vector)){
												isValidMove=true;
												break;
											}else{
												objectsInWay=true;
												break;
											}
											
										}
									}
								}


								if(startPieceName=="Queen"){
									Console.WriteLine(xVector);
									Console.WriteLine(yVector);
									if(xVector!= 0 && yVector!=0){
										//Console.WriteLine("Diag");
										if(Math.Abs(xVector) == Math.Abs(yVector)){
											// Check for collisions here
											if(checkCollisions(startCoordinates,endCoordinates,vector)){

												isValidMove=true;
												break;
											}else{
												objectsInWay=true;
												break;
											}
										}
									}else if(xVector==0 && yVector!=0){
										//Console.WriteLine("X Dir");
										if(checkCollisions(startCoordinates,endCoordinates,vector)){


											isValidMove = true;
											break;
										}else{
											objectsInWay=true;
											break;
										}
										
									}else if(yVector==0 && xVector!=0){
										//Console.WriteLine("Y Dir");
										if(checkCollisions(startCoordinates,endCoordinates,vector)){


											isValidMove=true;
											break;

										}else{
											objectsInWay=true;
											break;
										}
										
									}
								}

								if(startPieceName=="Rook"){
									if(xVector==0 && yVector!=0){
										//Console.WriteLine("X Dir");
										if(checkCollisions(startCoordinates,endCoordinates,vector)){


											isValidMove = true;
											break;
										}else{
											objectsInWay=true;
											break;
										}
										
									}else if(yVector==0 && xVector!=0){
										//Console.WriteLine("Y Dir");
										if(checkCollisions(startCoordinates,endCoordinates,vector)){


											isValidMove=true;
											break;

										}else{
											objectsInWay=true;
											break;
										}
										
									}
								}

								

							
								
							}
						}
						if(isValidMove){
							break;
						}else if(objectsInWay){
							break;
						}
					}

					



				}

			}else{
				Console.WriteLine("Piece 1 doesn't exist. Valid input needed to proceed");
			}

			

			//string startPiece = grid[startCoordinates[0],startCoordinates[1]].getName();
			//string endPiece = grid[endCoordinates[0],endCoordinates[1]].getName();
			return isValidMove;
			
			
		}

		// To check whether there will be collisions
		// Input Start coordinate, end coordinates, vector
		// output if any collisions. True if okay to move
		public bool checkCollisions(int[] startCoordinates,int[] endCoordinates,int[] vector){
			bool ableToMove = true;

			int xIncrementer=0;
			int yIncrementer=0;
			int incrementsNeededToCheck = 0;

			if(vector[0]>0){
				xIncrementer=1;
			}else if(vector[0]<0){
				xIncrementer=-1;
			}else{
				xIncrementer=0;
			}
			if(vector[1]>0){
				yIncrementer = -1;
			}else if(vector[1]<0){
				yIncrementer=1;
			}else{
				yIncrementer=0;
			}

			if(Math.Abs(vector[0])>Math.Abs(vector[1])){
				incrementsNeededToCheck=Math.Abs(vector[0]);
			}else{
				incrementsNeededToCheck=Math.Abs(vector[1]);
			}

			Console.WriteLine("incrementsNeededToCheck "+incrementsNeededToCheck);
			Console.WriteLine("xdir "+xIncrementer);
			Console.WriteLine("ydir "+yIncrementer);
			Console.WriteLine("startX "+startCoordinates[0]+" startY "+startCoordinates[1]);
			Console.WriteLine("endX "+endCoordinates[0]+" endY "+endCoordinates[1]);

			int X = startCoordinates[0];
			int Y = startCoordinates[1];
			for(int i=0; i< incrementsNeededToCheck;i++){
				X += xIncrementer;
				Y += yIncrementer;
				if(grid[X,Y]!=null){
					Console.WriteLine("Stuff in WAY!");
					ableToMove=false;
					break;
				}
			}
			/*
			for(int i=startCoordinates[0];i!=endCoordinates[0];i+=xIncrementer){
				for(int j=startCoordinates[1];j!=endCoordinates[1];j+=yIncrementer){
					Console.WriteLine(i+" "+j);
					if(grid[i,j]!=null){
						ableToMove=false;
					}
				}
			}
			*/

			return ableToMove;
		}

		// to move the piece once verified
		public void movePiece(int startX, int startY, int endX, int endY){
			grid[endX,endY] = grid[startX,startY];
			grid[endX,endY].setMoved();

			grid[startX,startY] = null;

		}

		// Given a start coordinate pair, end coordinate pair, and modiifying vector, returns if the vector modifies start to end
		public bool checkTransform(int[] startCoordinates, int[] endCoordinates,int[] vector){
			bool success= false;

			if((startCoordinates[0]+vector[0]==endCoordinates[0]) && (startCoordinates[1]+vector[1]==endCoordinates[1])){
				success = true;			
			}
			return success;
		}



		// To take a coordinate input into something readable
		// Returns an array where val[0]=x, val[1]=y
		// Depreciated. Morgan will be parsing inputs
		public int[] getFormattedCoordinate(string coordinate){
			int[] returnable = new int[2];

			string XRaw;
			string YRaw;

			XRaw = coordinate[1].ToString();
			YRaw = coordinate[0].ToString();

			int XFinal=0;
			int YFinal=0;


			switch(YRaw){
				case "A":
					YFinal = 1;
					break;
				case "B":
					YFinal = 2;
					break;
				case "C":
					YFinal = 3;
					break;
				case "D":
					YFinal = 4;
					break;
				case "E":
					YFinal = 5;
					break;
				case "F":
					YFinal = 6;
					break;
				case "G":
					YFinal = 7;
					break;
				case "H":
					YFinal = 8;
					break;
				default:
					Console.WriteLine("Invalid move was given");
					break;

				



			}
			XFinal = Int32.Parse(XRaw);

			// We need to subtract by 1 for the matrix locations
			returnable[0] = XFinal-1;
			returnable[1] = YFinal-1;

			return returnable;

		}

		// To hold the game of chess ( our matrix)
		protected ChessPiece[,] grid = new ChessPiece[8,8];

		// to initialize a game of chess
		public void initialize(){
			// initialize the rooks
			grid[0,0]=new Rook("Rook","Team1");
			grid[0,7]=new Rook("Rook","Team1");
			grid[7,0]=new Rook("Rook","Team2");
			grid[7,7]=new Rook("Rook","Team2");

			// initialize the knights
			grid[0,1]=new Knight("Knight","Team1");
			grid[0,6]=new Knight("Knight","Team1");
			grid[7,1]=new Knight("Knight","Team2");
			grid[7,6]=new Knight("Knight","Team2");

			// initialize the Bishops
			grid[0,2]=new Bishop("Bishop","Team1");
			grid[0,5]=new Bishop("Bishop","Team1");
			grid[7,2]=new Bishop("Bishop","Team2");
			grid[7,5]=new Bishop("Bishop","Team2");

			// initialize the Kings
			grid[0,4]=new King("King","Team1");
			grid[7,4]=new King("King","Team2");

			// initialize the Kings
			grid[0,3]=new Queen("Queen","Team1");
			grid[7,3]=new Queen("Queen","Team2");

			// initialize pawns
			for (int i=1; i< 7; i+=5){
				for (int k = 0; k< 8; k++){
					if(i==1){
						grid[i,k] = new Pawn("Pawn","Team1");
					}else{
						grid[i,k] = new Pawn("Pawn","Team2");
					}
					
				}

			}
			
			
			

			


		}

		public void printNodes(){

			int ASCIIA = 64;

			for(int k=-1; k< grid.GetLength(0);k++){
				string output = "";
				for(int j=-1; j < grid.GetLength(1);j++){
					if (k==-1){
						if(j==-1){
							//output+= " |           | ";
							Console.Write(" |           | ");
						}else{	
							//output+=" |    "+(char)ASCIIA+"     | ";
							Console.Write(" |    "+(char)ASCIIA+"     | ");
						}
						ASCIIA++;

					}else{
						if(j ==-1 && k ==-1){
							Console.Write(" |          | ");
							
						}else if(j==-1){
							Console.Write(" |     "+(k+1)+"     | ");
						}else{
							if(grid[k,j] ==null){
								//output+=" |          | ";
								Console.Write(" |          | ");
							}else{
								string spacer = grid[k,j].getName();
								int spacerIndex = 0;
								spacerIndex = 10 - grid[k,j].getName().Length;
								for(int i=0; i< spacerIndex; i++){
									spacer+=" ";
								}
								//output+= " |"+spacer+"| ";
								Console.Write(" |");
								string team = grid[k,j].getTeamName();
								if(team=="Team1"){
									Console.ForegroundColor = ConsoleColor.Blue;
								}else{
									Console.ForegroundColor = ConsoleColor.Red;
								}
								Console.Write(spacer);
								Console.ResetColor();
								Console.Write("| ");
							}
						}

					}
				}
				//Console.WriteLine(output);
				Console.Write("\n");
			}
		}

		

		


	}





	

}

