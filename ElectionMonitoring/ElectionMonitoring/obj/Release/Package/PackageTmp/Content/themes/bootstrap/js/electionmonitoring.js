$(document).ready(function () {
    docReady();
    initializeLists();
    if ($('#map-canvas').length > 0) {
        //initializeMap();
        UpdateScreen();
        setInterval(UpdateScreen, 60000);
        $('#region').change(function () {
            UpdateScreen();
        });

        blink("#candidatename", -1, 500); //blink a div with the ID of myDiv
        /*
        // Radialize the colors
        Highcharts.getOptions().colors = Highcharts.map(Highcharts.getOptions().colors, function (color) {
            return {
                radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
                stops: [
		                [0, color],
		                [1, Highcharts.Color(color).brighten(-0.3).get('rgb')] // darken
		            ]
            };
        });*/
    } else {
        
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
                    Announce('Entered successfully', 'top', 'success', true, true);
                },
                error : function () {
                    Announce('Error submitting result', 'center', 'error', true, false);
                },
                dataType: 'json'
            });
        });
        
        $('#candidate').change(function () {
            $.ajax ({
                type: 'POST',
                url: window.location.origin + '/ElectionResult/GetParty',
                data: {'partyid' : $('#candidate').val()},
                success: function (party) {
                    console.log(party);
                   $('#partyname').html(party.Name +  ' (' + party.Acronym + ')');
                    $('#party').val(party.Name +  ' (' + party.Acronym + ')');
                },
                error : function () {
                    Announce('Error submitting result', 'center', 'error', true, false);
                },
                dataType: 'json'
            });
        });


        $('#region').change(function () {
            $.ajax({
                type: 'POST',
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
                error : function () {
                    Announce('Error submitting result', 'center', 'error', true, false);
                },
                dataType: 'json'
            });
        });
    }

});

function initializeLists() {
    $.ajax({
        type: 'POST',
        url: window.location.origin + '/ElectionResult/GetLists',
        //data: vm,
        success: function (lists) {
            //console.log(lists);
            if ( lists.Message >0 )
            {
                msg = "ERROR!!!";
                msg += "<br/>Message: " + lists.Message;
                msg += "<br/>StackTrace: " + lists.StackTrace;
                msg += "<br/>InnerException: " + lists.InnerException;
                 Announce(msg, 'center', 'error', true, false);
            }else 
            {
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
            }
        },
        error : function () {
            Announce('Error getting data', 'center', 'error', true, false);
        },
        dataType: 'json'
    });
}

function UpdateScreen() {
    // Get data
    var selectedregion = $('#region option:selected');
    Announce('Retrieving data for ' + selectedregion.text() + " state(s) ...", 'center', 'information', false);
    var vm = { "RaceID": 1, "RegionCode": selectedregion.val() };
    $.ajax({
        type: 'POST',
        url: window.location.origin + '/ElectionResult/RaceResults',
        data: vm,
        success: function (result) {
            //console.log('printing result');
            //console.log(result);
            initializeMap(result.RegionalResults);
            populateResultTable(result.SelectedRegionResults.Results);
            getResultAnalysis(result.SelectedRegionResults.Results, selectedregion);
            drawCharts(result.SelectedRegionResults.Results);
        }
    });
    
//    regionItem = {};
//    regionItem['name'] = $('#region option:selected').text();
//    regionItem['regioncode'] = $('#region').val();
//    Announce('Retrieving data for ' + regionItem.name + " state(s) ...", 'center', 'information', false);
//    drawCharts(regionItem);
//    var timeStr = getDateTime();
//    htmlStr = "<center>This page refreshes automatically every 2 minutes ";
//    htmlStr += " (Last refresh at " + timeStr + ")";
//    htmlStr += "</center>";
//    $('#refreshnotice').html(htmlStr);
}


