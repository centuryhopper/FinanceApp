<!-- components/BarChart.vue -->
<script setup lang="ts">
import { Bar } from "vue-chartjs";
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
} from "chart.js";

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

const chartData = {
  labels: ["Red", "Blue", "Yellow"],
  datasets: [
    {
      label: "Spendings",
      data: [12, 19, 3],
      backgroundColor: ["#f87171", "#60a5fa", "#facc15"],
    },
  ],
};

const chartOptions = {
  responsive: true,
  plugins: {
    title: {
      display: true,
      text: "Color Popularity",
      color: "#ffffff", // White title text
      font: {
        size: 18,
        weight: "bold",
      },
      padding: {
        top: 10,
        bottom: 20,
      },
    },
    legend: {
      position: "top",
      labels: {
        color: "#ffffff", // Optional: make legend text white too
      },
    },
  },
  scales: {
    x: {
      ticks: {
        color: "#ffffff", // Make x-axis labels white
      },
    },
    y: {
      ticks: {
        color: "#ffffff", // Make y-axis labels white
      },
    },
  },
};
</script>

<template>
  <div style="width: 25rem; height: 25rem">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>
