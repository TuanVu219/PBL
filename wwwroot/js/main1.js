
// add hovered class to selected list item


let list = document.querySelectorAll(".navigation li");

function activeLink() {
    list.forEach((item) => {
        item.classList.remove("hovered");
    });
    this.classList.add("hovered");
}

list.forEach((item) => item.addEventListener("mouseover", activeLink));

// Menu Toggle
let toggle = document.querySelector(".toggle");
let navigation = document.querySelector(".navigation");
let main = document.querySelector(".main");

toggle.onclick = function () {
    navigation.classList.toggle("active");
    main.classList.toggle("active");
};

var options1 = {
    series: [{
        name: "Patients",
        data: Object.values(jsonData)
    }],
    chart: {
        height: 350,
        type: 'line',
        zoom: {
            enabled: false
        }
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        curve: 'straight'
    },
    title: {
        text: 'Patients in Month',
        align: 'left'
    },
    grid: {
        row: {
            colors: ['#f3f3f3', 'transparent'], // takes an array which will be repeated on columns
            opacity: 0.5
        },
    },
    xaxis: {
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'],
    }
};

var chart = new ApexCharts(document.querySelector("#area-chart"), options1);
chart.render();

var data = jsonData2;
var series = [];
var categories = [];

Object.keys(data).forEach(month => {
    categories.push(month);
    data[month].forEach(item => {
        var specialist = series.find(s => s.name === item.Item1);
        if (specialist) {
            specialist.data.push(item.Item2);
        } else {
            series.push({ name: item.Item1, data: [item.Item2] });
        }
    });
});
var options3 = {
     series: series,
    chart: {
        type: 'bar',
        height: 450
    },
    plotOptions: {
        bar: {
            horizontal: false,
            columnWidth: '55%',
            endingShape: 'rounded'
        },
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        show: true,
        width: 2,
        colors: ['transparent']
    },
    xaxis: {
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep'],
    },
    yaxis: {
        title: {
            text: 'person'
        }
    },
    fill: {
        opacity: 1
    },
    tooltip: {
        y: {
            formatter: function (val) {
                return val + " patients"
            }
        }
    }
};

var chart = new ApexCharts(document.querySelector("#col-chart"), options3);
chart.render();
