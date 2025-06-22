<template>
  <div class="container m-5 p-5">
    <h1 class="fw-bold fs-3 mt-5 mb-3 text-center">Dashboard</h1>
    <!-- <div class="d-flex justify-content-center">
          <div class="card bg-dark bg-gradient" style="width: 25rem">

            <div class="card-body">
              <h5 class="card-title" style="color: white">Total Balance</h5>
              <p class="card-text">
                The total balance for this bank account (if there is one)
              </p>
            </div>

            <div class="card-footer text-muted">
              <div class="row">
                <div class="col-md-5">
                  <div>Income</div>
                  <div>Enter income amount here</div>
                </div>

                <div class="col-md-2">
                  <div class="d-flex justify-content-center"
                    style="height: 5rem">
                    <div class="vr"></div>
                  </div>
                </div>

                <div class="col-md-5">
                  <div>Expenses</div>
                  <div>Enter expense amount here</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="d-flex justify-content-center mt-3">
          <div
            class="card bg-dark bg-gradient">
            <div class="card-body">
              <h5 class="card-title" style="color: white">Recent Transactions</h5>
              <p class="card-text">
                Show the 5 most recent transactions here
              </p>
            </div>
          </div>
        </div> -->

    <div class="row m-3">
      <div class="col-lg-3"></div>
      <div class="col-lg-6">
        <DoughnutChart />
      </div>
      <div class="col-lg-3"></div>
    </div>

    <div class="row m-3">
      <div class="col-lg-3"></div>
      <div class="col-lg-6">
        <BarChart />
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

    <div class="m-3">
      <TransactionsGrid :transactions="transactions" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import TransactionsGrid from "./TransactionsGrid.vue";
import BarChart from "./chartjs/BarChart.vue";
import DoughnutChart from "./chartjs/DoughnutChart.vue";
import HorizontalStackedChart from "./chartjs/HorizontalStackedChart.vue";

const transactions = ref([
  {
    date: "2025-06-01",
    name: "Starbucks",
    amount: 5.75,
    category: "Coffee",
    note: "Morning latte",
  },
  {
    date: "2025-06-03",
    name: "Walmart",
    amount: 45.2,
    category: "Groceries",
    note: "Weekly grocery trip",
  },
  {
    date: "2025-06-04",
    name: "Spotify",
    amount: 10.99,
    category: "Subscription",
    note: "Monthly music subscription",
  },
  {
    date: "2025-06-05",
    name: "Shell",
    amount: 32.5,
    category: "Gas",
    note: "Filled up car",
  },
  {
    date: "2025-06-06",
    name: "Chipotle",
    amount: 12.0,
    category: "Dining",
    note: "Lunch with friend",
  },
  {
    date: "2025-06-08",
    name: "Amazon",
    amount: 79.99,
    category: "Shopping",
    note: "New headphones",
  },
  {
    date: "2025-06-10",
    name: "Netflix",
    amount: 15.49,
    category: "Subscription",
    note: "Monthly Netflix plan",
  },
  {
    date: "2025-06-11",
    name: "Target",
    amount: 24.75,
    category: "Household",
    note: "Cleaning supplies",
  },
  {
    date: "2025-06-12",
    name: "Uber",
    amount: 18.6,
    category: "Transport",
    note: "Ride to airport",
  },
  {
    date: "2025-06-13",
    name: "Apple Store",
    amount: 999.0,
    category: "Electronics",
    note: "Bought new iPhone",
  },
]);
</script>

<style scoped>
* {
  color: white;
}
</style>
