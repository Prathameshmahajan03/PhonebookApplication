<script setup>

import { computed } from 'vue'

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

const emit = defineEmits(['previous', 'next', 'go-to-page'])

const totalPages = computed(() => {
  const value = Math.floor(Number(props.totalPages))
  return Number.isFinite(value) ? Math.max(value, 0) : 0
})

const activePage = computed(() => {
  if (totalPages.value === 0) {
    return 1
  }

  const value = Math.floor(Number(props.currentPage))
  const safeValue = Number.isFinite(value) ? value : 1

  return Math.min(Math.max(safeValue, 1), totalPages.value)
})

const paginationItems = computed(() => {
  if (totalPages.value === 0) {
    return []
  }

  const pages = new Set([1, totalPages.value])
  const start = Math.max(2, activePage.value - 2)
  const end = Math.min(totalPages.value - 1, activePage.value + 2)

  for (let page = start; page <= end; page += 1) {
    pages.add(page)
  }

  const items = []
  let previousPage = 0

  for (const page of [...pages].sort((a, b) => a - b)) {
    if (previousPage !== 0 && page - previousPage > 1) {
      items.push({
        type: 'ellipsis',
        key: `ellipsis-${previousPage}-${page}`
      })
    }

    items.push({
      type: 'page',
      key: `page-${page}`,
      page
    })

    previousPage = page
  }

  return items
})

function goToPage(page) {
  if (
    page >= 1 &&
    page <= totalPages.value &&
    page !== activePage.value
  ) {
    emit('go-to-page', page)
  }
}

</script>

<template>
  <div class="pagination">

    <button
      type="button"
      class="pagination-btn"
      aria-label="Go to previous page"
      @click="emit('previous')"
      :disabled="activePage <= 1"
    >
      ← Previous
    </button>

    <template v-for="item in paginationItems" :key="item.key">
      <span
        v-if="item.type === 'ellipsis'"
        class="pagination-ellipsis"
        aria-hidden="true"
      >
        ...
      </span>

      <button
        v-else
        type="button"
        class="pagination-page-btn"
        :class="{ 'is-active': item.page === activePage }"
        :aria-current="item.page === activePage ? 'page' : undefined"
        :aria-label="`Go to page ${item.page}`"
        @click="goToPage(item.page)"
      >
        {{ item.page }}
      </button>
    </template>

    <button
      type="button"
      class="pagination-btn"
      aria-label="Go to next page"
      @click="emit('next')"
      :disabled="activePage >= totalPages"
    >
      Next →
    </button>

  </div>
</template>

<style scoped>

.pagination {
  flex-wrap: wrap;
  gap: 6px;
}

.pagination-page-btn,
.pagination-ellipsis {
  display: inline-flex;
  align-items: center;
  justify-content: center;

  width: 34px;
  height: 34px;

  box-sizing: border-box;
}

.pagination-page-btn {
  padding: 0;

  border: 1px solid #dbe2ea;
  border-radius: 8px;

  background: #ffffff;
  color: #334155;

  font-family: inherit;
  font-size: 13px;
  font-weight: 600;

  cursor: pointer;

  transition:
    background 0.2s ease,
    border-color 0.2s ease,
    color 0.2s ease,
    transform 0.15s ease;
}

.pagination-page-btn:hover {
  background: #eff6ff;
  border-color: #93c5fd;
  color: #2563eb;

  transform: translateY(-1px);
}

.pagination-page-btn.is-active {
  background: #2563eb;
  border-color: #2563eb;
  color: #ffffff;
}

.pagination-ellipsis {
  color: #64748b;
  font-size: 13px;
  font-weight: 600;
}

@media (max-width: 600px) {

  .pagination-page-btn,
  .pagination-ellipsis {
    width: 30px;
    height: 30px;
  }

  .pagination-page-btn {
    font-size: 12px;
  }

}

</style>
