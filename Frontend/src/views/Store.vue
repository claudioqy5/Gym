<template>
  <div class="store-view">
    <div class="page-header" style="display: flex; justify-content: space-between; align-items: flex-end; margin-bottom: 2rem;">
      <div>
        <h1 class="page-title">Tienda e Inventario</h1>
        <div class="tabs" style="display: flex; gap: 1.5rem; margin-top: 1rem; border-bottom: 1px solid var(--border-color);">
          <button @click="activeTab = 'pos'" :class="['tab-btn', { active: activeTab === 'pos' }]">Punto de Venta</button>
          <button @click="activeTab = 'inventory'" :class="['tab-btn', { active: activeTab === 'inventory' }]">Gestión de Inventario</button>
        </div>
      </div>
      <button class="btn" @click="activeTab === 'pos' ? (cart = []) : showProductModal = true">
        <span v-if="activeTab === 'pos'">Vaciar Venta</span>
        <span v-else>+ Nuevo Producto</span>
      </button>
    </div>

    <!-- PESTAÑA: PUNTO DE VENTA (POS) -->
    <div v-if="activeTab === 'pos'" class="store-layout">
      <div class="products-grid">
        <div class="form-group" style="margin-bottom: 1rem;">
          <input type="text" v-model="searchQuery" class="form-control" placeholder="Buscar producto por nombre...">
        </div>
        
        <div class="grid">
          <div v-if="filteredProducts.length === 0" style="grid-column: 1 / -1; text-align: center; color: var(--text-muted); padding: 2rem;">
            No hay productos registrados en el inventario.
          </div>
          <div v-for="product in filteredProducts" :key="product.id" class="product-card" @click="addToCart(product)">
            <div class="stock-badge" :class="{ 'danger': product.stockQuantity === 0, 'warn': product.stockQuantity > 0 && product.stockQuantity <= 5 }">
              {{ product.stockQuantity > 0 ? product.stockQuantity + ' en stock' : 'Agotado' }}
            </div>
            <div class="product-img" :class="{'opacity-50': product.stockQuantity === 0}">📦</div>
            <div class="product-info" :class="{'opacity-50': product.stockQuantity === 0}">
              <h4>{{ product.name }}</h4>
              <p class="price">${{ product.sellingPrice.toFixed(2) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Resumen de Venta -->
      <div class="cart-section card">
        <h3 style="margin-bottom: 1.5rem; border-bottom: 1px solid var(--border-color); padding-bottom: 0.5rem;">Venta Actual</h3>
        <div class="cart-items">
          <div v-if="cart.length === 0" style="text-align: center; color: var(--text-muted); padding: 2rem;">Carrito vacío</div>
          <div v-for="item in cart" :key="item.id" class="cart-item">
            <div class="item-desc">
              <span class="qty">{{ item.qty }}x</span>
              <span class="name">{{ item.name }}</span>
            </div>
            <span class="item-total">${{ (item.sellingPrice * item.qty).toFixed(2) }}</span>
          </div>
        </div>

        <div class="cart-summary">
          <div style="display: flex; justify-content: space-between; margin-bottom: 0.5rem;">
            <span>Subtotal</span>
            <span>${{ cartTotal.toFixed(2) }}</span>
          </div>
          <div style="display: flex; justify-content: space-between; font-weight: 700; font-size: 1.25rem; color: var(--primary-color);">
            <span>Total a Cobrar</span>
            <span>${{ cartTotal.toFixed(2) }}</span>
          </div>
        </div>

        <div style="display: flex; flex-direction: column; gap: 0.75rem;">
          <button class="btn btn-block" style="background-color: var(--success-color);" :disabled="cart.length === 0 || isProcessing" @click="processSale('Efectivo')">Cobrar Efectivo</button>
          <button class="btn btn-block" style="background-color: #3730a3;" :disabled="cart.length === 0 || isProcessing" @click="processSale('POS')">Cobrar POS / Transferencia</button>
        </div>
      </div>
    </div>

    <!-- PESTAÑA: INVENTARIO -->
    <div v-else class="card">
      <div class="table-responsive" style="overflow-x: auto;">
        <table class="styled-table">
          <thead>
            <tr>
              <th>Producto</th>
              <th>Categoría</th>
              <th style="text-align: right;">Costo</th>
              <th style="text-align: right;">Precio Venta</th>
              <th style="text-align: center;">Stock</th>
              <th style="text-align: right;">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="products.length === 0">
              <td colspan="6" style="text-align: center; color: var(--text-muted); padding: 2rem;">Sin productos.</td>
            </tr>
            <tr v-for="product in products" :key="product.id">
              <td>
                <div style="font-weight: 600; color: var(--text-main);">{{ product.name }}</div>
                <div style="font-size: 0.75rem; color: var(--text-muted);">Cod: {{ product.barcode || 'N/A' }}</div>
              </td>
              <td>{{ product.category }}</td>
              <td style="text-align: right;">${{ product.costPrice.toFixed(2) }}</td>
              <td style="text-align: right; font-weight: 600;">${{ product.sellingPrice.toFixed(2) }}</td>
              <td style="text-align: center;">
                <span class="badge" :class="{'badge-success': product.stockQuantity > 5, 'badge-warn': product.stockQuantity > 0 && product.stockQuantity <= 5, 'badge-danger': product.stockQuantity === 0}">
                  {{ product.stockQuantity }} u.
                </span>
              </td>
              <td style="text-align: right;">
                <button class="action-btn">Editar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Nuevo Producto -->
    <div v-if="showProductModal" class="modal-overlay" @click.self="showProductModal = false">
      <div class="modal-content">
        <h2 style="margin-bottom: 1.5rem;">Registrar Producto</h2>
        <form @submit.prevent="saveProduct">
          <div class="form-group">
            <label>Nombre del Producto</label>
            <input type="text" v-model="prodForm.name" class="form-control" required>
          </div>
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div class="form-group">
              <label>Categoría</label>
              <input type="text" v-model="prodForm.category" class="form-control" required>
            </div>
            <div class="form-group">
              <label>Código de Barras</label>
              <input type="text" v-model="prodForm.barcode" class="form-control">
            </div>
          </div>
          <div style="display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 1rem;">
            <div class="form-group">
              <label>Costo</label>
              <input type="number" step="0.01" v-model="prodForm.costPrice" class="form-control" required>
            </div>
            <div class="form-group">
              <label>Precio Venta</label>
              <input type="number" step="0.01" v-model="prodForm.sellingPrice" class="form-control" required>
            </div>
            <div class="form-group">
              <label>Stock Inicial</label>
              <input type="number" v-model="prodForm.stockQuantity" class="form-control" required>
            </div>
          </div>
          <div style="display: flex; justify-content: flex-end; gap: 1rem; margin-top: 2rem;">
            <button type="button" class="btn-outline" @click="showProductModal = false">Cancelar</button>
            <button type="submit" class="btn" :disabled="isSavingProd">{{ isSavingProd ? 'Guardando...' : 'Guardar' }}</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const activeTab = ref('pos')
const products = ref([])
const cart = ref([])
const searchQuery = ref('')
const showProductModal = ref(false)
const isSavingProd = ref(false)
const isProcessing = ref(false)

const prodForm = ref({
  name: '',
  category: 'Bebidas',
  barcode: '',
  costPrice: 0,
  sellingPrice: 0,
  stockQuantity: 0
})

const filteredProducts = computed(() => {
  return products.value.filter(p => p.name.toLowerCase().includes(searchQuery.value.toLowerCase()))
})

const cartTotal = computed(() => {
  return cart.value.reduce((acc, item) => acc + (item.sellingPrice * item.qty), 0)
})

const fetchProducts = async () => {
  try {
    const res = await fetch('http://localhost:5243/api/products')
    products.value = await res.json()
  } catch (error) {
    console.error('Error:', error)
  }
}

const saveProduct = async () => {
  isSavingProd.value = true
  try {
    const res = await fetch('http://localhost:5243/api/products', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(prodForm.value)
    })
    if (res.ok) {
      showProductModal.value = false
      prodForm.value = { name: '', category: 'Bebidas', barcode: '', costPrice: 0, sellingPrice: 0, stockQuantity: 0 }
      fetchProducts()
    }
  } catch (error) {
    console.error(error)
  } finally {
    isSavingProd.value = false
  }
}

const addToCart = (product) => {
  if (product.stockQuantity <= 0) return
  const existing = cart.value.find(i => i.id === product.id)
  if (existing) {
    if (existing.qty < product.stockQuantity) existing.qty++
  } else {
    cart.value.push({ ...product, qty: 1 })
  }
}

const processSale = async (method) => {
  isProcessing.value = true
  try {
    const payment = {
      memberId: 'Venta Directa',
      memberName: 'Cliente Tienda',
      concept: `Venta Tienda (${cart.value.length} items)`,
      amount: cartTotal.value,
      paymentMethod: method
    }
    const res = await fetch('http://localhost:5243/api/payments', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payment)
    })
    if (res.ok) {
      alert('Venta registrada con éxito y enviada a Caja.')
      cart.value = []
    }
  } catch (error) {
    console.error(error)
  } finally {
    isProcessing.value = false
  }
}

