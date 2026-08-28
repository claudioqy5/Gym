import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from '../views/Dashboard.vue'
import Login from '../views/Login.vue'
import Members from '../views/Members.vue'
import Memberships from '../views/Memberships.vue'
import Store from '../views/Store.vue'
import Payments from '../views/Payments.vue'
import Classes from '../views/Classes.vue'
import Retention from '../views/Retention.vue'
import WhatsApp from '../views/WhatsApp.vue'
import Settings from '../views/Settings.vue'

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
      component: Members
    },
    {
      path: '/memberships',
      name: 'memberships',
      component: Memberships
    },
    {
      path: '/store',
      name: 'store',
      component: Store
    },
    {
      path: '/payments',
      name: 'payments',
      component: Payments
    },
    {
      path: '/classes',
      name: 'classes',
      component: Classes
    },
    {
      path: '/retention',
      name: 'retention',
      component: Retention
    },
    {
      path: '/whatsapp',
      name: 'whatsapp',
      component: WhatsApp
    },
    {
      path: '/settings',
      name: 'settings',
      component: Settings
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
