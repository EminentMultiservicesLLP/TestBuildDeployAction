//$('#AdmissionDate').datetimepicker({ format: 'DD-MMM-YYYY HH:mm:ss A', extraFormats: ['DD-MM-YYYY', 'DD-MM-YY'], defaultDate: new Date() });
//$('#DischargeDate').datetimepicker({ format: 'DD-MMM-YYYY HH:mm:ss A', extraFormats: ['DD-MM-YYYY', 'DD-MM-YY'], defaultDate: new Date() });
$("#desableIdForAllControl :input").prop("disabled", true);

var btnSelectService;
ClearAllSession();
GetAllgeneratedRequest();
$("#RequestNo").attr("disabled", "disabled");
var arrRoomType = [];
var arrServiceType = [];
var arrOTServiceType = [];


var dataSearchGrid = { location: "local" };
var colSearchGrid = [
    { title: "", dataIndx: "RequestId", dataType: "integer", hidden: true },
    { title: "Request No", dataIndx: "RequestNo", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
    { title: "File No", dataIndx: "FileNo", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
    { title: "Patient Name", dataIndx: "PatientName", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
    { title: "IPD No", dataIndx: "IpdNo", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } }
];


var setSearchGrid = {
    width: '100%',
    height: 300,
    sortable: false,
    numberCell: { show: true },
    hoverMode: 'cell',
    showTop: true,
    title: 'Generated Request No',
    resizable: true,
    scrollModel: { autoFit: true },
    draggable: true,
    wrap: false,
    filterModel: { off: false, mode: "AND", header: true },
    editable: false,
    selectionModel: { type: 'cell' },
    colModel: colSearchGrid,
    dataModel: dataSearchGrid,
    swipeModel: { on: true },
    virtualX: false,
    virtualY: false,
    rowClick: function (evt, ui) {
        var rowIndx = parseInt(ui.rowIndx);
        var details = ui.rowData;
        $("#RequestId").val(details.RequestId);
        $("#RequestNo").val(details.RequestNo);
        $("#FileNo").val(details.FileNo);
        $("#ddlHospitalType").val(details.HospitalTypeId);
        $("#IpdNo").val(details.IpdNo);
        $("#ddlMangementType").val(details.ManagementTypeId);
        $("#PatientName").val(details.PatientName);
        $("#LifesavingMdcnAmt").val(details.LifesavingMdcnAmt);
        $("#PatientAge").val(details.PatientAge);
        $("#ddlPatientgender").val(details.GenderId);
        $("#PatientAddress").val(details.PatientAddress);
        $("#DrugsAmount").val(details.DrugsAmount);
        $("#LeftDcDetail").val(details.LeftDcDetail);
        $("#ddlPatientgender").val(details.GenderId);
        $("#RightDcDetail").val(details.RightDcDetail);
        $("#ddlPatientType").val(details.PatientTypeId);
        $("#balance").val(details.BillAmount);
        $("#ddlState").val(details.StateId);
        $("#ddlTypeOfAddmission").val(details.TypeOfAddmissionId);
        GetRequestDetailById(details.RequestId);
        GetAdmissionDetailById(details.RequestId);
        GetRequestPharmacyDetailById(details.RequestId);
        fnShowAttachment(details.RequestId);
        GetRequestOtDetailById(details.RequestId);
        rowclickShowcity(details.StateId, details.CityId);
        if (details.ManagementTypeId == 1) $("#btnOTDetailleft").prop("disabled", true);
        else { $("#btnOTDetailleft").prop("disabled", false); }
    }
}
var $SearchGrid1 = $("#GeneratedRequestGrid").pqGrid(setSearchGrid);



/*--------------------Admission Grid------------------------*/

var datatAdmissionDetaisGrid = { location: "local" };
var coltAdmissionDetaisGrid = [
    { title: "", dataIndx: "RequestId", dataType: "integer", hidden: true },
    { title: "", dataIndx: "AdmissionDtlId", dataType: "integer", hidden: true },
    { title: "", dataIndx: "ServiceId", dataType: "integer", hidden: true },
    { title: "In DateTime", dataIndx: "StrAdmissionDateTime", width: 400, editor: { type: gridDateTimeEditor } },
    {
        title: "Out DateTime", dataIndx: "StrDischargeDateTime", width: 400,
        editor: { type: gridDateTimeEditor }
    },
    { title: "", dataIndx: "RoomTypeId", dataType: "integer", hidden: true },
    {
        title: "Room Type", dataIndx: "RoomType", width: 400,
        editor: {
            type: "select",
            mapIndices: { "value": "RoomTypeId", "text": "RoomType" },
            valueIndx: "value",
            labelIndx: "text",
            options: arrRoomType
        }
    },
    {
        title: "Service", dataIndx: "ServiceName", width: 400,
        editor: {
            type: "select",
            mapIndices: { "value": "ServiceId", "text": "ServiceName" },
            valueIndx: "value",
            labelIndx: "text",
            options: arrServiceType
        }
    },
];
var setAdmissionDetaisGrid = gridCommonObject;
setAdmissionDetaisGrid.title = 'Add Admision Details';
setAdmissionDetaisGrid.width = '100%';
setAdmissionDetaisGrid.height = 350;
setAdmissionDetaisGrid.colModel = coltAdmissionDetaisGrid;
setAdmissionDetaisGrid.dataModel = datatAdmissionDetaisGrid;
setAdmissionDetaisGrid.editable = true;
setAdmissionDetaisGrid.filterModel = false;
setAdmissionDetaisGrid.pageModel = { type: "local", rPP: 100 };
setAdmissionDetaisGrid.postRenderInterval=-1;
setAdmissionDetaisGrid.cellBeforeSave = function (event, ui) {
    var dataIndx = ui.dataIndx,
        newVal = ui.newVal;
    var data = ui.rowData;
    if (dataIndx === 'StrDischargeDateTime') {
        if (new Date(Date.parse(newVal)) < new Date(Date.parse(data.StrAdmissionDateTime))) {
            ShowAlert('info', "Discharge date time is less than Admission datetime");
            return;
        }
    }
    else if (dataIndx === 'StrAdmissionDateTime') {
        var getData = $("#AdmissionDetaisPoPupModalGrid").pqGrid("option", "dataModel.data");
        if (getData.length > 1) {
            var previousEndDate;
            $.each(getData,
                function(key, value) {
                    if (key > 0) {
                        if (new Date(Date.parse(newVal)) < previousEndDate) {
                            ShowAlert('info', "This date time cannot less than last datetime");
                            return;
                        }
                    }
                    previousEndDate = new Date(Date.parse(value.StrDischargeDateTime));
                });
        }
    }
    else if (dataIndx === 'RoomType') {
        GetServiceMasterByCategoryRoomId(newVal.RoomTypeId);
    }
};
var $AdmisionDetailsGrid = $("#AdmissionDetaisPoPupModalGrid").pqGrid(setAdmissionDetaisGrid);

function GetServiceMasterByCategoryRoomId(param) {
    //arrServiceType = [];, gender: $("#ddlPatientgender").val()
    $.ajax({
        type: "GET",
        url: "/RequestSubmission/GetServiceMasterByCategoryRoomId",
        data: { categoryId: 6, hospitalType: $("#ddlHospitalType").val(), patientType: $("#ddlPatientType").val(), roomTypeId: param, stateId: $("#ddlState").val(), cityId: $("#ddlCity").val(), gender: $("#ddlPatientgender").val() },
        success: function (data) {
            $.each(data,
                function (index, value) {
                    arrServiceType.push({ value: value.ServiceId, text: value.ServiceName });
                });
        }
    });
}

$('#AddAdmisionDetails').on('click', function () {
    if (!showAlertOnBlank($("#ddlHospitalType"), "Hospital Type is missing! Please Select Hospital type")) return;
    if (!showAlertOnBlank($("#ddlPatientType"), "Patient Type is missing! Please Select Patient type")) return;
    $("#AdmissionDetaisPoPupModal").dialog({
        height: 450,
        width: 700,
        modal: true,
        open: function (evt, ui) {
            var reqId = $("#RequestId").val();
            var getData = $("#AdmissionDetaisPoPupModalGrid").pqGrid("option", "dataModel.data");
            if (parseInt(reqId) > 0 && getData.length<=0) GetAdmissionDetailById(reqId);
            else if(getData.length<=0){
                $("#AdmissionDetaisPoPupModalGrid").pqGrid("refreshDataAndView");
            }
        }
    });
});
/********* Add Item to  Detail grid*********/
$('#btnAddRowAdmissionDetais').on('click', function () {
    var dataMGrid = [];
    var getData = $("#AdmissionDetaisPoPupModalGrid").pqGrid("option", "dataModel.data");
    var lastRowData, grdnewData;
    $.each(getData, function (key, value) {
        if (key === getData.length - 1) lastRowData = value;
        var grdData = {
            'RequestId': value.RequestId, 'AdmissionDtlId': value.AdmissionDtlId, 'RoomTypeId': value.RoomTypeId, 'RoomType': value.RoomType, 'ServiceId': value.ServiceId, 'ServiceName': value.ServiceName, 'StrAdmissionDateTime': value.StrAdmissionDateTime, 'StrDischargeDateTime': value.StrDischargeDateTime
        }
        dataMGrid.push(grdData);
    });
    if (getData.length > 0) {
        grdnewData = {
            'RequestId': 0,
            'AdmissionDtlId': 0,
            'RoomTypeId': 0,
            'RoomType': "",
            'StrAdmissionDateTime': lastRowData.StrDischargeDateTime,
            'StrDischargeDateTime': "",
            'ServiceId': 0,
            'ServiceName': "",
        }
    } else {
        grdnewData = {
            'RequestId': 0,
            'AdmissionDtlId': 0,
            'RoomTypeId': 0,
            'RoomType': "",
            'StrAdmissionDateTime': "",
            'StrDischargeDateTime': "",
            'ServiceId': 0,
            'ServiceName': "",
        }
    }
        dataMGrid.push(grdnewData);
        $("#AdmissionDetaisPoPupModalGrid").pqGrid("option", "dataModel.data", dataMGrid);
        $("#AdmissionDetaisPoPupModalGrid").pqGrid("refreshDataAndView");
});

$.ajax({
    type: "POST",
    traditional: true,
    contentType: 'application/json; charset=utf-8',
    url: '/RequestSubmission/ServicesConsumedPartialLeftDiv', // Controller/View   
    //data: dd,
    dataType: "html",
    success: function (response) {
        $('#ServicesConsumedpartialView').html(response);
        $('#ServicesConsumedpartialView').dialog('open');
    },
    error: function (a,b,response) {
        ShowAlert("error", "error");
    },
   
});
$.ajax({
    type: "POST",
    traditional: true,
    contentType: 'application/json; charset=utf-8',
    url: '/RequestSubmission/ServicesConsumedDivRightPane', // Controller/View   
    //data: dd,
    dataType: "html",
    success: function (response) {
        $('#ServicesConsumedpartialViewRight').html(response);
        $('#ServicesConsumedpartialViewRight').dialog('open');
    },
    error: function (a, b, response) {
        ShowAlert("error", "ASDFAEW33333");
    },

});



/*--------------------ot details Grid------------------------*/
$(document).ready(function () {
    var dataModel = { location: 'remote', sorting: 'local', paging: 'local', dataType: 'JSON' };
    $gridMain = $("div#OTdetailleftPoPupModalGrid").pqGrid({
        height: 400,
        sortable: false,
        editable: false,
        numberCell: { show: true },
        //selectionModel: { type: 'row' },
        showTop: true,
        title: "Surgical Details",
        hoverMode: 'cell',
        resizable: true,
        virtualX: true,
        virtualY: true,
        scrollModel: { autoFit: true },
       
        draggable: false,
        wrap: false,
        colModel: [
            { title: "", minWidth: 30, maxWidth: 30, type: "detail", dataIndx: "detail", resizable: false, sortable: false },
            { title: "", dataIndx: "RequestId", dataType: "integer", hidden: true },
            { title: "", dataIndx: "AdmissionDtlId", dataType: "integer", hidden: true },
            { title: "", dataIndx: "ServiceId", dataType: "integer", hidden: true },
            { title: "In DateTime", dataIndx: "StrAdmissionDateTime", width: 400, editor: { type: gridDateEditor } },
            {
                title: "Service", dataIndx: "ServiceName", width: 400,
                editor: {
                    type: "select",
                    mapIndices: { "value": "ServiceId", "text": "ServiceName" },
                    valueIndx: "value",
                    labelIndx: "text",
                    options: arrOTServiceType
                }
            },
        ],
        dataModel: dataModel,
        scrollModel: { autoFit: true },
        trackModel: { on: true }, //to turn on the track changes.
        pageModel: { type: "local", rPP: 50 },
        detailModel: {
            cache: true,
            collapseIcon: "ui-icon-plus",
            expandIcon: "ui-icon-minus",
            init: function (ui) {
                var rowData = ui.rowData,
                    detailobj = gridDetailModel($(this), rowData), //get a copy of gridDetailModel
                    $grid = $("<div id=detailgrid></div>").pqGrid(detailobj); //init the detail grid.
                return $grid;
            }
        }
    });

    var gridDetailModel = function ($gridMain, rowData) {
        return {
            width: '90%',
            scrollModel: { autoFit: true },
            height: 130,
            pageModel: { type: "local", rPP: 5, strRpp: "" },
            selectionModel: { type: 'row' },
            dataModel: {
                location: "remote",
                sorting: "local",
                dataType: "json",
                method: "GET",
                url: "/AutoServiceAllocation/GetLinkedServicesByServiceTypeServiceId/?ServiceId=" + rowData.ServiceId + "&hospitalType=" + $("#ddlHospitalType").val() + "&patientType=" + $("#ddlPatientType").val()
                    + "&stateId=" + $("#ddlState").val() + "&cityId=" + $("#ddlCity").val() + "&gender=" + $("#ddlPatientgender").val(),
                getData: function (dataJSON) {
                    var data = dataJSON;
                    return { curPage: dataJSON.curPage, totalRecords: dataJSON.totalRecords, data: data };
                }
            },
            colModel: [
                { title: "", dataIndx: "ServiceId", dataType: "integer", hidden: true },
                { title: "Service Name", dataIndx: "ServiceName", width: 250, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
                { title: "Code", hidden: true, dataIndx: "Code", width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
                { title: "", dataIndx: "AllowToChangeRate", width: 100, dataType: "integer", hidden: false },
                { title: "Rate", dataIndx: "BillRate", width: 100, dataType: "integer", hidden: false },
                { title: "Change Rate", width: 150, dataIndx: "ChangeBillRate", dataType: "integer", hidden: false },
            ],
            editable: true,
            groupModel: {
                dataIndx: ["ServiceName"],
                dir: ["up"],
                title: ["{0} - Total : {1}"],
                icon: [["ui-icon-triangle-1-se", "ui-icon-triangle-1-e"]]
            },
            flexHeight: true,
            flexWidth: true,
            numberCell: { show: false },
            title: "Service Details",
            showTop: false,
            showBottom: false
        };
    };
});

//var dataLeftDetaisGrid = { location: 'local' };

//var colLeftDetaisGrid = [
//            { title: "", dataIndx: "RequestId", dataType: "integer", hidden: true },
//            { title: "", dataIndx: "AdmissionDtlId", dataType: "integer", hidden: true },
//            { title: "", dataIndx: "ServiceId", dataType: "integer", hidden: true },
//            { title: "In DateTime", dataIndx: "StrAdmissionDateTime", width: 400, editor: { type: gridDateEditor } },
            
//            {
//                title: "Service", dataIndx: "ServiceName", width: 400,
//                editor: {
//                    type: "select",
//                    mapIndices: { "value": "ServiceId", "text": "ServiceName" },
//                    valueIndx: "value",
//                    labelIndx: "text",
//                    options: arrOTServiceType
//                }
//            },
//        ]

//var setLeftDetaisGrid = gridCommonObject;
//setLeftDetaisGrid.title = 'Add OT Details';
//setLeftDetaisGrid.width = '100%';
//setLeftDetaisGrid.height = 350;
//setLeftDetaisGrid.colModel = colLeftDetaisGrid;
//setLeftDetaisGrid.dataModel = dataLeftDetaisGrid;
//setLeftDetaisGrid.editable = true;
//setLeftDetaisGrid.pageModel = { type: "local", rPP: 100 };
//setLeftDetaisGrid.filterModel = false;
//setLeftDetaisGrid.postRenderInterval = -1;

//var $LeftOTDetailsGrid = $("#OTdetailleftPoPupModalGrid").pqGrid(setLeftDetaisGrid);


/*--------------------OT Grid------------------------*/

var dataRightOTDetaisGrid = { location: "local" };
var colRightOTDetaisGrid = [
    { title: "", dataIndx: "RequestId", dataType: "integer", hidden: true },
    { title: "", dataIndx: "AdmissionDtlId", dataType: "integer", hidden: true },
    { title: "", dataIndx: "ServiceId", dataType: "integer", hidden: true },
    { title: "In DateTime", dataIndx: "StrAdmissionDateTime", width: 400, editor: { type: gridDateEditor } },
   
    {
        title: "Service", dataIndx: "ServiceName", width: 400,
        editor: {
            type: "select",
            mapIndices: { "value": "ServiceId", "text": "ServiceName" },
            valueIndx: "value",
            labelIndx: "text",
            options: arrOTServiceType
        }
    },
    
];
var setRightOTDetaisGrid = gridCommonObject;
setRightOTDetaisGrid.title = 'Add Admision Details';
setRightOTDetaisGrid.width = '100%';
setRightOTDetaisGrid.height = 350;
setRightOTDetaisGrid.colModel = colRightOTDetaisGrid;
setRightOTDetaisGrid.dataModel = dataRightOTDetaisGrid;
setRightOTDetaisGrid.editable = true;
setRightOTDetaisGrid.filterModel = false;
setRightOTDetaisGrid.postRenderInterval = -1;
setRightOTDetaisGrid.pageModel = { type: "local", rPP: 100 };
var $RightOTDetailsGrid = $("#OTdetailRightPoPupModalGrid").pqGrid(setRightOTDetaisGrid);

/*-----------------------------Pharmacy Detail grid-------------------------------- */

var dataPharmacyDetaisGrid = { location: "local" };
var colPharmacyDetaisGrid = [
    { title: "", dataIndx: "RequestId", dataType: "integer", hidden: true },
    { title: "", dataIndx: "PharmacyDtlId", dataType: "integer", hidden: true },
    { title: "DateTime", dataIndx: "ConsumeDate", width: 400, editor: { type: gridDateTimeEditor } },
    { title: "Amount", dataIndx: "BillRate", width: 400 },
];
var setPharmacyDetaisGrid = gridCommonObject;
setPharmacyDetaisGrid.title = 'Add Pharmacy Details';
setPharmacyDetaisGrid.width = '100%';
setPharmacyDetaisGrid.height = 350;
setPharmacyDetaisGrid.colModel = colPharmacyDetaisGrid;
setPharmacyDetaisGrid.dataModel = dataPharmacyDetaisGrid;
setPharmacyDetaisGrid.editable = true;
setPharmacyDetaisGrid.filterModel = false;
setPharmacyDetaisGrid.pageModel = { type: "local", rPP: 100 };
var $PharmacyDetailsGrid = $("#PharmacyServicePoPupModalGrid").pqGrid(setPharmacyDetaisGrid);

$('#btnPhamacyDetails').on('click', function () {
    $("#PharmacyServicePoPupModal").dialog({
        height: 450,
        width: 700,
        modal: true,
        open: function (evt, ui) {
            var reqId = $("#RequestId").val();
            var getData = $("#PharmacyServicePoPupModalGrid").pqGrid("option", "dataModel.data");
            if(parseInt(reqId)>0 && getData.length<=0)GetRequestPharmacyDetailById(reqId);
        }
    });
});

/*-----------------------Popup grid Area----------------------*/
var datatpopupServiceGridPartial = { location: "local" };
var coltpopupServiceGridPartial = [
    {
        dataIndx: "State", Width: 15, align: "center", type: 'checkBoxSelection', cls: 'ui-state-default', sortable: false,
        editor: false, dataType: 'bool',
        title: "<input type='checkbox' />",
        cb: { select: true, all: false, header: true },
        editable: function (ui) {
            if (ui.rowData != undefined) {
                var column = this.getColumn({ dataIndx: 'ConsumeDate' }),
                    obj = ui.rowData.ConsumeDate,
                    val = column.val;
                if (obj === null || obj === "") {
                    return false;
                }
                else
                    return true;
            }
        }
    },
    { title: "In DateTime", dataIndx: "ConsumeDate", width: 400, editor: { type: gridDateTimeEditor } },
    { title: "", dataIndx: "ServiceTypeId", dataType: "integer", hidden: true, editable: false },
    { title: "", dataIndx: "BillRate", dataType: "integer", hidden: true, editable: false },
    { title: "", dataIndx: "ServiceId", dataType: "integer", hidden: true },
    { title: "", dataIndx: "RoomTypeId", dataType: "integer", hidden: true },
    { title: "Room Type", dataIndx: "RoomType", editable: false, width: 350, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
    { title: "Service Type", dataIndx: "ServiceType", editable: false, width: 350, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
    { title: "Service Name", dataIndx: "ServiceName", editable: false, width: 400, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
    { title: "Code", dataIndx: "Code", editable: false, width: 200, filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] } },
    {
        title: "Qty", dataIndx: "Qty", dataType: "integer", hidden: false,editable:true,
        postRender: function (ui) {
            UpdateQty(ui);
        }
    }
];
var setpopupServiceGridPartial = {
    width: '100%',
    height: 510,
    sortable: false,
    numberCell: { show: true },
    hoverMode: 'cell',
    showTop: true,
    title: 'Service Master Data',
    resizable: true,
    scrollModel: { autoFit: true },
    draggable: true,
    wrap: false,
    filterModel: { off: false, mode: "AND", header: true },
    pageModel : { type: "local", rPP: 100 },
    editable: true,
    selectionModel: { type: 'cell' },
    colModel: coltpopupServiceGridPartial,
    dataModel: datatpopupServiceGridPartial,
    swipeModel: { on: true },
    virtualX: false,
    virtualY: false,
    postRenderInterval: -1,
    check : function(evt, ui) {
        getCheckServices(ui);
    }
}
var $popupServiceGrid = $("#ServicePoPupModalGrid").pqGrid(setpopupServiceGridPartial);


function LoadServicePoPupModal(gridName) {
    debugger;
    onFocusOfElement(gridName);
    if (!showAlertOnBlank($("#ddlHospitalType"), "Hospital Type is missing! Please Select Hospital type")) return;
    if (!showAlertOnBlank($("#ddlPatientType"), "Patient Type is missing! Please Select Patient type")) return;
    if (!showAlertOnBlank($("#ddlState"), "State is missing! Please Select state")) return;
    if (!showAlertOnBlank($("#ddlCity"), "Cityt  is missing! Please Select city type")) return;
    if (!showAlertOnBlank($("#ddlPatientgender"), "Gender  is missing! Please Select Gender type")) return;
    btnSelectService = gridName;
    var sendData = gridName.split("_");
    $("#ServicePoPupModal").dialog({
        height: 600,
        width: 850,
        modal: true,
        open: function (evt, ui) {
            $.ajax({
                type: "GET",
                data: { sessionName: gridName, categoryName: sendData[1], hospitalType: $("#ddlHospitalType").val(), patientType: $("#ddlPatientType").val(), stateId: $("#ddlState").val(), cityId: $("#ddlCity").val(), gender: $("#ddlPatientgender").val() },
                url: "/RequestSubmission/DisplayServicesConsumedSession",
                datatype: "Json",
                success: function (data) {
                    ClearParamGrid('ServicePoPupModalGrid');
                    $("#ServicePoPupModalGrid").pqGrid("option", "dataModel.data", data.sessionRecord);
                    $("#ServicePoPupModalGrid").pqGrid("refreshDataAndView");
                }
            });
        }
    });
}

function getCheckServices(ui) {
    if (ui.rowData) {
        var data = ui.rowData;
        $.ajax({
            type: "GET",
            data: { sessionName: btnSelectService, serviceId: data.ServiceId, state: data.State, rate: data.BillRate, roomType: data.RoomTypeId, qty: data.Qty, date: data.ConsumeDate },
            url: "/RequestSubmission/AddServicesConsumedSession",
            datatype: "Json",
            success: function (data) {
            }
        });
    }
}
function UpdateQty(ui) {
    if (ui.rowData) {
        var data = ui.rowData;
        $.ajax({
            type: "GET",
            data: { sessionName: btnSelectService, serviceId: data.ServiceId, state: data.State, rate: data.BillRate, roomType: data.RoomTypeId, qty: data.Qty },
            url: "/RequestSubmission/UpdateQty",
            datatype: "Json",
            success: function (data) {

            }
        });
    }
}
$('#btnShowBillAmt').on('click', function () {
    $.ajax({
        type: "POST",
        traditional: true,
        contentType: 'application/json; charset=utf-8',
        url: '/RequestSubmission/CalculateGenralBill', // Controller/View    
        data: jsonstring(),
        success: function (bill) {
            $("#balance").val(bill.billAmount);
        }
    });
});
/*-------------------------Save Function from Here-----------------------*/
$('#btnSave').on('click', function () {
    if (!showAlertOnBlank($("#FileNo"), "FileNo is missing! Please enter FileNo")) return;
    if (!showAlertOnBlank($("#ddlHospitalType"), "Hospital type is missing! Please select Hospital Type")) return;
    if (!showAlertOnBlank($("#PatientName"), "Patient Name is missing! Please enter Patient Name")) return;
    if (!showAlertOnBlank($("#PatientAge"), "Patient Age is missing! Please enter Patient Age")) return;
    if (!showAlertOnBlank($("#PatientAddress"), "Patient Address is missing! Please enter Patient Address")) return;
    if (!showAlertOnBlank($("#IpdNo"), "IPD No is missing! Please enter IPD No")) return;
    var AdmissionSummary = $("#AdmissionDetaisPoPupModalGrid").pqGrid("option", "dataModel.data");
    if (AdmissionSummary.length<=0) {
        ShowAlert('info', 'Admission and Discharge detail not added');
        return;
    }
    saveFunction();
});

function saveFunction() {
    DisableClick('btnSave');
    $.ajax({
        type: "POST",
        traditional: true,
        contentType: 'application/json; charset=utf-8',
        url: '/RequestSubmission/CreateRequest', // Controller/View    
        data: jsonstring(),
        success: function (msg) {
            $('input[type=file]').each(function () {
                var splt = this.id.split("inputBrowse");
                var filecontrol = $("#" + this.id);
                var fdata = new FormData();
                var files = filecontrol.get(0).files;
                for (i = 0; i < files.length; i++) {
                    fdata.append("files" + i, files[i]);
                }
                if (files.length > 0 && this.id !== "inputBrowseRight_OtDetails" && this.id !== "inputBrowseRight_DCDetails") {
                    SaveScandoc(this.id, msg.requestId, "RequestSubmissionForm", splt[1]);
                }
            });
            SaveScandoc("inputBrowseRight_OtDetails", msg.requestId, "RequestSubmissionForm", "RightOtDetails");
            SaveScandoc("inputBrowseRight_DCDetails", msg.requestId, "RequestSubmissionForm", "RightDCDetails");
            if (msg.success === false) {
                ShowAlert("error", "Server Error, Kindly try again later");
            }
            if (msg.success) {
                ShowAlert("success", "REQUEST NO " + msg.requestId+" GENERATED SUCCESSFULLY");
                clearAllFields();
                ClearAllSession();
                GetAllgeneratedRequest();

            }

        }
    });
}

function jsonstring() {
    var admissionSummary = $("#AdmissionDetaisPoPupModalGrid").pqGrid("option", "dataModel.data");
    var leftOtDetail = $("#OTdetailleftPoPupModalGrid").pqGrid("option", "dataModel.data");
    var rightOtDetail = $("#OTdetailRightPoPupModalGrid").pqGrid("option", "dataModel.data");
    var pharmacyDetails = $("#PharmacyServicePoPupModalGrid").pqGrid("option", "dataModel.data");
    var pjsondata = JSON.stringify({
        RequestId: $("#RequestId").val(),
        FileNo: $("#FileNo").val(),
        HospitalTypeId: $("#ddlHospitalType").val(),
        PatientName: $("#PatientName").val(),
        PatientAge: $("#PatientAge").val(),
        PatientTypeId: $("#ddlPatientType").val(),
        PatientAddress: $("#PatientAddress").val(),
        IpdNo: $("#IpdNo").val(),
        GenderId: $("#ddlPatientgender").val(),
        DrugsAmount: $("#DrugsAmount").val(),
        LifesavingMdcnAmt: $("#LifesavingMdcnAmt").val(),
        ManagementTypeId: $("#ddlMangementType").val(),
        LeftDcDetail: $("#LeftDcDetail").val(),
        RightDcDetail: $("#RightDcDetail").val(),
        StateId: $("#ddlState").val(),
        CityId: $("#ddlCity").val(),
        TypeOfAddmissionId: $("#ddlTypeOfAddmission").val(),
        AdmissionSummaries: admissionSummary,
        //LeftOtDetail: leftOtDetail,
        //RightOtDetail: rightOtDetail,
        PharmacyDetails: pharmacyDetails
    });
    return pjsondata;
}
function GetAllgeneratedRequest() {
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/RequestSubmission/GetAllgeneratedRequest",
        success: function (response) {
            $("#GeneratedRequestGrid").pqGrid("option", "dataModel.data", response);
            $("#GeneratedRequestGrid").pqGrid("refreshDataAndView");
        },
        error: function (a, b, response) {
            ShowAlert("error", "Server error");
        },
    });
}
function GetAdmissionDetailById(paramRequestId) {
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/RequestSubmission/AdmissionDetailById",
        data: { requestId: paramRequestId },
        success: function (response) {
            $('AdmissionDetaisPoPupModalGrid').html("");
            ClearParamGrid('AdmissionDetaisPoPupModalGrid');
            $("#AdmissionDetaisPoPupModalGrid").pqGrid("option", "dataModel.data", response);
            $("#AdmissionDetaisPoPupModalGrid").pqGrid("refreshDataAndView");
        },
        error: function (a, b, response) {
            ShowAlert("error", "");
        },

    });
} 
function GetRequestDetailById(paramRequestId) {
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/RequestSubmission/GetRequestDetailById",
        data: { requestId: paramRequestId },
        success: function (response) {
        },
        error: function (a, b, response) {
            ShowAlert("error", "Please try again or contact to your superior");
        },

    });
}
function GetRequestOtDetailById(paramRequestId) {
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/RequestSubmission/GetRequestOtDetailById",
        data: { requestId: paramRequestId },
        success: function (response) {
            var left = jLinq.from(response).equals("OtType", 1).select();
            var right = jLinq.from(response).equals("OtType", 2).select();
            $("#OTdetailleftPoPupModalGrid").pqGrid("option", "dataModel.data", left);
            $("#OTdetailleftPoPupModalGrid").pqGrid("refreshDataAndView");
            $("#OTdetailRightPoPupModalGrid").pqGrid("option", "dataModel.data", right);
            $("#OTdetailRightPoPupModalGrid").pqGrid("refreshDataAndView");
        },
        error: function (a, b, response) {
            ShowAlert("error", "Please try again or contact to your superior");
        },

    });
}
$('#btnOTDetailleft').on('click', function () {
    if (!showAlertOnBlank($("#ddlHospitalType"), "Hospital Type is missing! Please Select Hospital type")) return;
    if (!showAlertOnBlank($("#ddlPatientType"), "Patient Type is missing! Please Select Patient type")) return;
    //arrOTServiceType = [];
    fillOtDetailGrid();
    $("#OTdetailleftPoPupModal").dialog({
        height: 450,
        width: 700,
        modal: true,
        open: function (evt, ui) {
            var reqId = $("#RequestId").val();
            if(parseInt(reqId)>0)GetRequestOtDetailById(reqId);
        }
    });
});
$('#btnOTDetailRight').on('click', function () {
    if (!showAlertOnBlank($("#ddlHospitalType"), "Hospital Type is missing! Please Select Hospital type")) return;
    if (!showAlertOnBlank($("#ddlPatientType"), "Patient Type is missing! Please Select Patient type")) return;
    fillOtDetailGrid();
    $("#OTdetailRightPoPupModal").dialog({
        height: 450,
        width: 700,
        modal: true,
        open: function (evt, ui) {
            var reqId = $("#RequestId").val();
            GetRequestOtDetailById(reqId);
        }
    });
});

