<template>
  <div class="dashboard">
    <div class="page-header">
      <h1 class="page-title">Dashboard Overview</h1>
      <p class="text-muted">Welcome back. Here is the current status of the gym.</p>
    </div>

    <div class="stats-grid">
      <div class="card stat-card">
        <h3>Total Members</h3>
        <div class="stat-value">1,248</div>
        <div class="stat-trend success">+12 this week</div>
      </div>
      <div class="card stat-card">
        <h3>Active Subscriptions</h3>
        <div class="stat-value">982</div>
        <div class="stat-trend danger">-5 from last month</div>
      </div>
      <div class="card stat-card">
        <h3>Pending Payments</h3>
        <div class="stat-value">45</div>
        <div class="stat-trend">Needs attention</div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}

.stat-card {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.stat-card h3 {
  font-size: 0.875rem;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin: 0;
}

.stat-value {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--text-main);
}

.stat-trend {
  font-size: 0.875rem;
  color: var(--text-muted);
}

.stat-trend.success {
  color: var(--success-color);
}

.stat-trend.danger {
  color: var(--danger-color);
}
</style>
