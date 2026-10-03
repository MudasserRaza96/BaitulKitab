
// dsl char no 1 
$(document).ready(function () {
    // Pagination configuration
    const itemsPerPage = 10; // Number of items per page
    let currentPage = 1; // Current page

    function renderChart(data, page) {
        // Calculate start and end indices for slicing data
        const startIndex = (page - 1) * itemsPerPage;
        const endIndex = page * itemsPerPage;
        const pageData = data.slice(startIndex, endIndex);

        // Prepare chart data
        const categories = pageData.map(x => x.eqptName || "Unknown");
        const availQty = pageData.map(x => x.availQty || 0);
        const percentAvail = pageData.map(x => x.percentAvail || 0);
        const qty = pageData.map(x => x.qty || 0);
        const percentReq = pageData.map(x => x.percentReq || 0);

        // Chart options
        const options = {
            series: [
                { name: 'Avail Qty', group: 'Availability', data: availQty },
                { name: 'Avail %', group: 'Availability', data: percentAvail },
                { name: 'Req Qty', group: 'Requirement', data: qty },
                { name: 'Req %', group: 'Requirement', data: percentReq }
            ],
            chart: {
                type: 'bar',
                height: 400,
                stacked: true,
            },
            noData: {
                text: "No data available",
                align: "center",
                verticalAlign: "middle",
                style: {
                    fontSize: "16px",
                    color: "#999",
                },
            },
            xaxis: {
                categories: categories,
                title: { text: 'Equipment' },
                labels: {
                    style: { fontSize: '12px', fontWeight: 'bold', colors: ['#333'] },
                    rotate: 320,
                    formatter: value => `${value} `
                }
            },
            yaxis: {
                title: { text: 'Values' },
                labels: { formatter: val => val.toFixed(0) }
            },
            legend: {
                position: 'top',
                horizontalAlign: 'left',
                offsetX: -15,
            },
            tooltip: {
                shared: false,
                intersect: true,
                y: { formatter: val => val.toFixed(2) }
            },
            colors: ['#008FFB', '#00E396', '#FEB019', '#FF4560'],
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: "60%",
                    dataLabels: {
                        position: "center",
                    },
                    borderRadius: 5,
                    borderRadiusApplication: 'end',
                },
            },
        };

        // Render chart
        const chartElement = document.querySelector("#dslChart");
        chartElement.innerHTML = ""; // Clear the previous chart
        const chart = new ApexCharts(chartElement, options);
        chart.render();
    }

    function updatePaginationControls(data) {
        const totalPages = Math.max(1, Math.ceil(data.length / itemsPerPage)); // Ensure at least one page exists
        const hasData = data.length > 0;

        $("#paginationControls").html(`
        <button class="btn btn-outline-primary btn-sm" id="prevPage" ${currentPage === 1 ? "disabled" : ""}>Previous</button>
        <span>Page ${currentPage} of ${totalPages}</span>
        <button class="btn btn-outline-primary btn-sm" id="nextPage" ${!hasData || currentPage === totalPages ? "disabled" : ""}>Next</button>
    `);

        // Handle navigation buttons
        $("#prevPage").off("click").on("click", function () {
            if (currentPage > 1) {
                currentPage--;
                renderChart(data, currentPage);
                updatePaginationControls(data);
            }
        });

        $("#nextPage").off("click").on("click", function () {
            if (currentPage < totalPages) {
                currentPage++;
                renderChart(data, currentPage);
                updatePaginationControls(data);
            }
        });
    }

    $.ajax({
        //url: '/SCM-CSS/DSL/GetDSLGraphData',
        method: 'GET',
        
        success: function (data) {
            if (!data || data.length === 0) {
                console.error('No data available for the chart.');
                renderChart([], currentPage);
                updatePaginationControls([]);
                return;
            }

            //var data = [];
            //for (var i = 1; i <= 10000; i++) {
            //    data.push({
            //        eqptName: `Test Eqpt ${i}`,
            //        availQty: Math.floor(Math.random() * 50) + 1, // Random between 1 and 50
            //        percentAvail: Math.floor(Math.random() * 151), // Random between 0 and 150
            //        qty: Math.floor(Math.random() * 50) + 1, // Random between 1 and 50
            //        percentReq: Math.floor(Math.random() * 101) - 50 // Random between -50 and 50
            //    });
            //}

            // Take a sample of the first 50 records for visualization
            var data = data.slice(0, 100);

            renderChart(data, currentPage);
            updatePaginationControls(data);
        },
        error: function (err) {
            renderChart([], currentPage);
            updatePaginationControls([]);
            console.error('Error loading chart data', err);
        }
    });
});

