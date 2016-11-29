﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Chess;
using WizardsChess.Movement;
using WizardsChess.Movement.Drv;
using WizardsChess.VoiceControl;
using WizardsChess.VoiceControl.Events;

namespace WizardsChess
{
	class GameManager
	{
		private GameManager(ICommandInterpreter commandInterpreter, ChessLogic logic, IMoveManager movementManager)
		{
			cmdInterpreter.CommandReceived += CommandReceived;

			cmdInterpreter = commandInterpreter;
			chessLogic = logic;
			moveManager = movementManager;
		}

		public static async Task<GameManager> CreateAsync()
		{
			var commandInterpreterConstructor = CommandInterpreter.CreateAsync();

			ChessLogic logic = new ChessLogic();

			var motorDriverX = new MotorDrv(23, 24);
			var motorDriverY = new MotorDrv(20, 21);
			var magnetDriver = new MagnetDrv(26);
			var stepCounterX = new StepCounter(6, 19);
			var stepCounterY = new StepCounter(5, 13);
			var movePerformer = new MovePerformer(motorDriverX, motorDriverY, magnetDriver, stepCounterX, stepCounterY);

			var movePlanner = new MovePlanner(logic.Board);

			var moveManager = new MoveManager(movePlanner, movePerformer);

			GameManager manager = new GameManager(await commandInterpreterConstructor, logic, moveManager);

			return manager;
		}

		public async Task PlayGameAsync()
		{
			await cmdInterpreter.StartAsync();
			
			// Use a monitor?
		}

		private ICommandInterpreter cmdInterpreter;
		private ChessLogic chessLogic;
		private IMoveManager moveManager;

		private void CommandReceived(Object sender, CommandEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine($"Received command of type {args.Command.Type}.");
		}
	}
}
