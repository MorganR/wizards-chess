﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IMotorMover
	{
		/// <summary>
		/// Drive the motor up to the given position, then stop. 
		/// Does not return until the motor is completely stopped.
		/// </summary>
		/// <param name="position">The target position to drive the motor to. Not necessarily where the motor stops.</param>
		/// <param name="timeout">The max timeout for the move when the motors are stopped if the position is not reached.</param>
		/// <returns>The position when the motor stops.</returns>
		Task<int> GoToPositionAsync(int position, TimeSpan timeout);

		/// <summary>
		/// Cancel an in-progress move.
		/// </summary>
		void CancelMove();
	}
}
