<script setup>

import { ref, onMounted, watch } from 'vue'

import { getContacts } from '../api/contactsApi'

const props = defineProps({
  searchTerm: {
    type: String,
    default: ''
  },

  currentPage: {
    type: Number,
    default: 1
  },

  pageSize: {
    type: Number,
    default: 10
  }
})

const emit = defineEmits(['total-pages', 'edit', 'delete'])

const contacts = ref([])
const totalPages = ref(1)
const loading = ref(false)
const error = ref('')

async function loadContacts() {

  loading.value = true
  error.value = ''

  try {

    const response = await getContacts({
      pageNumber: props.currentPage,
      pageSize: props.pageSize,
      searchTerm: props.searchTerm
    })

    if (!response.ok) {
      throw new Error('Failed to load contacts.')
    }

    const data = await response.json()

    contacts.value = data.items
    totalPages.value = data.totalPages

    emit('total-pages', data.totalPages)

  } catch (err) {

    error.value = err.message

  } finally {

    loading.value = false

  }
}

onMounted(() => {
  loadContacts()
})

watch(
  [() => props.searchTerm, () => props.currentPage],
  () => {
    loadContacts()
  }
)

</script>

<template>
  <div class="contact-list">

    <div class="contact-list-header">
      <div>
        <h2>Contacts</h2>
        <p>View, search and manage your contacts</p>
      </div>

      <div class="contact-count">
        {{ contacts.length }} Contacts
      </div>
    </div>

    <div v-if="loading" class="table-loading">
      <span class="loading-spinner"></span>
      <span>Loading contacts...</span>
    </div>

    <p v-if="error" class="table-error">
      {{ error }}
    </p>

    <div v-if="!loading && !error" class="table-wrapper">

      <table class="contacts-table">

        <thead>
          <tr>
            <th>Name</th>
            <th>Phone Number</th>
            <th>Email</th>
            <th>Address</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>

          <tr
            v-for="contact in contacts"
            :key="contact.id"
          >

            <td class="contact-name">
              {{ contact.name }}
            </td>

            <td class="contact-phone">
              {{ contact.phoneNumber }}
            </td>

            <td class="contact-email">
              {{ contact.email || '-' }}
            </td>

            <td class="contact-address">
              {{ contact.address || '-' }}
            </td>

            <td class="contact-actions">

              <button
                class="edit-btn"
                @click="emit('edit', contact)"
              >
                Edit
              </button>

              <button
                class="delete-btn"
                @click="emit('delete', contact.id)"
              >
                Delete
              </button>

            </td>

          </tr>

          <tr v-if="contacts.length === 0">

            <td
              colspan="5"
              class="empty-state"
            >
              No contacts found.
            </td>

          </tr>

        </tbody>

      </table>

    </div>

  </div>
</template>