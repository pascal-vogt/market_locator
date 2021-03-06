/////////////////////////////////////////
// This script transforms data from the "Marktstände-Manuell", "Input" and "Ref" data into a format compatible with "Marktstände-Manuell"
// The resulting data gets written into the "Marktstände-Script" tab (prod) or the "Script Test Output" tab (test)
//
// How to use:
// - Open spreadsheet https://docs.google.com/spreadsheets/d/1VFZRShq8bMyWcN0ANQdLmPPc-CoPWMwr29MkdVboyRA/edit
// - Click on Tools > Script editor
// - Accept permissions to edit sheets
// - Create a file "market-data-transform.gs" with the contents of this file
// - Save
// - Choose transformData_Test() in the dropdown above
// - Click the Run or the Debug button
// - Check if the output in "Script Test Output" looks OK
// - Choose transformData_Prod() in the dropdown above
// - Click the Run or the Debug button
//
// Additional Important steps
// - This script has to be run daily, preferrably in the morning (because the data it outputs depends on the current day)
/////////////////////////////////////////

// Those IDs can be retrieved by going to the tab and then looking for #gid= in the URL
var GPS_COORDS_SHEET_ID = 1072628029;
var MAPPING_SHEET_ID = 992876922;
var MANUAL_INPUT_SHEET_ID = 363229505;
var OUTPUT_SHEET_ID = 2132362629;
var TEST_OUTPUT_SHEET_ID = 1330311051;

function getSheetById_(gid){
  for (var sheet of SpreadsheetApp.getActive().getSheets()) {
    if(sheet.getSheetId()==gid){
      return sheet;
    }
  }
}

function getGPSMap_() {
  var gpsCoordSheet = getSheetById_(GPS_COORDS_SHEET_ID);
  var data = gpsCoordSheet.getDataRange().getValues();
  var map = {};
  for(var i = data.length-1 ; i >=0 ; i--){
    if (data[i][0] != null && data[i][0] != ''){
      map[data[i][0]] = data[i][2];
    }
  }
  return map;
}

function getManuallyInputData_() {
  var sheet = getSheetById_(MANUAL_INPUT_SHEET_ID);
  var data = sheet.getDataRange().getValues();
  var entries = [];
  var headers = data[0];
  for(var i = 1; i < data.length; ++i){
    var row = data[i];
    var entry = {};
    for (var j = 0; j < row.length; ++j) {
      entry[headers[j]] = row[j];
    }
    if (entry['Name kommt noch'].length > 0) {
      entries.push(entry);
    }
  }
  return {
    headers: headers,
    entries: entries
  };
}

function formatDate_(date) {
  var d = date.getDate();
  var m = date.getMonth() + 1;
  var y = date.getFullYear();
  return (d < 10 ? '0' + d : d) + '.' + (m < 10 ? '0' + m : m) + '.' + y;
}

function formatWeekday_(date) {
  return ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'][date.getDay()];
}

function getMappingData_() {
  var now = new Date();
  var today = new Date(now.getFullYear(), now.getMonth(), now.getDate());

  var mappingSheet = getSheetById_(MAPPING_SHEET_ID);
  var data = mappingSheet.getDataRange().getValues();
  var header = data[0];
  var relevantColumns = [];
  var metadataIndexes = {};
  for (var i = 1;  i < header.length; ++i) {
    var cell = header[i];
    if (/^[A-Z]{2} \d{1,2}\.\d{1,2}\.\d{4}$/.test(cell)) {
      var dateStr = cell.split(/\s+/)[1].split('.');
      var date = new Date(parseInt(dateStr[2], 10), parseInt(dateStr[1], 10) - 1, parseInt(dateStr[0], 10));
      var dateDiffInDays = (date - today) / (1000 * 60 * 60 * 24);
      // sliding week window
      if (dateDiffInDays >= 0 && dateDiffInDays < 7) {
        relevantColumns.push({
          index: i,
          weekday: formatWeekday_(date),
          date: formatDate_(date)
        });
      }
    } else {
      metadataIndexes[cell] = i;
    }
  }
  var entries = [];
  for(var i = 1; i < data.length; ++i){
    var name = data[i][0];
    for (var relevantColumn of relevantColumns) {
      var cell = data[i][relevantColumn.index];
      if (cell != null && cell != '') {
        var metadata = {};
        for (let key of Object.keys(metadataIndexes)) {
          metadata[key] = data[i][metadataIndexes[key]];
        }
        entries.push({
          name: name,
          weekday: relevantColumn.weekday,
          date: relevantColumn.date,
          gpsRef: cell,
          metadata: metadata
        });
      }
    }
  }
  return entries;
}

function transformData_Prod() {
  transformDataToSheetId_(OUTPUT_SHEET_ID);
}

function transformData_Test() {
  transformDataToSheetId_(TEST_OUTPUT_SHEET_ID);
}

function transformDataToSheetId_(outputSheetId) {
  var gpsMap = getGPSMap_();
  var mappingData = getMappingData_();
  
  var manualyInputData = getManuallyInputData_();
  var headers = manualyInputData.headers;
  var newData = [];
  var entries = manualyInputData.entries.slice(); // makes a copy
  for (let mappingEntry of mappingData) {
    let entry = {
      'Name kommt noch': mappingEntry.name,
      'Tag': mappingEntry.weekday,
      'Datum': mappingEntry.date,
      'Adresse / Koordinaten': gpsMap[mappingEntry.gpsRef]
    };
    for (let key of Object.keys(mappingEntry.metadata)) {
      entry[key] = mappingEntry.metadata[key];
    }
    entries.push(entry);
  }
  for (let entry of entries) {
    var row = [];
    for (let header of headers) {
      if (entry[header]) {
        row.push(entry[header]);
      } else {
        row.push('');
      }
    }
    newData.push(row);
  }
  // sort by: name asc, weekday asc, date asc
  newData.sort(function (lhs, rhs) {
    if (lhs[0] > rhs[0]) {
     return 1; 
    }
    if (lhs[0] < rhs[0]) {
     return -1; 
    }
    if (lhs[2] > rhs[2]) {
     return 1; 
    }
    if (lhs[2] < rhs[2]) {
     return -1; 
    }
    if (lhs[3] > rhs[3]) {
     return 1; 
    }
    if (lhs[3] < rhs[3]) {
     return -1; 
    }
    return 0;
  });
  newData.unshift(headers);
  
  var outSheet = getSheetById_(outputSheetId);
  outSheet.getDataRange().clearContent();
  outSheet.getRange(1, 1, newData.length, newData[0].length).setValues(newData);
}