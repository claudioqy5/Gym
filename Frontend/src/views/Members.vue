<template>
  <div class="members-view">
    <div class="page-header" style="display: flex; justify-content: space-between; align-items: flex-end;">
      <div>
        <h1 class="page-title">CRM & Socios</h1>
      </div>
      <button class="btn" @click="showMemberModal = true">+ Nuevo Socio</button>
    </div>

    <!-- DIRECTORIO DE SOCIOS -->
    <div class="card" style="margin-top: 2rem;">
      <div style="display: flex; justify-content: space-between; margin-bottom: 1.5rem; gap: 1rem;">
        <div class="form-group" style="margin-bottom: 0; flex: 1; max-width: 300px;">
          <input type="text" v-model="searchQuery" class="form-control" placeholder="Buscar por nombre o DNI...">
        </div>
      </div>
      
      <div class="table-responsive" style="overflow-x: auto;">
        <table class="styled-table">
          <thead>
            <tr>
              <th>Nombre Completo</th>
              <th>Email</th>
              <th>Teléfono</th>
              <th>Rol</th>
              <th>Estado</th>
              <th style="text-align: right;">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="filteredMembers.length === 0">
              <td colspan="6" style="text-align: center; color: var(--text-muted); padding: 2rem;">
                No hay socios registrados.
              </td>
            </tr>
            <tr v-for="member in filteredMembers" :key="member.id">
              <td>
                <div style="display:flex; align-items:center; gap: 10px;">
                  <div class="avatar">{{ member.name.substring(0, 2).toUpperCase() }}</div>
                  <div style="font-weight: 600; color: var(--text-main);">{{ member.name }}</div>
                </div>
              </td>
              <td>{{ member.email }}</td>
              <td>{{ member.phone || 'N/A' }}</td>
              <td><span class="badge badge-primary">{{ member.role }}</span></td>
              <td><span class="badge" :class="member.status === 'active' ? 'badge-success' : 'badge-danger'">{{ member.status }}</span></td>
              <td style="text-align: right; display: flex; justify-content: flex-end; gap: 0.5rem; align-items: center; height: 100%;">
                <button class="action-btn-icon edit" @click="editMember(member)" title="Editar">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20h9"/><path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z"/></svg>
                </button>
                <button class="action-btn-icon suspend" @click="suspendMember(member)" title="Suspender">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="10" y1="15" x2="10" y2="9"/><line x1="14" y1="15" x2="14" y2="9"/></svg>
                </button>
                <button class="action-btn-icon delete" @click="deleteMember(member)" title="Eliminar">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/><line x1="10" y1="11" x2="10" y2="17"/><line x1="14" y1="11" x2="14" y2="17"/></svg>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Registrar Socio -->
    <div v-if="showMemberModal" class="modal-overlay" @click.self="showMemberModal = false">
      <div class="modal-content">
        <h2 style="margin-bottom: 1.5rem;">Registrar Nuevo Socio</h2>
        <form @submit.prevent="saveMember">
          <div class="form-group">
            <label>Nombre Completo</label>
            <input type="text" v-model="form.name" class="form-control" required>
          </div>
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div class="form-group">
              <label>Correo Electrónico</label>
              <input type="email" v-model="form.email" class="form-control" required>
            </div>
            <div class="form-group">
              <label>Teléfono</label>
              <input type="text" v-model="form.phone" class="form-control" required>
            </div>
          </div>
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div class="form-group">
              <label>Contraseña (App Móvil)</label>
              <input type="password" v-model="form.passwordHash" class="form-control" required>
            </div>
            <div class="form-group">
              <label>Rol</label>
              <select v-model="form.role" class="form-control">
                <option value="member">Socio (Cliente)</option>
                <option value="trainer">Entrenador</option>
              </select>
            </div>
          </div>
          <div style="display: flex; justify-content: flex-end; gap: 1rem; margin-top: 2rem;">
            <button type="button" class="btn-outline" @click="showMemberModal = false">Cancelar</button>
            <button type="submit" class="btn" :disabled="isSaving">{{ isSaving ? 'Guardando...' : 'Guardar Socio' }}</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal Ver Perfil -->
    <div v-if="showProfileModal && selectedMember" class="modal-overlay" @click.self="showProfileModal = false">
      <div class="modal-content" style="max-width: 450px;">
        <div style="display: flex; flex-direction: column; align-items: center; text-align: center; margin-bottom: 1.5rem;">
          <div class="avatar" style="width: 80px; height: 80px; font-size: 2rem; margin-bottom: 1rem;">
            {{ selectedMember.name.substring(0, 2).toUpperCase() }}
          </div>
          <h2 style="margin: 0; font-size: 1.5rem;">{{ selectedMember.name }}</h2>
          <span class="badge badge-primary" style="margin-top: 0.5rem;">{{ selectedMember.role }}</span>
        </div>
        
        <div style="border-top: 1px solid var(--border-color); padding-top: 1.5rem;">
          <p style="margin-bottom: 0.75rem;"><strong>Email:</strong> {{ selectedMember.email }}</p>
          <p style="margin-bottom: 0.75rem;"><strong>Teléfono:</strong> {{ selectedMember.phone }}</p>
          <p style="margin-bottom: 0.75rem;">
            <strong>Estado:</strong> 
            <span class="badge" :class="selectedMember.status === 'active' ? 'badge-success' : 'badge-danger'" style="margin-left: 0.5rem;">
              {{ selectedMember.status }}
            </span>
          </p>
        </div>

        <div style="display: flex; justify-content: space-between; gap: 1rem; margin-top: 2.5rem;">
          <button class="btn btn-outline" style="color: var(--danger-color); border-color: var(--danger-color);" @click="showProfileModal = false">Suspender Socio</button>
          <button class="btn" @click="showProfileModal = false">Cerrar Perfil</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const showMemberModal = ref(false)