function initializeMap(regionalResults) {
    //console.log('Map is getting reintialise');
    var center = new google.maps.LatLng(9.568251, 8.644524);
    map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: center,
        zoom: 6,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        preserveViewPort: true,
        suppressInfoWindows: true
    });

    $.each(regions, function (i, regionItem) {

        var polyPaths = [];
        for (var index in regionItem.coordinates) {
            polyPaths.push(new google.maps.LatLng(regionItem.coordinates[index][0], regionItem.coordinates[index][1]));
        }

        var regionResult = getRegionalResult(regionItem.regioncode, regionalResults);
        var winner = getWinner(regionResult);

        var color = winner.PartyColor;
        var regionPoly = new google.maps.Polygon({
            paths: polyPaths,
            strokeColor: '#000000',
            strokeOpacity: 1,
            strokeWeight: 1.5,
            fillColor: getColor(winner), //getColor(i),
            fillOpacity: 0.5,
            map: map,
            preserveViewport: true
        });

        google.maps.event.addListener(regionPoly, "mouseover", function (e) {
            var polyOptions = { strokeWeight: 4.0, fillOpacity: 0.9, fillColor: getColor(winner) };
            regionPoly.setOptions(polyOptions);
            toggleInfobox(regionItem, event, true);
        });

        google.maps.event.addListener(regionPoly, "mouseout", function (e) {
            var polyOptions = { strokeWeight: 1.5, fillOpacity: 0.5, fillColor: getColor(winner) };
            regionPoly.setOptions(polyOptions);
            toggleInfobox(regionItem, event, false);
        });

        google.maps.event.addListener(regionPoly, 'click', function (e) {
            //console.log('mouse clicked on ' + regionItem.name + '(' + regionItem.regioncode + ')');
            $("#region").val(regionItem.regioncode);
            UpdateScreen();
        });
    });

}

function getDateTime() {
    var time = new Date();
    timeStr = time.getDate() + "/" + time.getMonth() + "/" + time.getFullYear() + " " + time.getHours() + ":" + time.getMinutes() + ":" + time.getSeconds();
    return timeStr;
}

function drawCharts(selectedRegionResult) {
    if (selectedRegionResult.length > 0) {
        $('#piechart').show();
        $('#columnchart').show();
    } else {
        $('#piechart').hide();
        $('#columnchart').hide();
    }
    var totVotes = 0;
    var piedata = []; // holds series for piechart
    var categories = [];
    var coldata = []; //holds series for column chart
    var colcolor = [];
    for (var index in selectedRegionResult) {
        var resultItem = selectedRegionResult[index];
        totVotes += resultItem.TotalVotes;        
        

        coldata.push((resultItem.TotalVotes) / 1000);
        categories.push(resultItem.PartyAcronym);
        colcolor.push({
                        linearGradient:{ cx: 0.5, cy: 0.3, r: 0.7 },
                        stops: [
                            [0, resultItem.PartyColor],
                            [1, '#000']
                        ]});

        var pieitem = {};
        pieitem['name'] = resultItem.PartyAcronym;
        pieitem['y'] = parseFloat(resultItem.TotalVotes);
        pieitem['color'] = {
                            radialGradient:{ cx: 0.5, cy: 0.3, r: 0.7 },
                            stops: [
                                [0, resultItem.PartyColor],
                                [1, Highcharts.Color(resultItem.PartyColor).brighten(-0.3).get('rgb')]
                            ]},
        piedata.push(pieitem);
    }
    //console.log(columndata);
    var title = getTitle();
    // Draw column chart
    chartsubtitle = "Correct as at " + getDateTime();
    $('#columnchart').highcharts({
        chart: {
            type: 'column'
        },
        title: {
            text: title
        },
        subtitle: {
            text: 'Real time data'
        },
        series: [{ data : coldata }], //columndata,
        xAxis: {
            title: { text: 'Parties' },
            categories: categories //['']
        },
        yAxis: {
            min: 0,
            title: { text: 'Votes (thousands)' }
        },
        tooltip: {
            backgroundColor: {
                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                stops: [
                [0, 'rgba(96, 96, 96, .8)'],
                [1, 'rgba(16, 16, 16, .8)']
                ]
            },
            borderWidth: 1,
            style: {
                color: '#FFF'
            },
            formatter: function () {
                return 'Party : <b>'+ this.x +'</b><br/>'+
                        'Votes : '+ (this.y * 1000).toLocaleString();
            }
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0,
                colorByPoint: true
            }
        },
        colors: colcolor,
        dataLabels: {
            enabled: true,
            rotation: -90,
                    color: '#FFFFFF',
                    align: 'right',
                    x: 4,
                    y: 10
            },
        legend: {
             enabled: false
            /*itemStyle: {
                color: '#000'
            },
            itemHoverStyle: {
                color: '#CCC'
            },
            itemHiddenStyle: {
                color: '#333'
            }*/
        }
    });




    // Draw pie chart
    $('#piechart').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false
        },
        title: {
            text: title
        },
        subtitle: {
            text: 'Real time data'
        },
        series: [{
            type: 'pie',
            shadow: true,
            data: piedata
        }],
        tooltip: {            
            backgroundColor: {
                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                stops: [
                [0, 'rgba(96, 96, 96, .8)'],
                [1, 'rgba(16, 16, 16, .8)']
                ]
            },
            borderWidth: 1,
            style: {
                color: '#FFF'
            },
            formatter: function () {
                /*if (this.point.name) { // the pie chart
                    return '' + this.point.name + ': ' + (this.y).toLocaleString() + ' votes';
                } else {
                    return '' + (this.y * 1000).toLocaleString() + ' votes';
                }*/
                return 'Party : <b>'+ this.point.name +'</b><br/>'+
                        'Votes : '+ (this.y).toLocaleString();
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false,
                    color: '#000000',
                    connectorColor: '#000000',
                    formatter: function() {
                        return '<b>'+ this.point.name +'</b>: '+ this.percentage.toFixed(2) +' %';
                    }
                },
                showInLegend: true
            }
        },
        legend: {
            itemStyle: {
                color: '#000'
            },
            itemHoverStyle: {
                color: '#CCC'
            }
        },
        color: {
            linearGradient: { x1: 0, x2: 0, y1: 0, y1: 1 },
            stops: [
                [0, '#003399'],
                [1, '#3366AA']
            ]
        }
    });
}

