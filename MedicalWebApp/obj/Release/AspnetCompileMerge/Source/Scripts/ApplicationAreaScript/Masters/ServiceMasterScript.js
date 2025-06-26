var antiForgeryToken = $("input[name=__RequestVerificationToken]").val();
$.ajax({
    type: "GET",
    traditional: true,
    url: "/PatientTypeMaster/GetAllActivePatientTypeMaster",
    headers: {
        "__RequestVerificationToken": antiForgeryToken
    },
    success: function (response) {
        $.each(response,
            function (index, value) {
                $('#ddlPatientType').append('<option value="' + value.PatientTypeId + '">' + value.PatientType + '</option>');
            });
    }
});
$.ajax({
    type: "GET",
    traditional: true,
    url: "/ServiceTypeMaster/GetAllActiveServiceTypeMaster",
    headers: {
        "__RequestVerificationToken": antiForgeryToken
    },
    success: function (response) {
        $.each(response,
            function (index, value) {
                $('#ddlService').append('<option value="' + value.ServiceTypeId + '">' + value.ServiceType + '</option>');
            });
    }
});

function afterFail() {
    ShowAlert("error", "Something went wrong If not resolve contact to your superior");
}

loadSearchgrid();

function searchLoadCall() {
}

var dataSearchGrid = { location: "local" };
var colSearchGrid = [
        { title: "", dataIndx: "ServiceTypeId", dataType: "integer", hidden: true },
        { title: "", dataIndx: "ServiceId", dataType: "integer", hidden: true },
        { title: "", dataIndx: "CghsCode", hidden: true },
        { title: "Service Type", dataIndx: "ServiceType", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        { title: "Service Name", dataIndx: "ServiceName", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        { title: "Code", dataIndx: "Code", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
        { title: "Seq No", dataIndx: "Sequence", hidden: true , width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } }
];
var setSearchGrid = gridCommonObject;
setSearchGrid.title = 'Service Master';
setSearchGrid.width = '100%';
setSearchGrid.height = 500;
setSearchGrid.colModel = colSearchGrid;
setSearchGrid.dataModel = dataSearchGrid;
setSearchGrid.pageModel = false;
setSearchGrid.rowClick = function (evt, ui) {
    if (ui.rowData) {
        var rowIndx = parseInt(ui.rowIndx);
        var details = ui.rowData;
        debugger;
        $("#ddlService").val(details.ServiceTypeId);
        $("#ServiceId").val(details.ServiceId);
        $("#Code").val(details.Code);
        $("#ServiceName").val(details.ServiceName);
        $("#Sequence").val(details.Sequence);
        $("#CghsCode").val(details.CghsCode);
        $("#ddlPatientgender").val(details.GenderId);
        $("#ddlPatientType").val(details.PatientTypeId);
        $("#Deactive").prop('checked', details.Deactive);
        $("#AllowToChangeRate").prop('checked', details.AllowToChangeRate);
        $("#Default").prop('checked', details.Default);
        $("#Qty").val(details.Qty);
        $("#Surgery").prop('checked', details.Surgery);
        $("#NoOfDays").val(details.NoOfDays);
        GetLinkedGenderWithService(details.ServiceId);
    }
}
var $SearchGrid = $("#searchgrid").pqGrid(setSearchGrid);


$("#btnSave").click(function () {
    if (!showAlertOnBlank($("#ddlService"), "Service Type is missing! Please enter Service Type")) return;
    if (!showAlertOnBlank($("#Code"), "Service Code is missing! Please enter Service Code")) return;
    if (!showAlertOnBlank($("#ServiceName"), "Service Name is missing! Please enter Service Name")) return;
    var getData = $("#GenderLinkingGrid").pqGrid("option", "dataModel.data");
    var selectedItems = jLinq.from(getData).equals("State", true).select();
    if (selectedItems.length <= 0) {
        ShowAlert('info', 'No Gender linked with this service');
        return;
    }
    DisableClick("btnSave");
    var jsonData = JSON.stringify({
        ServiceTypeId: $("#ddlService").val(),
        ServiceId: $("#ServiceId").val(),
        Code: $("#Code").val(),
        ServiceName: $("#ServiceName").val(),
        Sequence: $("#Sequence").val(),
        CghsCode: $("#CghsCode").val(),
        GenderId: $("#ddlPatientgender").val(),
        PatientTypeId: $("#ddlPatientType").val(),
        Deactive: $("#Deactive").prop('checked'),
        AllowToChangeRate: $("#AllowToChangeRate").prop('checked'),
        Default: $("#Default").prop('checked'),
        Qty: $("#Qty").val(),
        Surgery: $("#Surgery").prop('checked'),
        NoOfDays: $("#NoOfDays").val(),
        ServiceGender: selectedItems
    });
    antiForgeryToken = $("input[name=__RequestVerificationToken]").val();
    $.ajax({
        type: "POST",
        traditional: true,
        contentType: 'application/json; charset=utf-8',
        url: "/ServiceMaster/SaveServicemaster",
        data: jsonData,
        headers: {
            "__RequestVerificationToken": antiForgeryToken
        },
        success: function (msg) {
            debugger;
            if (msg.success) {
                ShowAlert("success", "Record Saved Successfully");
                ClearAllField();
                loadSearchgrid();
            }
            else {
                ShowAlert("warning", msg.Message);
            }
        },
        error: function (a, exception, b) {
            
        }
    });

});
$("#btnAdd").click(function () {
    ClearAllField();
});

function ClearAllField() {
    ClearAllControl("EntryArea");
    $("#Deactive").prop('checked', false);
    $("#AllowToChangeRate").prop('checked', false);
    $("#Default").prop('checked', false);
    $("#Surgery").prop('checked', false);
    loadGenderMaster();
}
function loadSearchgrid() {
    antiForgeryToken = $("input[name=__RequestVerificationToken]").val();
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/ServiceMaster/GetAllServiceMaster",
        headers: {
            "__RequestVerificationToken": antiForgeryToken
        },
        success: function (response) {            
            $("#searchgrid").pqGrid("hideLoading");
            $("#searchgrid").pqGrid("option", "dataModel.data", response);
            $("#searchgrid").pqGrid("refreshDataAndView");
        },

        error: function (a, exception, b) {            
        }
    });
}

