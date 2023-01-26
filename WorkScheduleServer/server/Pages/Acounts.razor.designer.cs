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
    public partial class AcountsComponent : ComponentBase
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
        protected RadzenDataGrid<WorkScheduleServer.Models.WorkScheduleDb.Acount> grid0;

        IEnumerable<WorkScheduleServer.Models.WorkScheduleDb.Acount> _getAcountsResult;
        protected IEnumerable<WorkScheduleServer.Models.WorkScheduleDb.Acount> getAcountsResult
        {
            get
            {
                return _getAcountsResult;
            }
            set
            {
                if (!object.Equals(_getAcountsResult, value))
                {
                    var args = new PropertyChangedEventArgs(){ Name = "getAcountsResult", NewValue = value, OldValue = _getAcountsResult };
                    _getAcountsResult = value;
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
            var workScheduleDbGetAcountsResult = await WorkScheduleDb.GetAcounts();
            getAcountsResult = workScheduleDbGetAcountsResult;
        }

        protected async System.Threading.Tasks.Task Button0Click(MouseEventArgs args)
        {
            var dialogResult = await DialogService.OpenAsync<AddAcount>("Add Acount", null);
            await grid0.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task Splitbutton0Click(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await WorkScheduleDb.ExportAcountsToCSV(new Query() { Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", OrderBy = $"{grid0.Query.OrderBy}", Expand = "", Select = "Id,Name,Password,Notes" }, $"Acounts");

            }

            if (args == null || args.Value == "xlsx")
            {
                await WorkScheduleDb.ExportAcountsToExcel(new Query() { Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", OrderBy = $"{grid0.Query.OrderBy}", Expand = "", Select = "Id,Name,Password,Notes" }, $"Acounts");

            }
        }

        protected async System.Threading.Tasks.Task Grid0RowSelect(WorkScheduleServer.Models.WorkScheduleDb.Acount args)
        {
            var dialogResult = await DialogService.OpenAsync<EditAcount>("Edit Acount", new Dictionary<string, object>() { {"Id", args.Id} });
            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async System.Threading.Tasks.Task GridDeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var workScheduleDbDeleteAcountResult = await WorkScheduleDb.DeleteAcount($"{data.Id}");
                    if (workScheduleDbDeleteAcountResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (System.Exception workScheduleDbDeleteAcountException)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error,Summary = $"Error",Detail = $"Unable to delete Acount" });
            }
        }
    }
}
