
namespace Client.Services;

using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorClient.Models;
using HandyBlazorComponents.Abstracts;
using HandyBlazorComponents.Models;
using Microsoft.AspNetCore.Components;
using Shared.Models;
using static HandyBlazorComponents.Models.HandyServiceResponses;


public class TransactionsGridStateService : HandyGridStateAbstract<TransactionsEntity, StreamlinedTransactionDTO>
{
    public TransactionsGridStateService(List<TransactionsEntity> Items, int PageSize = 5, bool CanAddNewItems = true, string? ExampleFileUploadUrl = null, string AddNewItemsText = "Add New Items", bool Exportable = false, bool IsReadonly = true, bool ShowRowIndex = true, bool ShowFilters = true, Func<IEnumerable<TransactionsEntity>, Task>? OnCreate = null, Func<TransactionsEntity, Task>? OnUpdate = null, Func<TransactionsEntity, Task>? OnDelete = null, Func<IEnumerable<TransactionsEntity>, Task>? OnSubmitFile = null, List<string>? ColumnsToHide = null, List<string>? ReadonlyColumns = null, List<HandyNamedRenderFragment<TransactionsEntity>>? ViewModeFragments = null, List<HandyNamedRenderFragment<TransactionsEntity>>? EditModeFragments = null) : base(Items, PageSize, CanAddNewItems, ExampleFileUploadUrl, AddNewItemsText, Exportable, IsReadonly, ShowRowIndex, ShowFilters, OnCreate, OnUpdate, OnDelete, OnSubmitFile, ColumnsToHide, ReadonlyColumns, ViewModeFragments, EditModeFragments)
    {
    }

    public override HandyGridValidationResponse ValidationChecks(TransactionsEntity item)
    {
        return new HandyGridValidationResponse(Flag: true, null);
    }


}
