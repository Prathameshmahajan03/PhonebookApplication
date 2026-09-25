<script setup>
import { onBeforeUnmount, ref } from 'vue'

const searchTerm = ref('')

const emit = defineEmits(['search'])

let searchTimer

function scheduleSearch() {
  clearTimeout(searchTimer)

  searchTimer = setTimeout(() => {
    searchTimer = undefined
    emit('search', searchTerm.value)
  }, 300)
}

function searchContacts() {
  clearTimeout(searchTimer)
  searchTimer = undefined
  emit('search', searchTerm.value)
}

onBeforeUnmount(() => {
  clearTimeout(searchTimer)
})
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
        @input="scheduleSearch"
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