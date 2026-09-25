const TOKEN_STORAGE_KEY = 'jwtToken'
const USERNAME_STORAGE_KEY = 'username'

export const AUTH_UNAUTHORIZED_EVENT = 'phonebook:auth-unauthorized'

export function clearAuthStorage() {
  localStorage.removeItem(TOKEN_STORAGE_KEY)
  localStorage.removeItem(USERNAME_STORAGE_KEY)
}

function notifyUnauthorized() {
  clearAuthStorage()
  window.dispatchEvent(new Event(AUTH_UNAUTHORIZED_EVENT))
}

function getAuthHeaders(includeJsonContentType = false) {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY)

  if (!token) {
    notifyUnauthorized()
    throw new Error('Authentication is required.')
  }

  const headers = {
    Authorization: `Bearer ${token}`
  }

  if (includeJsonContentType) {
    headers['Content-Type'] = 'application/json'
  }

  return headers
}

async function sendRequest(url, options) {
  const response = await fetch(url, options)

  if (response.status === 401) {
    notifyUnauthorized()
  }

  return response
}

export function getContacts({ pageNumber, pageSize, searchTerm }) {
  const query = new URLSearchParams({
    pageNumber,
    pageSize,
    searchTerm: searchTerm || ''
  })

  return sendRequest(`/api/contacts?${query.toString()}`, {
    method: 'GET',
    headers: getAuthHeaders()
  })
}

export function getContactById(id) {
  return sendRequest(`/api/contacts/${id}`, {
    method: 'GET',
    headers: getAuthHeaders()
  })
}

export function createContact(contact) {
  return sendRequest('/api/contacts', {
    method: 'POST',
    headers: getAuthHeaders(true),
    body: JSON.stringify(contact)
  })
}

export function updateContact(id, contact) {
  return sendRequest(`/api/contacts/${id}`, {
    method: 'PUT',
    headers: getAuthHeaders(true),
    body: JSON.stringify(contact)
  })
}

export function deleteContact(id) {
  return sendRequest(`/api/contacts/${id}`, {
    method: 'DELETE',
    headers: getAuthHeaders()
  })
}
