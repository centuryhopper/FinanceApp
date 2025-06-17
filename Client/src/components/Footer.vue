<template>
  <footer class="bg-dark bg-gradient text-light text-center py-3 mt-5">
    <div class="container">
      <small>&copy; {{ year }} My Finance App. All rights reserved.</small>
    </div>
  </footer>
</template>

<script setup lang="ts">
    const year = new Date().getFullYear()
</script>