onMounted(() => {
  fetchProducts()
})
</script>

<style scoped>
/* Tabs */
.tab-btn {
  background: none;
  border: none;
  padding: 0.5rem 1rem;
  font-family: inherit;
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-muted);
  cursor: pointer;
  border-bottom: 3px solid transparent;
  margin-bottom: -1px;
}
.tab-btn.active { color: var(--primary-color); border-bottom-color: var(--primary-color); }
.store-layout { display: grid; grid-template-columns: 2fr 1fr; gap: 1.5rem; }
@media (max-width: 1024px) { .store-layout { grid-template-columns: 1fr; } }
.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 1rem; }
.product-card {
  background: var(--surface-color); border: 1px solid var(--border-color); border-radius: 12px;
  padding: 1rem; text-align: center; cursor: pointer; position: relative;
}
.product-card:hover { border-color: var(--primary-color); }
.stock-badge { position: absolute; top: 8px; right: 8px; background: #f1f5f9; color: var(--text-muted); font-size: 0.65rem; padding: 0.15rem 0.4rem; border-radius: 4px; font-weight: 700; }
.stock-badge.warn { background: #fef3c7; color: #d97706; }
.stock-badge.danger { background: #fee2e2; color: var(--danger-color); }
.product-img { font-size: 3rem; margin: 1rem 0; }
.product-info h4 { font-size: 0.9rem; margin: 0; color: var(--text-main); }
.product-info .price { font-weight: 700; color: var(--primary-color); }
.opacity-50 { opacity: 0.5; }
.cart-section { display: flex; flex-direction: column; }
.cart-items { flex: 1; min-height: 150px; margin-bottom: 1.5rem; }
.cart-item { display: flex; justify-content: space-between; padding: 0.75rem 0; border-bottom: 1px dashed var(--border-color); }
.cart-item .qty { font-weight: 700; color: var(--primary-color); margin-right: 0.5rem; }
.styled-table { width: 100%; border-collapse: collapse; text-align: left; }
.styled-table th { padding: 1rem; font-weight: 600; color: var(--text-muted); border-bottom: 1px solid var(--border-color); font-size: 0.875rem; text-transform: uppercase; }
.styled-table td { padding: 1rem; border-bottom: 1px solid var(--border-color); vertical-align: middle; }
.badge { padding: 0.25rem 0.75rem; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; }
.badge-success { background: #d1fae5; color: var(--success-color); }
.badge-warn { background: #fef3c7; color: #d97706; }
.badge-danger { background: #fee2e2; color: var(--danger-color); }
.action-btn { background: none; border: 1px solid var(--border-color); padding: 0.5rem 1rem; border-radius: 6px; font-size: 0.875rem; font-weight: 600; cursor: pointer; }
.action-btn:hover { background: var(--primary-color); color: white; }
.modal-overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 1000; }
.modal-content { background: var(--surface-color); padding: 2rem; border-radius: 12px; width: 100%; max-width: 600px; }
.btn-outline { background: transparent; border: 1px solid var(--border-color); padding: 0.75rem 1.5rem; border-radius: 0.5rem; cursor: pointer; }
</style>
