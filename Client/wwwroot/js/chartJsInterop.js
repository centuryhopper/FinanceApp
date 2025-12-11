Chart.register(ChartDataLabels);

function randomHexColor()
{
    return "#" + Math.floor(Math.random() * 16777215).toString(16);
}

CHARTJS = {

    createDoughnutChart: function (canvas, labels, values) {

        const background = labels.map(_ => randomHexColor());

        const chart = new Chart(canvas, {
            type: "doughnut",
            data: {
                labels: labels,
                datasets: [
                    {
                        data: values,
                        backgroundColor: background
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    datalabels: {
                        color: "#ffffff",
                        anchor: "center",
                        align: "center",
                        font: { weight: "bold" },
                        formatter: (v) => v.toFixed(2)
                    },
                    legend: {
                        display: false
                    },
                    title: {
                        display: true,
                        text: "Spendings",
                        color: "#000000",
                        font: { size: 18, weight: "bold" }
                    }
                }
            }
        });

        // Return a destroy wrapper for Blazor
        return {
            destroy: () => chart.destroy()
        };
    },

    createBarChart: function (canvas, labels, values) {
        if (!canvas) return;

        const backgroundColors = labels.map(_ => randomHexColor());

        // Destroy if already exists
        if (canvas._chartInstance) {
            canvas._chartInstance.destroy();
            canvas._chartInstance = null;
        }

        const chart = new Chart(canvas, {
            type: "bar",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: "",
                        data: values,
                        backgroundColor: backgroundColors
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    datalabels: {
                        color: "#ffffff",
                        anchor: "center",
                        align: "center",
                        font: { weight: "bold" },
                        formatter: (v) => v.toFixed(2)
                    },
                    legend: { display: false },
                    title: {
                        display: true,
                        text: "Spendings",
                        color: "#ffffff",
                        font: { size: 18, weight: "bold" }
                    }
                },
                scales: {
                    x: { ticks: { color: "#ffffff" } },
                    y: { ticks: { color: "#ffffff" } }
                }
            }
        });

        canvas._chartInstance = chart;

        return {
            destroy: () => { chart.destroy(); }
        }
    },

    createHorizontalStackedChart: function (canvas, labels, datasets, maxValue) {
        if (!window.Chart) {
            console.error("Chart.js is not loaded. Include it via CDN in _Host.cshtml or index.html");
            return null;
        }

         // Destroy if already exists
        if (canvas._chartInstance) {
            canvas._chartInstance.destroy();
            canvas._chartInstance = null;
        }

        const ctx = canvas.getContext('2d');

        const chart = new Chart(ctx, {
            type: 'bar',
            data: { labels, datasets },
            options: {
                indexAxis: 'y',  // Horizontal bars
                responsive: true,
                plugins: {
                    legend: { position: 'top', labels: { color: '#ffffff' } },
                    title: {
                        display: true,
                        text: 'Monthly Spending by Category',
                        color: '#ffffff',
                        font: { size: 18, weight: 'bold' }
                    }
                },
                scales: {
                    x: { stacked: true, min: 0, max: maxValue + 50, ticks: { color: '#ffffff' } },
                    y: { stacked: true, ticks: { color: '#ffffff' } }
                }
            }
        });

        canvas._chartInstance = chart;

        return {
            destroy: () => { chart.destroy(); }
        }
    },

};
