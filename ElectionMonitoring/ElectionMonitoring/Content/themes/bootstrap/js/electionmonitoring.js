$(document).ready(function () {
    docReady();
    initializeLists();
    if ($('#map-canvas').length > 0) {
        initializeMap();
        UpdateScreen();
        setInterval(UpdateScreen, 60000);
        $('#region').change(function () {
            UpdateScreen();
        });

        // Radialize the colors
        Highcharts.getOptions().colors = Highcharts.map(Highcharts.getOptions().colors, function (color) {
            return {
                radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
                stops: [
		                [0, color],
		                [1, Highcharts.Color(color).brighten(-0.3).get('rgb')] // darken
		            ]
            };
        });
    } else {
        //alert('hi;');
        $('#save-result').click(function (e) {
            e.preventDefault();
            var vm = { 'RaceID': $('#race').val(), 'CandidateID': $('#candidate').val(), 
                'RegionID': $('#lga').val(), 'NoOfVotes': $('#votes').val(), 
                'SubmittedBy': 1, 'SubmittedOn': new Date(),
            };
            console.log(vm);
            $.ajax ({
                type: 'POST',
                url: window.location.origin + '/ElectionResult/EnterResults',
                data: vm,
                success: function (result) {
                    Announce('Entered successfully', 'top', 'success', true, false);
                },
                error : function () {
                    Announce('Error submitting result', 'center', 'error', true, false);
                },
                dataType: 'json'
            });
        });
        
        $('#candidate').change(function () {
            $.ajax ({
                url: window.location.origin + '/ElectionResult/GetParty',
                data: {'partyid' : $('#candidate').val()},
                success: function (party) {
                    console.log(party);
                   $('#partyname').html(party.Name +  ' (' + party.Acronym + ')');
                },
                dataType: 'json'
            });
        });


        $('#region').change(function () {
            $.ajax({
                url: window.location.origin + '/ElectionResult/GetSubRegions',
                data: { 'regioncode': $('#region').val() },
                success: function (result) {
                    console.log(result.SubRegions);
                    $('#lga').empty();
                    $("#lga").trigger('liszt:updated');
                    if ($('#lga').length) {
                        $.each(result.SubRegions, function (index, region) {
                            $("#lga").append('<option value="' + region.RegionID + '">' + region.Name + '</option>');
                            $("#lga").trigger('liszt:updated');
                        });
                    }
                },
                dataType: 'json'
            });
        });
    }

});

function initializeLists() {
    $.get(window.location.origin + '/ElectionResult/GetLists', function (lists) {
        if ($('#region').length) {
            $.each(lists.Regions, function (index, region) {
                $("#region").append('<option value="' + region.RegionCode + '">' + region.Name + '</option>');
                $("#region").trigger('liszt:updated');
            });
        }
        if ($('#raceType').length) {
            $.each(lists.RaceTypes, function (index, raceType) {
                $("#raceType").append('<option value="' + raceType.RaceTypeID + '" >' + raceType.Name + '</option>');
                $("#raceType").trigger('liszt:updated');
            });
        }
        if ($('#candidate').length) {
            $.each(lists.Candidates, function (index, candidate) {
                var text = candidate.FirstName + ' ' + candidate.LastName ;
                $("#candidate").append('<option  value="' + candidate.CandidateID + '" >' + text + '</option>');
                $("#candidate").trigger('liszt:updated');
            });
        }
        if ($('#party').length) {
            console.log(lists.Parties);
            $.each(lists.Parties, function (index, party) {
                var partyname = ' (' + party.Acronym + ') ' + party.Name ;
                $("#party").append('<option  value="' + party.PartyID + '" >' + partyname + '</option>');
                $("#party").trigger('liszt:updated');
            });
        }
    });
}

