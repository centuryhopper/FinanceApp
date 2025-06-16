<!-- src/components/Navbar.vue -->
<template>
  <nav class="navbar navbar-expand-md bg-dark border-bottom border-secondary fixed-top shadow">
    <div class="container-fluid px-4 py-2 d-flex justify-content-between align-items-center">
      <!-- hamburger - mobile only -->
      <button
          @click="isMenuOpen = true"
          class="d-md-none btn btn-link text-white p-0"
          aria-label="Open menu"
          >
          <Menu :size=28 />
      </button>

      <div class="position-absolute top-50 start-50 translate-middle text-white fw-bold text-uppercase text-center d-md-none">
          <router-link class="nav-link text-white text-decoration-none" to="/">Personal Spending</router-link>
      </div>

      <div className="d-none d-md-block text-white fw-bold text-uppercase">
          <router-link class="nav-link text-white text-decoration-none" to="/">Personal Spending</router-link>
        </div>

      <!-- desktop nav -->
       <div class="d-none d-md-flex gap-4 fw-medium">
        <router-link class="nav-link btn-dark text-white text-decoration-none"
        v-for="link in navLinks"
        :key="link.name"
        :to="link.href"
        >{{link.name}}</router-link>
      </div>

      <!-- mobile menu overlay -->
      <div v-if="isMenuOpen">
        <div
            class="position-fixed top-0 start-0 w-100 h-100 bg-black bg-opacity-75"
            style="backdropFilter: blur(4px)"
            @click="isMenuOpen = false"
          />
          <aside
            class="position-fixed top-0 start-0 bg-dark text-white p-4 d-flex flex-column gap-4 h-100 shadow animate-slide-in"
            style="width: 250px; zIndex: 1050; overflowY: auto"
          >
            <button
              @click="isMenuOpen = false"
              class="btn btn-link text-white align-self-end p-0"
              aria-label="Close menu"
            >
              <X :size=28 />
            </button>
            <router-link class="nav-link text-white text-decoration-none fs-5"
            v-for="link in navLinks"
            :key="link.name"
            :to="link.href"
            >{{link.name}}</router-link>
          </aside>
      </div>

    </div>
  </nav>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {Menu, X} from 'lucide-vue-next'

const isMenuOpen = ref(false)

const isAuthenticated = true


const navLinks = isAuthenticated
    ? [
        { name: "Home", href: "/" },
        { name: "Dashboard", href: "/dashboard" },
        { name: "Transactions", href: "/transactions" },
        { name: "Budgets", href: "/budgets" },
        { name: "Settings", href: "/settings" },
        { name: "Logout", href: "/logout" },
      ]
    : [
        {
          name: "Login",
          href: "/login",
        },
      ];

</script>

<style scoped>
  @keyframes slide-in {
    from { transform: translateX(-100%); }
    to { transform: translateX(0); }
  }
  .animate-slide-in {
    animation: slide-in 0.3s ease forwards;
  }

  .navbar .btn.btn-dark {
  position: relative;
  overflow: hidden;
  transition: color 0.3s ease;
  z-index: 0;
}

.navbar .btn.btn-dark::before {
  content: "";
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  /*background: linear-gradient(90deg, #06b6d4, #3b82f6, #8b5cf6);/* /* cyan to blue to purple */
  background-image: linear-gradient(90deg, #4b6cb7, #182848);
  opacity: 0;
  transition: opacity 0.3s ease;
  z-index: -1;
}

.navbar .btn.btn-dark:hover::before {
  opacity: 1;
}

.navbar .btn.btn-dark:hover {
  color: white; /* keep text white on hover */
}
</style>
