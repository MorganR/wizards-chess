﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardsChess.Movement
{
	public interface IMovePerformer
	{
		Task MovePiece(List<Point2D> steps);
		void MoveMotor(Axis axis, int gridUnits);
		void GoHome();
		void EnableMagnet(bool enable);
	}
}
