// testing Sharepoint Multilookup to checkpoint


$("#Abrangencia_f315eae6-5dac-4efa-a1cf-d08846afef83_SelectCandidate").children().each(function(i, x){ 
	$("Abrangencia_f315eae6-5dac-4efa-a1cf-d08846afef83_SelectResult").apppend(this);
});

$("#Abrangencia_f315eae6-5dac-4efa-a1cf-d08846afef83_SelectCandidate").children().each(function(i){ 
	$("Abrangencia_f315eae6-5dac-4efa-a1cf-d08846afef83_SelectResult").append(new Option(this.title, this.value));
});

$("#Abrangencia_f315eae6-5dac-4efa-a1cf-d08846afef83_SelectResult").append(new Option('1','fd'))


function createCheckbox(value, name, text, jiddiv){
	var checkbox = document.createElement('input');
	checkbox.type = "checkbox";
	checkbox.name = name;
	checkbox.value = value;
	checkbox.id = "id"+ text;

	var label = document.createElement('label')
	label.htmlFor = "id"+text;
	label.appendChild(document.createTextNode(text));

	$(jiddiv).append(checkbox);
	$(jiddiv).append(label);	
}

function generateCheckboxes(jid){
	$(jid).children().each(function(){ 
		createCheckbox(this.value, "abrangencia", this.title, "#WebPartWPQ2");
	});
}

function appendOption(value, title, jid){
	var opt = new Option(title, value);
	opt.label = title;
	$(jid).append(opt);
}

function addData(value, title){
	var multilookupPickerVal = $("[id^='Abrangencia_'][id$='MultiLookup']").val();
    if ($("[id^='Abrangencia_'][id$='MultiLookup']").val() == undefined || $("[id^='Abrangencia_'][id$='MultiLookup']").val().length == 0) {
        $("[id^='Abrangencia_'][id$='MultiLookup']").val(value + "|t" + title);
    }
    else {
        $("[id^='Abrangencia_'][id$='MultiLookup']").val(multilookupPickerVal + "|t" + value + "|t" + title);
    }

}

function updateOptions(){
	//var values = [];
	$("input[name='abrangencia']:checked").each(function(){
		//values.push(this.value);
		//appendOption(this.value, this.id.replace("id", ""), "#Abrangencia_f315eae6-5dac-4efa-a1cf-d08846afef83_SelectResult");
		addData(this.value, this.id.replace("id", ""));
	});

	//for (var i = 0; i < values.length; i++) {
	//	$("#Abrangencia_f315eae6-5dac-4efa-a1cf-d08846afef83_SelectCandidate").find("[value='"+ values[i] + "']").remove();
	//}
}

function alterSaveClick(){

	var old = $("#ctl00_ctl33_g_1bfedc6c_1c91_49e6_990d_7d4fbb2feff3_ctl00_toolBarTbl_RightRptControls_ctl00_ctl00_diidIOSaveItem").attr("onclick");
	 $("#ctl00_ctl33_g_1bfedc6c_1c91_49e6_990d_7d4fbb2feff3_ctl00_toolBarTbl_RightRptControls_ctl00_ctl00_diidIOSaveItem").attr("onclick", "updateOptions();" + old);
}

generateCheckboxes("#Abrangencia_f315eae6-5dac-4efa-a1cf-d08846afef83_SelectCandidate");
alterSaveClick();
