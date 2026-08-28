<template>
  <div class="payments-view">
    <div class="page-header" style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
      <h1 class="page-title">Caja y Pagos</h1>
    </div>

    <!-- Indicadores -->
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1.5rem; margin-bottom: 2rem;">
      <div class="card stat-mini">
        <p class="label">Total Recaudado (Histórico)</p>
        <p class="val success">${{ totalAmount.toFixed(2) }}</p>
      </div>
      <div class="card stat-mini">
        <p class="label">Transacciones Registradas</p>
        <p class="val primary">{{ payments.length }}</p>
      </div>
      <div class="card stat-mini" style="display:flex; align-items:center; justify-content:center;">
        <button class="btn btn-block" @click="fetchPayments">↻ Refrescar Caja</button>
      </div>
    </div>

    <!-- Tabla -->
    <div class="card">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
        <h2 style="font-size: 1.25rem; margin: 0;">Historial de Transacciones</h2>
      </div>
      
      <div class="table-responsive" style="overflow-x: auto;">
        <table class="styled-table">
          <thead>
            <tr>
              <th>Fecha y Hora</th>
              <th>Socio / Origen</th>
              <th>Concepto</th>
              <th>Método</th>
              <th style="text-align: right;">Monto</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="payments.length === 0">
              <td colspan="5" style="text-align: center; color: var(--text-muted); padding: 2rem;">
                No hay pagos registrados en caja. (Intenta hacer una venta en la tienda)
              </td>
            </tr>
            <tr v-for="payment in payments" :key="payment.id">
              <td>{{ new Date(payment.paymentDate).toLocaleString() }}</td>
              <td style="font-weight: 600;">{{ payment.memberName || payment.memberId }}</td>
              <td>{{ payment.concept }}</td>
              <td><span class="badge" :class="payment.paymentMethod === 'Efectivo' ? 'badge-cash' : 'badge-transfer'">{{ payment.paymentMethod }}</span></td>
              <td style="text-align: right; font-weight: 700; color: var(--success-color);">+${{ payment.amount.toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
const payments = ref([])

const totalAmount = computed(() => payments.value.reduce((acc, p) => acc + p.amount, 0))

const fetchPayments = async () => {
  try {
    const res = await fetch('http://localhost:5243/api/payments')
    payments.value = await res.json()
  } catch (error) {
    console.error(error)
  }
}

onMounted(() => {
  fetchPayments()
})
</script>

<style scoped>
.stat-mini { padding: 1.5rem; }
.stat-mini .label { color: var(--text-muted); font-size: 0.85rem; font-weight: 600; text-transform: uppercase; margin-bottom: 0.5rem; }
.stat-mini .val { font-size: 2rem; font-weight: 700; }
.val.success { color: var(--success-color); }
.val.primary { color: var(--primary-color); }
.styled-table { width: 100%; border-collapse: collapse; text-align: left; }
.styled-table th { padding: 1rem; font-weight: 600; color: var(--text-muted); border-bottom: 1px solid var(--border-color); font-size: 0.875rem; text-transform: uppercase; }
.styled-table td { padding: 1rem; border-bottom: 1px solid var(--border-color); vertical-align: middle; }
.badge { padding: 0.25rem 0.75rem; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; }
.badge-cash { background: #dcfce7; color: #166534; }
.badge-transfer { background: #e0e7ff; color: #3730a3; }
</style>