function initializeMap() {
    var center = new google.maps.LatLng(9.568251, 8.644524);
    map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: center,
        zoom: 6,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        preserveViewPort: true,
        suppressInfoWindows: true
    });

    $.each(regions, function (i, regionItem) {
        //console.log(regionItem);
        var polyPaths = [];
        for (var index in regionItem.coordinates) {
            polyPaths.push(new google.maps.LatLng(regionItem.coordinates[index][0], regionItem.coordinates[index][1]));

        }
        var regionPoly = new google.maps.Polygon({
            paths: polyPaths,
            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 1,
            fillColor: GetColor(i),
            fillOpacity: 0.35,
            map: map,
            preserveViewport: true
        });

        google.maps.event.addListener(regionPoly, "mouseover", function (e) {
            var polyOptions = { strokeWeight: 4.0, fillColor: 'red' };
            regionPoly.setOptions(polyOptions);
            toggleInfobox(regionItem, event, true);
        });

        google.maps.event.addListener(regionPoly, "mouseout", function (e) {
            var polyOptions = { strokeWeight: 2.0, fillColor: GetColor(2) };
            regionPoly.setOptions(polyOptions);
            toggleInfobox(regionItem, event, false);
        });

        google.maps.event.addListener(regionPoly, 'click', function (e) {
            console.log('mouse clicked on ' + regionItem.name + '(' + regionItem.regioncode + ')');
            $("#region").val(regionItem.regioncode);
            drawCharts(regionItem);
        });
    });

}


function drawCharts(region) {
    //var data = [];
    var categories = [];
    //var columndata = [];
    var piedata = [];
    var chartdata = [];
    $.ajax({
        type: 'POST',
        url: window.location.origin + '/ElectionResult/RaceResults',
        data: { "regioncode": region.regioncode },
        //contentType: "application/json",
        success: function (result) {
            //console.log(result.PieData);
            for (var index in result.PieData) {
                categories.push(result.PieData[index][0]);
                var columnitem = {}
                columnitem['type'] = 'column';
                columnitem['name'] = result.PieData[index][0];
                columnitem['data'] = [(result.PieData[index][1]) / 1000];
                chartdata.push(columnitem);

                var pieitem = {};
                pieitem['name'] = result.PieData[index][0];
                pieitem['y'] = parseFloat(result.PieData[index][1]);
                pieitem['color'] = Highcharts.getOptions().colors[index];
                piedata.push(pieitem);
            }
            var pied = {};
            pied['type'] = 'pie';
            pied['name'] = 'total votes breakdown';
            pied['data'] = piedata;
            pied['center'] = [30, 80];
            pied['size'] = 110
            pied['showInLegend'] = false;
            pied['dataLabels'] = '';

            chartdata.push(pied);

            console.log(piedata);
            console.log(chartdata);

            charttitle = result.Title;
            $('#piechart').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: charttitle
                },
                yAxis: {
                    title: { text: 'Votes in thousands' }
                },
                xAxis: {
                    title: { text: 'Political Parties' },
                    categories: ['']//categories
                },
                tooltip: {
                    //pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b> ({point.y})'
                    //pointFormat: '<b>No. of votes :</b>  {point.y} <br/> <b>% of total votes: </b>{point.percentage:.1f}%',
                    //useHTML: true,
                    //headerFormat:'<b>{point.key}</b><hr/>'
                    formatter: function () {
                        var s;
                        if (this.point.name) { // the pie chart
                            s = '' +
                                this.point.name + ': ' + (this.y).toLocaleString() + ' votes';
                        } else {
                            s = '' +
                                //this.x + ': ' + (this.y * 1000).toLocaleString() + ' votes';
                                (this.y * 1000).toLocaleString() + ' votes';
                        }
                        return s;
                    }
                },
                labels: {
                    items: [{
                        html: 'Total votes breakdown',
                        style: {
                            left: '40px',
                            top: '8px',
                            color: 'black'
                        }
                    }]
                },
                series: chartdata
            });
            console.log(new Date().getTime());
        },
        error: function (jqXHR, error, errorThrown) {
            var msg = '';
            if (jqXHR.status === 0) {
                msg = ('Not connected.\nPlease verify your network connection.');
            } else if (jqXHR.status == 404) {
                msg = ('The requested page not found. [404]');
            } else if (jqXHR.status == 500) {
                msg = ('Internal Server Error [500].');
            } else if (exception === 'parsererror') {
                msg = ('Requested JSON parse failed.');
            } else if (exception === 'timeout') {
                msg = ('Time out error.');
            } else if (exception === 'abort') {
                msg = ('Ajax request aborted.');
            } else {
                msg = ('Uncaught Error.\n' + jqXHR.responseText);
            }
            Announce(msg, 'center', 'error', true, false);
        }
    });
}


