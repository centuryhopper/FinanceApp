<template>
  <div class="m-5 container">
    <!-- Toggle Mode -->
    <button class="btn btn-primary mb-3" @click="toggleMode">
      {{ mode === Mode.Readonly ? 'Edit' : 'View' }}
    </button>

    <!-- READONLY MODE -->
    <DataTable v-if="mode === Mode.Readonly" :data="data" class="display" style="width: 100%">
      <thead>
        <tr>
          <th>Date</th>
          <th>Name</th>
          <th>Amount</th>
          <th>Category</th>
          <th>Note</th>
        </tr>
      </thead>
    </DataTable>

    <!-- EDIT MODE -->
    <div v-else>
      <!-- Filter/Search Controls -->
      <div class="d-flex justify-content-between mb-3">
        <input v-model="searchText" placeholder="Search..." class="form-control w-50 me-2" />

        <select v-model="categoryFilter" class="form-select w-25">
          <option value="">All Categories</option>
          <option v-for="category in uniqueCategories" :key="category">{{ category }}</option>
        </select>
      </div>

      <!-- Editable Table -->
      <table class="table table-bordered" style="width: 100%">
        <thead>
          <tr>
            <th>Date</th>
            <th>Name</th>
            <th>Amount</th>
            <th>Category</th>
            <th>Note</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(row, rowIndex) in paginatedData"
            :key="rowIndex"
          >
            <td><input type="date" v-model="row[0]" class="form-control" /></td>
            <td><input type="text" v-model="row[1]" class="form-control" /></td>
            <td><input type="text" v-model="row[2]" class="form-control" /></td>
            <td><input type="text" v-model="row[3]" class="form-control" /></td>
            <td><input type="text" v-model="row[4]" class="form-control" /></td>
            <td>
              <button
                class="btn btn-success btn-sm"
                @click="saveRow(row, getGlobalIndex(row))"
                :disabled="savingRowIndex === getGlobalIndex(row)"
              >
                <span v-if="savingRowIndex === getGlobalIndex(row)">Saving...</span>
                <span v-else>Save</span>
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination Controls -->
      <div class="d-flex justify-content-between align-items-center">
        <div>
          <label>Rows per page:</label>
          <select v-model.number="rowsPerPage" class="form-select d-inline-block w-auto ms-2">
            <option :value="5">5</option>
            <option :value="10">10</option>
            <option :value="15">15</option>
          </select>
        </div>

        <div>
          <button
            class="btn btn-secondary me-2"
            :disabled="currentPage === 1"
            @click="currentPage--"
          >
            Prev
          </button>
          <span>Page {{ currentPage }} / {{ totalPages }}</span>
          <button
            class="btn btn-secondary ms-2"
            :disabled="currentPage === totalPages"
            @click="currentPage++"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, reactive, ref } from 'vue';
import DataTablesCore from 'datatables.net';
import DataTable from 'datatables.net-vue3';
import 'datatables.net-dt/css/dataTables.dataTables.css';

DataTable.use(DataTablesCore);

enum Mode {
  Readonly = 'readonly',
  Edit = 'edit',
}

const mode = ref<Mode>(Mode.Readonly);
function toggleMode() {
  mode.value = mode.value === Mode.Readonly ? Mode.Edit : Mode.Readonly;
}

const data = reactive([
  ['2023-11-14', 'Company Co.', '$2000', 'Salary', 'Monthly pay'],
  ['2023-11-15', 'Netflix', '-$15', 'Subscription', 'Monthly sub'],
  ['2023-11-16', 'Apple', '-$99', 'App Store', 'One-time app'],
  ['2023-11-17', 'Spotify', '-$10', 'Entertainment', 'Music'],
  ['2023-11-18', 'Upwork', '$520', 'Freelance', 'Side project'],
  ['2023-11-19', 'Google', '$1800', 'Salary', 'Second payment'],
  ['2023-11-20', 'YouTube Premium', '-$12', 'Subscription', 'Streaming'],
]);

// Filters
const searchText = ref('');
const categoryFilter = ref('');

// Pagination
const currentPage = ref(1);
const rowsPerPage = ref(5);

// Filtered & Searched
const filteredData = computed(() =>
  data.filter((row) => {
    const matchesSearch =
      searchText.value === '' ||
      row.some((cell) =>
        cell.toLowerCase?.().includes(searchText.value.toLowerCase())
      );

    const matchesCategory =
      categoryFilter.value === '' || row[3] === categoryFilter.value;

    return matchesSearch && matchesCategory;
  })
);

// Pagination logic
const totalPages = computed(() =>
  Math.ceil(filteredData.value.length / rowsPerPage.value)
);

const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * rowsPerPage.value;
  return filteredData.value.slice(start, start + rowsPerPage.value);
});

// Extract unique categories
const uniqueCategories = computed(() => {
  const set = new Set<string>();
  data.forEach((row) => set.add(row[3]));
  return [...set];
});

// Track saving row
const savingRowIndex = ref<number | null>(null);

// Get global row index from paginated row reference
function getGlobalIndex(row: string[]): number {
  return data.findIndex((r) => r === row);
}

// Save row to "API"
async function saveRow(row: string[], index: number) {
  savingRowIndex.value = index;
  try {
    console.log('Saving row to API:', row);
    await new Promise((resolve) => setTimeout(resolve, 1000));
    alert(`Row ${index + 1} saved!`);
  } catch (err) {
    alert(`Failed to save row ${index + 1}`);
  } finally {
    savingRowIndex.value = null;
  }
}

defineProps<{ transactions: Array<any> }>();
</script>

<style scoped>
input.form-control {
  width: 100%;
  padding: 4px;
  box-sizing: border-box;
}
</style>
