var dataList = { location: 'local', sorting: 'local', paging: 'local', dataType: 'JSON' };
var setUserCol = [
    {
        title: "User Code", dataIndx: "UserCode", width: 100,
        filter: { type: 'textbox', condition: 'begin', listeners: ['keyup'] }
    },
    {
        title: "User Name", dataIndx: "UserName", width: '30%',
        filter: { type: 'textbox', condition: 'begin', listeners: ['keyup'] }
    },
    {
        title: "Client Name", dataIndx: "ClientName", width: '30%',
        filter: { type: 'textbox', condition: 'begin', listeners: ['keyup'] }
    },
    {
        title: "Last Login Date", dataIndx: "strLastLoginDate", width: '15%',
        filter: { type: 'textbox', condition: 'begin', listeners: ['keyup'] }
    },
    {
        title: "Expiry Date", dataIndx: "strExpiryDate", width: '15%',
        filter: { type: 'textbox', condition: 'begin', listeners: ['keyup'] }
    }
];
var setUserList = {
    width: "auto",
    height: 600,
    autoSizeInterval: 0,
    dragColumns: { enabled: false },
    hoverMode: 'cell',
    editable: false,
    filterModel: { on: true, mode: "AND", header: true },
    showTop: true,
    resizable: true,
    virtualX: true,
    colModel: setUserCol,
    selectionModel: { type: 'row', subtype: 'incr', cbHeader: true, cbAll: true },
    dataModel: dataList,
    pageModel: { type: "local", rPP: 50 },
    scrollModel: { autoFit: true },
};
$("#Searchgrid").pqGrid(setUserList);

function getUserDetails() {
    $.ajax({
        type: "GET",
        url: "/UserCreation/GetClientTracker",
        datatype: "Json",
        async: true,
        success: function (data) {
            $("#Searchgrid").pqGrid("option", "dataModel.data", data);
            $("#Searchgrid").pqGrid("refreshDataAndView");
        }
    });
}
getUserDetails();