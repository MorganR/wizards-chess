﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardsChess.Movement.Drv;
using WizardsChess.Movement.Drv.Events;
using WizardsChess.Movement.Events;

namespace WizardsChess.Movement
{
	public class MotorLocator : IMotorLocator, IDisposable
	{
		public MotorLocator(IGpioPin clearCounter, IMotorInformation motorInformation)
		{
			stepCounter = motorInformation.SteppingPin;
			clearCounterPin = clearCounter;
			motorInfo = motorInformation;
			position = 0;
			lastMoveDirection = MoveDirection.Stopped;

			stepCounter.ValueChanged += pinValueChanged;
		}

		public int Position { get { return position; } }
		public MoveDirection LastMoveDirection { get { return lastMoveDirection; } }

		public event PositionChangedEventHandler PositionChanged;

		public void ShiftPosition(int shift)
		{
			lock (lockObject)
			{
				// Doesn't affect lastMoveDirection because this is a calibration, not a real motor move.
				position += shift;
			}
		}

		private volatile int position;
		private volatile MoveDirection lastMoveDirection;
		private IGpioPin stepCounter;
		private IGpioPin clearCounterPin;
		private IMotorInformation motorInfo;
		private object lockObject = new object();

		private void pinValueChanged(object sender, GpioValueChangedEventArgs e)
		{
			if (e.Edge == GpioEdge.FallingEdge)
			{
				// If motor hasn't move and latestActiveMoveDirection is still Stopped, this will not impact anything because the Stopped value is 0.
				var pos = 0;
				var direction = motorInfo.Direction;
				lock (lockObject)
				{
					lastMoveDirection = direction;
					position += (int)direction;
					pos = position;
				}
				onStepCounted(pos, direction);
			}
		}

		private void onStepCounted(int pos, MoveDirection direction)
		{
			PositionChanged?.Invoke(this, new PositionChangedEventArgs(pos, direction));
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					stepCounter.ValueChanged -= pinValueChanged;
				}

				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
