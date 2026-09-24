<script setup>

const props = defineProps({
  currentPage: {
    type: Number,
    default: 1
  },

  totalPages: {
    type: Number,
    default: 1
  }
})

const emit = defineEmits(['previous', 'next'])

</script>

<template>
  <div class="pagination">

    <button
      class="pagination-btn"
      @click="emit('previous')"
      :disabled="props.currentPage <= 1"
    >
      ← Previous
    </button>

    <span class="pagination-info">
      Page {{ props.currentPage }} of {{ props.totalPages }}
    </span>

    <button
      class="pagination-btn"
      @click="emit('next')"
      :disabled="props.currentPage >= props.totalPages"
    >
      Next →
    </button>

  </div>
</template>