function getResultAnalysis(selectedRegionResult, selectedregion) {    
    var totVotes = 0;
    for (var index in selectedRegionResult) {
        totVotes += selectedRegionResult[index].TotalVotes;
    }
    console.log(selectedRegionResult);
    var winner = getWinner(selectedRegionResult)

    var title = getTitle();
    $('#result-analysis-header').html(title);

    if (selectedRegionResult.length > 0) {
        $('#result-analysis-comments').html("<br/>With <strong>" + winner.TotalVotes.toLocaleString() + "</strong> votes <strong> <span id='candidatename'>" + winner.FirstName + ' ' + winner.LastName + "<span></strong> of <strong>" + winner.PartyAcronym + "</strong> has a <strong>" + ((winner.TotalVotes / totVotes) * 100).toFixed(2) + "%</strong> majority of a total of <strong>" + totVotes.toLocaleString() + " votes</strong>. More gibberish can be added here.. something like '... There were a total of x registered voters...  ");
    } else {
        $('#result-analysis-comments').html("<br/><br/><br/><strong>Data currently not available for " + selectedregion.text() + "</strong>");
    }
}

function populateResultTable(selectedRegionResult)
{
    // next 2 lines; dodgy way of refreshing  jquery datatable
    oTable = $('#result-analysis-table').dataTable();
    $('#result-analysis-table').dataTable().fnDestroy();            
    //instance of datatable
    oTable = $('#result-analysis-table').dataTable({
        "aaData": selectedRegionResult,
        "aoColumns": [{ "sTitle": "Candidate", "mDataProp": "FullName" },
            { "sTitle": "Party", "mDataProp": "PartyAcronym" }, 
            { "sTitle": "Votes", "mDataProp": "TotalVotes", "sClass": "alignRight"},
//            { "sTitle": "Remarks" }
        ],
        "aaSorting": [[2, "desc"]],
        "bRetrieve": true, 
        "bProcessing": true,
        "bDestroy": true,
        "bFilter": false,
		"bInfo" : false,
		"bLengthChange" : false,
        "sSortAsc": "header headerSortDown",
        "sSortDesc": "header headerSortUp",
        "sSortable": "header",
        "sDom": "<'row-fluid'<'span6'l><'span6'f><'span12 center'>r>t<'row-fluid'<'span12'i><'span12 center'p>>",
        "iDisplayLength": '5'
    });
    
}


