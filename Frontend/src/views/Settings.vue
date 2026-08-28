<template>
  <div class="settings-view">
    <div class="page-header">
      <h1 class="page-title">Configuración</h1>
    </div>

    <div class="card" style="max-width: 600px;">
      <h2 style="font-size: 1.25rem; border-bottom: 1px solid var(--border-color); padding-bottom: 0.5rem; margin-bottom: 1.5rem;">Datos del Gimnasio</h2>
      
      <div class="form-group">
        <label>Nombre del Gimnasio</label>
        <input type="text" class="form-control" value="GymPro Fitness">
      </div>
      
      <div class="form-group">
        <label>Moneda</label>
        <select class="form-control">
          <option value="PEN">Soles (PEN)</option>
          <option value="USD">Dólares (USD)</option>
          <option value="EUR">Euros (EUR)</option>
        </select>
      </div>
      
      <div class="form-group">
        <label>Mensaje de Bienvenida (Tickets)</label>
        <textarea class="form-control" rows="3" placeholder="¡Gracias por entrenar con nosotros!"></textarea>
      </div>

      <button class="btn" style="margin-top: 1rem;">Guardar Cambios</button>
    </div>
  </div>
</template>

<script setup>
</script>
