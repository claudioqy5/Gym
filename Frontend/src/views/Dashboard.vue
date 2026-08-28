<template>
  <div class="dashboard-view">
    <div class="page-header" style="margin-bottom: 2rem;">
      <h1 class="page-title">Dashboard</h1>
      <p style="color: var(--text-muted);">Bienvenido al panel de control de GymPro. Aquí tienes el resumen de tu negocio hoy.</p>
    </div>

    <!-- KPIs -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon bg-primary-light">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--primary-color)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
        </div>
        <div class="stat-details">
          <p class="stat-title">Usuarios Totales</p>
          <p class="stat-value">{{ membersCount }}</p>
          <p class="stat-trend trend-up">Registrados</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon bg-success-light">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--success-color)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>
        </div>
        <div class="stat-details">
          <p class="stat-title">Ingresos Históricos</p>
          <p class="stat-value">${{ totalRevenue.toFixed(2) }}</p>
          <p class="stat-trend trend-up">Total de transacciones</p>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon bg-warn-light">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#d97706" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/></svg>
        </div>
        <div class="stat-details">
          <p class="stat-title">Clases Programadas</p>
          <p class="stat-value">{{ classesCount }}</p>
          <p class="stat-trend">En el sistema</p>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon bg-danger-light">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="var(--danger-color)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M22 12h-4l-3 9L9 3l-3 9H2"/></svg>
        </div>
        <div class="stat-details">
          <p class="stat-title">Productos en Tienda</p>
          <p class="stat-value">{{ productsCount }}</p>
          <p class="stat-trend trend-down">Gestión de inventario</p>
        </div>
      </div>
    </div>

    <!-- Charts / Tables section -->
    <div class="dashboard-widgets">
      <div class="card widget-list" style="grid-column: 1 / -1;">
        <h3 style="margin-bottom: 1rem; font-size: 1.1rem;">Próximas Clases</h3>
        <ul class="class-list">
          <li v-if="upcomingClasses.length === 0" style="text-align: center; color: var(--text-muted); border: none;">
            No hay clases próximas registradas.
          </li>
          <li v-for="c in upcomingClasses" :key="c.id">
            <div class="time">{{ new Date(c.startTime).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) }}</div>
            <div class="info">
              <strong>{{ c.name }}</strong>
              <span>Prof. {{ c.instructor }} | {{ new Date(c.startTime).toLocaleDateString() }}</span>
            </div>
            <div class="status" :class="{'full': c.reservedUserIds.length >= c.maxCapacity}">
              {{ c.reservedUserIds.length }}/{{ c.maxCapacity }}
            </div>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const membersCount = ref(0)
const totalRevenue = ref(0)
const classesCount = ref(0)
const productsCount = ref(0)
const upcomingClasses = ref([])

const fetchDashboardData = async () => {
  try {
    // Miembros (Filtrado solo a administradores según solicitud)
    const resUsers = await fetch('http://localhost:5243/api/users')
    const users = await resUsers.json()
    membersCount.value = users.filter(u => u.role === 'admin').length

    // Ingresos
    const resPayments = await fetch('http://localhost:5243/api/payments')
    const payments = await resPayments.json()
    totalRevenue.value = payments.reduce((acc, p) => acc + p.amount, 0)

    // Productos
    const resProducts = await fetch('http://localhost:5243/api/products')
    const products = await resProducts.json()
    productsCount.value = products.length

    // Clases
    const resClasses = await fetch('http://localhost:5243/api/classes')
    const classes = await resClasses.json()
    classesCount.value = classes.length
    upcomingClasses.value = classes.slice(0, 5) // Top 5
  } catch (error) {
    console.error('Error cargando dashboard', error)
  }
}

onMounted(() => {
  fetchDashboardData()
})
</script>

<style scoped>
.stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 1.5rem; margin-bottom: 2rem; }
.stat-card { background: var(--surface-color); padding: 1.5rem; border-radius: 12px; border: 1px solid var(--border-color); display: flex; align-items: center; gap: 1rem; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05); transition: transform 0.2s; }
.stat-card:hover { transform: translateY(-3px); }
.stat-icon { width: 56px; height: 56px; border-radius: 12px; display: flex; align-items: center; justify-content: center; }
.bg-primary-light { background: #e0e7ff; }
.bg-success-light { background: #d1fae5; }
.bg-danger-light { background: #fee2e2; }
.bg-warn-light { background: #fef3c7; }
.stat-details { display: flex; flex-direction: column; }
.stat-title { color: var(--text-muted); font-size: 0.85rem; font-weight: 600; text-transform: uppercase; }
.stat-value { font-size: 1.75rem; font-weight: 700; color: var(--text-main); line-height: 1.2; margin: 0.25rem 0; }
.stat-trend { font-size: 0.75rem; color: var(--text-muted); }
.trend-up { color: var(--success-color); font-weight: 600;}
.trend-down { color: var(--danger-color); font-weight: 600;}
.dashboard-widgets { display: grid; grid-template-columns: 1fr; gap: 1.5rem; }
.class-list { list-style: none; padding: 0; margin: 0; }
.class-list li { display: flex; align-items: center; padding: 1rem 0; border-bottom: 1px solid var(--border-color); }
.class-list li:last-child { border-bottom: none; }
.class-list .time { font-weight: 700; color: var(--primary-color); width: 80px; }
.class-list .info { flex: 1; display: flex; flex-direction: column; }
.class-list .info span { font-size: 0.8rem; color: var(--text-muted); }
.class-list .status { background: #e0e7ff; color: var(--primary-color); padding: 0.25rem 0.5rem; border-radius: 4px; font-size: 0.8rem; font-weight: 600; }
.class-list .status.full { background: #fee2e2; color: var(--danger-color); }
</style>
