<template>
  <div id="app">
    <header>
      <Navbar />
    </header>

    <main class="app-content">
      <router-view />
    </main>

    <Footer />
  </div>
</template>


<script setup lang="ts">
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'

</script>

<style scoped>

</style>
