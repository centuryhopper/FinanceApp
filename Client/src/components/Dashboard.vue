<template>
  <div v-if="didSelectBank">
    <div class="container m-5 p-5">
      <h1 class="fw-bold fs-3 mt-5 mb-3 text-center">Dashboard</h1>
      <div class="row m-3">
        <div class="col-lg-3"></div>
        <div class="col-lg-6">
          <DoughnutChart :categoryTotals="readonlyCategoryTotals" />
        </div>
        <div class="col-lg-3"></div>
      </div>

      <div class="row m-3">
        <div class="col-lg-3"></div>
        <div class="col-lg-6">
          <BarChart :categoryTotals="readonlyCategoryTotals"/>
        </div>
        <div class="col-lg-3"></div>
      </div>

      <div class="row">
        <div class="col-lg-3"></div>
        <div class="col-lg-6">
          <HorizontalStackedChart />
        </div>
        <div class="col-lg-3"></div>
      </div>

      <!-- <div class="m-3">
        <TransactionsGrid :transactions="transactions" />
      </div> -->
    </div>
  </div>
  <div v-else>
    <h1 class="text-center">
      Please select a bank from the home page to visualize its spendings.
    </h1>
  </div>
</template>

<script setup lang="ts">
import axios from "axios";
import { computed, onMounted, ref } from "vue";
import { authStore } from "../stores/auth";
import type { CategorySum } from "../types/CategorySum";
import type { Transaction } from "../types/Transactions";
import BarChart from "./chartjs/BarChart.vue";
import DoughnutChart from "./chartjs/DoughnutChart.vue";
import HorizontalStackedChart from "./chartjs/HorizontalStackedChart.vue";

const selectedBank = sessionStorage.getItem("selectedBank");
const didSelectBank = ref(!!selectedBank);
const authStre = authStore();
const transactions = ref<Transaction[]>([]);

const categoryTotals = ref<CategorySum>({});
const readonlyCategoryTotals = computed(() => categoryTotals.value ?? {});

onMounted(async () => {
  if (didSelectBank.value) {
    const response = await axios.get<Transaction[]>(
      "api/Dashboard/transactions/" + selectedBank,
      {
        headers: {
          Authorization: `Bearer ${authStre.token}`,
        },
      }
    );

    transactions.value = response.data;

    // Get all its unique categories and use an obj to store them as keys and values will be the sum of all the amounts spent in those categories:
    const uniqueCategories = [
      ...new Set(transactions.value.map((t) => t.category).filter((c) => !!c)),
    ];

    // console.log(uniqueCategories);

    const tempTotals: CategorySum = {};

    for (const category of uniqueCategories) {
      let sumTotal = transactions.value.reduce((acc, obj) => {
        return obj.category === category ? acc + obj.amount : acc;
      }, 0);

      sumTotal = Math.max(0, sumTotal);

      if (sumTotal > 0) {
        tempTotals[category!] = Number(sumTotal.toFixed(2));
      }
    }

    categoryTotals.value = tempTotals;
  }
});

// const transactions = ref([
//   {
//     date: "2025-06-01",
//     name: "Starbucks",
//     amount: 5.75,
//     category: "Coffee",
//     note: "Morning latte",
//   },
//   {
//     date: "2025-06-03",
//     name: "Walmart",
//     amount: 45.2,
//     category: "Groceries",
//     note: "Weekly grocery trip",
//   },
//   {
//     date: "2025-06-04",
//     name: "Spotify",
//     amount: 10.99,
//     category: "Subscription",
//     note: "Monthly music subscription",
//   },
//   {
//     date: "2025-06-05",
//     name: "Shell",
//     amount: 32.5,
//     category: "Gas",
//     note: "Filled up car",
//   },
//   {
//     date: "2025-06-06",
//     name: "Chipotle",
//     amount: 12.0,
//     category: "Dining",
//     note: "Lunch with friend",
//   },
//   {
//     date: "2025-06-08",
//     name: "Amazon",
//     amount: 79.99,
//     category: "Shopping",
//     note: "New headphones",
//   },
//   {
//     date: "2025-06-10",
//     name: "Netflix",
//     amount: 15.49,
//     category: "Subscription",
//     note: "Monthly Netflix plan",
//   },
//   {
//     date: "2025-06-11",
//     name: "Target",
//     amount: 24.75,
//     category: "Household",
//     note: "Cleaning supplies",
//   },
//   {
//     date: "2025-06-12",
//     name: "Uber",
//     amount: 18.6,
//     category: "Transport",
//     note: "Ride to airport",
//   },
//   {
//     date: "2025-06-13",
//     name: "Apple Store",
//     amount: 999.0,
//     category: "Electronics",
//     note: "Bought new iPhone",
//   },
// ]);
</script>

<style scoped>
/* * {
  color: white;
} */
</style>