var dataLinkingGrid = { location: "local" };
var colLinkingGrid = [
    {
        dataIndx: "State", Width: 15, align: "center", type: 'checkBoxSelection', cls: 'ui-state-default', sortable: false,
        editor: false, dataType: 'bool',
        title: "<input type='checkbox' />",
        cb: { select: true, all: false, header: true }
    },
    { title: "", dataIndx: "Id", dataType: "integer", hidden: true },
    { title: "Gender", dataIndx: "Name", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
];
var setLinkingGrid = {
    width: '100%',
    height: 200,
    sortable: false,
    numberCell: { show: true },
    hoverMode: 'cell',
    showTop: false,
    title: 'Gender',
    resizable: true,
    scrollModel: { autoFit: true },
    selectionModel: { type: 'cell' },
    colModel: colLinkingGrid,
    dataModel: dataLinkingGrid,
    swipeModel: { on: true },
    virtualX: false,
    virtualY: false,
}
var $LinkingGrid = $("#GenderLinkingGrid").pqGrid(setLinkingGrid);
loadGenderMaster();
function loadGenderMaster() {
    antiForgeryToken = $("input[name=__RequestVerificationToken]").val();
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/CommonMaster/Gender",
        headers: {
            "__RequestVerificationToken": antiForgeryToken
        },
        success: function(response) {            
            $("#GenderLinkingGrid").pqGrid("hideLoading");
            $("#GenderLinkingGrid").pqGrid("option", "dataModel.data", response);
            $("#GenderLinkingGrid").pqGrid("refreshDataAndView");
        },
        error: function(a, exception, b) {
        }
    });
}

function GetLinkedGenderWithService(param) {
    antiForgeryToken = $("input[name=__RequestVerificationToken]").val();
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/ServiceMaster/GetAllLinkedGenderByServiceId",
        headers: {
            "__RequestVerificationToken": antiForgeryToken
        },
        data: { ServiceId :param},
        success: function (response) {
            debugger;
            $("#GenderLinkingGrid").pqGrid("hideLoading");
            $("#GenderLinkingGrid").pqGrid("option", "dataModel.data", response);
            $("#GenderLinkingGrid").pqGrid("refreshDataAndView");
        },
        error: function (a, exception, b) {
            debugger;
        }
    });
}