<template>
  <div v-if="authStre.isAuthenticated">
    <div class="container m-5 p-5">
      <h1 class="fw-bold fs-3 text-center m-3">Welcome to My Finance App</h1>
      <div class="text-center m-3">
        <button class="btn btn-primary mb-4" @click="linkPlaid">
          Connect a bank account
        </button>
      </div>

      <div class="row g-4">
        <div class="col-md-6">
          <div
            class="card p-3 shadow-sm w-100 h-100 d-flex flex-column"
            style="max-width: 300px"
          >
            <h5 class="text-center">My accounts</h5>
            <div v-if="!banksLoaded">
              <div class="d-flex justify-content-center">
                <div class="spinner-border" role="status">
                  <span class="visually-hidden">Loading...</span>
                </div>
              </div>
            </div>
            <div v-else-if="banks.length === 0 && banksLoaded">
              <p>No bank accounts were added yet</p>
            </div>
            <div v-else>
              <ul
                class="list-unstyled mb-0 flex-grow-1 overflow-auto align-content-around"
              >
                <li
                  :key="bank.bankinfoid"
                  v-for="bank in banks"
                  class="d-flex justify-content-between mb-1 bank-selection"
                  @click="onBankSelection(bank.bankname)"
                >
                  <span>{{ bank.bankname }}</span>
                  <strong> ${{ bank.totalbankbalance }}</strong>
                </li>
              </ul>

              <hr class="my-2" />
              <div class="d-flex justify-content-between mt-auto pt-2">
                <span>Total</span>
                <strong>${{ balanceTotal }}</strong>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-6">
          <div class="card p-3 shadow-sm w-100 h-100" style="max-width: 500px">
            <h5 class="mb-3 fw-semibold text-center">Most recent transactions</h5>
            <div v-if="!transactionsLoaded">
              <div class="d-flex justify-content-center">
                <div class="spinner-border" role="status">
                  <span class="visually-hidden">Loading...</span>
                </div>
              </div>
            </div>
            <div v-else-if="recentTransactions.length === 0 && transactionsLoaded">
              <p>No transactions were saved yet</p>
            </div>
            <div v-else>
              <ul class="list-unstyled">
                <li
                  :key="transaction.id"
                  v-for="transaction in recentTransactions"
                  class="d-flex align-items-center mb-2 gap-2"
                >
                  <div class="text-nowrap text-center" style="width: 90px">
                    {{ transaction.date }}
                  </div>
                  <div
                    :class="`fw-semibold  ${
                      transaction.amount < 0
                        ? 'badge bg-danger-subtle text-danger'
                        : 'badge bg-success-subtle text-success'
                    } rounded-pill text-center`"
                    style="width: 90px"
                  >
                    {{ transaction.amount > 0 ? "+" : "-" }}${{
                      Math.abs(transaction.amount)
                    }}
                  </div>
                  <div class="text-center flex-grow-1 text-center">
                    {{ transaction.name }}
                  </div>
                  <span
                    class="badge bg-success-subtle text-success rounded-pill text-center"
                  >
                    {{ transaction.category }}
                  </span>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div v-else>
    <div class="container m-5 p-5">
      <h1 className="fw-bold fs-3 text-center">Please Sign in to continue</h1>
      <!-- <button class="btn btn-success m-3" @click="bankLink('test', true)">
        success button
      </button>

      <button class="btn btn-danger m-3" @click="bankLink('test', false)">
        error button
      </button> -->
    </div>
  </div>
</template>

<script setup lang="ts">
import axios from "axios";
import { computed, onMounted, ref } from "vue";
import { usePlaid } from "../composables/usePlaid";
import useSweetAlertPopups from "../composables/useSweetAlertPopups";
import { authStore } from "../stores/auth";
import type { BankInfo } from "../types/BankInfo";
import type { Transaction } from "../types/Transactions";

const authStre = authStore();
// console.log(authStre.claims);
const { linkPlaid } = usePlaid();
const { showFeedbackPopup } = useSweetAlertPopups();

const banks = ref<BankInfo[]>([]);
let banksLoaded = ref(false);
const balanceTotal = computed(() =>
  banks.value.reduce((acc, currentAccount) => currentAccount.totalbankbalance + acc, 0)
);

const recentTransactions = ref<Transaction[]>([]);
const transactionsLoaded = ref(false);

const onBankSelection = async (bankName: string) => {
  // get transactions depending on the bank selected
  const currentInstitution = sessionStorage.getItem("selectedBank");
  if (bankName === currentInstitution) {
    console.log("already selected");
    return;
  }

  transactionsLoaded.value = false;

  const transactionsResponse = await axios.get<Transaction[]>(
    "api/Bank/recent-transactions/" + bankName,
    {
      headers: {
        Authorization: `Bearer ${authStre.token}`,
      },
    }
  );

  recentTransactions.value = transactionsResponse.data;
  transactionsLoaded.value = true;
};

onMounted(async () => {
  if (!authStre.isAuthenticated) {
    return;
  }
  try {
    const bankResponse = await axios.get<BankInfo[]>("api/Bank/get-banks", {
      headers: {
        Authorization: `Bearer ${authStre.token}`,
      },
    });
    // console.log(transactionsResponse);
    banks.value = bankResponse.data;
    banksLoaded.value = true;

    if (banks.value.length === 0) {
      return;
    }

    const firstInstitution = banks.value[0].bankname;
    sessionStorage.setItem("selectedBank", firstInstitution);

    const transactionsResponse = await axios.get<Transaction[]>(
      "api/Bank/recent-transactions/" + firstInstitution,
      {
        headers: {
          Authorization: `Bearer ${authStre.token}`,
        },
      }
    );

    recentTransactions.value = transactionsResponse.data;
    transactionsLoaded.value = true;
  } catch (e) {
    console.error("Error fetching banks:", e);
    await showFeedbackPopup(false, "", "Please try again later.");
  }
});

/*
transaction model:
  {
      date: ...,
      amount: ...,
      merchant: ...,
      category: ...,
  }
*/
</script>

<style scoped>
.bank-selection {
  cursor: pointer;
}
.bank-selection:hover {
  background-image: linear-gradient(135deg, #6e8efb, #a777e3);
}
</style>
