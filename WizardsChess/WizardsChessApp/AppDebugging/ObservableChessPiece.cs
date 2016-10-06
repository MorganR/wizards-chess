﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using WizardsChessApp.Chess.Pieces;

namespace WizardsChessApp.AppDebugging
{
	public class ObservableChessPiece : INotifyPropertyChanged
	{
		public ObservableChessPiece(ChessPiece piece, int row, int col)
		{
			this.piece = piece;
			Symbol = Piece.ToShortString();
			switch (piece.Team)
			{
				case Chess.ChessTeam.Black:
					Color = new SolidColorBrush(Windows.UI.Colors.Black);
					break;
				case Chess.ChessTeam.White:
					Color = new SolidColorBrush(Windows.UI.Colors.White);
					break;
				default:
					break;
			}
			UpdatePosition(row, col);
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string Symbol
		{
			get { return this.symbol; }
			set
			{
				if (this.symbol != value)
				{
					this.symbol = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ChessPiece Piece
		{
			get { return this.piece; }
		}

		public SolidColorBrush Color
		{
			get { return this.color; }
			set
			{
				if (this.color != value)
				{
					this.color = value;
					NotifyPropertyChanged();
				}
			}
		}
		
		public int GridRow
		{
			get { return gridRow; }
			private set
			{
				if (this.gridRow != value)
				{
					this.gridRow = value;
					NotifyPropertyChanged();
				}
			}
		}

		public int GridColumn
		{
			get { return this.gridColumn; }
			private set
			{
				if (this.gridColumn != value)
				{
					this.gridColumn = value;
					NotifyPropertyChanged();
				}
			}
		}

		public void UpdatePosition(int row, int col)
		{
			GridRow = 7 - row;
			GridColumn = col;
		}

		private string symbol;
		private ChessPiece piece;
		private SolidColorBrush color;
		private int gridRow;
		private int gridColumn;
	}
}
