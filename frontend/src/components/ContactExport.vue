<script setup>

import { ref } from 'vue'

import { exportContacts } from '../api/contactsApi'

const exporting = ref('')
const error = ref('')

async function downloadContacts(format) {
  if (exporting.value) {
    return
  }

  exporting.value = format
  error.value = ''

  try {
    const response = await exportContacts(format)

    if (!response.ok) {
      throw new Error('Unable to export contacts.')
    }

    const blob = await response.blob()
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')

    link.href = url
    link.download = `contacts.${format}`

    document.body.appendChild(link)
    link.click()
    link.remove()

    URL.revokeObjectURL(url)
  } catch (err) {
    error.value = err instanceof Error
      ? err.message
      : 'Unable to export contacts.'
  } finally {
    exporting.value = ''
  }
}

</script>

<template>

  <div class="contact-export">

    <span class="export-label">
      Export:
    </span>

    <button
      class="export-btn"
      type="button"
      :disabled="Boolean(exporting)"
      @click="downloadContacts('csv')"
    >
      {{ exporting === 'csv' ? 'Exporting...' : 'Export CSV' }}
    </button>

    <button
      class="export-btn"
      type="button"
      :disabled="Boolean(exporting)"
      @click="downloadContacts('json')"
    >
      {{ exporting === 'json' ? 'Exporting...' : 'Export JSON' }}
    </button>

    <p
      v-if="error"
      class="export-error"
      role="alert"
    >
      {{ error }}
    </p>

  </div>

</template>

<style scoped>

.contact-export {
  display: flex;
  align-items: center;
  justify-content: flex-end;

  gap: 8px;

  margin: -6px 0 18px;
}

.export-label {
  color: #64748b;
  font-size: 13px;
  font-weight: 600;
}

.export-btn {
  padding: 9px 12px;

  border: 1px solid #bfdbfe;
  border-radius: 8px;

  background: #eff6ff;
  color: #2563eb;

  font-family: inherit;
  font-size: 12px;
  font-weight: 600;

  cursor: pointer;

  transition:
    background 0.2s ease,
    border-color 0.2s ease,
    transform 0.15s ease;
}

.export-btn:hover:not(:disabled) {
  background: #dbeafe;
  border-color: #93c5fd;

  transform: translateY(-1px);
}

.export-btn:active:not(:disabled) {
  transform: translateY(0);
}

.export-btn:disabled {
  background: #f8fafc;
  border-color: #e2e8f0;
  color: #94a3b8;

  cursor: not-allowed;
}

.export-error {
  margin: 0;

  color: #dc2626;
  font-size: 12px;
}

@media (max-width: 600px) {

  .contact-export {
    justify-content: flex-start;
    flex-wrap: wrap;
  }

}

</style>