// floating stock chart
$(document).ready(function () {
    const itemsPerPage = 10;
    let currentPage = 1;
    function renderChart(data, page) {
        const startIndex = (page - 1) * itemsPerPage;
        const endIndex = page * itemsPerPage;
        const pageData = data.slice(startIndex, endIndex);

        // Get unique categories
        const uniqueCategories = [...new Set(pageData.map(x => x.eqptName || "Unknown"))];

        // Prepare chart data
        const spareQtyServicable = uniqueCategories.map(category => {
            const item = pageData.find(x => x.eqptName === category && x.stockStatus == "serviceable");
            return item ? item.spareQty : 0;
        });

        const spareQtyRepairable = uniqueCategories.map(category => {
            const item = pageData.find(x => x.eqptName === category && x.stockStatus == "repairable");
            return item ? item.spareQty : 0;
        });

        // Chart options
        const options = {
            series: [
                { name: "Servicable", data: spareQtyServicable },
                { name: "Repairable", data: spareQtyRepairable },
            ],
            chart: {
                type: "bar",
                height: 400,
                stacked: false,
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: "60%",
                    dataLabels: {
                        position: "center",
                    },
                    borderRadius: 5,
                    borderRadiusApplication: 'end',
                },
            },
            dataLabels: {
                enabled: true,
                formatter: function (val) {
                    return `${val}`;
                },
                offsetY: -5,
                style: {
                    fontSize: "10px",
                    colors: ["#000"],
                },
            },
            xaxis: {
                categories: uniqueCategories,
                title: { text: "Equipment" },
                labels: {
                    style: { fontSize: "12px", fontWeight: "bold", colors: ["#333"] },
                    rotate: 320,
                },
            },
            yaxis: {
                title: { text: "Quantity" },
                labels: {
                    formatter: (val) => `${val}`,
                },
            },
            legend: {
                position: "top",
                horizontalAlign: "center",
            },
            colors: ["#00E396", "#FF4560"],
            tooltip: {
                shared: false,
                intersect: false,
                y: {
                    formatter: function (val) {
                        return `${val}`;
                    },
                },
            },
            noData: {
                text: "No data available",
                align: "center",
                verticalAlign: "middle",
                style: {
                    fontSize: "16px",
                    color: "#999",
                },
            },
        };

        // Render chart
        const chartElement = document.querySelector("#FloatingStockLevelChart");
        chartElement.innerHTML = ""; // Clear the previous chart
        const chart = new ApexCharts(chartElement, options);
        chart.render();
    }

    function updatePaginationControls(data) {
        const totalPages = Math.max(1, Math.ceil(data.length / itemsPerPage)); 
        const hasData = data.length > 0;

        $("#FloatingPaginationControls").html(`
        <button class="btn btn-outline-primary btn-sm" id="prevPage_floating" ${currentPage === 1 ? "disabled" : ""}>Previous</button>
        <span>Page ${currentPage} of ${totalPages}</span>
        <button class="btn btn-outline-primary btn-sm" id="nextPage_floating" ${!hasData || currentPage === totalPages ? "disabled" : ""}>Next</button>
    `);

        // Handle navigation buttons
        $("#prevPage_floating").off("click").on("click", function () {
            if (currentPage > 1) {
                currentPage--;
                renderChart(data, currentPage);
                updatePaginationControls(data);
            }
        });

        $("#nextPage_floating").off("click").on("click", function () {
            if (currentPage < totalPages) {
                currentPage++;
                renderChart(data, currentPage);
                updatePaginationControls(data);
            }
        });
    }

    $.ajax({
        //url: "/SCM-CSS/DSL/GetFloatingStockGraph",
        method: "GET",
        success: function (data) {
            if (!data || data.length === 0) {
                console.error("No data available for the chart.");
                renderChart([], currentPage); 
                updatePaginationControls([]);
                return;
            }

            renderChart(data, currentPage);
            updatePaginationControls(data);
        },
        error: function (err) {
            renderChart([], currentPage);
            updatePaginationControls([]);
            console.error("Error loading chart data", err);
        },
    });
});

