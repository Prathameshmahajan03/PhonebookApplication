<script setup>
import { ref } from 'vue'

const searchTerm = ref('')

const emit = defineEmits(['search'])

function searchContacts() {
  emit('search', searchTerm.value)
}
</script>

<template>
  <div class="search-box">

    <div class="search-input-wrapper">

      <span class="search-icon">🔍</span>

      <input
        class="search-input"
        type="text"
        v-model="searchTerm"
        placeholder="Search contacts..."
      />

    </div>

    <button
      class="search-btn"
      @click="searchContacts"
    >
      Search
    </button>

  </div>
</template>