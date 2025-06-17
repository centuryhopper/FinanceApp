<template>
    <div class="container m-5 p-5">
        <h1 class="fw-bold fs-3 mt-5 mb-3 text-center">Budgets</h1>
        <div class="d-flex justify-content-center">
          <div class="card bg-dark bg-gradient">
            <div class="card-body">
              <p class="card-text">
                <Budgetsbar category="Groceries" :spent="255" :budget="400" />
                <Budgetsbar category="Rent" :spent="1200" :budget="1200" />
                <Budgetsbar category="Entertainment" :spent="400" :budget="300" />
              </p>
            </div>
          </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import Budgetsbar from './Budgetsbar.vue'
</script>

<style scoped>
 * {
    color: white;
 }
</style>