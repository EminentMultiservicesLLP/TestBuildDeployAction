var ParentId;
var TempUserId;
var TempRoleId = 0;
var TempRoleId1;

$(document).ready(function () {
    getMenuDetails();
   // getAllStdSub();
    $("#btnAccessSave").prop('disabled', true);
    /**********************Load Search User Grid**************/


    function loadSearchUserGrid() {
        $.ajax({
            type: "GET",
            url: "/UserCreation/GetUserDetails",
            datatype: "Json",
            async: true,
            success: function (data) {
                $("#Usergrid").pqGrid("option", "dataModel.data", data);
                $("#Usergrid").pqGrid("refreshDataAndView");
            }
        });
    }

    /*******************Set Search User grid******************/
    var dataSearchUserGrid = { location: "local" };
    var colSearchUserGrid = [
            { title: "User ID", dataIndx: "UserId", dataType: "integer", hidden: true },

            {
                title: "User Name", dataIndx: "UserName", width: 400,
                filter: { type: 'textbox', condition: 'contain', listeners: ['keyup'] }
            }


    ];

    var setSearchUserGrid = {
        width: '100%',
        height: 250,
        sortable: false,
        numberCell: { show: false },
        hoverMode: 'cell',
        showTop: true,
        resizable: true,
        scrollModel: { autoFit: true },
        draggable: false,
        wrap: false,
        editable: false,
        filterModel: { on: true, mode: "AND", header: true },
        selectionModel: { type: 'row', subtype: 'incr', cbHeader: true, cbAll: true },
        colModel: colSearchUserGrid,
        dataModel: dataSearchUserGrid,
        pageModel: { type: "local", rPP: 20 },
        rowClick: function (evt, ui) {
            //clearSession();
            ClearParamGrid("ChildMenugrid");
            var record = ui.rowData;
            $("#UserId").val(record.UserID);
            $("#UserName").val(record.UserName),
            TempUserId = record.UserID;
            $.ajax({
                type: "GET",
                url: "/UserAccess/GetMenuByUser/",
                data: { userId: record.UserID },
                datatype: "Json",
                beforeSend: function () {
                    $("#ParentMenugrid").pqGrid("showLoading");
                },
                complete: function () {
                    $("#ParentMenugrid").pqGrid("hideLoading");
                },
                success: function (data) {

                    $("#ParentMenugrid").pqGrid("hideLoading");
                    $("#ParentMenugrid").pqGrid("option", "dataModel.data", data.data);
                    $("#ParentMenugrid").pqGrid("refreshDataAndView");
                    $("#btnAccessSave").prop('disabled', false);
                },
                error: function (request, status, error) {
                    $("#ParentMenugrid").pqGrid("hideLoading");
                    ShowAlert("error", "Error while loading list");
                    return;
                }
            });
            
        }
    }

    $("#Usergrid").pqGrid(setSearchUserGrid);
    loadSearchUserGrid();

    ///*********************Get Users End*****************/


    function getMenuDetails() {
        $.ajax({
            type: "GET",
            url: "/UserAccess/GetMenuDetails",
            datatype: "Json",
            async: true,
            success: function (data) {
                $("#ParentMenugrid").pqGrid("option", "dataModel.data", data);
                $("#ParentMenugrid").pqGrid("refreshDataAndView");
            }
        });
    }

    /*********************GetMenuData Common fn*****************/

    function getMenuData(ui, gridName) {
        var getMenuUrl;
        $("#btnAccessSave").prop('disabled', false);
        $("#btnDelete").prop('disabled', false);
        if (ui.rowData) {
            var details = ui.rowData;
            var dataMGrid1 = [];
          
                getMenuUrl = "/UserAccess/GetUserMenuByparentid";

            $.ajax({
                type: "GET",
                url: getMenuUrl,
                datatype: "Json",
                async: false,
                data: { menuid: details.MenuId, state: details.State },
                success: function (data) {
                    $.each(data, function (key, value) {
                        var grdData1 = {

                            'MenuName': value.MenuName,
                            'MenuId': value.MenuId,
                            'State': value.State

                        };
                        dataMGrid1.push(grdData1);
                    });
                }
            });

            var gr = $("#" + gridName);
            gr.pqGrid("option", "dataModel.data", dataMGrid1);
            gr.pqGrid("refreshDataAndView");

        }
    }


      /*********************Parent Menu Grid*****************/

    var parentid;
   var dataList = { location: 'local', sorting: 'local', paging: 'local', dataType: 'JSON' };
    var setUserCol = [{ title: "Menu Name", dataIndx: "MenuName", width: 200, editable: false },
        { title: "MenuId", dataIndx: "MenuId", width: 90, hidden: true },
         {
             dataIndx: "State", Width: 03, align: "right", type: 'checkBoxSelection', cls: 'ui-state-default', sortable: false,
             editor: false, dataType: 'bool',
             title: "<input type='checkbox' onclick='getMenuData(ui, ChildMenugrid)'/>",
             cb: { select: true, all: false, header: true },
             //render: function () { getMenuData(ui, "ChildMenugrid"); }
         }
    ];
    var setUserList = {
        width: "300",
        height: 400,
        selectionModel: { type: 'cell' },
        autoSizeInterval: 0,
        dragColumns: { enabled: false },
        hoverMode: 'cell',
        editor: { type: 'textbox' },
        showTop: false,
        resizable: true,
        virtualX: true,
        editable: true,
        colModel: setUserCol,
        dataModel: dataList,
        cellSave: function (evt, ui) {
            this.refreshRow(ui);
        },
        check: function (event, ui) {
            getMenuData(ui, "ChildMenugrid");

        },

        rowClick: function (evt, ui) {
            getMenuData(ui, "ChildMenugrid");
            ClearParamGrid("SubChildMenugrid");
            var record = ui.rowData;
            $("#MenuId").val(record.MenuId);
            $.ajax({
                type: "GET",
                url: "/UserAccess/GetSubMenuDetails",
                datatype: "Json",
                data: { MenuId: record.MenuId },
                async: true,
                success: function (data) {
                    $("#ChildMenugrid").pqGrid("option", "dataModel.data", data);
                    $("#ChildMenugrid").pqGrid("refreshDataAndView");
                }
            });


        }

    };
    var $ParentMenugrid = $("#ParentMenugrid").pqGrid(setUserList);




    function clearSession() {
        $.ajax({
            type: "POST",
            url: "/UserAccess/ClearSession",
            datatype: "Json",
            success: function (data) {

            }
        });
    }



   
    ///////***************Child Grd*********************/////////////////

    var parentid;
    dataList = { location: 'local', sorting: 'local', paging: 'local', dataType: 'JSON' };
    setUserCol = [
        { title: "Menu Name", dataIndx: "MenuName", width: 150, editable: false },
        { title: "MenuId", dataIndx: "MenuId", width: 10, hidden: true },
         {
             dataIndx: "State", Width: 03, align: "right", type: 'checkBoxSelection', cls: 'ui-state-default', sortable: false,
             editor: false, dataType: 'bool',
             title: "<input type='checkbox' onclick='getMenuData(ui, ChildMenugrid)' />",
             cb: { select: true, all: true, header: true }

         }
    ];
    setUserList = {
        width: "300",
        height: 400,
        selectionModel: { type: 'cell' },
        autoSizeInterval: 0,
        dragColumns: { enabled: false },
        hoverMode: 'cell',
        editor: { type: 'textbox' },
        showTop: false,
        resizable: true,
        virtualX: true,
        editable: true,
        colModel: setUserCol,
        dataModel: dataList,
        cellSave: function (evt, ui) {
            this.refreshRow(ui);
        },
        check: function (event, ui) {

            getMenuData(ui, "SubChildMenugrid");
        },

        rowClick: function (evt, ui) {
            getMenuData(ui, "SubChildMenugrid");
            ClearParamGrid("SubChildMenugrid");
            var record = ui.rowData;
            $("#MenuId").val(record.MenuId);
            $.ajax({
                type: "GET",
                url: "/UserAccess/GetSubMenuDetails",
                datatype: "Json",
                data: { MenuId: record.MenuId },
                async: true,
                success: function (data) {
                    $("#SubChildMenugrid").pqGrid("option", "dataModel.data", data);
                    $("#SubChildMenugrid").pqGrid("refreshDataAndView");
                }
            });

        }

    };
    var $ChildMenugrid = $("#ChildMenugrid").pqGrid(setUserList);
   // ///***********************Sub Menu Grid********************************/
   // dataList = { location: 'local', sorting: 'local', paging: 'local', dataType: 'JSON' };
   //setUserCol = [{ title: "Menu Name", dataIndx: "MenuName", width: 200, editable: false },
   //     { title: "MenuId", dataIndx: "MenuId", width: 90, hidden: true },
   //      {
   //          dataIndx: "State", Width: 03, align: "right", type: 'checkBoxSelection', cls: 'ui-state-default', sortable: false,
   //          editor: false, dataType: 'bool',
   //          title: "<input type='checkbox' />",
   //          cb: { select: true, all: false, header: true }
   //      },
   // ];
   // setUserList = {
   //     width: "300",
   //     height: 400,
   //     selectionModel: { type: 'cell' },
   //     autoSizeInterval: 0,
   //     dragColumns: { enabled: false },
   //     hoverMode: 'cell',
   //     editor: { type: 'textbox' },
   //     showTop: false,
   //     resizable: true,
   //     virtualX: true,
   //     editable: true,
   //     colModel: setUserCol,
   //     dataModel: dataList,
   //     cellSave: function (evt, ui) {
   //         this.refreshRow(ui);
   //     },
   //     check: function (event, ui) {

   //         getMenuData(ui, "");

   //     }
   // };
   // var $SubChildMenugrid = $("#SubChildMenugrid").pqGrid(setUserList);


    ///***********************Save details*****************************/


    $('#btnAccessSave').on('click', function () {
            if ($("#UserId").val() === "") {
                ShowAlert("error", "Please Select User");
                return;
            }

        DisableClick("btnSave");
          var accessData = JSON.stringify({
              UserId: TempUserId,
          });
        $.ajax({
            type: "POST",
            traditional: true,
            contentType: 'application/json; charset=utf-8',
            url: '/UserAccess/SaveUserAccess',
            data: accessData,
            success: function (msg) {
                if (msg.success === true) {
                    ShowAlert("success", " Access Saved successfully.");
                    clearForm();
                }
                else {
                    ShowAlert("error", msg.Message);
                }
            },
            error: function (jqXhr, exception) {
                ShowAlert("error", "Failed To Save");
            }

        });

    });

   

    /*********************SubStandard Data Common fn*****************/

    $("#btnReset").on("click", clearForm);
    function clearForm() {
        TempUserId = 0;
        $("#UserName").val("");
        $("#UserId").val("");
        ClearParamGrid("ChildMenugrid");
        ClearParamGrid("SubChildMenugrid");
        ClearParamGrid("ParentMenugrid");
        getMenuDetails();
        loadSearchGrid();
    }

});
