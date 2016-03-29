(function(){
    
    window.CSRManager =  window.CSRManager || {};

    window.CSRManager.Parser = {
        parseObjToHtml : function (html, obj){
            var sub = function(o){
                var c = o.indexOf(":");
                if (c == -1)
                    return undefined;
                return { id: o.substr(0, c), v: o.substr(c+1) }
            }
            for(var i =0, l = obj.length; i < l; i++){
                var v = sub(obj[i]);
                if(v != undefined)
                    html = html.replace("{{"+v.id+"}}", v.v);
            }
            return html;
        },
        parseMultiLookupValue : function(cs, toreplace){
            var m ="";
            for(var i = 0; i < cs.length; i++){
                if (m == undefined || m == "") {
                    m = cs[i].id.replace(toreplace, "") + ";#" + cs[i].value;
                }
                else {
                    m = m + ";#" + cs[i].id.replace(toreplace, "") + ";#" + cs[i].value;
                }        
            }
            return m;
        }
    }

    window.CSRManager.Elements = {
        createCheckboxes : function(choices, name, values){
            var box = "";
            var getIds = function(v){
            	var nv = [];
            	for (var i = 0; i < v.length; i+=2) {
            		nv.push(v[i]);
            	}
            	return nv;
            }
            var idValues = getIds(values.split(';#'));

            var isChecked = function(nv, id){
            	for (var i = 0; i < nv.length; i++) {
            		if(nv[i] == id)
            			return true;
            	}
            	return false;
            }

            for(var i = 0, l = choices.length; i < l; i++){
            	var c;
            	if(isChecked(idValues, choices[i].LookupId))
            		c = this.createCheckbox(choices[i], name, true);
                else
                	c = this.createCheckbox(choices[i], name, false);
                box += c.label;
                box += c.checkbox;
            }
            return box;
        },
        createCheckbox : function(choice, name, checked){
            var checkbox =  "<input id=\"{{id}}\" type=\"checkbox\" name=\"{{name}}\" value=\"{{value}}\" {{checked}} />";
            var label = "<label for=\"{{id}}\">{{value}}</label>";

            if(checked)
            	checkbox = checkbox.replace("{{checked}}", "checked");
            else
            	checkbox = checkbox.replace('{{checked}}', "");
            var id = "id_" + name  + choice.LookupId;
            checkbox = window.CSRManager.Parser.parseObjToHtml(checkbox, ["id:"+id, "name:" +name, "value:" + choice.LookupValue]);
            label = window.CSRManager.Parser.parseObjToHtml(label, [ "id:" + id, "value:" + choice.LookupValue] );

            return { checkbox: checkbox, label : label, value: label + checkbox };
        }
    }

    window.CSRManager.Render = {
        checkbox: function(ctx){
            var choices = ctx.CurrentFieldSchema.Choices; //choices array

      			var fieldCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(ctx);
      			var fieldName = fieldCtx.fieldName;

            var div = "<div id=\"{{id}}\">{{box}}</div>";

            var boxes = window.CSRManager.Elements.createCheckboxes(choices, fieldName, fieldCtx.fieldValue);
            div = window.CSRManager.Parser.parseObjToHtml(div, ["id:" +fieldName + "_box", "box:"+boxes]);

            SPClientTemplates.Utility.GetFormContextForCurrentField(ctx).registerGetValueCallback(ctx.CurrentFieldSchema.Name, function(){
                var cs = document.querySelectorAll("#" + fieldName +"_box input[type='checkbox']:checked");
                return window.CSRManager.Parser.parseMultiLookupValue(cs, "id_" +fieldName);
            });

            return div;
        }    
    }    
})();
