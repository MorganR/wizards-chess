﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace WizardsChess
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
			//
			// Create the deferral by requesting it from the task instance.
			//
			BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

			GameManager gameManager = await GameManager.CreateAsync();

			// Loop continuously
			await gameManager.PlayGameAsync();

			//
			// Once the asynchronous method(s) are done, close the deferral.
			//
			deferral.Complete();
		}
    }
}
