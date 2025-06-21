<template>
  <div v-if="authStre.isAuthenticated">
    <div class="container m-5 p-5">
      <h1 class="fw-bold fs-3 text-center">Welcome to My Finance App</h1>
      <button class="btn btn-primary mb-4" @click="linkPlaid">
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
              <p>List the five most recent transactions as a preview to the user</p>
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
import Swal from "sweetalert2";
import { ref, onMounted } from "vue";

import { usePlaid } from "../composables/usePlaid";
import { authStore } from "../stores/auth";

const authStre = authStore();
// console.log(authStre.claims);
const { linkPlaid } = usePlaid();

const bankLink = async (
  bankAccountName: string,
  success: boolean = true
): Promise<void> => {
  const result = await Swal.fire({
    title: success ? "Success!" : "Failure...",
    text: success
      ? `Link to ${bankAccountName} successful!`
      : `Link to ${bankAccountName} unsuccessful :/`,
    icon: success ? "success" : "error",
    confirmButtonText: "OK",
    customClass: {
      popup: "swal-dark",
    },
    allowOutsideClick: false,
  });
};

// onMounted(() => {
//   console.log('home mounted');
// })
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
