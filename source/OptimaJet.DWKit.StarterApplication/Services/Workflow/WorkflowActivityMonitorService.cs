﻿using System;
using System.Linq;
using System.Threading.Tasks;
using OptimaJet.Workflow.Core.Persistence;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.DWKit.StarterApplication.Utility;
using Serilog;
using OptimaJet.DWKit.StarterApplication.Repositories;
using OptimaJet.DWKit.StarterApplication.Models;
using Microsoft.EntityFrameworkCore;
using Hangfire;

namespace OptimaJet.DWKit.StarterApplication.Services.Workflow
{
    public class WorkflowActivityMonitorService
    {
        public void RegisterEventHandlers(WorkflowRuntime runtime)
        {
            runtime.ProcessActivityChanged += (sender, args) => { ActivityChanged(args, runtime); };
            runtime.ProcessStatusChanged += (sender, args) => { ProcessChanged(args, runtime); };
        }

        private void ActivityChanged(ProcessActivityChangedEventArgs args, WorkflowRuntime runtime)
        {
            if (!args.TransitionalProcessWasCompleted)
                return;

            Log.Information($":::::::::: ActivityChanged: pid={args.ProcessId.ToString()}, scheme={args.SchemeCode}, activity={args.CurrentActivityName}, state={args.CurrentState}, last={args.PreviousState}");
            var serviceArgs = new WorkflowProductService.ProductProcessChangedArgs
            {
                ProcessId = args.ProcessId,
                CurrentActivityName = args.CurrentActivityName,
                PreviousActivityName = args.PreviousActivityName,
                CurrentState = args.CurrentState,
                PreviousState = args.PreviousState,
                ExecutingCommand = args.ProcessInstance.CurrentCommand
            };
            // There is a bit of a timing window before the ProcessId is assigned to the Product.  So delay this a little bit (15 seconds in the default minimum time.
            BackgroundJob.Enqueue<WorkflowProductService>(service => service.ProductProcessChanged(serviceArgs));

            //TODO change Document transition history and WorkflowInbox
        }

        private void ProcessChanged(ProcessStatusChangedEventArgs args, WorkflowRuntime runtime)
        {
            Log.Information($":::::::::: ProcessChanged: pid={args.ProcessId.ToString()}, scheme={args.SchemeCode}, new={args.NewStatus.StatusString()}, old={args.OldStatus.StatusString()}");
            if (String.IsNullOrWhiteSpace(args.ProcessInstance.CurrentActivityName)) return;
        }

        public void CheckActivityStatus()
        {
        }
    }
}
