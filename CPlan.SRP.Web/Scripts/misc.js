// srp4net 1.0
// SRP for .NET - A JavaScript/C# .NET library for implementing the SRP authentication protocol
// http://code.google.com/p/srp4net/
// Copyright 2010, Sorin Ostafiev (http://www.ostafiev.com/)
// License: GPL v3 (http://www.gnu.org/licenses/gpl-3.0.txt)

(function($)
{
    $.extend({
        toJSON: function(object)
        {
            return JSON.stringify(object);
        }
    });
})(jQuery);



function compareTo(big1, big2)
{
    // -1 if big1 < big2
    //  1 if big1 > big2
    //  0 if equal
    if (greater(big1, big2))
    {
        return 1;
    }
    else
    {
        if (equals(big1, big2))
        {
            return 0;
        }
    }
    return -1;
}

function WS(fnServer, data, fnSuccess)
{
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        async: false,
        url: "ScriptAuthentication.asmx/" + fnServer,
        data: $.toJSON(data),
        dataType: "json",

        success: function(msg)
        {
            fnSuccess(msg.d);
        },

        error: function(xhr, msg, error)
        {
            var err = eval("(" + xhr.responseText + ")");
            $.log(err.Message + "\n" + err.StackTrace);
        }
    })
}