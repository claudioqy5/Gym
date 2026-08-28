<template>
  <div class="retention-view">
    <div class="page-header">
      <h1 class="page-title">Predictor de Abandono (IA)</h1>
      <p style="color: var(--text-muted); margin-top: 0.5rem;">Analiza los patrones de asistencia para identificar qué socios tienen alto riesgo de no renovar.</p>
    </div>

    <div class="card">
      <div style="display: flex; justify-content: center; align-items: center; padding: 4rem 0; flex-direction: column; gap: 1rem;">
        <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="var(--primary-color)" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M22 12h-4l-3 9L9 3l-3 9H2"/></svg>
        <h2 style="margin:0;">Módulo de Inteligencia Artificial</h2>
        <p style="color: var(--text-muted); text-align: center; max-width: 500px;">
          Esta función requerirá acumular suficientes datos de asistencia (al menos 30 días) para que el modelo pueda empezar a hacer predicciones precisas sobre el comportamiento de tus socios.
        </p>
        <button class="btn" style="margin-top: 1rem;">Sincronizar Datos Actuales</button>
      </div>
    </div>
  </div>
</template>

<script setup>
</script>