// CSS Stock Level
$(document).ready(function () {
    const itemsPerPage = 5; 
    let currentPage = 1;
    let totalPages = 0; 
    let allData = []; 
    function renderChart(data) {
        debugger
        const categories = data.length > 0 ? data.map((x) => x.eqptName || "Unknown") : ["No Data"];
        const spareCounts = data.length > 0 ? data.map((x) => x.spareCount || 0) : [0];

        const options = {
            series: spareCounts,
            chart: {
                height: 390,
                type: "radialBar",
            },
            plotOptions: {
                radialBar: {
                    offsetY: 0,
                    startAngle: 0,
                    endAngle: 270,
                    hollow: {
                        margin: 5,
                        size: "30%",
                        background: "transparent",
                    },
                    dataLabels: {
                        name: {
                            show: false,
                        },
                        value: {
                            show: false,
                        },
                    },
                    barLabels: {
                        enabled: true,
                        useSeriesColors: false, 
                        offsetX: -12,
                        fontSize: "12px",
                        formatter: function (seriesName, opts) {
                            return categories[opts.seriesIndex] + ": " + opts.w.globals.series[opts.seriesIndex];
                        },
                        style: {
                            fontSize: '2px',
                           // fontWeight: 'bold', 
                            colors: '#FF5733', 
                        },
                    },
                },
            },
            // Set colors for bars only (no impact on labels)
            colors: data.map((x) => {
                return x.spareCount > 25 ? "#00E396" : "#FF5733"; // Red for spareCount > 25, green otherwise
            }),
            labels: categories,
            responsive: [
                {
                    breakpoint: 480,
                    options: {
                        legend: {
                            show: false,
                        },
                    },
                },
            ],
        };



        const chartElement = document.querySelector("#cssStockLevelChart");
        chartElement.innerHTML = ""; 
        const chart = new ApexCharts(chartElement, options);
        chart.render();
    }
    function paginateData(data, page) {
        const startIndex = (page - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        return data.slice(startIndex, endIndex);
    }
    function updatePaginationControls() {
        debugger
        const paginationContainer = $("#cssPaginationControls");


        let totalPages = Math.ceil(allData.length / itemsPerPage);
        if (totalPages == 0 || totalPages == null) {
            totalPages = 1;
        }

        paginationContainer.html(`
            <button class="btn btn-outline-primary btn-sm" id="CssprevPage" ${currentPage === 1 ? "disabled" : ""}>Previous</button>
            <span>Page ${currentPage} of ${totalPages}</span>
            <button class="btn btn-outline-primary btn-sm" id="CssnextPage" ${currentPage === totalPages ? "disabled" : ""}>Next</button>
        `);


        $("#CssprevPage").off("click").on("click", function () {
            debugger
            if (currentPage > 1) {
                currentPage--;
                const paginatedData = paginateData(allData, currentPage);
                renderChart(paginatedData);
                updatePaginationControls(); 
            }
        });


        $("#CssnextPage").off("click").on("click", function () {
            if (currentPage < totalPages) {
                currentPage++;
                const paginatedData = paginateData(allData, currentPage);
                renderChart(paginatedData);
                updatePaginationControls(); 
            }
        });
    }

    function cssStock(shedId) {
        $.ajax({
            //url: "/SCM-CSS/DSL/GetCSSGraphData",
            method: "GET",
            data: { shedId: shedId },
            success: function (data) {
                //if (!data || data.length === 0) {
                //    renderChart([]); 
                //    updatePaginationControls();
                //   /* $("#cssPaginationControls").empty();*/
                //    return;
                //}
                data = [];
                for (let i = 1; i <= 10000; i++) {
                    data.push({
                        eqptName: `TestEqpt ${i}`, 
                        spareCount: Math.floor(Math.random() * 50) + 1
                    });
                  }
                  data = data.slice(0, 100);

                allData = data;
                const paginatedData = paginateData(allData, currentPage);
                renderChart(paginatedData); 
                updatePaginationControls(); 
            },
            error: function (err) {
                console.error("Error loading chart data:", err);
                renderChart([]); 
                $("#cssPaginationControls").empty(); 
            },
        });
    }

    cssStock($("#shedDropdownId option:selected").val());
    $("#shedDropdownId").on("change", function () {
        currentPage = 1; 
        const shedId = $("#shedDropdownId option:selected").val();
        cssStock(shedId);
    });
});

// UnServicable chart 
$(document).ready(function () {
    const itemsPerPage = 10;
    let currentPage = 1;

    function renderUnserviceableChart(data, page) {
        const startIndex = (page - 1) * itemsPerPage;
        const endIndex = page * itemsPerPage;
        const pageData = data.slice(startIndex, endIndex);

        // Get unique categories
        const uniqueCategories = [...new Set(pageData.map(x => x.eqptName || "Unknown"))];

        // Prepare chart data
        const unserviceableQty = uniqueCategories.map(category => {
            const item = pageData.find(x => x.eqptName === category && x.stockStatus == "unserviceable");
            return item ? item.spareQty : 0;
        });

        // Chart options
        const options = {
            series: [
                { name: "Unserviceable", data: unserviceableQty },
            ],
            chart: {
                type: "bar",
                height: 400,
                stacked: false,
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: "60%",
                    dataLabels: {
                        position: "center",
                    },
                    borderRadius: 5,
                    borderRadiusApplication: 'end',
                },
            },
            dataLabels: {
                enabled: true,
                formatter: function (val) {
                    return `${val}`;
                },
                offsetY: -5,
                style: {
                    fontSize: "10px",
                    colors: ["#000"],
                },
            },
            xaxis: {
                categories: uniqueCategories,
                title: { text: "Equipment" },
                labels: {
                    style: { fontSize: "12px", fontWeight: "bold", colors: ["#333"] },
                    rotate: 320,
                },
            },
            yaxis: {
                title: { text: "Quantity" },
                labels: {
                    formatter: (val) => `${val}`,
                },
            },
            legend: {
                position: "top",
                horizontalAlign: "center",
            },
            colors: ["#008FFB"],
            tooltip: {
                shared: false,
                intersect: false,
                y: {
                    formatter: function (val) {
                        return `${val}`;
                    },
                },
            },
            noData: {
                text: "No data available",
                align: "center",
                verticalAlign: "middle",
                style: {
                    fontSize: "16px",
                    color: "#999",
                },
            },
        };

        // Render chart
        const chartElement = document.querySelector("#UnserviceableStockChart");
        chartElement.innerHTML = ""; // Clear the previous chart
        const chart = new ApexCharts(chartElement, options);
        chart.render();
    }

    function updatePaginationControls(data) {
        const totalPages = Math.max(1, Math.ceil(data.length / itemsPerPage));
        const hasData = data.length > 0;

        $("#UnserviceablePaginationControls").html(`
        <button class="btn btn-outline-primary btn-sm" id="prevPage_unserviceable" ${currentPage === 1 ? "disabled" : ""}>Previous</button>
        <span>Page ${currentPage} of ${totalPages}</span>
        <button class="btn btn-outline-primary btn-sm" id="nextPage_unserviceable" ${!hasData || currentPage === totalPages ? "disabled" : ""}>Next</button>
    `);

        // Handle navigation buttons
        $("#prevPage_unserviceable").off("click").on("click", function () {
            if (currentPage > 1) {
                currentPage--;
                renderUnserviceableChart(data, currentPage);
                updatePaginationControls(data);
            }
        });

        $("#nextPage_unserviceable").off("click").on("click", function () {
            if (currentPage < totalPages) {
                currentPage++;
                renderUnserviceableChart(data, currentPage);
                updatePaginationControls(data);
            }
        });
    }

    $.ajax({
        //url: "/SCM-CSS/DSL/GetUnServicableGraph", 
        method: "GET",
        success: function (data) {
            if (!data || data.length === 0) {
                console.error("No data available for the chart.");
                renderUnserviceableChart([], currentPage);
                updatePaginationControls([]);
                return;
            }

            renderUnserviceableChart(data, currentPage);
            updatePaginationControls(data);
        },
        error: function (err) {
            renderUnserviceableChart([], currentPage);
            updatePaginationControls([]);
            console.error("Error loading chart data", err);
        },
    });
});