/********* Add Item to  Detail grid*********/
$('#btnAddRowOTdetailleft').on('click', function () {
    var dataMGrid = [];
    var getData = $("#OTdetailleftPoPupModalGrid").pqGrid("option", "dataModel.data");
    $.each(getData, function (key, value) {
        var grdData = {
            'RequestId': value.RequestId, 'OTDtlId': value.AdmissionDtlId, 'ServiceId': value.ServiceId, 'ServiceName': value.ServiceName, 'StrAdmissionDateTime': value.StrAdmissionDateTime, 'StrDischargeDateTime': value.StrDischargeDateTime
        }
        dataMGrid.push(grdData);
    });
    var grdnewData = {
        'RequestId': 0, 'OTDtlId': 0, 'ServiceId': 0, 'ServiceName': "", 'StrAdmissionDateTime': "", 'StrDischargeDateTime': ""
    }
    dataMGrid.push(grdnewData);
    $("#OTdetailleftPoPupModalGrid").pqGrid("option", "dataModel.data", dataMGrid);
    $("#OTdetailleftPoPupModalGrid").pqGrid("refreshDataAndView");
});



$('#btnRemoveRowOTdetailleft').on('click', function () {
    var getData = $("#OTdetailleftPoPupModalGrid").pqGrid("option", "dataModel.data");
    var rowIndx = getRowIndxLeft();
    if (rowIndx == null) {
        return;
    }
    $("#OTdetailleftPoPupModalGrid").pqGrid("deleteRow", { rowIndx: rowIndx });
});
$('#btnRemoveRowAdmissionDetaisPoPupModal').on('click', function () {
    var getData = $("#AdmissionDetaisPoPupModalGrid").pqGrid("option", "dataModel.data");
    var rowIndx = getRowIndx();
    if (rowIndx == null) {
        return;
    }
    $("#AdmissionDetaisPoPupModalGrid").pqGrid("deleteRow", { rowIndx: rowIndx });
});
$('#btnRemoveRowOTdetailRight').on('click', function () {
    var getData = $("#OTdetailRightPoPupModalGrid").pqGrid("option", "dataModel.data");
    var rowIndx = getRowIndx();
    if (rowIndx == null) {
        return;
    }
    $("#OTdetailRightPoPupModalGrid").pqGrid("deleteRow", { rowIndx: rowIndx });
});

