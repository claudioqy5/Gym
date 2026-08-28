<template>
  <div class="classes-view">
    <div class="page-header" style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
      <div>
        <h1 class="page-title">Agenda y Reservas</h1>
        <p style="color: var(--text-muted); margin-top: 0.5rem;">Gestiona los horarios, cupos limitados y reservas de tus socios.</p>
      </div>
      <button class="btn" @click="showNewClassModal = true">+ Programar Clase</button>
    </div>

    <div class="classes-grid">
      <div v-if="classes.length === 0" style="text-align: center; color: var(--text-muted); padding: 3rem;">
        No hay clases programadas.
      </div>
      
      <div v-for="gymClass in classes" :key="gymClass.id" class="class-card" :class="{'opacity-50': gymClass.reservedUserIds.length >= gymClass.maxCapacity}">
        <div class="class-time">
          <span class="hour">{{ new Date(gymClass.startTime).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) }}</span>
          <span class="duration">{{ new Date(gymClass.startTime).toLocaleDateString() }}</span>
          <span class="duration">{{ gymClass.durationMinutes }} min</span>
        </div>
        <div class="class-info">
          <div style="display: flex; justify-content: space-between; align-items: flex-start;">
            <div>
              <h3>{{ gymClass.name }}</h3>
              <p class="instructor">Prof. {{ gymClass.instructor }}</p>
            </div>
            <span class="badge" 
              :class="{'badge-danger': gymClass.reservedUserIds.length >= gymClass.maxCapacity, 'badge-success': gymClass.reservedUserIds.length < gymClass.maxCapacity}">
              {{ gymClass.reservedUserIds.length >= gymClass.maxCapacity ? 'Lleno' : 'Disponible' }}
            </span>
          </div>
          
          <div class="capacity-bar-container">
            <div style="display: flex; justify-content: space-between; font-size: 0.75rem; margin-bottom: 0.25rem;">
              <span style="font-weight: 600;">Reservas</span>
              <span :style="gymClass.reservedUserIds.length >= gymClass.maxCapacity ? 'color: var(--danger-color); font-weight: 700;' : ''">
                {{ gymClass.reservedUserIds.length }} / {{ gymClass.maxCapacity }} cupos
              </span>
            </div>
            <div class="capacity-bar">
              <div class="capacity-fill" 
                :style="`width: ${(gymClass.reservedUserIds.length / gymClass.maxCapacity) * 100}%; background-color: ${gymClass.reservedUserIds.length >= gymClass.maxCapacity ? 'var(--danger-color)' : 'var(--success-color)'};`">
              </div>
            </div>
          </div>
          
          <div class="class-actions">
            <button class="btn-sm" :disabled="gymClass.reservedUserIds.length >= gymClass.maxCapacity || isProcessing" @click="reserveSpot(gymClass.id)">
              {{ gymClass.reservedUserIds.length >= gymClass.maxCapacity ? 'Sin Cupo' : 'Reservar Mi Cupo' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Programar Clase -->
    <div v-if="showNewClassModal" class="modal-overlay" @click.self="showNewClassModal = false">
      <div class="modal-content">
        <h2 style="margin-bottom: 1.5rem;">Programar Nueva Clase</h2>
        <form @submit.prevent="saveClass">
          <div class="form-group">
            <label>Nombre de la Actividad</label>
            <input type="text" v-model="classForm.name" class="form-control" required placeholder="Ej. Spinning, Pilates, Yoga...">
          </div>
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div class="form-group">
              <label>Instructor</label>
              <input type="text" v-model="classForm.instructor" class="form-control" required>
            </div>
            <div class="form-group">
              <label>Cupo Máximo</label>
              <input type="number" v-model="classForm.maxCapacity" class="form-control" required>
            </div>
          </div>
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div class="form-group">
              <label>Fecha y Hora</label>
              <input type="datetime-local" v-model="classForm.startTime" class="form-control" required>
            </div>
            <div class="form-group">
              <label>Duración (Minutos)</label>
              <input type="number" v-model="classForm.durationMinutes" class="form-control" required>
            </div>
          </div>
          <div style="display: flex; justify-content: flex-end; gap: 1rem; margin-top: 2rem;">
            <button type="button" class="btn-outline" @click="showNewClassModal = false">Cancelar</button>
            <button type="submit" class="btn" :disabled="isProcessing">Guardar Clase</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
const showNewClassModal = ref(false)
const classes = ref([])
const isProcessing = ref(false)

const classForm = ref({
  name: '',
  instructor: '',
  maxCapacity: 20,
  startTime: '',
  durationMinutes: 60,
  reservedUserIds: []
})

const fetchClasses = async () => {
  try {
    const res = await fetch('http://localhost:5243/api/classes')
    classes.value = await res.json()
  } catch (error) {
    console.error(error)
  }
}

const saveClass = async () => {
  isProcessing.value = true
  try {
    const res = await fetch('http://localhost:5243/api/classes', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        ...classForm.value,
        startTime: new Date(classForm.value.startTime).toISOString()
      })
    })
    if (res.ok) {
      showNewClassModal.value = false
      fetchClasses()
    }
  } catch (error) {
    console.error(error)
  } finally {
    isProcessing.value = false
  }
}