function getRegionalResult(regioncode, result) {
    //console.log(regioncode);    
    var regionResult = [];
    for (var index in result) {
        if (regioncode == result[index].Region.RegionCode) {
            //console.log(result[index].Results);
            regionResult = result[index].Results;
        }
    }
    //console.log('');
    //console.log(regionResult);
    return regionResult;
}

function getWinner(result) {
    //console.log(result);
    var winner = {};
    if (result.length > 0) {
        winner = result[0];
    }
    for (var index in result) {
        if (winner.TotalVotes < result[index].TotalVotes) {
            winner = result[index];
        }
    }
    //if (winner.FirstName != null);
        //console.log(winner);
    return winner;
}

function getColor(contestant) {
    var color = (typeof contestant.PartyColor == 'undefined') ? '#088A29' : contestant.PartyColor;
    return color;
}

function getTitle() {
    var selectedRegion = $('#region option:selected');
    var selectedRaceType = $('#raceType option:selected');
    var title = selectedRaceType.text() + ' election result for ' + selectedRegion.text();
    if (selectedRegion.val() === '') {
        title += " states.";
    } else if (selectedRegion.val() === 'CT') {
        title += " ";
    } else {
        title += " state.";
    }
    return title;
}

function toggleInfobox(region, event, show) {
    //alert(region.name);
    var infoContent = '';
    // make ajax call and get info from server
    infoContent += "<b>" + region.name + "</b><br />";
    //infoContent += "-------------------------<br />";
    //infoContent += "State Code: " + region.regioncode + "<br />";
    if (show) {
        $('#infobox').html(infoContent);
        $('#infobox').css({ left: (event.pageX +2) + 'px', top: (event.pageY+2) + 'px', opacity: 0.9, background: '#555555', border: '2px solid #000', color: '#ffffff' }).show();
        //$('#infobox').show();
    } else {
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
	$('[rel="tooltip"],[data-rel="tooltip"]').tooltip({"placement":"bottom",delay: { show: 400, hide: 200 }});


	//popover
	//$('[rel="popover"],[data-rel="popover"]').popover();

    //tabs
	$('#myTab a:first').tab('show');
	$('#myTab a').click(function (e) {
	  e.preventDefault();
	  $(this).tab('show');
	});

    //donation
	$('#donate-button').click(function (e) {
	    e.preventDefault();
         var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth()+1; //January is 0!
        //alert('hre');
        var url =  window.location.origin + '/api/donationmanagement/donations';
        Announce(url, 'center', 'success', true, false);
        var yyyy = today.getFullYear();
        if(dd<10){dd='0'+dd} if(mm<10){mm='0'+mm} today = yyyy+'-'+mm+'-'+dd;
	    $.ajax ({
            type: 'POST',
            url: url,
            data: {'DonorID' : 1, 'Amount' : $('#amount').val(), 'DonationDate' : today},
            success: function (response) {
                console.log(response);
                 Announce('Request processed successfully', 'center', 'success', true, false);
            },
            error : function () {
                Announce('Error processing request result', 'center', 'error', true, false);
            },
            dataType: 'json'
        });
	});


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

function blink(elem, times, speed) {
    console.log('I am blinking');
    if (times > 0 || times < 0) {
        if ($(elem).hasClass("blink")) 
            $(elem).removeClass("blink");
        else
            $(elem).addClass("blink");
    }

    clearTimeout(function () {
        blink(elem, times, speed);
    });

    if (times > 0 || times < 0) {
        setTimeout(function () {
            blink(elem, times, speed);
        }, speed);
        times -= .5;
    }
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