function UpdateScreen()
{
    regionItem = {};
    regionItem['name']= $('#regioncode option:selected').text();
    regionItem['regioncode']= $('#regioncode').val();
    Announce('Retrieving data for ' +  regionItem.name + " state(s) ...", 'center', 'information', false);
    drawCharts(regionItem);
    var time = new Date();
    timeStr = time.getDate()  + "/" + time.getMonth() + "/" + time.getFullYear() + " " + time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds();
    htmlStr = "<center>This page refreshes automatically every minutes no need to manually refresh" ;
    htmlStr += " (Last refresh at " + timeStr + ")";
    htmlStr += "</center>";
    $('#refreshnotice').html(htmlStr);
    

}
		
function toggleInfobox(region, event, show) {
	var infoContent= '';
	// make ajax call and get info from server
	infoContent += "<b> State: " + region.name + "</b><br />";	
	infoContent += "-------------------------<br />";	
	infoContent += "State Code: " + region.regioncode+ "<br />"	;
	if (show) {
		$('#infobox').html(infoContent);
		$('#infobox').css({ left: event.pageX + 'px', top: event.pageY + 'px', opacity: 0.9, background: '#ccc', border: '2px solid #00ff00' }).show(); 
		//$('#infobox').show();
	}else
	{
		$('#infobox').hide();
	}

}

function docReady() {

	//prevent # links from moving to top
	$('a[href="#"][data-top!=true]').click(function(e){
		e.preventDefault();
	});
    	
	//notifications
	$('.noty').click(function(e){
		e.preventDefault();
		var options = $.parseJSON($(this).attr('data-noty-options'));
		noty(options);
	});


	//uniform - styler for checkbox, radio and file input
	$("input:checkbox, input:radio, input:file").not('[data-no-uniform="true"],#uniform-is-ajax').uniform();

	//chosen - improves select
	$('[data-rel="chosen"],[rel="chosen"]').chosen();

	
	//tooltip
	//$('[rel="tooltip"],[data-rel="tooltip"]').tooltip({"placement":"bottom",delay: { show: 400, hide: 200 }});


	//popover
	//$('[rel="popover"],[data-rel="popover"]').popover();


	$('.btn-close').click(function(e){
		e.preventDefault();
		$(this).parent().parent().parent().fadeOut();
	});
	$('.btn-minimize').click(function (e) {
	    e.preventDefault();
	    var $target = $(this).parent().parent().next('.box-content');
	    if ($target.is(':visible')) {
	        $('i', $(this)).removeClass('icon-chevron-up').addClass('icon-chevron-down');
	         $(this).attr('title', 'expand');
	    } else {
	         $('i', $(this)).removeClass('icon-chevron-down').addClass('icon-chevron-up');
	         $(this).attr('title', 'collapse');
	    }
	    $target.slideToggle();
	});

}

   
function Announce(text, layout, type, modal, timeout) {
    var noty_id = noty({
        text: text,
        layout: layout,
        type: type,
        animateOpen: { opacity: "show", speed: 100,  easing: 'swing'},
        modal: modal,
        timeout: timeout
    });
}

function GetColor(state) {
	var state_color =  (state % 2) == 1 ? '#00ff00' : '#00ff00'
	try{
	}catch(e){}
	return state_color;
}

