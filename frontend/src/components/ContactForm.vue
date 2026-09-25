<script setup>

import { ref, watch } from 'vue'

import { createContact, updateContact } from '../api/contactsApi'

const props = defineProps({
  selectedContact: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['saved'])

const form = ref({
  name: '',
  phoneNumber: '',
  email: '',
  address: ''
})

const error = ref('')

const errors = ref({
  name: '',
  phoneNumber: '',
  email: ''
})

const saving = ref(false)

watch(
  () => props.selectedContact,
  (contact) => {

    if (contact) {
      form.value.name = contact.name
      form.value.phoneNumber = contact.phoneNumber
      form.value.email = contact.email || ''
      form.value.address = contact.address || ''
    }

  }
)

function validateForm() {

    console.log('Phone entered:', form.value.phoneNumber)

  errors.value = {
    name: '',
    phoneNumber: '',
    email: ''
  }

  let isValid = true

  // Name validation
  if (!form.value.name.trim()) {
    errors.value.name = 'Name is required.'
    isValid = false
  }

  // Phone number validation
  if (!form.value.phoneNumber.trim()) {
    errors.value.phoneNumber = 'Phone number is required.'
    isValid = false
  } else if (!/^\d{10}$/.test(form.value.phoneNumber)) {
    errors.value.phoneNumber =
      'Phone number must contain exactly 10 digits.'
    isValid = false
  }

  // Email validation
  if (
    form.value.email &&
    !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)
  ) {
    errors.value.email = 'Please enter a valid email address.'
    isValid = false
  }

  return isValid
}


function resetForm() {

  form.value = {
    name: '',
    phoneNumber: '',
    email: '',
    address: ''
  }

  errors.value = {
    name: '',
    phoneNumber: '',
    email: ''
  }

  error.value = ''
}


async function handleSubmit() {

  if (!validateForm()) {
    return
  }

  error.value = ''
  saving.value = true

  try {

    const response = props.selectedContact
      ? await updateContact(props.selectedContact.id, form.value)
      : await createContact(form.value)

    if (!response.ok) {
      const errorText = await response.text()
        
      let message = 'Failed to save contact.'
        
      try {
        const data = JSON.parse(errorText)
      
        if (typeof data === 'string') {
          message = data
        } else if (data.detail) {
          message = data.detail
        } else if (data.title) {
          message = data.title
        }
      } catch {
        if (errorText) {
          message = errorText
        }
      }
    
      throw new Error(message)
    }

    alert('Contact saved successfully.')

    resetForm()

    emit('saved')

  } catch (err) {

    error.value = err.message

  } finally {

    saving.value = false

  }
}

</script>


<template>

  <div class="contact-form">

    <div class="form-header">

      <div class="form-icon">
        👤
      </div>

      <div class="form-title">

        <h2>
          {{ props.selectedContact ? 'Edit Contact' : 'Add Contact' }}
        </h2>

        <p>
          {{ props.selectedContact
            ? 'Update the contact details'
            : 'Enter the details to add a new contact'
          }}
        </p>

      </div>

    </div>


    <p v-if="error" class="form-error">
      {{ error }}
    </p>


    <form class="contact-form-fields" @submit.prevent="handleSubmit">


      <!-- Name -->

      <div class="form-group">

        <label for="contact-name">
          Name
        </label>

        <input
          id="contact-name"
          type="text"
          v-model="form.name"
          placeholder="Enter full name"
        />

        <p
          v-if="errors.name"
          class="field-error"
        >
          {{ errors.name }}
        </p>

      </div>


      <!-- Phone Number -->

      <div class="form-group">

        <label for="contact-phone">
          Phone Number
        </label>

        <input
          id="contact-phone"
          type="text"
          v-model="form.phoneNumber"
          placeholder="Enter 10 digit phone number"
        />

        <p
          v-if="errors.phoneNumber"
          class="field-error"
        >
          {{ errors.phoneNumber }}
        </p>

      </div>


      <!-- Email -->

      <div class="form-group">

        <label for="contact-email">
          Email
        </label>

        <input
          id="contact-email"
          type="text"
          v-model="form.email"
          placeholder="Enter email address"
        />

        <p
          v-if="errors.email"
          class="field-error"
        >
          {{ errors.email }}
        </p>

      </div>


      <!-- Address -->

      <div class="form-group">

        <label for="contact-address">
          Address
        </label>

        <textarea
          id="contact-address"
          v-model="form.address"
          placeholder="Enter address"
          rows="4"
        ></textarea>

      </div>


      <!-- Save Button -->

      <button
        class="save-contact-btn"
        type="submit"
        :disabled="saving"
      >
        {{ saving ? 'Saving...' : 'Save Contact' }}
      </button>


    </form>

  </div>

</template>