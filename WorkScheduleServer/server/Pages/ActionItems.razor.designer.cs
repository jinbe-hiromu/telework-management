using System;
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
    public partial class ActionItemsComponent : ComponentBase
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
        protected RadzenDataGrid<WorkScheduleServer.Models.WorkScheduleDb.ActionItem> grid0;

        IEnumerable<WorkScheduleServer.Models.WorkScheduleDb.ActionItem> _getActionItemsResult;
        protected IEnumerable<WorkScheduleServer.Models.WorkScheduleDb.ActionItem> getActionItemsResult
        {
            get
            {
                return _getActionItemsResult;
            }
            set
            {
                if (!object.Equals(_getActionItemsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getActionItemsResult", NewValue = value, OldValue = _getActionItemsResult };
                    _getActionItemsResult = value;
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
            var workScheduleDbGetActionItemsResult = await WorkScheduleDb.GetActionItems();
            getActionItemsResult = workScheduleDbGetActionItemsResult;
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            var dialogResult = await DialogService.OpenAsync<AddActionItem>("Add Action Item", null);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Splitbutton0Click(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await WorkScheduleDb.ExportActionItemsToCSV(new Query() { Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", OrderBy = $"{grid0.Query.OrderBy}", Expand = "", Select = "Id,From,To,EventName,Notes,BackgroudColor" }, $"Action Items");

            }

            if (args == null || args.Value == "xlsx")
            {
                await WorkScheduleDb.ExportActionItemsToExcel(new Query() { Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", OrderBy = $"{grid0.Query.OrderBy}", Expand = "", Select = "Id,From,To,EventName,Notes,BackgroudColor" }, $"Action Items");

            }
        }

        protected async System.Threading.Tasks.Task Grid0RowSelect(WorkScheduleServer.Models.WorkScheduleDb.ActionItem args)
        {
            var dialogResult = await DialogService.OpenAsync<EditActionItem>("Edit Action Item", new Dictionary<string, object>() { {"Id", args.Id} });
            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var workScheduleDbDeleteActionItemResult = await WorkScheduleDb.DeleteActionItem(data.Id);
                    if (workScheduleDbDeleteActionItemResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception workScheduleDbDeleteActionItemException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete ActionItem" });
            }
        }
    }
}
