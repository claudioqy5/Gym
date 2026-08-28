<template>
  <div class="memberships-view">
    <div class="page-header" style="display: flex; justify-content: space-between; align-items: center;">
      <h1 class="page-title">Planes y Membresías</h1>
      <button class="btn" @click="showModal = true">
        + Nuevo Plan
      </button>
    </div>

    <div v-if="isLoading" style="text-align: center; padding: 3rem; color: var(--text-muted);">Cargando planes...</div>
    
    <div v-else style="display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem;">
      <div v-for="plan in memberships" :key="plan.id" class="card" style="text-align: center;">
        <h3 style="color: var(--primary-color);">{{ plan.name }}</h3>
        <p style="font-size: 2rem; font-weight: 700; margin: 1rem 0;">${{ plan.price.toFixed(2) }}</p>
        <p style="color: var(--text-muted); margin-bottom: 0.5rem;">Duración: {{ plan.durationInDays }} días</p>
        <p style="margin-bottom: 1.5rem;">
          <span class="badge" :class="plan.isActive ? 'badge-success' : 'badge-danger'">{{ plan.isActive ? 'Activo' : 'Inactivo' }}</span>
        </p>
        <button class="btn btn-block" style="background-color: var(--bg-color); color: var(--text-main); border: 1px solid var(--border-color);">Editar Plan</button>
      </div>

      <div class="card" @click="showModal = true" style="text-align: center; border: 1px dashed var(--border-color); background-color: transparent; display: flex; flex-direction: column; justify-content: center; align-items: center; cursor: pointer; transition: all 0.2s;" @mouseover="hover = true" @mouseleave="hover = false" :style="hover ? 'border-color: var(--primary-color);' : ''">
        <div style="font-size: 3rem; color: var(--text-muted); margin-bottom: 1rem;">+</div>
        <h3 style="color: var(--text-muted);">Crear Nuevo Plan</h3>
      </div>
    </div>

    <!-- Modal Crear Plan -->
    <div v-if="showModal" class="modal-overlay" @click.self="showModal = false">
      <div class="modal-content">
        <h2 style="margin-bottom: 1.5rem;">Crear Nuevo Plan</h2>
        <form @submit.prevent="savePlan">
          <div class="form-group">
            <label>Nombre del Plan</label>
            <input type="text" v-model="form.name" class="form-control" required placeholder="Ej. Plan Anual VIP">
          </div>
          
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div class="form-group">
              <label>Precio ($)</label>
              <input type="number" step="0.01" v-model="form.price" class="form-control" required>
            </div>
            <div class="form-group">
              <label>Duración (Días)</label>
              <input type="number" v-model="form.durationInDays" class="form-control" required>
            </div>
          </div>
          
          <div class="form-group">
            <label style="display: flex; align-items: center; gap: 0.5rem; font-weight: normal;">
              <input type="checkbox" v-model="form.isActive"> Plan Activo (Visible para ventas)
            </label>
          </div>

          <div style="display: flex; justify-content: flex-end; gap: 1rem; margin-top: 2rem;">
            <button type="button" class="btn-outline" @click="showModal = false">Cancelar</button>
            <button type="submit" class="btn" :disabled="isSaving">{{ isSaving ? 'Guardando...' : 'Guardar Plan' }}</button>
          </div>
        </form>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const hover = ref(false)
const showModal = ref(false)
const isLoading = ref(false)
const isSaving = ref(false)
const memberships = ref([])

const form = ref({
  name: '',
  price: 0,
  durationInDays: 30,
  isActive: true
})

const fetchMemberships = async () => {
  isLoading.value = true
  try {
    const res = await fetch('http://localhost:5243/api/memberships')
    const data = await res.json()
    memberships.value = data
  } catch (error) {
    console.error('Error fetching memberships:', error)
  } finally {
    isLoading.value = false
  }
}

const savePlan = async () => {
  isSaving.value = true
  try {
    const res = await fetch('http://localhost:5243/api/memberships', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form.value)
    })
    if (res.ok) {
      showModal.value = false
      form.value = { name: '', price: 0, durationInDays: 30, isActive: true }
      await fetchMemberships()
    }
  } catch (error) {
    console.error('Error saving membership:', error)
  } finally {
    isSaving.value = false
  }
}

onMounted(() => {
  fetchMemberships()
})
</script>

<style scoped>
.badge {
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
}
.badge-success { background: #d1fae5; color: var(--success-color); }
.badge-danger { background: #fee2e2; color: var(--danger-color); }

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal-content {
  background: var(--surface-color);
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 500px;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}
.btn-outline {
  background: transparent;
  color: var(--text-main);
  border: 1px solid var(--border-color);
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-weight: 600;
  cursor: pointer;
  font-family: inherit;
}
.btn-outline:hover {
  background: #f1f5f9;
}
</style>
