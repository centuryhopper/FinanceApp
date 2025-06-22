<template>
  <div class="card">
    <DataTable :value="transactions" tableStyle="min-width: 50rem">
      <Column field="date" header="Date"></Column>
      <Column field="name" header="Name"></Column>
      <Column field="amount" header="Amount"></Column>
      <Column field="category" header="Category"></Column>
      <Column field="note" header="Note"></Column>
    </DataTable>
  </div>
</template>

<script setup lang="ts">
import Column from "primevue/column";
import DataTable from "primevue/datatable";
import { onMounted, ref } from "vue";

defineProps<{ transactions: Array<any> }>()


</script>
