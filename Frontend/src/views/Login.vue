<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const email = ref('')
const password = ref('')
const errorMsg = ref('')
const isLoading = ref(false)

const handleLogin = async () => {
  errorMsg.value = ''
  isLoading.value = true
  
  try {
    // Por ahora simularemos la respuesta de la API o dejaremos el fetch listo
    /*
    const response = await fetch('http://localhost:5000/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value, password: password.value })
    })
    
    if (!response.ok) throw new Error('Credenciales incorrectas')
    const data = await response.json()
    localStorage.setItem('token', data.token)
    */

    // Simulación temporal mientras conectamos el backend
    if (email.value === 'admin@gym.com' && password.value === '123456') {
       localStorage.setItem('token', 'dummy-token-123') // Guardamos un token de prueba
       router.push('/')
    } else {
       throw new Error('Correo o contraseña incorrectos.')
    }
    
  } catch (err) {
    errorMsg.value = err.message
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="login-page">
    <div class="login-card card">
      <div class="login-header">
        <h1 class="brand-title">GymPro</h1>
        <p class="text-muted">Inicia sesión en tu cuenta</p>
      </div>

      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="email">Correo Electrónico</label>
          <input 
            type="email" 
            id="email" 
            v-model="email" 
            class="form-control" 
            placeholder="admin@gym.com" 
            required
          >
        </div>
        
        <div class="form-group">
          <label for="password">Contraseña</label>
          <input 
            type="password" 
            id="password" 
            v-model="password" 
            class="form-control" 
            placeholder="••••••••" 
            required
          >
        </div>

        <div v-if="errorMsg" class="error-message">
          {{ errorMsg }}
        </div>

        <button type="submit" class="btn btn-block" :disabled="isLoading">
          {{ isLoading ? 'Ingresando...' : 'Iniciar Sesión' }}
        </button>
      </form>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--bg-color);
  padding: 1rem;
}

.login-card {
  width: 100%;
  max-width: 400px;
  padding: 2.5rem 2rem;
}

.login-header {
  text-align: center;
  margin-bottom: 2rem;
}

.brand-title {
  color: var(--primary-color);
  font-size: 2rem;
  margin-bottom: 0.5rem;
}

.error-message {
  color: var(--danger-color);
  font-size: 0.875rem;
  margin-bottom: 1rem;
  text-align: center;
  font-weight: 500;
}
</style>