const showProfileModal = ref(false)
const selectedMember = ref(null)
const isSaving = ref(false)
const members = ref([])
const searchQuery = ref('')

const form = ref({
  name: '',
  email: '',
  phone: '',
  passwordHash: '',
  role: 'member',
  status: 'active'
})

const filteredMembers = computed(() => {
  return members.value.filter(m => m.name.toLowerCase().includes(searchQuery.value.toLowerCase()) || m.email.toLowerCase().includes(searchQuery.value.toLowerCase()))
})

const fetchMembers = async () => {
  try {
    const res = await fetch('http://localhost:5243/api/users')
    members.value = await res.json()
  } catch (error) {
    console.error(error)
  }
}

const isEditing = ref(false)
const editingId = ref(null)

const editMember = (member) => {
  isEditing.value = true
  editingId.value = member.id
  form.value = { ...member }
  showMemberModal.value = true
}

const saveMember = async () => {
  isSaving.value = true
  try {
    const url = isEditing.value 
      ? `http://localhost:5243/api/users/${editingId.value}` 
      : 'http://localhost:5243/api/users'
    const method = isEditing.value ? 'PUT' : 'POST'

    const res = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form.value)
    })
    
    if (res.ok) {
      showMemberModal.value = false
      form.value = { name: '', email: '', phone: '', passwordHash: '', role: 'member', status: 'active' }
      isEditing.value = false
      editingId.value = null
      fetchMembers()
    } else {
      alert('Error guardando socio')
    }
  } catch (error) {
    console.error(error)
  } finally {
    isSaving.value = false
  }
}

const suspendMember = async (member) => {
  if (confirm(`¿Estás seguro de suspender a ${member.name}?`)) {
    try {
      await fetch(`http://localhost:5243/api/users/${member.id}/suspend`, { method: 'PUT' })
      fetchMembers()
    } catch (e) {
      console.error(e)
    }
  }
}

const deleteMember = async (member) => {
  if (confirm(`¿Estás seguro de ELIMINAR permanentemente a ${member.name}? Esta acción no se puede deshacer.`)) {
    try {
      await fetch(`http://localhost:5243/api/users/${member.id}`, { method: 'DELETE' })
      fetchMembers()
    } catch (e) {
      console.error(e)
    }
  }
}

onMounted(() => {
  fetchMembers()
})
</script>

<style scoped>
.styled-table { width: 100%; border-collapse: collapse; text-align: left; }
.styled-table th { padding: 1rem; font-weight: 600; color: var(--text-muted); border-bottom: 1px solid var(--border-color); font-size: 0.875rem; text-transform: uppercase; }
.styled-table td { padding: 1rem; border-bottom: 1px solid var(--border-color); vertical-align: middle; }
.avatar { width: 40px; height: 40px; border-radius: 50%; background-color: #e0e7ff; color: var(--primary-color); display: flex; align-items: center; justify-content: center; font-weight: 700; font-size: 0.875rem; }
.badge { padding: 0.25rem 0.75rem; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; }
.badge-primary { background: #e0e7ff; color: var(--primary-color); }
.badge-success { background: #d1fae5; color: var(--success-color); }
.badge-danger { background: #fee2e2; color: var(--danger-color); }

.action-btn-icon {
  background: transparent;
  border: 1px solid var(--border-color);
  padding: 0.5rem;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  color: var(--text-muted);
}
.action-btn-icon.edit:hover {
  background: #e0e7ff;
  color: var(--primary-color);
  border-color: var(--primary-color);
}
.action-btn-icon.suspend:hover {
  background: #fef3c7;
  color: #d97706;
  border-color: #d97706;
}
.action-btn-icon.delete:hover {
  background: #fee2e2;
  color: var(--danger-color);
  border-color: var(--danger-color);
}

.modal-overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 1000; }
.modal-content { background: var(--surface-color); padding: 2rem; border-radius: 12px; width: 100%; max-width: 600px; box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1); }
.btn-outline { background: transparent; border: 1px solid var(--border-color); padding: 0.75rem 1.5rem; border-radius: 0.5rem; cursor: pointer; }
</style>
