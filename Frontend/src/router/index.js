import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from '../views/Dashboard.vue'
import Login from '../views/Login.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: Login
    },
    {
      path: '/',
      name: 'home',
      component: Dashboard
    },
    {
      path: '/members',
      name: 'members',
      component: () => import('../views/Dashboard.vue') // Placeholder
    }
  ]
})

router.beforeEach((to, from, next) => {
  const isAuthenticated = localStorage.getItem('token')

  if (to.name !== 'login' && !isAuthenticated) {
    // Si intenta ir a cualquier lado sin token, lo mandamos al login
    next({ name: 'login' })
  } else if (to.name === 'login' && isAuthenticated) {
    // Si ya tiene sesión y quiere ir al login, lo mandamos al dashboard
    next({ name: 'home' })
  } else {
    // Todo bien, que pase
    next()
  }
})

export default router