const reserveSpot = async (classId) => {
  isProcessing.value = true
  try {
    // Simulamos que el usuario actual es un usuario guardado (en un sistema real saldría del JWT)
    const storedUser = JSON.parse(localStorage.getItem('user')) || { Id: 'Invitado_' + Math.random().toString(36).substr(2, 5) }
    
    const res = await fetch(`http://localhost:5243/api/classes/${classId}/reserve`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(storedUser.Id || storedUser.id)
    })
    if (res.ok) {
      alert('Reserva confirmada con éxito.')
      fetchClasses()
    } else {
      const err = await res.text()
      alert('Error: ' + err)
    }
  } catch (error) {
    console.error(error)
  } finally {
    isProcessing.value = false
  }
}

onMounted(() => {
  fetchClasses()
})
</script>

<style scoped>
.classes-grid { display: flex; flex-direction: column; gap: 1rem; }
.class-card { display: flex; background: var(--surface-color); border: 1px solid var(--border-color); border-radius: 12px; overflow: hidden; transition: transform 0.2s, box-shadow 0.2s; }
.class-card:hover { transform: translateY(-2px); box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.05); }
.opacity-50 { opacity: 0.8; }
.class-time { background: #f8fafc; padding: 1.5rem; display: flex; flex-direction: column; justify-content: center; align-items: center; border-right: 1px solid var(--border-color); min-width: 120px; }
.class-time .hour { font-size: 1.5rem; font-weight: 700; color: var(--primary-color); }
.class-time .duration { font-size: 0.75rem; color: var(--text-muted); font-weight: 600; margin-top: 0.25rem; }
.class-info { padding: 1.5rem; flex: 1; display: flex; flex-direction: column; }
.class-info h3 { margin: 0; font-size: 1.15rem; }
.instructor { color: var(--text-muted); font-size: 0.85rem; margin-top: 0.25rem; }
.capacity-bar-container { margin: 1.25rem 0; }
.capacity-bar { height: 8px; background-color: #e2e8f0; border-radius: 999px; overflow: hidden; }
.capacity-fill { height: 100%; border-radius: 999px; transition: width 0.5s ease; }
.class-actions { display: flex; justify-content: flex-end; gap: 1rem; margin-top: auto; }
.btn-sm { padding: 0.5rem 1rem; border-radius: 6px; font-size: 0.85rem; font-weight: 600; cursor: pointer; background-color: var(--primary-color); color: white; border: none; }
.badge { padding: 0.25rem 0.75rem; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; }
.badge-success { background: #d1fae5; color: var(--success-color); }
.badge-danger { background: #fee2e2; color: var(--danger-color); }
.modal-overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 1000; }
.modal-content { background: var(--surface-color); padding: 2rem; border-radius: 12px; width: 100%; max-width: 500px; }
.btn-outline { background: transparent; border: 1px solid var(--border-color); padding: 0.75rem 1.5rem; border-radius: 0.5rem; cursor: pointer; }
</style>