function getRowIndx() {
    var arr = $("#AdmissionDetaisPoPupModalGrid").pqGrid('selection', { type: 'row', method: 'getSelection' });
    if (arr && arr.length > 0) {
        return arr[0].rowIndx;
    }
    else {
        ShowAlert("info", "No row Selected.");
        return null;
    }
}
function getRowIndxLeft() {
    var arr = $("#OTdetailleftPoPupModalGrid").pqGrid('selection', { type: 'row', method: 'getSelection' });
    if (arr && arr.length > 0) {
        return arr[0].rowIndx;
    }
    else {
        ShowAlert("info", "No row Selected.");
        return null;
    }
}
function getRowIndxRight() {
    var arr = $("#OTdetailRightPoPupModalGrid").pqGrid('selection', { type: 'row', method: 'getSelection' });
    if (arr && arr.length > 0) {
        return arr[0].rowIndx;
    }
    else {
        ShowAlert("info", "No row Selected.");
        return null;
    }
}
/********* Add Item to  Detail grid*********/
$('#btnAddRowOTdetailRight').on('click', function () {
    var dataMGrid = [];
    var getData = $("#OTdetailRightPoPupModalGrid").pqGrid("option", "dataModel.data");
    $.each(getData, function (key, value) {
        var grdData = {
            'RequestId': value.RequestId, 'OTDtlId': value.AdmissionDtlId, 'ServiceId': value.ServiceId, 'ServiceName': value.ServiceName, 'StrAdmissionDateTime': value.StrAdmissionDateTime, 'StrDischargeDateTime': value.StrDischargeDateTime
        }
        dataMGrid.push(grdData);
    });
    var grdnewData = {
        'RequestId': 0, 'OTDtlId': 0, 'ServiceId': 0, 'ServiceName': "", 'StrAdmissionDateTime': "", 'StrDischargeDateTime': ""
    }
    dataMGrid.push(grdnewData);
    $("#OTdetailRightPoPupModalGrid").pqGrid("option", "dataModel.data", dataMGrid);
    $("#OTdetailRightPoPupModalGrid").pqGrid("refreshDataAndView");
});

