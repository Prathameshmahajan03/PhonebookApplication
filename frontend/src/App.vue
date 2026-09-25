<script setup>

import { computed, ref } from 'vue'

import {
  AUTH_UNAUTHORIZED_EVENT,
  clearAuthStorage,
  deleteContact
} from './api/contactsApi'
import SearchBox from './components/SearchBox.vue'
import ContactForm from './components/ContactForm.vue'
import ContactList from './components/ContactList.vue'
import ContactExport from './components/ContactExport.vue'
import LoginPage from './components/LoginPage.vue'
import Pagination from './components/Pagination.vue'

const JWT_TOKEN_STORAGE_KEY = 'jwtToken'
const USERNAME_STORAGE_KEY = 'username'

const authToken = ref(localStorage.getItem(JWT_TOKEN_STORAGE_KEY) || '')
const username = ref(localStorage.getItem(USERNAME_STORAGE_KEY) || '')
const isAuthenticated = computed(() => Boolean(authToken.value))

const searchTerm = ref('')
const currentPage = ref(1)
const pageSize = ref(10)
const totalPages = ref(1)
const selectedContact = ref(null)
const refreshKey = ref(0)

function handleLogin(auth) {
  localStorage.setItem(JWT_TOKEN_STORAGE_KEY, auth.token)
  localStorage.setItem(USERNAME_STORAGE_KEY, auth.username)

  authToken.value = auth.token
  username.value = auth.username
}

function resetAuthentication() {
  clearAuthStorage()

  authToken.value = ''
  username.value = ''
}

function handleLogout() {
  resetAuthentication()
}

function handleUnauthorized() {
  resetAuthentication()
}

window.addEventListener(AUTH_UNAUTHORIZED_EVENT, handleUnauthorized)

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

  const response = await deleteContact(id)

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

  <LoginPage
    v-if="!isAuthenticated"
    @login="handleLogin"
  />

  <div
    v-else
    class="app"
  >

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

        <div class="header-actions">

          <span
            v-if="username"
            class="header-user"
          >
            {{ username }}
          </span>

          <div class="header-badge">
            Organize <span>•</span> Manage <span>•</span> Stay Connected
          </div>

          <button
            class="logout-btn"
            type="button"
            @click="handleLogout"
          >
            Logout
          </button>

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

        <ContactExport />

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

<style scoped>

.header-actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.header-user {
  max-width: 180px;

  overflow: hidden;
  text-overflow: ellipsis;

  color: #e2e8f0;
  font-size: 13px;
  font-weight: 600;

  white-space: nowrap;
}

.logout-btn {
  padding: 10px 16px;

  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 30px;

  background: rgba(255, 255, 255, 0.08);

  color: #ffffff;
  font-family: inherit;
  font-size: 13px;
  font-weight: 600;

  cursor: pointer;

  transition:
    background 0.2s ease,
    border-color 0.2s ease;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.16);
  border-color: rgba(255, 255, 255, 0.35);
}

@media (max-width: 1000px) {

  .header-actions {
    align-items: flex-start;
    flex-wrap: wrap;
  }

}

</style>