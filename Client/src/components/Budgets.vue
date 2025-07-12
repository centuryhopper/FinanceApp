<template>
  <div class="container m-5 p-5">
    <h1 class="fw-bold fs-3 mt-5 mb-3 text-center">
      Budget for {{ currentMonthString }}
    </h1>
    <div v-if="!spendingsLoaded">
      <div class="d-flex justify-content-center">
        <div class="spinner-border" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
    </div>
    <div v-else-if="spendings.items.length === 0 && spendingsLoaded">
      <div class="m-3">No budget logistics to show</div>
    </div>
    <div v-else>
      <div class="d-flex justify-content-center">
        <div :class="`card ${isDark ? 'bg-dark' : ''} bg-gradient`">
          <div class="card-body">
            <div class="card-text">
              <Budgetsbar
                v-for="spending in spendings.items"
                :key="spending.id"
                :id="spending.id"
                :category="spending.category"
                :spent="spending.spent"
                :budget="spending.budgetCap"
                @update:budgetLimit="handleBudgetLimitUpdate"
              />
              <!-- listen for that change -->
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import axios from "axios";
import { onMounted, reactive, ref } from "vue";
import { authStore } from "../stores/auth";
import { useTheme } from "../stores/theme-store";
import type { CurrentMonthlySpending } from "../types/CurrentMonthlySpending";
import Budgetsbar from "./Budgetsbar.vue";

const { isDark } = useTheme();

const handleBudgetLimitUpdate = (id: number, newBudgetLimit: number) => {
  updatebudgetLimit(id, newBudgetLimit);
};

const updatebudgetLimit = (id: number, newLimit: number) => {
  const item = spendings.items.find((b) => b.id === id);
  if (item) item.budgetCap = newLimit;
  console.log("updated budget limit", id);
  // TODO: make api call here
};

const monthNames = [
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
];

const currentMonthString = monthNames[new Date().getMonth()];
const selectedBank = sessionStorage.getItem("selectedBank");
const didSelectBank = ref(!!selectedBank);
const authStre = authStore();
const spendings = reactive<{ items: CurrentMonthlySpending[] }>({
  items: [],
});
const spendingsLoaded = ref(false);

onMounted(async () => {
  if (!didSelectBank.value) {
    // console.log("didSelectBank.value", didSelectBank.value);
    return;
  }
  const response = await axios.get<{ payload: CurrentMonthlySpending[] }>(
    "api/Budgets/current-month-spending-by-category/" + selectedBank,
    {
      headers: {
        Authorization: `Bearer ${authStre.token}`,
      },
    }
  );
  spendingsLoaded.value = true;
  spendings.items = response.data.payload;
});

// const budgetInfo = ref([
//   {
//     id: 0,
//     category: "Groceries",
//     budgetCap: 400,
//     spent: 255,
//   },
//   {
//     id: 1,
//     category: "Rent",
//     budgetCap: 1200,
//     spent: 1200,
//   },
//   {
//     id: 2,
//     category: "Entertainment",
//     budgetCap: 400,
//     spent: 300,
//   },
// ]);
</script>

<style scoped>
/* * {
  color: white;
} */
</style>
