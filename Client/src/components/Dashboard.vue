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

    <div class="row">
      <div class="col-lg-4">
        <DoughnutChart />
      </div>
      <div class="col-lg-4">
        <BarChart />
      </div>
      <div class="col-lg-4">
        <HorizontalStackedChart />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import DoughnutChart from "./chartjs/DoughnutChart.vue";
import BarChart from "./chartjs/BarChart.vue";
import HorizontalStackedChart from "./chartjs/HorizontalStackedChart.vue";
</script>

<style scoped>
* {
  color: white;
}
</style>
