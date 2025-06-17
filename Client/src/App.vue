<template>
  <div id="app" class="bg-black">
    <header>
      <!-- your navbar -->
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
