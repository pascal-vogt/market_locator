/////////////////////////////////////////
// This script transforms data from the "Input" and "Ref" data into a format compatible with 
// The resulting data gets written into the "ScriptOutput" tab
//
// How to use:
// - Open spreadsheet https://docs.google.com/spreadsheets/d/1VFZRShq8bMyWcN0ANQdLmPPc-CoPWMwr29MkdVboyRA/edit
// - Click on Tools > Script editor
// - Accept permissions to edit sheets
// - Create a file "market-data-transform.gs" with the contents of this file
// - Save
// - Choose transformData() in the dropdown above
// - Click the Run or the Debug button
//
// Additional Important steps
// - This script has to be run daily, preferrably in the morning (because the data it outputs depends on the current day)
/////////////////////////////////////////

// Those IDs can be retrieved by going to the tab and then looking for #gid= in the URL
var GPS_COORDS_SHEET_ID = 1072628029;
var MAPPING_SHEET_ID = 992876922;
var OUTPUT_SHEET_ID = 1330311051;

function getActiveSheetId(){
  var id  = SpreadsheetApp.getActiveSheet().getSheetId();
  Logger.log(id.toString());
  return id;
}

function getSheetById(gid){
  for (var sheet of SpreadsheetApp.getActive().getSheets()) {
    if(sheet.getSheetId()==gid){
      return sheet;
    }
  }
}

function getGPSMap() {
  var gpsCoordSheet = getSheetById(GPS_COORDS_SHEET_ID);
  var data = gpsCoordSheet.getDataRange().getValues();
  var map = {};
  for(var i = data.length-1 ; i >=0 ; i--){
    if (data[i][0] != null && data[i][0] != ''){
      map[data[i][0]] = data[i][2];
    }
  }
  return map;
}

function formatDate(date) {
  var d = date.getDate();
  var m = date.getMonth() + 1;
  var y = date.getFullYear();
  return (d < 10 ? '0' + d : d) + '.' + (m < 10 ? '0' + m : m) + '.' + y;
}

function getMappingData() {
  var now = new Date();
  var today = new Date(now.getFullYear(), now.getMonth(), now.getDate());

  var mappingSheet = getSheetById(MAPPING_SHEET_ID);
  var data = mappingSheet.getDataRange().getValues();
  var header = data[0];
  var minCol = undefined;
  var maxCol = undefined;
  var weekdayOffset = undefined;
  var weekdayDates = [];
  for (var i = 1;  i < header.length; ++i) {
    var cell = header[i];
    var dateStr = cell.split(/\s+/)[1].split('.');
    var date = new Date(parseInt(dateStr[2], 10), parseInt(dateStr[1], 10) - 1, parseInt(dateStr[0], 10));
    var dateDiffInDays = (date - today) / (1000 * 60 * 60 * 24);
    // sliding week window
    if (dateDiffInDays >= 0 && dateDiffInDays < 7) {
      if (minCol === undefined) {
        minCol = i;
        weekdayOffset = date.getDay(); // 0 for sunday
      }
      maxCol = i;
      weekdayDates.push(formatDate(date));
    }
  }
  var entries = [];
  for(var i = 1; i < data.length; ++i){
    var name = data[i][0];
    for (var j = minCol; j <= maxCol; ++j) {
      var weekday = (weekdayOffset + (j - minCol)) % 7;
      if (data[i][j] != null && data[i][j] != '') {
        entries.push({
          name: name,
          weekday: ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'][weekday],
          weekdayIdx: weekday,
          gpsRef: data[i][j],
          weekdayDate: weekdayDates[j - minCol]
        });
      }
    }
  }
  return entries;
}
// Name	Kategorie	Tag	Datum	Zeiten	Adresse / Koordinaten	Tel	Typ	Cover	Website	Video	Social
function transformData() {
  var gpsMap = getGPSMap();
  var mappingData = getMappingData();
  mappingData.sort(function (lhs, rhs) {
    if (lhs.name > rhs.name) {
     return 1; 
    }
    if (lhs.name < rhs.name) {
     return -1; 
    }
    if (lhs.weekdayIdx > rhs.weekdayIdx) {
     return 1; 
    }
    if (lhs.weekdayIdx < rhs.weekdayIdx) {
     return -1; 
    }
    return 0;
  });
  
  var header = ['Name', 'Kategorie', 'Tag', 'Datum', 'Zeiten', 'Adresse / Koordinaten', 'Tel', 'Typ', 'Cover', 'Website', 'Video', 'Social'];
  var newData = [header];
  for (let entry of mappingData) {
    newData.push([entry.name, '', entry.weekday, entry.weekdayDate, '', gpsMap[entry.gpsRef], '', '', '', '', '', '']);
  }
  
  var outSheet = getSheetById(OUTPUT_SHEET_ID);
  outSheet.getDataRange().clearContent();
  outSheet.getRange(1, 1, newData.length, newData[0].length).setValues(newData);
}