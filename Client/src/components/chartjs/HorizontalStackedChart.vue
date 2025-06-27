<script setup lang="ts">
import {
  BarElement,
  CategoryScale,
  Chart as ChartJS,
  Legend,
  LinearScale,
  Title,
  Tooltip,
  type ChartOptions,
} from "chart.js";
import { Bar } from "vue-chartjs";

ChartJS.register(BarElement, CategoryScale, LinearScale, Title, Tooltip, Legend);

const chartData = {
  labels: ["January", "February", "March"],
  datasets: [
    {
      label: "Groceries",
      data: [200, 150, 100],
      backgroundColor: "#22c55e",
    },
    {
      label: "Transport",
      data: [100, 120, 130],
      backgroundColor: "#3b82f6",
    },
    {
      label: "Utilities",
      data: [50, 60, 90],
      backgroundColor: "#f59e0b",
    },
  ],
};

const chartOptions: ChartOptions<"bar"> = {
  responsive: true,
  indexAxis: "y", // ✅ Now TS knows this is one of the allowed values
  plugins: {
    legend: {
      position: "top", // ✅ No need for 'as const' when typed properly
    },
    title: {
      display: true,
      text: "Monthly Total",
      color: "#ffffff",
    },
  },
  scales: {
    x: {
      stacked: true,
    },
    y: {
      stacked: true,
    },
  },
};
</script>

<template>
  <Bar :data="chartData" :options="chartOptions" />
</template>
