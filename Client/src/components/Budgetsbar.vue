<template>
    <div class="m-1 p-3 rounded shadow-sm bg-black" style="width: 30rem">
      <div class="d-flex justify-content-between align-items-center mb-1">
        <span class="fw-semibold text-white">{{category}}</span>
        <span class="fw-bold text-white">${{budget.toFixed(2)}}</span>
      </div>
      <div class="progress" style="height: 8px">
        <div
          class="progress-bar bg-primary"
          role="progressbar"
          :style="{width: progress + '%'}"
          :aria-valuenow="spent"
          aria-valuemin="0"
          :aria-valuemax="budget"
        ></div>
      </div>
      <div class="text-end mt-1 small" :class="'text-' + spentColor"
      >${{spent.toFixed(2)}}</div>
    </div>
</template>

<script setup lang="ts">
    import type { BudgetBarProps } from '../types/BudgetTypes'
    const {
        category,
        spent,
        budget,
    } = defineProps<BudgetBarProps>()
    const progress = Math.min((spent / budget) * 100, 100);
    const spentColor = spent <= budget ? 'white' : 'danger'

    // export default {
    //   name: "Budgetsbar"
    // }

</script>