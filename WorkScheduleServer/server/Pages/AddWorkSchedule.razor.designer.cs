﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using WorkScheduleServer.Models.WorkScheduleDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WorkScheduleServer.Models;

namespace WorkScheduleServer.Pages
{
    public partial class AddWorkScheduleComponent : ComponentBase
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, dynamic> Attributes { get; set; }

        public void Reload()
        {
            InvokeAsync(StateHasChanged);
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
        }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager UriHelper { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        protected WorkScheduleDbService WorkScheduleDb { get; set; }

        WorkScheduleServer.Models.WorkScheduleDb.WorkSchedule _workschedule;
        protected WorkScheduleServer.Models.WorkScheduleDb.WorkSchedule workschedule
        {
            get
            {
                return _workschedule;
            }
            set
            {
                if (!object.Equals(_workschedule, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "workschedule", NewValue = value, OldValue = _workschedule };
                    _workschedule = value;
                    OnPropertyChanged(args);
                    Reload();
                }
            }
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await Security.InitializeAsync(AuthenticationStateProvider);
            if (!Security.IsAuthenticated())
            {
                UriHelper.NavigateTo("Login", true);
            }
            else
            {
                await Load();
            }
        }
        protected async System.Threading.Tasks.Task Load()
        {
            workschedule = new WorkScheduleServer.Models.WorkScheduleDb.WorkSchedule(){};
        }

        protected async System.Threading.Tasks.Task Form0Submit(WorkScheduleServer.Models.WorkScheduleDb.WorkSchedule args)
        {
            try
            {
                var workScheduleDbCreateWorkScheduleResult = await WorkScheduleDb.CreateWorkSchedule(workschedule);
                DialogService.Close(workschedule);
            }
            catch (System.Exception workScheduleDbCreateWorkScheduleException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to create new WorkSchedule!" });
            }
        }

        protected async System.Threading.Tasks.Task Button2Click(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}
