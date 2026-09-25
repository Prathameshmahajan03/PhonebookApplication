<script setup>

import { ref } from 'vue'

const emit = defineEmits(['login'])

const username = ref('')
const password = ref('')
const loading = ref(false)
const errorMessage = ref('')
const errors = ref({
  username: '',
  password: ''
})

function validateForm() {
  errors.value = {
    username: '',
    password: ''
  }

  let isValid = true

  if (!username.value.trim()) {
    errors.value.username = 'Username is required.'
    isValid = false
  }

  if (!password.value) {
    errors.value.password = 'Password is required.'
    isValid = false
  }

  return isValid
}

function clearFieldError(field) {
  errors.value[field] = ''
  errorMessage.value = ''
}

async function handleSubmit() {
  if (loading.value || !validateForm()) {
    return
  }

  errorMessage.value = ''
  loading.value = true

  try {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        username: username.value.trim(),
        password: password.value
      })
    })

    if (!response.ok) {
      if (response.status === 401) {
        errorMessage.value = 'Invalid username or password.'
      } else if (response.status === 400) {
        errorMessage.value = 'Enter a valid username and password.'
      } else {
        errorMessage.value = 'Unable to log in right now. Please try again.'
      }

      return
    }

    const data = await response.json()

    if (!data?.token) {
      errorMessage.value = 'The server did not return a valid login token.'
      return
    }

    emit('login', {
      token: data.token,
      username: data.username || username.value.trim()
    })

    password.value = ''
  } catch {
    errorMessage.value = 'Unable to log in right now. Please try again.'
  } finally {
    loading.value = false
  }
}

</script>

<template>

  <div class="app login-page">

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

    <main class="login-main">

      <section
        class="login-card"
        aria-labelledby="login-title"
      >

        <div class="login-card-header">

          <div
            class="login-card-icon"
            aria-hidden="true"
          >
            <svg
              width="36"
              height="36"
              viewBox="0 0 48 48"
              xmlns="http://www.w3.org/2000/svg"
              aria-hidden="true"
              focusable="false"
            >
              <g fill="#14b8a6">
                <circle cx="8.5" cy="19" r="4.5" />
                <path d="M2 36.5c0-5 3.1-9 6.9-9 1.4 0 2.7.4 3.8 1.1-1.7 2.3-2.7 5.2-2.7 8.3 0 1.4-1.1 2.5-2.5 2.5h-3A2.5 2.5 0 0 1 2 36.5Z" />
                <circle cx="39.5" cy="19" r="4.5" />
                <path d="M46 36.5c0-5-3.1-9-6.9-9-1.4 0-2.7.4-3.8 1.1 1.7 2.3 2.7 5.2 2.7 8.3 0 1.4 1.1 2.5 2.5 2.5h3a2.5 2.5 0 0 0 2.5-2.5Z" />
              </g>
              <g fill="#0f766e">
                <circle cx="24" cy="15" r="6.5" />
                <path d="M12.5 39.5c0-7.2 5.1-13 11.5-13s11.5 5.8 11.5 13c0 1.4-1.1 2.5-2.5 2.5H15c-1.4 0-2.5-1.1-2.5-2.5Z" />
              </g>
            </svg>
          </div>

          <h2 id="login-title">
            Welcome back
          </h2>

          <p>
            Sign in to access your Phonebook Application.
          </p>

        </div>

        <p
          v-if="errorMessage"
          class="form-error login-error"
          role="alert"
        >
          {{ errorMessage }}
        </p>

        <form
          class="login-form"
          novalidate
          @submit.prevent="handleSubmit"
        >

          <div class="form-group">

            <label for="login-username">
              Username
            </label>

            <input
              id="login-username"
              v-model="username"
              type="text"
              placeholder="Enter your username"
              autocomplete="username"
              autocapitalize="none"
              spellcheck="false"
              maxlength="100"
              :disabled="loading"
              :aria-invalid="Boolean(errors.username)"
              :aria-describedby="errors.username ? 'login-username-error' : undefined"
              @input="clearFieldError('username')"
            />

            <p
              v-if="errors.username"
              id="login-username-error"
              class="field-error"
            >
              {{ errors.username }}
            </p>

          </div>

          <div class="form-group">

            <label for="login-password">
              Password
            </label>

            <input
              id="login-password"
              v-model="password"
              type="password"
              placeholder="Enter your password"
              autocomplete="current-password"
              maxlength="256"
              :disabled="loading"
              :aria-invalid="Boolean(errors.password)"
              :aria-describedby="errors.password ? 'login-password-error' : undefined"
              @input="clearFieldError('password')"
            />

            <p
              v-if="errors.password"
              id="login-password-error"
              class="field-error"
            >
              {{ errors.password }}
            </p>

          </div>

          <button
            class="login-button"
            type="submit"
            :disabled="loading"
          >
            {{ loading ? 'Logging in...' : 'Login' }}
          </button>

        </form>

        <p class="login-note">
          Use your Phonebook Application account to continue.
        </p>

      </section>

    </main>

    <footer class="app-footer">
      <span>
        © 2026 Phonebook Application
      </span>

      <span class="footer-highlight">
        Small App • Big Connections
      </span>
    </footer>

  </div>

</template>

<style scoped>

.login-main {
  width: 100%;

  padding: 48px 20px;

  display: flex;
  align-items: center;
  justify-content: center;

  flex: 1;
}

.login-card {
  width: 100%;
  max-width: 440px;

  padding: 34px;

  background: #ffffff;

  border: 1px solid #e5e7eb;
  border-radius: 18px;

  box-shadow: 0 8px 25px rgba(15, 23, 42, 0.06);
}

.login-card-header {
  margin-bottom: 28px;

  text-align: center;
}

.login-card-icon {
  width: 52px;
  height: 52px;

  margin: 0 auto 16px;

  display: flex;
  align-items: center;
  justify-content: center;

  background: #eff6ff;
  border-radius: 14px;

  font-size: 24px;
}

.login-card-header h2 {
  margin: 0;

  color: #0f172a;
  font-size: 22px;
  font-weight: 700;
}

.login-card-header p {
  margin: 7px 0 0;

  color: #64748b;
  font-size: 14px;
  line-height: 1.5;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.login-form input:disabled {
  background: #f8fafc;
  cursor: not-allowed;
}

.login-error {
  margin-bottom: 20px;
}

.login-button {
  width: 100%;
  height: 45px;

  margin-top: 4px;

  border: none;
  border-radius: 9px;

  background: #2563eb;
  color: #ffffff;

  font-family: inherit;
  font-size: 14px;
  font-weight: 600;

  cursor: pointer;

  transition:
    background 0.2s ease,
    transform 0.15s ease,
    box-shadow 0.2s ease;
}

.login-button:hover:not(:disabled) {
  background: #1d4ed8;

  box-shadow: 0 5px 12px rgba(37, 99, 235, 0.25);

  transform: translateY(-1px);
}

.login-button:active:not(:disabled) {
  transform: translateY(0);
}

.login-button:disabled {
  background: #93c5fd;
  cursor: not-allowed;
}

.login-note {
  margin: 22px 0 0;

  color: #94a3b8;
  font-size: 12px;
  line-height: 1.5;
  text-align: center;
}

@media (max-width: 600px) {

  .login-main {
    padding: 28px 16px;
  }

  .login-card {
    padding: 26px 20px;
  }

}

</style>
