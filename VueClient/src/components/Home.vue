<template>
  <div class="container m-5 p-5">
    <h1 class="fw-bold fs-3">Welcome to My Finance App</h1>
    <button class="btn btn-primary mb-4" @click="console.log('hello')">
        Connect a bank account
    </button>
    <div class="row g-4">
        <div class="col-md-6">
          <div class="bg-secondary text-white p-4 rounded shadow">
            <div class="text-center">
              <h2 class="fs-4 fw-semibold mb-2">My accounts</h2>
              <p>Display list of bank accounts you are connected to</p>
            </div>
          </div>
        </div>
        <div class="col-md-6">
          <div class="bg-secondary text-white p-4 rounded shadow">
            <div class="text-center">
              <h2 class="fs-4 fw-semibold mb-2">Recent transactions</h2>
              <p>
                List the five most recent transactions as a preview to the user
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Second row of cards -->
      <div class="row g-4 mt-4">
        <div class="col-md-3"></div>
        <div class="col-md-6">
          <div class="bg-secondary text-white p-4 rounded shadow">
            <div class="text-center">
              <h2 class="fs-4 fw-semibold mb-2">Inflow & Income</h2>
              <p>Display list of bank accounts you are connected to</p>
            </div>
          </div>
        </div>
        <div class="col-md-3"></div>
      </div>

  </div>
</template>

<script setup lang="ts">
/*
transaction model:
  {
      date: ...,
      amount: ...,
      merchant: ...,
      category
  }
*/
</script>

<style scoped>
  * {
    color: white;
  }
</style>