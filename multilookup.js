//CSRManager
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
        createCheckboxes : function(choices, name){
            var box = "";
            for(var i = 0, l = choices.length; i < l; i++){
                var c = this.createCheckbox(choices[i], name);
                box += c.label;
                box += c.checkbox;
            }
            return box;
        },
        createCheckbox : function(choice, name){
            var checkbox =  "<input id=\"{{id}}\" type=\"checkbox\" name=\"{{name}}\" value=\"{{value}}\" />";
            var label = "<label for=\"{{id}}\">{{value}}</label>";

            var id = "id_" + name  + choice.LookupId;
            checkbox = CSRManager.Parser.parseObjToHtml(checkbox, ["id:"+id, "name:" +name, "value:" + choice.LookupValue]);
            label = CSRManager.Parser.parseObjToHtml(label, [ "id:" + id, "value:" + choice.LookupValue] );

            return { checkbox: checkbox, label : label, value: label + checkbox };
        }
    }

    window.CSRManager.Render = {
        checkbox: function(ctx){
            var choices = ctx.CurrentFieldSchema.Choices; //choices array

            var div = "<div id=\"{{id}}\">{{box}}</div>";

            var boxes = CSRManager.Elements.createCheckboxes(choices, "produto");
            div = CSRManager.Parser.parseObjToHtml(div, ["id:produto_box", "box:"+boxes]);

            SPClientTemplates.Utility.GetFormContextForCurrentField(ctx).registerGetValueCallback(ctx.CurrentFieldSchema.Name, function(){
                var cs = document.querySelectorAll("#produto_box input[type='checkbox']:checked");
                return CSRManager.Parser.parseMultiLookupValue(cs, "id_produto");
            });
        }    
    }    

})();

// exemplo de uso
(function(){
    
   var overrideCtx = {};
   overrideCtx.Templates = {};
   overrideCtx.Templates.Fields = {
     'Produtos' : {
       'NewForm' : CSRManager.Render.checkbox
     }
   };
    
   SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
    
})();
