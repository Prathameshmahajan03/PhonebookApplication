<script setup>

import { ref } from 'vue'

import SearchBox from './components/SearchBox.vue'
import ContactForm from './components/ContactForm.vue'
import ContactList from './components/ContactList.vue'
import Pagination from './components/Pagination.vue'

const searchTerm = ref('')
const currentPage = ref(1)
const pageSize = ref(10)
const totalPages = ref(1)
const selectedContact = ref(null)
const refreshKey = ref(0)

function refreshContacts() {
  refreshKey.value++
}

function handleSaved() {
  selectedContact.value = null
  refreshContacts()
}

function handleSearch(term) {
  searchTerm.value = term
  currentPage.value = 1
}

function handleEdit(contact) {
  selectedContact.value = contact
}

async function handleDelete(id) {

  const confirmed = confirm('Do you want to delete this contact?')

  if (!confirmed) {
    return
  }

  const response = await fetch(`/api/contacts/${id}`, {
    method: 'DELETE'
  })

  if (!response.ok) {
    throw new Error('Failed to delete contact.')
  }

  alert('Contact deleted successfully.')

  refreshKey.value++
}

function goToPreviousPage() {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

function goToNextPage() {
  currentPage.value++
}

function goToPage(page) {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
  }
}

</script>

<template>

  <div class="app">

    <!-- Header -->
    <header class="app-header">

      <div class="header-content">

        <div class="brand">

          <div class="brand-icon">
            📞
          </div>

          <div class="brand-text">
            <h1>Phonebook Application</h1>
            <p>Keep your important contacts always with you</p>
          </div>

        </div>

        <div class="header-badge">
          Organize <span>•</span> Manage <span>•</span> Stay Connected
        </div>

      </div>

    </header>


    <!-- Main Content -->
    <main class="main-container">

      <!-- Add / Edit Contact -->
      <section class="form-card">

        <ContactForm
          :selected-contact="selectedContact"
          @saved="handleSaved"
        />

      </section>


      <!-- Contacts -->
      <section class="contacts-card">

        <SearchBox @search="handleSearch" />

        <p
          v-if="searchTerm"
          class="search-info"
        >
          Searching for:
          <strong>{{ searchTerm }}</strong>
        </p>

        <ContactList
          :key="refreshKey"
          :search-term="searchTerm"
          :current-page="currentPage"
          :page-size="pageSize"
          @total-pages="totalPages = $event"
          @edit="handleEdit"
          @delete="handleDelete"
        />

        <Pagination
          :current-page="currentPage"
          :total-pages="totalPages"
          @previous="goToPreviousPage"
          @next="goToNextPage"
          @go-to-page="goToPage"
        />

      </section>

    </main>


    <!-- Footer -->
    <footer class="app-footer">

      <span>
        © 2026 Phonebook Application
      </span>

      <span class="footer-highlight">
        Small App • Big Connections
      </span>

      <span>
        A simple way to manage your important contacts ❤️
      </span>

    </footer>

  </div>

</template>