$('#btnReset').on('click', function () {
    clearAllFields();
});
function clearAllFields() {
    ClearAllControl("EntryForm");
}
function ClearAllSession() {
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/RequestSubmission/ClearAllSession",
        success: function (response) {
        }
    });

}
function ValidateGridDate() {
    var state;
    $.ajax({
        type: "GET",
        traditional: true,
        url: "/RequestSubmission/ValidateDate",
        success: function (response) {
            state = response;
        }
    });
    return state;
}
$('#btnReport').on('click', function () {
    if ($("#RequestId").val() === "" || $("#RequestId").val() === undefined) {
        ShowAlert("info", "Please Select Request");
        return;
    }
    var requestId = $("#RequestId").val();
    reportValidationCheck(requestId);
});


function LoadAttachmentPoPupModal(gridName) {
    var btnSelected = gridName.split("Attachment");
    var popupName = "popupModel" + btnSelected[1];
    $("#" + popupName).dialog({
        height: 400,
        width: 650,
        modal: true,
        open: function (evt, ui) {
         
        }
    });
}

function LoadViewAttachmentPoPupModal(gridName) {
    var btnSelected = gridName.split("ViewAttachment");
    
    var popupName = "displayModel" + btnSelected[1];
    $("#" + popupName).dialog({
        height: 400,
        width: 650,
        modal: true,
        open: function (evt, ui) {

        }
    });
}
function LoadOtDetailAttachmentPoPupModal() {
    $("#popupModelRight_OtDetails").dialog({
        height: 400,
        width: 650,
        modal: true,
        open: function (evt, ui) {

        }
    });
}
function LoadviewOtDetailAttachmentPoPupModal() {
    $("#displayModelRight_OtDetails").dialog({
        height: 400,
        width: 650,
        modal: true,
        open: function (evt, ui) {

        }
    });
}
function fnShowAttachment(param) {
    $('.attachment').each(function () {
        $("#" + this.id).html("");
    });
    $.ajax({
        type: "GET",
        url: '/RequestSubmission/GetScanDocUrl',
        data: { scanDocId: param },
        datatype: "Json",
        success: function (data) {
            $.each(data, function (key, value) {
                $('.attachment').each(function () {
                    var splt = this.id.split("displayBrowse");
                    if (splt[1] === value.ScanDocSubType) {
                        DisplayUploadedImages(this.id, value.FilePath);
                    }
                });
                if ("RightOtDetails" === value.ScanDocSubType) {
                    DisplayUploadedImages("displayBrowseRight_OtDetails", value.FilePath);
                }
                if ("RightDCDetails" === value.ScanDocSubType) {
                    DisplayUploadedImages("displayBrowseRight_DCDetails", value.FilePath);
                }
                });
        },
        error:function(a, b, c) {
        }
    });
}

function reportValidationCheck(requestId) {
    $("#Reportpopup-dialog-crud").dialog({
        cache: false,
        position: {
            my: "center",
            at: "center",
            of: window
        },
        height: 720,
        width: 900
    }).dialog("open");
    var url = "";
    url = "../../Reports/ReportViewer.aspx?reportid=" + 1 + "&requestId=" + requestId;
    console.log(url);
    var myframe = document.getElementById("iframeReportViewer");
    if (myframe != null) {
        if (myframe.src) {
            myframe.src = url;
        } else if (myframe.contentWindow != null && myframe.contentWindow.location != null) {
            myframe.contentWindow.location = url;
        } else {
            myframe.setAttribute('src', url);
        }
        return false;
    }
}
