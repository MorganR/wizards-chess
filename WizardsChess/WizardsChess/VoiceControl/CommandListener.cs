﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using WizardsChess.Chess;
using WizardsChess.VoiceControl.Commands;
using WizardsChess.VoiceControl.Events;

namespace WizardsChess.VoiceControl
{
	/// <summary>
	/// CommandListener converts commands from voice recognition to valid ICommands for an ICommandInterpreter.
	/// </summary>
	class CommandListener : ICommandListener
	{
		#region Public Events
		public event CommandEventHandler ReceivedCommand;
		public event CommandHypothesisEventHandler ReceivedCommandHypothesis;

		private void onCommandRecognized(CommandEventArgs e)
		{
			ReceivedCommand?.Invoke(this, e);
		}

		private void onCommandHypothesized(CommandHypothesisEventArgs e)
		{
			ReceivedCommandHypothesis?.Invoke(this, e);
		}
		#endregion

		public bool IsListening { get { return isListening; } }

		public CommandListener(SpeechRecognizer speechRecognizer)
		{
			isListening = false;
			recognizer = speechRecognizer;
			continuousSession = recognizer.ContinuousRecognitionSession;
			continuousSession.AutoStopSilenceTimeout = TimeSpan.FromMinutes(5);
			continuousSession.ResultGenerated += respondToSpeechRecognition;
		}

		public async Task StartListeningAsync()
		{
			if (!isListening)
			{
				try
				{
					await continuousSession.StartAsync().AsTask();
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine($"Could not start listening. Threw exception {e}");
				}
				isListening = true;
			}
		}

		public async Task StopListeningAsync()
		{
			if (isListening)
			{
				try
				{
					await continuousSession.StopAsync();
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine($"Could not stop listening. Threw exception {e}");
				}
				isListening = false;
			}
		}

		#region Private Members
		private bool isListening;
		private SpeechRecognizer recognizer;
		private SpeechContinuousRecognitionSession continuousSession;
		#endregion

		#region Private Methods
		private void respondToSpeechRecognition(
			SpeechContinuousRecognitionSession sender,
			SpeechContinuousRecognitionResultGeneratedEventArgs args)
		{
			if (args.Result.Status == SpeechRecognitionResultStatus.Success)
			{
				System.Diagnostics.Debug.WriteLine($"Recognized speech: {args.Result.Text}");
				System.Diagnostics.Debug.WriteLine($"Recognition confidence: {args.Result.Confidence}");
				if (args.Result.Confidence == SpeechRecognitionConfidence.Rejected)
				{
					return;
				}
				else if (args.Result.Confidence == SpeechRecognitionConfidence.Low)
				{
					// Throw the response away
					//onCommandHypothesized(new CommandHypothesisEventArgs(convertSpeechToCommand(args.Result), args.Result.Text));
				}
				else
				{
					var command = convertSpeechToCommand(args.Result);
					if (command.Type == CommandType.Move)
					{
						var moveCommand = command as MoveCommand;
						if (moveCommand.Position.HasValue && !moveCommand.PositionUsedNatoAlphabet)
						{
							if (isPositionAmbiguous(moveCommand.Position.Value, args.Result.Confidence))
							{
                                // Throw the response away
                                //onCommandHypothesized(new CommandHypothesisEventArgs(command, args.Result.Text));
								return;
							}
						}
						if (!moveCommand.DestinationUsedNatoAlphabet)
						{
							if (isPositionAmbiguous(moveCommand.Destination, args.Result.Confidence))
							{
                                // Throw the response away
                                //onCommandHypothesized(new CommandHypothesisEventArgs(command, args.Result.Text));
								return;
							}
						}
					}
					else if (command.Type == CommandType.ConfirmPiece)
					{
						var confirmPieceCommand = command as ConfirmPieceCommand;
						if (!confirmPieceCommand.PositionUsedNatoAlphabet)
						{
							if (isPositionAmbiguous(confirmPieceCommand.Position, args.Result.Confidence))
							{
								onCommandHypothesized(new CommandHypothesisEventArgs(command, args.Result.Text));
								return;
							}
						}
					}
					onCommandRecognized(new CommandEventArgs(convertSpeechToCommand(args.Result)));
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Received continuous speech result of {args.Result.Status}");
			}
		}

		private ICommand convertSpeechToCommand(SpeechRecognitionResult speech)
		{
			var cmdType = CommandTypeMethods.Parse(speech.SemanticInterpretation.Properties);
			switch (cmdType)
			{
				case CommandType.Move:
					return new MoveCommand(speech.SemanticInterpretation.Properties);
				case CommandType.ConfirmPiece:
					return new ConfirmPieceCommand(speech.SemanticInterpretation.Properties);
				case CommandType.MotorMove:
					return new MotorMoveCommand(speech.SemanticInterpretation.Properties);
				case CommandType.Magnet:
					return new MagnetCommand(speech.SemanticInterpretation.Properties);
                case CommandType.Castle:
                    return new CastleCommand(speech.SemanticInterpretation.Properties);
				default:
					return new Command(speech.SemanticInterpretation.Properties);
			}
		}

		private bool isPositionAmbiguous(Position p, SpeechRecognitionConfidence confidence)
		{
			if (confidence == SpeechRecognitionConfidence.High)
			{
				return false;
			}

			return p.ColumnLetter == ColumnLetter.B
				|| p.ColumnLetter == ColumnLetter.C
				|| p.ColumnLetter == ColumnLetter.D
				|| p.ColumnLetter == ColumnLetter.E
				|| p.ColumnLetter == ColumnLetter.G;
		}
		#endregion
	}
}
